using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;

[Preserve]
//�߻�ü�� �׻� ���ŵǸ� �ڵ��Ҹ��� ����ؾ� �ϱ� ������, SystemMainThreadFilter�� ��ӹ޾� Update�� ����� �� �ְ� �Ѵ�.
public unsafe class PlayerProjectileSystem : SystemMainThreadFilter<PlayerProjectileSystem.Filter>, ISignalPlayerShoot, ISignalOnCollisionProjectileHitEnemy {
    //���Ϳ� �߻�ü ������Ʈ�� ������ �߻�ü�� ����޵��� �Ѵ�.
    public struct Filter {
        public EntityRef Entity;
        public PlayerProjectile* Projectile;
    }

    public override void Update(Frame f, ref Filter filter) {
        filter.Projectile->TTL -= f.DeltaTime;
        if (filter.Projectile->TTL <= 0) {
            f.Destroy(filter.Entity);
        }
    }

    public void OnCollisionProjectileHitEnemy(Frame f, CollisionInfo2D info, PlayerProjectile* projectile, EnemysEnemy* asteroid) {
        if (asteroid->ChildAsteroid != null) {
            f.Signals.SpawnEnemy(asteroid->ChildAsteroid, info.Other);
            f.Signals.SpawnEnemy(asteroid->ChildAsteroid, info.Other);
        }

        f.Destroy(info.Entity);
        f.Destroy(info.Other);
    }


    public void PlayerShoot(Frame f, EntityRef owner, FPVector2 spawnPosition, AssetRef<EntityPrototype> projectilePrototype) {

        //���� ������Ÿ���� �������� �Ű������� �������� �����´�.
        EntityRef projectileEntity = f.Create(projectilePrototype);
        Transform2D* projectileTransform = f.Unsafe.GetPointer<Transform2D>(projectileEntity);
        Transform2D* ownerTransform = f.Unsafe.GetPointer<Transform2D>(owner);

        //��ġ �� ���� ����
        projectileTransform->Rotation = ownerTransform->Rotation;
        projectileTransform->Position = spawnPosition;

        PlayerProjectile* projectile = f.Unsafe.GetPointer<PlayerProjectile>(projectileEntity);
        var config = f.FindAsset(projectile->ProjectileConfig);
        projectile->TTL = config.ProjectileTTL;
        projectile->Owner = owner;

        PhysicsBody2D* body = f.Unsafe.GetPointer<PhysicsBody2D>(projectileEntity);
        body->Velocity = ownerTransform->Up * config.ProjectileInitialSpeed;
    }

    
}
