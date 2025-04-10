using Quantum;
using UnityEngine.Scripting;

[Preserve]
public unsafe class PlayerSpawnSystem : SystemSignalsOnly, ISignalOnPlayerAdded {
    public void OnPlayerAdded(Frame frame, PlayerRef player, bool firstTime) {
        Log.Debug($"OnPlayerAdded , PlayerRef : {player}");

        EntityPrototype playerPrototype = UnityEngine.Resources.Load<EntityPrototype>("Prefabs/PlayerEntityPrototype");
        var playerEntity = frame.Create(playerPrototype);

        frame.Add(playerEntity, new PlayerComponent { PlayerRef = player });

        frame.Global->PlayerCount++;
    }
}