using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class EnemyWaveSpawnerSystem : SystemSignalsOnly, ISignalSpawnEnemy, ISignalOnComponentRemoved<EnemysEnemy> {

    //게임이 시작되면 실행되는 함수
    public override void OnInit(Frame f) {
        SpawnEnemydWave(f);
    }

    private void SpawnEnemydWave(Frame f) {
        //Runner에 등록한 GameConfig를 찾아온다.
        EnemyGameConfig config = f.FindAsset(f.RuntimeConfig.EnemyGameConfig);
        for (int i = 0; i < f.Global->EnemyWaveCount + config.InitialEnemysCount; i++) {
            SpawnEnemy(f, config.EnemyPrototype, EntityRef.None);
        }

        f.Global->EnemyWaveCount++;
    }

    public void SpawnEnemy(Frame f, AssetRef<EntityPrototype> childPrototype, EntityRef parent) {
        //애너미 에셋을 불러옴
        EnemyGameConfig config = f.FindAsset(f.RuntimeConfig.EnemyGameConfig);
        //Config에 등록한 엔티티 프로토타입을 생성
        EntityRef enemy = f.Create(childPrototype);
        //해당 엔티티의 트랜스폼 정보를 불러옴
        Transform2D* enemyTransform = f.Unsafe.GetPointer<Transform2D>(enemy);

        //트랜스폼 정보로 위치, 회전값 변경
        if (parent == EntityRef.None) {
            enemyTransform->Position = GetRandomEdgePointOnCircle(f, config.EnemySpawnDistanceToCenter);
        } else {
            enemyTransform->Position = f.Get<Transform2D>(parent).Position;
        }
        enemyTransform->Rotation = GetRandomRotation(f);

        //기본 속도 및 각 할당
        if (f.Unsafe.TryGetPointer<PhysicsBody2D>(enemy, out var body)) {
            body->Velocity = enemyTransform->Up * config.EnemyInitialSpeed;
            body->AddTorque(f.RNG->Next(config.EnemyInitialTorqueMin, config.EnemyInitialTorqueMax));
        }
    }

    public static FP GetRandomRotation(Frame f) {
        //결정론적인 랜덤 함수
        return f.RNG->Next(0, 360);
    }

    public static FPVector2 GetRandomEdgePointOnCircle(Frame f, FP radius) {
        //결정론적인 랜덤 함수
        return FPVector2.Rotate(FPVector2.Up * radius, f.RNG->Next() * FP.PiTimes2);
    }

    public void OnRemoved(Frame f, EntityRef entity, EnemysEnemy* component) {
        //if (f.ComponentCount<AsteroidsAsteroid>() <= 1) {
        //    SpawnEnemydWave(f);
        //}
    }
}
