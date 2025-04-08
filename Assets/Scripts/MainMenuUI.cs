using ExitGames.Client.Photon;
using Photon.Client;
using Photon.Deterministic;
using Photon.Realtime;
using Quantum;
using Quantum.Prototypes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;


public class MainMenuUI : MonoBehaviour, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IInRoomCallbacks
{
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private GameObject loadingPanel;
    private RealtimeClient client;
    private long mapGuid = 0L;


    private void Start() {
        //startButton.onClick.AddListener(() => StartSingleGame());
        startButton.onClick.AddListener(() => OnQuickPlayClicked());
    }

    private void Update() {
        client?.Service();
    }

    public void OnQuickPlayClicked() {
        ConnectToMaster();
    }

   

    private bool ConnectToMaster() {
        var appSetting = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
        appSetting.FixedRegion = "kr";

        client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);
        client.AddCallbackTarget(this);

        if (string.IsNullOrEmpty(appSetting.AppIdQuantum.Trim())) {
            Debug.Log("appid 오류");
            return false;
        }

        if (!client.ConnectUsingSettings(appSetting)) {
            Debug.Log("unable to issue connect to master command");
            return false;
        }

        Debug.Log($"attempting to connect to region {appSetting.FixedRegion}");
        startButton.gameObject.SetActive(false);
        loadingPanel.SetActive(true);
        return true;
    }

    public void OnConnectedToMaster() {
        Debug.Log($"OnConnectedToMaster server is region: {client.CloudRegion}");

        client?.OpJoinRandomRoom();
    }

    public void OnJoinedLobby() {
        Debug.Log("OnJoinedLobby");
    }

    private void StartSingleGame() {
        var map = UnityEngine.Resources.Load<Map>("QuantumMap");
        var simulationConfig = UnityEngine.Resources.Load<SimulationConfig>("GameSimulationConfig");
        var systemConfig = UnityEngine.Resources.Load<SystemsConfig>("GameSystemsConfig");

        var runtimeConfig = new RuntimeConfig();
        runtimeConfig.Map.Id = map.Guid;
        runtimeConfig.SimulationConfig = simulationConfig;
        runtimeConfig.SystemsConfig = systemConfig;

        var playerPorototype = Resources.Load<EntityPrototype>("Prefabs/PlayerEntityPrototype");

        var playerData = new RuntimePlayer {
            PlayerAvatar = playerPorototype.Guid,
            PlayerNickname = client.LocalPlayer.NickName
        };

        var config = QuantumDeterministicSessionConfigAsset.Instance.Config;
        config.PlayerCount = 1;

        var startParams = new SessionRunner.Arguments {
            FrameData = null,
            RuntimeConfig = runtimeConfig,
            RecordingFlags = RecordingFlags.None,
            SessionConfig = config,
            Communicator = null,
            GameMode = DeterministicGameMode.Local,
            PlayerCount = 1,
        };

        QuantumRunner.StartGame(startParams);

        Debug.Log($"QuantumRunner created: {QuantumRunner.Default != null}");
        Debug.Log($"QuantumRunner running? {QuantumRunner.Default?.IsRunning}");


        StartCoroutine(WaitAndLoadScene(playerData));
    }

    private void StartGame() {
        var map = UnityEngine.Resources.Load<Map>("QuantumMap");
        var simulationConfig = UnityEngine.Resources.Load<SimulationConfig>("GameSimulationConfig");
        var systemConfig = UnityEngine.Resources.Load<SystemsConfig>("GameSystemsConfig");

        var runtimeConfig = new RuntimeConfig();
        runtimeConfig.Map.Id = map.Guid;
        runtimeConfig.SimulationConfig = simulationConfig;
        runtimeConfig.SystemsConfig = systemConfig;

        var playerPorototype = Resources.Load<EntityPrototype>("Prefabs/PlayerEntityPrototype");

        var playerData = new RuntimePlayer {
            PlayerAvatar = playerPorototype.Guid,
            PlayerNickname = client.LocalPlayer.NickName
        };

        Debug.Log($"PlayerAvater : {playerData.PlayerAvatar}");


        var sessionConfig = Resources.Load<QuantumDeterministicSessionConfigAsset>("SessionConfig");
        sessionConfig.Config.PlayerCount = 2;

        var startParams = new SessionRunner.Arguments {
            FrameData = null,
            RunnerFactory = QuantumRunnerUnityFactory.DefaultFactory,
            GameParameters = QuantumRunnerUnityFactory.CreateGameParameters,
            ClientId = client.UserId,
            RuntimeConfig = runtimeConfig,
            RecordingFlags = RecordingFlags.None,
            SessionConfig = sessionConfig.Config,
            Communicator = new QuantumNetworkCommunicator(client),
            GameMode = DeterministicGameMode.Multiplayer,
            PlayerCount = 2,
            StartGameTimeoutInSeconds = 10,
        };

        try {
            QuantumRunner.StartGame(startParams);
            Debug.Log("StartGame 호출 완료");
        } catch (Exception ex) {
            Debug.LogError($"StartGame 예외 발생: {ex}");
        }

        Debug.Log($"QuantumRunner created: {QuantumRunner.Default != null}");
        Debug.Log($"QuantumRunner running? {QuantumRunner.Default?.IsRunning}");
        Debug.Log($"Photon client state: {client.State}");
        Debug.Log($"Room player count: {client.CurrentRoom?.PlayerCount}");

        StartCoroutine(WaitAndLoadScene(playerData));
    }

    private IEnumerator WaitAndLoadScene(RuntimePlayer playerData) {
        while (QuantumRunner.Default == null)
            yield return null;
       
        while (!QuantumRunner.Default.IsRunning)
            yield return null;


        QuantumRunner.Default.Game.AddPlayer(client.LocalPlayer.ActorNumber - 1, playerData);
        Debug.Log($"플레이어 아바타 추가. 인덱스 {client.LocalPlayer.ActorNumber - 1}");


        yield return new WaitForSeconds(0.5f);

        UnityEngine.Debug.Log("QuantumRunner 실행됨");
        SceneManager.LoadScene("GameScene");
    }

    public void OnJoinedRoom() {
        int actorNumber = client.LocalPlayer.ActorNumber;
        string nickName = $"Player{actorNumber}";
        client.LocalPlayer.NickName = nickName;
        Debug.Log($"Joined room successfully! my name : {nickName}");
        if (client.CurrentRoom.PlayerCount > 1) {
            Debug.Log("game start");
            StartGame();
        }
        else
            StartCoroutine(MatchingCoroutine());
    }

    private IEnumerator MatchingCoroutine() {
        while (true) {
            try {
                Debug.Log("Matching...");
            } catch (Exception ex) {
                Debug.LogError($"코루틴 예외 발생: {ex.Message}");
            }

            yield return new WaitForSeconds(3f);
        }
    }

    public void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log($"join room failed : {message}");
        RoomOptions options = new RoomOptions {
            MaxPlayers = 2,
        };
        

        client.OpCreateRoom(new EnterRoomArgs { RoomOptions = options });
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        Debug.Log($"플레이어 입장: {newPlayer.NickName}");

        if (client.CurrentRoom.PlayerCount > 1) {
            Debug.Log("매치 완료! 게임 시작");
            StopAllCoroutines();
            StartGame();
        }
    }
    public void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("No room, creat room...");
        client?.OpCreateRoom(new EnterRoomParams());
    }
    public void OnLeftRoom() {
    }
    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
    }
    public void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged) {
    }
    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, PhotonHashtable changedProps) {
    }
    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) {
    }
    public void OnConnected() {
        Debug.Log($"OnConnected UserId: {client.UserId}");
    }
    public void OnCustomAuthenticationFailed(string debugMessage) {
        Debug.Log($"OnCustomAuthenticationFailed");

    }
    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
        Debug.Log($"OnCustomAuthenticationResponse");
    }
    public void OnDisconnected(DisconnectCause cause) {
        Debug.Log($"OnDisconnected cause: {cause}");
    }
    public void OnRegionListReceived(RegionHandler regionHandler) {
        Debug.Log($"OnRegionListReceived");
    }
    public void OnLeftLobby() {
    }
    public void OnRoomListUpdate(List<RoomInfo> roomList) {
    }
    public void OnLobbyStatisticsUpdate(List<TypedLobbyInfo> lobbyStatistics) {
    }
    public void OnFriendListUpdate(List<FriendInfo> friendList) {
    }
    public void OnCreatedRoom() {
        Debug.Log("room create successfully!");
    }
    public void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("create room failed");
    }
}
