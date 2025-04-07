using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class EnemyCollisionsSystem : SystemSignalsOnly, ISignalOnCollisionEnter2D {
    public void OnCollisionEnter2D(Frame f, CollisionInfo2D info) {
        if (f.Unsafe.TryGetPointer<PlayerProjectile>(info.Entity, out var projectile)) {
            //발사체의 충돌 처리
            if (f.Unsafe.TryGetPointer<Player>(info.Other, out var ship)) {
                f.Signals.OnCollisionProjectileHitPlayer(info, projectile, ship);
                //발사체와 플레이어가 충돌했을 때
            } else if (f.Unsafe.TryGetPointer<EnemysEnemy>(info.Other, out var asteroid)) {
                f.Signals.OnCollisionProjectileHitEnemy(info, projectile, asteroid);
                //발사체와 적이 충돌했을 때
            }
        }

        else if (f.Unsafe.TryGetPointer<Player>(info.Entity, out var ship)) {
            //플레이어의 충돌 처리
            if (f.Unsafe.TryGetPointer<EnemysEnemy>(info.Other, out var asteroid)) {
                f.Signals.OnCollisionEnemyHitPlayer(info, ship, asteroid);
                //플레이어와 적이 충돌했을 때
            }
        }
    }
}
