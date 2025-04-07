using Photon.Client;
using Photon.Realtime;
using Quantum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour, IConnectionCallbacks, ILobbyCallbacks, IMatchmakingCallbacks, IInRoomCallbacks
{
    [SerializeField] private UnityEngine.UI.Button startButton;
    [SerializeField] private GameObject loadingPanel;

    private QuantumLoadBalancingClient client;

    private void Start() {
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

        if(string.IsNullOrEmpty(appSetting.AppIdQuantum.Trim())) {
            Debug.Log("appid 오류");
            return false;
        }

        if(!client.ConnectUsingSettings(appSetting, "name")) {
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

    public void OnJoinRoomFailed(short returnCode, string message) {
        Debug.Log("No room, creat room...");
        client?.OpCreateRoom(new EnterRoomParams());
    }

    public void OnJoinedRoom() {
        int actorNumber = client.LocalPlayer.ActorNumber;
        string nickName = $"Player{actorNumber}";
        client.LocalPlayer.NickName = nickName;
        Debug.Log($"Joined room successfully! my name : {nickName}");
        if (client.CurrentRoom.PlayerCount > 1) {
            Debug.Log("game start");
            SceneManager.LoadScene("GameScene");
        }
        else
            StartCoroutine(MatchingCoroutine());
    }

    private IEnumerator MatchingCoroutine() {
        while(true) {
            Debug.Log("Matching...");
            yield return new WaitForSeconds(3f);
        }
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

    

    public void OnJoinRandomFailed(short returnCode, string message) {
        Debug.Log($"join room failed : {message}");
        RoomOptions options = new RoomOptions {
            MaxPlayers = 2
        };

        client.OpCreateRoom(new EnterRoomParams { RoomOptions = options });
    }

    public void OnLeftRoom() {
    }

    public void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer) {
        Debug.Log($"플레이어 입장: {newPlayer.NickName}");

        if (client.CurrentRoom.PlayerCount > 1) {
            Debug.Log("매치 완료! 게임 시작");
            StopAllCoroutines();
            SceneManager.LoadScene("GameScene");
        }
    }

    public void OnPlayerLeftRoom(Photon.Realtime.Player otherPlayer) {
    }

    public void OnRoomPropertiesUpdate(PhotonHashtable propertiesThatChanged) {
    }

    public void OnPlayerPropertiesUpdate(Photon.Realtime.Player targetPlayer, PhotonHashtable changedProps) {
    }

    public void OnMasterClientSwitched(Photon.Realtime.Player newMasterClient) {
    }
}
