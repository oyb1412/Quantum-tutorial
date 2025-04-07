using UnityEngine.Scripting;
using Photon.Deterministic;
using Quantum;

[Preserve]
public unsafe class BoundarySystem : SystemMainThreadFilter<BoundarySystem.Filter> {
    public struct Filter {
        public EntityRef Entity;
        public Transform2D* Transform;
    }

    public override void Update(Frame f, ref Filter filter) {
        if (IsOutOfBounds(filter.Transform->Position, new FPVector2(15, 15), out FPVector2 newPosition)) {
            //�������� ������ ����
            filter.Transform->Position = newPosition;
            //�����̵� �ߴٴ� ����� ���ҿ� ��������� ����
            filter.Transform->Teleport(f, newPosition);
        }
    }

    public bool IsOutOfBounds(FPVector2 position, FPVector2 mapExtends, out FPVector2 newPosition) {
        newPosition = position;

        if (position.X >= -mapExtends.X && position.X <= mapExtends.X &&
            position.Y >= -mapExtends.Y && position.Y <= mapExtends.Y) {
            return false;
        }

        if (position.X < -mapExtends.X) {
            newPosition.X = mapExtends.X;
        } else if (position.X > mapExtends.X) {
            newPosition.X = -mapExtends.X;
        }

        if (position.Y < -mapExtends.Y) {
            newPosition.Y = mapExtends.Y;
        } else if (position.Y > mapExtends.Y) {
            newPosition.Y = -mapExtends.Y;
        }

        return true;
    }
}
