using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;


[Preserve] 

public unsafe class PlayerController : SystemMainThreadFilter<PlayerController.Filter>, ISignalOnTriggerEnemyBoundHitPlayer {

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public PhysicsBody2D* Body;
        public PlayerComponent* Player;
    }

  
    public override void Update(Frame f, ref Filter filter) {
        Input* input = default;
        filter.Player->PlayerAttackTimer += f.DeltaTime;

        if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerComponent* playerComponent)) {
            if (!playerComponent->PlayerRef.IsValid)
                return;

            input = f.GetPlayerInput(playerComponent->PlayerRef);
            UpdateMovement(f, ref filter, input);
            UpdateAttack(f, ref filter, input);
        }
    }

    private void UpdateAttack(Frame f, ref Filter filter, Input* input) {
        if (!input->Attack)
            return;

        if (filter.Player->PlayerAttackTimer < filter.Player->PlayerAttackSpeed)
            return;

        EntityRef attackBound = f.Create(filter.Player->AttackBoundPrototype);
        Transform2D* boundTransform = f.Unsafe.GetPointer<Transform2D>(attackBound);

        var playerBoundComponent = f.Unsafe.GetPointer<PlayerBoundComponent>(attackBound);

        playerBoundComponent->OwnerPlayer = filter.Entity;

        var boundComponent = f.Unsafe.GetPointer<BoundLifeTimeComponent>(attackBound);
        boundComponent->DestroyTime = FP._0_25;
        boundTransform->Position = filter.Transform->Position + (filter.Player->PlayerLastDirection * 2);
        filter.Player->PlayerAttackTimer = 0;
    }
    private void UpdateMovement(Frame f, ref Filter filter, Input* input) {
        FP playerMoveSpeed = filter.Player->PlayerMoveSpeed;

        filter.Player->PlayerDirection = FPVector2.Zero;
        if (input->Up) { 
            filter.Player->PlayerDirection += filter.Transform->Up;

        }
        if (input->Down) {
            filter.Player->PlayerDirection -= filter.Transform->Up;

        }
        if (input->Right) { 
            filter.Player->PlayerDirection += filter.Transform->Right;
        }
        if (input->Left) {
            filter.Player->PlayerDirection -= filter.Transform->Right;
        }

        if (filter.Player->PlayerDirection.Magnitude > 0) {
            filter.Player->PlayerDirection = filter.Player->PlayerDirection.Normalized;
            filter.Player->PlayerLastDirection = filter.Player->PlayerDirection;
        }


        filter.Body->Velocity = filter.Player->PlayerDirection * playerMoveSpeed;
    }

    public void OnTriggerEnemyBoundHitPlayer(Frame f, TriggerInfo2D info, EnemyBoundComponent* enemyBound, PlayerComponent* player) {
        player->CurrentPlayerHp--;
    }
}
