using Photon.Deterministic;
using Quantum;
using UnityEngine.Scripting;


[Preserve] 

public unsafe class PlayerController : SystemMainThreadFilter<PlayerController.Filter> {

    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
        public PhysicsBody2D* Body;
        public PlayerComponent* Player;
    }
    public override void Update(Frame f, ref Filter filter) {
        Input* input = default;

        if (f.Unsafe.TryGetPointer(filter.Entity, out PlayerComponent* playerComponent)) {
            input = f.GetPlayerInput(0);
            UpdateMovement(f, ref filter, input);
        }
    }
    private void UpdateMovement(Frame f, ref Filter filter, Input* input) {
        FP playerMoveSpeed = 10;

        FPVector2 direction = FPVector2.Zero;

        if (input->Up) direction += filter.Transform->Up;
        if (input->Down) direction -= filter.Transform->Up;
        if (input->Right) direction += filter.Transform->Right;
        if (input->Left) direction -= filter.Transform->Right;

        if (direction.Magnitude > 0)
            direction = direction.Normalized;

        filter.Body->Velocity = direction * playerMoveSpeed;
    }
}
