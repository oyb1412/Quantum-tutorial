using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;


[Preserve]

public unsafe class EnemyController : SystemMainThreadFilter<EnemyController.Filter>, ISignalOnTriggerPlayerBoundHitEnemy {

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public PhysicsBody2D* Body;
        public EnemyComponent* Enemy;
    }
    public override void Update(Frame f, ref Filter filter) {
        switch (filter.Enemy->State) {
            case EnemyState.Move:
                UpdateState(f, ref filter);
                UpdateMovement(f, ref filter);
            break;

            case EnemyState.AttackWait:
                UpdateAttackWait(f, ref filter);
            break;

            case EnemyState.Attack:
                UpdateAttack(f, ref filter);
            break;
        }
    }

    private void UpdateState(Frame f, ref Filter filter) {
        FP minDistanceSq = FP.UseableMax;
        filter.Enemy->PlayerEntity = default;
        for (int i = 0; i < f.Global->PlayerCount; i++) {

            switch (i) {
                case 0:
                    filter.Enemy->PlayerEntity = f.Global->PlayerList0;
                    break;
                case 1:
                    filter.Enemy->PlayerEntity = f.Global->PlayerList1;
                    break;
            }

            if (filter.Enemy->PlayerEntity == default)
                return;

            filter.Enemy->ClosestPlayerPos = FPVector2.Zero;

            if (f.Unsafe.TryGetPointer<Transform2D>(filter.Enemy->PlayerEntity, out var playerTransform)) {
                FPVector2 toPlayer = playerTransform->Position - filter.Transform->Position;
                FP distanceSq = toPlayer.Magnitude;

                if (distanceSq < minDistanceSq) {
                    minDistanceSq = distanceSq;
                    filter.Enemy->ClosestPlayerPos = playerTransform->Position;
                }
            }
        }

        if (filter.Enemy->ClosestPlayerPos != FPVector2.Zero ) {
            if(minDistanceSq < FP._1_50)
                filter.Enemy->State = EnemyState.AttackWait;
        }
    }
    private void UpdateMovement(Frame f, ref Filter filter) {
        if (filter.Enemy->PlayerEntity == default)
            return;

        FPVector2 direction = (filter.Enemy->ClosestPlayerPos - filter.Transform->Position).Normalized;
        filter.Transform->Position += direction * f.DeltaTime * filter.Enemy->EnemyMoveSpeed;
        filter.Enemy->Direction = direction;
    }
    private void UpdateAttackWait(Frame f, ref Filter filter) {
        filter.Enemy->EnemyAttackTimer += f.DeltaTime;

        if(filter.Enemy->EnemyAttackTimer >= filter.Enemy->EnemyAttackSpeed) {
            filter.Enemy->State = EnemyState.Attack;
        }
    }

    private void UpdateAttack(Frame f, ref Filter filter) {

        EntityRef attackBound = f.Create(filter.Enemy->AttackBoundPrototype);
        Transform2D* boundTransform = f.Unsafe.GetPointer<Transform2D>(attackBound);
        var boundComponent = f.Unsafe.GetPointer<BoundLifeTimeComponent>(attackBound);
        boundComponent->DestroyTime = FP._0_25;
        boundTransform->Position = filter.Transform->Position + (filter.Enemy->Direction * 2);
        filter.Enemy->EnemyAttackTimer = 0;
        filter.Enemy->State = EnemyState.Move;
    }

    public void OnTriggerPlayerBoundHitEnemy(Frame f, TriggerInfo2D info, PlayerBoundComponent* playerBound, EnemyComponent* enemy) {
        var playerEntity = playerBound->OwnerPlayer;
        if(f.Unsafe.TryGetPointer<PlayerComponent>(playerEntity, out var playerComp)) {
            playerComp->KillCount++;
            Log.Debug($"현재 {playerComp->PlayerRef} 플레이어 킬 : {playerComp->KillCount}");
            f.Destroy(info.Entity);
            EnemyManagerConfig config = f.FindAsset(f.RuntimeConfig.EnemyManagerConfig);
            var prototype = config.EnemyPrototype;
            f.Signals.SpawnsEnemy(prototype);
        }
        
    }
}
