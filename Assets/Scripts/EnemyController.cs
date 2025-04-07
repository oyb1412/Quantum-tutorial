using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using UnityEngine.Scripting;


[Preserve]

public unsafe class EnemyController : SystemMainThreadFilter<EnemyController.Filter> {

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public PhysicsBody2D* Body;
        public EnemyComponent* Enemy;
    }
    public override void Update(Frame f, ref Filter filter) {
        UpdateMovement(f, ref filter);
    }
    private void UpdateMovement(Frame f, ref Filter filter) {
        FP minDistanceSq = FP.UseableMax;
        FPVector2? closestPlayerPos = null;

        for (int i = 0; i < f.Global->PlayerCount; i++) {
            EntityRef playerEntity = default;

            switch(i) {
                case 0:
                    playerEntity = f.Global->PlayerList0;
                    break;
                case 1:
                    playerEntity = f.Global->PlayerList1;
                    break;
            }

            if (playerEntity == default)
                return;


            if (f.Unsafe.TryGetPointer<Transform2D>(playerEntity, out var playerTransform)) {
                FPVector2 toPlayer = playerTransform->Position - filter.Transform->Position;
                FP distanceSq = toPlayer.Magnitude;

                if (distanceSq < minDistanceSq) {
                    minDistanceSq = distanceSq;
                    closestPlayerPos = playerTransform->Position;
                }
            }

        }
        if (closestPlayerPos != null && minDistanceSq > FP._1_50) {

            var speed = f.FindAsset<EnemyManagerConfig>(f.RuntimeConfig.EnemyManagerConfig).EnemyMoveSpeed;
            FPVector2 direction = (closestPlayerPos.Value - filter.Transform->Position).Normalized;
            filter.Transform->Position += direction * f.DeltaTime * speed;
        }
    }
}
