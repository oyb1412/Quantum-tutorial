using Photon.Deterministic;
using Quantum;
using Quantum.Task;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Scripting;

[Preserve]
public unsafe class EnemySpawnerSystem : SystemSignalsOnly {

    public override void OnInit(Frame f) {
        for(int i = 0; i < 20; i ++) {
            SpawnEnemydWave(f);
            f.Global->EnemyCount++;
        }
    }

    private void SpawnEnemydWave(Frame f) {
        //Runner�� ����� GameConfig�� ã�ƿ´�.
        EnemyManagerConfig config = f.FindAsset(f.RuntimeConfig.EnemyManagerConfig);
        SpawnEnemy(f, config.EnemyPrototype);
    }

    public void SpawnEnemy(Frame f, AssetRef<EntityPrototype> childPrototype) {
        //�ֳʹ� ������ �ҷ���
        EnemyManagerConfig config = f.FindAsset(f.RuntimeConfig.EnemyManagerConfig);
        //Config�� ����� ��ƼƼ ������Ÿ���� ����
        EntityRef enemy = f.Create(childPrototype);
        //�ش� ��ƼƼ�� Ʈ������ ������ �ҷ���
        Transform2D* enemyTransform = f.Unsafe.GetPointer<Transform2D>(enemy);

        //Ʈ������ ������ ��ġ, ȸ���� ����
        enemyTransform->Position = GetRandomEdgePointOnCircle(f, config.EnemySpawnDistanceToCenter);
        enemyTransform->Rotation = GetRandomRotation(f);
        config.EnemyMoveSpeed = f.RNG->Next(2, 5);
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
