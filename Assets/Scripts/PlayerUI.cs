using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerUI : MonoBehaviour
{
    public TMPro.TextMeshProUGUI[] hpText;
    private EntityRef[] _playerEntityRef;
    private bool _initialized = false;

    private unsafe void TryInitializeUI() {
        var game = QuantumRunner.Default?.Game;
        if (game == null) return;

        var frame = game.Frames?.Predicted;
        if (frame == null) return;

        if (frame.Global->PlayerList0 != default && frame.Global->PlayerList1 != default) {
            _playerEntityRef[0] = frame.Global->PlayerList0;
            _playerEntityRef[1] = frame.Global->PlayerList1;

            Debug.Log("플레이어 UI 초기화 완료");

            _initialized = true;
        }
    }

    private unsafe void Start() {
        _playerEntityRef = new EntityRef[2];

        for (int i = 0; i < _playerEntityRef.Length; i++) {
            hpText[i].text = string.Empty;
        }
    }
  

    private unsafe void Update() {

        if (!_initialized)
            TryInitializeUI();
        else {
            var frame = QuantumRunner.Default.Game.Frames.Predicted;

            for (int i = 0; i < _playerEntityRef.Length; i++) {
                if (frame.Unsafe.TryGetPointer<PlayerComponent>(_playerEntityRef[i], out var player)) {
                    hpText[i].text = $"Player {i} HP: {player->CurrentPlayerHp}";
                }
            }
        }
    }
}
