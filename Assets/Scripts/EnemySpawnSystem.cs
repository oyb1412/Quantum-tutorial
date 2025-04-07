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
        //Runner에 등록한 GameConfig를 찾아온다.
        EnemyManagerConfig config = f.FindAsset(f.RuntimeConfig.EnemyManagerConfig);
        SpawnEnemy(f, config.EnemyPrototype);
    }

    public void SpawnEnemy(Frame f, AssetRef<EntityPrototype> childPrototype) {
        //애너미 에셋을 불러옴
        EnemyManagerConfig config = f.FindAsset(f.RuntimeConfig.EnemyManagerConfig);
        //Config에 등록한 엔티티 프로토타입을 생성
        EntityRef enemy = f.Create(childPrototype);
        //해당 엔티티의 트랜스폼 정보를 불러옴
        Transform2D* enemyTransform = f.Unsafe.GetPointer<Transform2D>(enemy);

        //트랜스폼 정보로 위치, 회전값 변경
        enemyTransform->Position = GetRandomEdgePointOnCircle(f, config.EnemySpawnDistanceToCenter);
        enemyTransform->Rotation = GetRandomRotation(f);
        config.EnemyMoveSpeed = f.RNG->Next(2, 5);
    }

    public static FP GetRandomRotation(Frame f) {
        //결정론적인 랜덤 함수
        return f.RNG->Next(0, 360);
    }

    public static FPVector2 GetRandomEdgePointOnCircle(Frame f, FP radius) {
        //결정론적인 랜덤 함수
        return FPVector2.Rotate(FPVector2.Up * radius, f.RNG->Next() * FP.PiTimes2);
    }
}
