using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class CollisionsSystem : SystemSignalsOnly, ISignalOnTriggerEnter2D {
   

    public void OnTriggerEnter2D(Frame f, TriggerInfo2D info) {
        if (f.Unsafe.TryGetPointer<PlayerComponent>(info.Entity, out var player)) {

            if (f.Unsafe.TryGetPointer<EnemyBoundComponent>(info.Other, out var enemyBound)) {
                f.Signals.OnTriggerEnemyBoundHitPlayer(info, enemyBound, player);
                //적 공격에 플레이어 충돌
            }
        }
        if (f.Unsafe.TryGetPointer<EnemyComponent>(info.Entity, out var enemy)) {

            if (f.Unsafe.TryGetPointer<PlayerBoundComponent>(info.Other, out var playerBound)) {
                f.Signals.OnTriggerPlayerBoundHitEnemy(info, playerBound, enemy);
                //플레이어 공격에 적 충돌
            }
        }
    }
}
