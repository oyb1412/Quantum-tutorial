using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class EnemyCollisionsSystem : SystemSignalsOnly, ISignalOnCollisionEnter2D {
    public void OnCollisionEnter2D(Frame f, CollisionInfo2D info) {
        if (f.Unsafe.TryGetPointer<PlayerProjectile>(info.Entity, out var projectile)) {
            //�߻�ü�� �浹 ó��
            if (f.Unsafe.TryGetPointer<Player>(info.Other, out var ship)) {
                f.Signals.OnCollisionProjectileHitPlayer(info, projectile, ship);
                //�߻�ü�� �÷��̾ �浹���� ��
            } else if (f.Unsafe.TryGetPointer<EnemysEnemy>(info.Other, out var asteroid)) {
                f.Signals.OnCollisionProjectileHitEnemy(info, projectile, asteroid);
                //�߻�ü�� ���� �浹���� ��
            }
        }

        else if (f.Unsafe.TryGetPointer<Player>(info.Entity, out var ship)) {
            //�÷��̾��� �浹 ó��
            if (f.Unsafe.TryGetPointer<EnemysEnemy>(info.Other, out var asteroid)) {
                f.Signals.OnCollisionEnemyHitPlayer(info, ship, asteroid);
                //�÷��̾�� ���� �浹���� ��
            }
        }
    }
}
