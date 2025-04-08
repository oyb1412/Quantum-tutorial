using Photon.Deterministic;
using Quantum;
using Quantum.Task;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public unsafe class EnemySpawnerSystem : SystemSignalsOnly, ISignalSpawnsEnemy {
    public override void OnInit(Frame f) {
        for(int i = 0; i < 20; i ++) {
            SpawnEnemydWave(f);
            f.Global->EnemyCount++;
        }
    }

    public void SpawnsEnemy(Frame f, AssetRef<EntityPrototype> childPrototype) {
        //�ֳʹ� ������ �ҷ���
        EnemyManagerConfig config = f.FindAsset(f.RuntimeConfig.EnemyManagerConfig);
        //Config�� ����� ��ƼƼ ������Ÿ���� ����
        EntityRef enemy = f.Create(childPrototype);
        //�ش� ��ƼƼ�� Ʈ������ ������ �ҷ���
        Transform2D* enemyTransform = f.Unsafe.GetPointer<Transform2D>(enemy);

        EnemyComponent* enemyComp = f.Unsafe.GetPointer<EnemyComponent>(enemy);

        enemyComp->EnemyMoveSpeed = f.RNG->Next(1, 3);
        enemyComp->MaxEnemyHp = enemyComp->CurrentEnemyHp = 10;
        enemyComp->EnemyAttackSpeed = 2;
        enemyComp->EnemyAttackTimer = 0;
        enemyComp->EnemySpawnDistanceToCenter = 18;


        //Ʈ������ ������ ��ġ, ȸ���� ����
        enemyTransform->Position = GetRandomEdgePointOnCircle(f, enemyComp->EnemySpawnDistanceToCenter);
        enemyTransform->Rotation = GetRandomRotation(f);
    }

    private void SpawnEnemydWave(Frame f) {
        //Runner�� ����� GameConfig�� ã�ƿ´�.
        EntityPrototype enemyPrototype = UnityEngine.Resources.Load<EntityPrototype>("Prefabs/EnemyEntityPrototype");
        SpawnsEnemy(f, enemyPrototype);
    }

 

    public static FP GetRandomRotation(Frame f) {
        //���������� ���� �Լ�
        return f.RNG->Next(0, 360);
    }

    public static FPVector2 GetRandomEdgePointOnCircle(Frame f, FP radius) {
        //���������� ���� �Լ�
        return FPVector2.Rotate(FPVector2.Up * radius, f.RNG->Next() * FP.PiTimes2);
    }
}
