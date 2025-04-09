using Quantum;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUIManager : MonoBehaviour {
    public GameObject playerInfoPanelPrefab;
    public Transform uiParent;

    private Dictionary<int, GameObject> playerUIMap = new();
     
    private unsafe void Update() {
        var frame = QuantumRunner.Default.Game.Frames.Verified;
        foreach (var player in frame.Unsafe.GetComponentBlockIterator<PlayerComponent>()) {

            var playerComponent = frame.Unsafe.GetPointer<PlayerComponent>(player.Entity);

            if (!playerUIMap.ContainsKey(playerComponent->PlayerRef)) {
                GameObject panel = Instantiate(playerInfoPanelPrefab, uiParent);
                playerUIMap[playerComponent->PlayerRef] = panel;
            }

            GameObject ui = playerUIMap[playerComponent->PlayerRef];
            var nameText = ui.transform.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[0];
            var hpText = ui.transform.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[1];
            var killText = ui.transform.GetComponentsInChildren<TMPro.TextMeshProUGUI>()[2];

            nameText.text = $"Player{playerComponent->PlayerRef._index}";
            hpText.text = $"HP :  {playerComponent->CurrentPlayerHp} / {playerComponent->MaxPlayerHp}";
            killText.text = $"Kill :  {playerComponent->KillCount}";

        }
    }
}
