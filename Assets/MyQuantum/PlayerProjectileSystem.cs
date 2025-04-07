using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;

[Preserve]
//발사체는 항상 갱신되며 자동소멸을 계산해야 하기 때문에, SystemMainThreadFilter를 상속받아 Update를 사용할 수 있게 한다.
public unsafe class PlayerProjectileSystem : SystemMainThreadFilter<PlayerProjectileSystem.Filter>, ISignalPlayerShoot, ISignalOnCollisionProjectileHitEnemy {
    //필터에 발사체 컴포넌트를 부착해 발사체만 적용받도록 한다.
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

        //실제 프로토타입의 정보들을 매개변수를 바탕으로 가져온다.
        EntityRef projectileEntity = f.Create(projectilePrototype);
        Transform2D* projectileTransform = f.Unsafe.GetPointer<Transform2D>(projectileEntity);
        Transform2D* ownerTransform = f.Unsafe.GetPointer<Transform2D>(owner);

        //위치 및 방향 조정
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
