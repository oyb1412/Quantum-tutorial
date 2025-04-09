using ExitGames.Client.Photon;
using Photon.Client;
using Photon.Realtime;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Diagnostics;

public class MatchStarter : MonoBehaviour, IConnectionCallbacks, IMatchmakingCallbacks, IOnEventCallback
{
    private string playerName;

    private QuantumLoadBalancingClient client;

    private long mapGuid;

    enum PhotonEventCode : byte {
        StartGame = 110,
    }

    private void Start() {
        ConnectToMaster();
    }
    private void Update() {
        client?.Service();

        if (UnityEngine.Input.GetKeyUp(KeyCode.Space)) {
            StartGame();
        }
    }
    private bool ConnectToMaster() {
        var appSetting = PhotonServerSettings.CloneAppSettings(PhotonServerSettings.Instance.AppSettings);
        appSetting.FixedRegion = "kr";

        client = new QuantumLoadBalancingClient(PhotonServerSettings.Instance.AppSettings.Protocol);

        client.AddCallbackTarget(this);

        if(string.IsNullOrEmpty(appSetting.AppIdQuantum.Trim())) {
            Debug.Log("Missing Quantum AppID");
            return false;
        }

        if (!client.ConnectUsingSettings(appSetting)) {
            Debug.Log("Unable to issue Connect to Master command");
            return false;
        }

        Debug.Log($"Connect to region {appSetting.FixedRegion}");
        return true;
    }

    private EnterRoomParams CreateEnterRoomParams(string roomName) {
        EnterRoomParams enterRoomParams = new EnterRoomParams();

        enterRoomParams.RoomOptions = new RoomOptions();
        enterRoomParams.RoomName = roomName;
        enterRoomParams.RoomOptions.IsVisible = true;
        enterRoomParams.RoomOptions.MaxPlayers = 1;
        enterRoomParams.RoomOptions.CustomRoomProperties = new ExitGames.Client.Photon.Hashtable {
            {"Map", mapGuid },
        };

        enterRoomParams.RoomOptions.PlayerTtl = PhotonServerSettings.Instance.PlayerTtlInSeconds * 1000;
        enterRoomParams.RoomOptions.EmptyRoomTtl = PhotonServerSettings.Instance.EmptyRoomTtlInSeconds * 1000;

        return enterRoomParams;

    }

    private void JoinRandomRoomorCreateRoom() {
        var MapResources = UnityEngine.Resources.Load<Map>("QuantumMap");
        mapGuid = MapResources.AssetObject.Guid.Value;

        Debug.Log($"mapGuid : {MapResources.AssetObject.Guid}");

        EnterRoomParams enterRoomParams = CreateEnterRoomParams("");

        OpJoinRandomRoomParams joinRandomParams = new OpJoinRandomRoomParams();

        if(!client.OpJoinRandomOrCreateRoom(joinRandomParams, enterRoomParams)) {

            Debug.Log("Fail Join or Create Random Room");
            return;
        }

        Debug.Log("Success Join or Create Random Room");
    }

    private void StartGame() {
        if(!client.OpRaiseEvent((byte)PhotonEventCode.StartGame, null, new RaiseEventArgs{ Receivers = ReceiverGroup.All}, Photon.Client.SendOptions.SendReliable)) {
            Debug.Log("Fail Start Game");
        }
    }

    private void StartQuantumGame() {
        if(QuantumRunner.Default != null) {
            Debug.Log($"Another Quantum Runner : {QuantumRunner.Default.name}");
            return;
        }

        RuntimeConfig runtimeConfig = new RuntimeConfig();
        runtimeConfig.Map.Id = mapGuid;

        //var param = new QuantumRunner.StartParameters {
        //    RuntimeConfig = runtimeConfig,
        //    DeterministicConfig = QuantumDeterministicSessionConfigAsset.Instance.Config,
        //    GameMode = Photon.Deterministic.DeterministicGameMode.Multiplayer,
        //    FrameData = null,
        //    InitialFrame = 0,
        //    PlayerCount = client.CurrentRoom.MaxPlayers,
        //    NetworkClient = client,
        //    StartGameTimeoutInSeconds = 10.0f,
        //};
        //var clientId = $"client{client.LocalPlayer.ActorNumber - 1}";

        //QuantumRunner.StartGame(clientId, param);

        var comu = new QuantumNetworkCommunicator(client);
        var args = new SessionRunner.Arguments {
            RuntimeConfig = runtimeConfig,
            SessionConfig = QuantumDeterministicSessionConfigAsset.Instance.Config,
            GameMode = Photon.Deterministic.DeterministicGameMode.Local,
            FrameData = null,
            InitialFrame = 0,
            PlayerCount = client.CurrentRoom.MaxPlayers,
            Communicator = comu,
            StartGameTimeoutInSeconds = 10.0f,
        };

        QuantumRunner.StartGame(args);

    }

    public void OnConnected() {
        Debug.Log("OnConnected");
    }

    public void OnConnectedToMaster() {
        Debug.Log("OnConnectedToMaster");

        JoinRandomRoomorCreateRoom();
    }

    public void OnJoinedRoom() {
        Debug.Log("OnJoinedRoom");

    }

    public void OnDisconnected(DisconnectCause cause) {
        Debug.Log("OnDisconnected");

    }

    public void OnRegionListReceived(RegionHandler regionHandler) {
        Debug.Log("OnRegionListReceived");
    }

    public void OnCustomAuthenticationResponse(Dictionary<string, object> data) {
        Debug.Log("OnCustomAuthenticationResponse");
    }

    public void OnCustomAuthenticationFailed(string debugMessage) {
        Debug.Log("OnCustomAuthenticationFailed");
    }

    public void OnFriendListUpdate(List<FriendInfo> friendList) {
        Debug.Log("OnFriendListUpdate");

    }

    public void OnCreatedRoom() {
        Debug.Log("OnCreatedRoom");
    }

    public void OnCreateRoomFailed(short returnCode, string message) {
        Debug.Log("OnCreateRoomFailed");
    }



    public void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("OnJoinRoomFailed");
    }

    public void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log("OnJoinRandomFailed");
    }

    public void OnLeftRoom() {
        Debug.Log("OnLeftRoom");
    }

    public void OnEvent(EventData photonEvent) {
        Debug.Log($"PhotonEevent received code {photonEvent.Code}");

        switch(photonEvent.Code) {
            case (byte)PhotonEventCode.StartGame:
                client.CurrentRoom.CustomProperties.TryGetValue("Map", out object mapGuidValue);

                if(mapGuidValue ==  null) {
                    Debug.Log("Failed to get map Guid");
                    client.Disconnect();
                    return;
                }

                if(client.LocalPlayer.IsMasterClient) {
                    client.CurrentRoom.IsVisible = false;
                    client.CurrentRoom.IsOpen = false;
                }

                StartQuantumGame();
                break;
        }
    }
}
