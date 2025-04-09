using Photon.Client;
using Photon.Deterministic;
using Quantum;
using UnityEngine;
using UnityEngine.SceneManagement;

public class QuantumSinglePlayerStarter : MonoBehaviour {

    [SerializeField] private UnityEngine.UI.Button startButton;

    private void Start() {
        startButton.onClick.AddListener(() => StartQuantumSession());
    }

    private void StartQuantumSession() {
        Debug.Log("[Quantum] �̱��÷��� ���� ����");
        var map = Resources.Load<Map>("QuantumMap");
        var simulationConfig = Resources.Load<SimulationConfig>("GameSimulationConfig");
        var systemsConfig = Resources.Load<SystemsConfig>("GameSystemsConfig");
        var sessionConfigAsset = Resources.Load<QuantumDeterministicSessionConfigAsset>("SessionConfig");

        var runtimeConfig = new RuntimeConfig {
            Map = { Id = map.Guid },
            SimulationConfig = simulationConfig,
            SystemsConfig = systemsConfig
        };

        var args = new SessionRunner.Arguments {
            GameMode = DeterministicGameMode.Local,
            RuntimeConfig = runtimeConfig,
            SessionConfig = sessionConfigAsset.Config,
            PlayerCount = 1,
            FrameData = null
        };

        QuantumRunner.StartGame(args);
        Debug.Log("[Quantum] QuantumRunner(Local) ���� �õ�");

        StartCoroutine(WaitAndAddPlayer());
    }

    private System.Collections.IEnumerator WaitAndAddPlayer() {
        while (QuantumRunner.Default == null || !QuantumRunner.Default.IsRunning)
            yield return null;

        var playerPrototype = Resources.Load<EntityPrototype>("Prefabs/PlayerEntityPrototype");

        var playerData = new RuntimePlayer {
            PlayerAvatar = playerPrototype.Guid,
            PlayerNickname = "Player1"
        };

        QuantumRunner.Default.Game.AddPlayer(0, playerData);

        Debug.Log("[Quantum] �̱� �÷��̾� ��� �Ϸ�");

        this.gameObject.SetActive(false);
    }
}
