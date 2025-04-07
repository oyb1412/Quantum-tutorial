using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;


[Preserve] // 항상 빌드에서 제외되지 않기 위해 사용

//포인터를 사용하기 위해 unsafe 키워드를 사용
public unsafe class MyPlayerSystem : SystemMainThreadFilter<MyPlayerSystem.Filter>, ISignalOnCollisionEnemyHitPlayer {
    //이 필터를 통과하는 엔티티에게만 Update 함수를 적용

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public PhysicsBody2D* Body;
        public Player* Player;
    }
    public override void Update(Frame f, ref Filter filter) {
        Input* input = default;

        //각 플레이어에게 개별적으로 부여되는 PlayerRef값을 토대로 플레이어의 인풋값을 가져온다.
        if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            input = f.GetPlayerInput(playerLink->PlayerRef);
        
        //받아온 인풋 값을 토대로 함수 작동
        UpdateMovement(f, ref filter, input);

        UpdateFire(f, ref filter, input);
    }

    public void OnCollisionEnemyHitPlayer(Frame f, CollisionInfo2D info, Player* ship, EnemysEnemy* asteroid) {
        f.Destroy(info.Entity);
    }

    private void UpdateFire(Frame f, ref Filter filter, Input* input) {
        //전역 등록해둔 플레이어 config를 가져온다.
        var config = f.FindAsset(filter.Player->MyPlayerConfig);

        //발사 입력이 들어왔고, 현재 발사 가능한 상태라면
        //if (input->Fire && filter.Player->FireInterval <= 0) {
        //    //발사 간격 초기화
        //    filter.Player->FireInterval = config.FireInterval;

        //    //발사 위치 및 방향 조정
        //    var relativeOffset = FPVector2.Up * config.ShotOffset;
        //    var spawnPosition = filter.Transform->TransformPoint(relativeOffset);
        //    //시그널 호출
        //    f.Signals.PlayerShoot(filter.Entity, spawnPosition, config.ProjectilePrototype);
        //} else {
        //    //발사가 불가능한 상황이라면, 발사 간격 조정
        //    filter.Player->FireInterval -= f.DeltaTime;
        //}
    }

    private void UpdateMovement(Frame f, ref Filter filter, Input* input) {
        //정확한 계산을 위해 float대신 FP 사용
        var config = f.FindAsset(filter.Player->MyPlayerConfig);
        FP playerAcceleration = config.ShipAceleration;
        FP playerTurnSpeed = config.ShipTurnSpeed;

        if(input->Up)
            filter.Body->AddForce(filter.Transform->Up * playerAcceleration);

        if (input->Left)
            filter.Body->AddTorque(playerTurnSpeed);

        if(input->Right)
            filter.Body->AddTorque(-playerTurnSpeed);

        filter.Body->AngularVelocity = FPMath.Clamp(filter.Body->AngularVelocity, -playerTurnSpeed, playerTurnSpeed);
    }
}
