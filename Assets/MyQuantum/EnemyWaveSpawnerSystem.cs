using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class EnemyWaveSpawnerSystem : SystemSignalsOnly, ISignalSpawnEnemy, ISignalOnComponentRemoved<EnemysEnemy> {

    //������ ���۵Ǹ� ����Ǵ� �Լ�
    public override void OnInit(Frame f) {
        SpawnEnemydWave(f);
    }

    private void SpawnEnemydWave(Frame f) {
        //Runner�� ����� GameConfig�� ã�ƿ´�.
        EnemyGameConfig config = f.FindAsset(f.RuntimeConfig.EnemyGameConfig);
        for (int i = 0; i < f.Global->EnemyWaveCount + config.InitialEnemysCount; i++) {
            SpawnEnemy(f, config.EnemyPrototype, EntityRef.None);
        }

        f.Global->EnemyWaveCount++;
    }

    public void SpawnEnemy(Frame f, AssetRef<EntityPrototype> childPrototype, EntityRef parent) {
        //�ֳʹ� ������ �ҷ���
        EnemyGameConfig config = f.FindAsset(f.RuntimeConfig.EnemyGameConfig);
        //Config�� ����� ��ƼƼ ������Ÿ���� ����
        EntityRef enemy = f.Create(childPrototype);
        //�ش� ��ƼƼ�� Ʈ������ ������ �ҷ���
        Transform2D* enemyTransform = f.Unsafe.GetPointer<Transform2D>(enemy);

        //Ʈ������ ������ ��ġ, ȸ���� ����
        if (parent == EntityRef.None) {
            enemyTransform->Position = GetRandomEdgePointOnCircle(f, config.EnemySpawnDistanceToCenter);
        } else {
            enemyTransform->Position = f.Get<Transform2D>(parent).Position;
        }
        enemyTransform->Rotation = GetRandomRotation(f);

        //�⺻ �ӵ� �� �� �Ҵ�
        if (f.Unsafe.TryGetPointer<PhysicsBody2D>(enemy, out var body)) {
            body->Velocity = enemyTransform->Up * config.EnemyInitialSpeed;
            body->AddTorque(f.RNG->Next(config.EnemyInitialTorqueMin, config.EnemyInitialTorqueMax));
        }
    }

    public static FP GetRandomRotation(Frame f) {
        //���������� ���� �Լ�
        return f.RNG->Next(0, 360);
    }

    public static FPVector2 GetRandomEdgePointOnCircle(Frame f, FP radius) {
        //���������� ���� �Լ�
        return FPVector2.Rotate(FPVector2.Up * radius, f.RNG->Next() * FP.PiTimes2);
    }

    public void OnRemoved(Frame f, EntityRef entity, EnemysEnemy* component) {
        //if (f.ComponentCount<AsteroidsAsteroid>() <= 1) {
        //    SpawnEnemydWave(f);
        //}
    }
}
