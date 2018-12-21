using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class PhotonLobby : MonoBehaviourPunCallbacks {

	public static PhotonLobby lobby;
	public GameObject cancelButton;
	public GameObject battleButton;

	RoomInfo[] rooms;

	private void Awake(){
		lobby = this; // izveido singleton. Dzīvo main menu scene
	}

	// Use this for initialization
	void Start () {
		PhotonNetwork.ConnectUsingSettings(); // connects to master Photon Server
	}
	
	// Triggers when a player hits battleButton and connects to master server
	public override void OnConnectedToMaster(){
		Debug.Log("Player has connected to the Master Server!");
		PhotonNetwork.AutomaticallySyncScene = true;
		battleButton.SetActive(true);
	}

	// Executes when a player clicks the battleButton
	public void OnBattleButtonClicked(){
		PhotonNetwork.JoinRandomRoom();
		battleButton.SetActive(false);
		cancelButton.SetActive(true);
		Debug.Log("Battle Button has been clicked");
	}

	// Triggers when failed to create a room. Always triggets when no rooms are active
	public override void OnJoinRandomFailed(short returnCode, string message){
		Debug.Log("Tried to join a random room but failed. most likeley, there is no room to connect to!");
		CreateRoom();
	}

	// A constructor that creates a room if no others are active
	void CreateRoom(){
		int randomRoomName = Random.Range(0, 10000);
		RoomOptions roomOps = new RoomOptions(){IsVisible = true, IsOpen = true, MaxPlayers = (byte)MultiplayerSettings.multiplayerSettings.maxPlayers};
		PhotonNetwork.CreateRoom("Room"+ randomRoomName, roomOps);
		Debug.Log("Trying to create a new room");
	}



	// Triggers, if cant create a room. For example, if such a roomName already exists
	public override void OnCreateRoomFailed(short returnCode, string message){
		Debug.Log("Tried to create a new room, but failed. There most likely must already be a room with the same name");
		CreateRoom();
	}

	// triggers when the cancleButton is clicked
	public void OnCancleButtonClicked(){
		cancelButton.SetActive(false);
		battleButton.SetActive(true);
		PhotonNetwork.LeaveRoom();
		Debug.Log("Cancel button clicked");
	}
	// Update is called once per frame
	void Update () {
		
	}
}
