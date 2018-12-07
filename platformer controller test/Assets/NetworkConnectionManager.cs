using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using Photon.Realtime;
using UnityEngine.SceneManagement;

namespace testprojekts{

public class NetworkConnectionManager : MonoBehaviourPunCallbacks {


	public Button ConnectToMasterBtn;
	public Button ConnectToRoomBtn;

	public bool TriesToConnectToMaster;
	public bool TriesToConnectToRoom;
	// Use this for initialization
	void Start () {
		DontDestroyOnLoad(gameObject);
		TriesToConnectToMaster = false;
		TriesToConnectToRoom = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(ConnectToMasterBtn != null)
		ConnectToMasterBtn.gameObject.SetActive(!PhotonNetwork.IsConnected && !TriesToConnectToMaster);
		if(ConnectToRoomBtn != null)
		ConnectToRoomBtn.gameObject.SetActive(PhotonNetwork.IsConnected && !TriesToConnectToMaster && !TriesToConnectToRoom);
	}
	public void OnClickConnectToMaster(){
		PhotonNetwork.OfflineMode = false;
		PhotonNetwork.NickName = "TestPlayer";
		//PhotonNetwork.AutomaticallySyncScene = true;
		PhotonNetwork.GameVersion = "v1";

		TriesToConnectToMaster = true;

		PhotonNetwork.ConnectUsingSettings();
	}
	public override void OnDisconnected(Photon.Realtime.DisconnectCause cause){
		base.OnDisconnected(cause);
		TriesToConnectToMaster = false;
		TriesToConnectToRoom = false;
		Debug.Log(cause);
	}
	public override void OnConnectedToMaster(){
		base.OnConnectedToMaster();
		TriesToConnectToMaster = false;
		Debug.Log("You are now connected to master!");
	}
	public void OnClickConnectToRoom(){
		if(!PhotonNetwork.IsConnected)
			return;

			PhotonNetwork.JoinRandomRoom();
	}
	public override void OnJoinedRoom(){
		base.OnJoinedRoom();
		TriesToConnectToRoom = false;
		Debug.Log("Master: " + PhotonNetwork.IsMasterClient + " |Players in room: "+ PhotonNetwork.CurrentRoom.PlayerCount);
		SceneManager.LoadScene("Demo");
	}
	public override void OnJoinRandomFailed(short returnCode, string message){
		base.OnJoinRandomFailed(returnCode, message);
			PhotonNetwork.CreateRoom(null, new RoomOptions {MaxPlayers = 20});
	}
	public override void OnCreateRoomFailed(short returnCode, string message){
		base.OnCreateRoomFailed(returnCode, message);
		Debug.Log(message);
		TriesToConnectToRoom = false;
	}

}
}