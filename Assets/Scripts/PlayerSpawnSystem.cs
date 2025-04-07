using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded {
    //ISignalOnPlayerAdded를 상속받아, 플레이어가 접속하면 해당 함수가 실행된다.
    public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime) {
        {
            //퀀텀 러너에서 플레이어 데이터를 받아온다.
            RuntimePlayer data = frame.GetPlayerData(player);

            //퀀텀 러너의 필드에 등록한 엔티티 프로토타입 데이터를 받아온다.
            var entityPrototypAsset = frame.FindAsset<EntityPrototype>(data.PlayerAvatar);

            //엔티티를 생성한다.
            var playerEntity = frame.Create(entityPrototypAsset);

            //PlayerLink 컴포넌트를 엔티티에 추가해 플레이어라는 점을 명확히 한다.
            frame.Add(playerEntity, new PlayerComponent { PlayerRef = player });

            if (frame.Global->PlayerCount == 0)
                frame.Global->PlayerList0 = playerEntity;
            else
                frame.Global->PlayerList1 = playerEntity;

            frame.Global->PlayerCount++;
        }
    }
}
