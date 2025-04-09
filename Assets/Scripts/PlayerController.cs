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
            input = f.GetPlayerInput(0);
            bool hasInput =
            input->Left || input->Right || input->Up || input->Down || input->Attack;

            PlayerStateMachine(f, ref filter, hasInput, input);
            UpdateMovement(f, ref filter, input);
            UpdateAttack(f, ref filter, input);
        }
    }

    private EntityRef SearchTarget(Frame f, ref Filter filter) {
        EntityRef closest = EntityRef.None;
        FP closestDistance = FP._1000;

        foreach (var enemy in f.Unsafe.GetComponentBlockIterator<EnemyComponent>()) {
            var transform = f.Unsafe.GetPointer<Transform2D>(enemy.Entity);

            if (transform == null)
                continue;

            FP distance = FPVector2.Distance(transform->Position, filter.Transform->Position);
            if (distance < closestDistance) {
                closestDistance = distance;
                closest = enemy.Entity;
            }
        }
        filter.Player->TargetEnemyEntity = closest;
        return closest;
    }

    private void TraceTarget(Frame f, ref Filter filter, EntityRef target, Input* input) {
        var targetTransform= f.Unsafe.GetPointer<Transform2D>(target);

        FPVector2 direction = (targetTransform->Position - filter.Transform->Position).Normalized;

        filter.Player->PlayerDirection = direction; 

        if (filter.Player->PlayerDirection.Magnitude > 0) {
            filter.Player->PlayerDirection = filter.Player->PlayerDirection.Normalized;
            filter.Player->PlayerLastDirection = filter.Player->PlayerDirection;
        }

        FPVector2 minDistanceSq = targetTransform->Position - filter.Transform->Position;

        if (minDistanceSq.Magnitude < FP._1_50) {
            UpdateAttack(f, ref filter);
        } else {
            filter.Transform->Position += direction * f.DeltaTime * filter.Player->PlayerMoveSpeed * FP._0_50;
        }

    }

    private void PlayerStateMachine(Frame f, ref Filter filter, bool hasInput, Input* input) {
        switch (filter.Player->State) {
            case PlayerState.ControlMode:
                if (hasInput)
                    filter.Player->NotInputTimer = 0;
                else
                    filter.Player->NotInputTimer += f.DeltaTime;

                if (filter.Player->NotInputTimer >= 3) {
                    filter.Player->NotInputTimer = 0;
                    filter.Player->State = PlayerState.FreeMode;
                }

                break;
            case PlayerState.FreeMode:
                if(hasInput) {
                    filter.Player->State = PlayerState.ControlMode;
                    return;
                }

                TraceTarget(f, ref filter, SearchTarget(f, ref filter), input);
                
                break;
            
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

    private void UpdateAttack(Frame f, ref Filter filter) {
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
