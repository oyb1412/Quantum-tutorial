using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded {
    //ISignalOnPlayerAdded�� ��ӹ޾�, �÷��̾ �����ϸ� �ش� �Լ��� ����ȴ�.
    public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime) {
        {
            //���� ���ʿ��� �÷��̾� �����͸� �޾ƿ´�.
            RuntimePlayer data = frame.GetPlayerData(player);

            //���� ������ �ʵ忡 ����� ��ƼƼ ������Ÿ�� �����͸� �޾ƿ´�.
            var entityPrototypAsset = frame.FindAsset<EntityPrototype>(data.PlayerAvatar);

            //��ƼƼ�� �����Ѵ�.
            var playerEntity = frame.Create(entityPrototypAsset);

            //PlayerLink ������Ʈ�� ��ƼƼ�� �߰��� �÷��̾��� ���� ��Ȯ�� �Ѵ�.
            frame.Add(playerEntity, new PlayerComponent { PlayerRef = player });

            if (frame.Global->PlayerCount == 0)
                frame.Global->PlayerList0 = playerEntity;
            else
                frame.Global->PlayerList1 = playerEntity;

            frame.Global->PlayerCount++;
        }
    }
}
