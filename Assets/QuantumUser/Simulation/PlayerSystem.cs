using Photon.Deterministic;
using UnityEngine;
using UnityEngine.Scripting;

namespace Quantum.Asteroids {

    [Preserve] // �׻� ���忡�� ���ܵ��� �ʱ� ���� ���

    //�����͸� ����ϱ� ���� unsafe Ű���带 ���
    public unsafe class PlayerSystem : SystemMainThreadFilter<PlayerSystem.Filter> {
        public override void Update(Frame f, ref Filter filter) {
            var input = f.GetPlayerInput(0);

            UpdateMovement(f, ref filter, input);
        }

        public struct Filter {
            public EntityRef Entity;
            public Transform2D* Transform;
            public PhysicsBody2D* Body;
        }

        private void UpdateMovement(Frame f, ref Filter filter, Input* input) {
            //��Ȯ�� ����� ���� float��� FP ���
            FP playerAcceleration = 7;
            FP playerTurnSpeed = 8;

            if(input->Up)
                filter.Body->AddForce(filter.Transform->Up * playerAcceleration);

            if (input->Left)
                filter.Body->AddTorque(playerTurnSpeed);

            if(input->Right)
                filter.Body->AddTorque(-playerTurnSpeed);

            filter.Body->AngularVelocity = FPMath.Clamp(filter.Body->AngularVelocity, -playerTurnSpeed, playerTurnSpeed);

        }

    }
}
