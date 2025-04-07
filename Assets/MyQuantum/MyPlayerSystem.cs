using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;


[Preserve] // �׻� ���忡�� ���ܵ��� �ʱ� ���� ���

//�����͸� ����ϱ� ���� unsafe Ű���带 ���
public unsafe class MyPlayerSystem : SystemMainThreadFilter<MyPlayerSystem.Filter>, ISignalOnCollisionEnemyHitPlayer {
    //�� ���͸� ����ϴ� ��ƼƼ���Ը� Update �Լ��� ����

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public PhysicsBody2D* Body;
        public Player* Player;
    }
    public override void Update(Frame f, ref Filter filter) {
        Input* input = default;

        //�� �÷��̾�� ���������� �ο��Ǵ� PlayerRef���� ���� �÷��̾��� ��ǲ���� �����´�.
        if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerLink* playerLink))
            input = f.GetPlayerInput(playerLink->PlayerRef);
        
        //�޾ƿ� ��ǲ ���� ���� �Լ� �۵�
        UpdateMovement(f, ref filter, input);

        UpdateFire(f, ref filter, input);
    }

    public void OnCollisionEnemyHitPlayer(Frame f, CollisionInfo2D info, Player* ship, EnemysEnemy* asteroid) {
        f.Destroy(info.Entity);
    }

    private void UpdateFire(Frame f, ref Filter filter, Input* input) {
        //���� ����ص� �÷��̾� config�� �����´�.
        var config = f.FindAsset(filter.Player->MyPlayerConfig);

        //�߻� �Է��� ���԰�, ���� �߻� ������ ���¶��
        //if (input->Fire && filter.Player->FireInterval <= 0) {
        //    //�߻� ���� �ʱ�ȭ
        //    filter.Player->FireInterval = config.FireInterval;

        //    //�߻� ��ġ �� ���� ����
        //    var relativeOffset = FPVector2.Up * config.ShotOffset;
        //    var spawnPosition = filter.Transform->TransformPoint(relativeOffset);
        //    //�ñ׳� ȣ��
        //    f.Signals.PlayerShoot(filter.Entity, spawnPosition, config.ProjectilePrototype);
        //} else {
        //    //�߻簡 �Ұ����� ��Ȳ�̶��, �߻� ���� ����
        //    filter.Player->FireInterval -= f.DeltaTime;
        //}
    }

    private void UpdateMovement(Frame f, ref Filter filter, Input* input) {
        //��Ȯ�� ����� ���� float��� FP ���
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
