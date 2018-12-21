using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.IO;

public class PhotonRoom : MonoBehaviourPunCallbacks, IInRoomCallbacks {

	public static PhotonRoom room;
	private PhotonView PV; 

	//Room info
	public bool IsGameLoaded;
	public int currentScene;

	//Player info
	Player[] photonPlayers;
	public int playersInRoom;
	public int myNumberInRoom;
	public int playerInGame;

	//Delayed start
	private bool readyToCount;
	private bool readyToStart;
	public float startingTime;
	private float lessThanMaxPlayers;
	private float atMaxPlayers;
	private float timeToStart;

	void Awake(){
		if(PhotonRoom.room == null){
			PhotonRoom.room = this;
		}
		else{
			if(PhotonRoom.room != this){
				Destroy(PhotonRoom.room.gameObject);
				PhotonRoom.room = this;
			}
		}
		DontDestroyOnLoad(this.gameObject);
	}

	public override void OnEnable(){
		base.OnEnable();
		PhotonNetwork.AddCallbackTarget(this);
		SceneManager.sceneLoaded += OnSceneFinishedLoading;
	}

	public override void OnDisable(){
		base.OnDisable();
		PhotonNetwork.RemoveCallbackTarget(this);
		SceneManager.sceneLoaded -= OnSceneFinishedLoading;
	}

	// Use this for initialization
	void Start () {
		PV = GetComponent<PhotonView>();
		readyToCount = false;
		readyToStart = false;
		lessThanMaxPlayers = startingTime;
		atMaxPlayers = 6;
		timeToStart = startingTime;
	}
	
	// Update is called once per frame
	void Update () {
		if(MultiplayerSettings.multiplayerSettings.delayStart){
			if(playersInRoom == 1){
				RestartTimer();
			}
			if(!IsGameLoaded){
				if(readyToStart){
					atMaxPlayers -= Time.deltaTime;
					lessThanMaxPlayers = atMaxPlayers;
					timeToStart = atMaxPlayers;
				}
				else if(readyToCount){
					lessThanMaxPlayers -= Time.deltaTime;
					timeToStart = lessThanMaxPlayers;
				}
				Debug.Log("Display time to start to player : " + timeToStart);
				if(timeToStart <= 0){
					StartGame();
				}
			}
		}
	}

		// Triggers when player joins a room
	public override void OnJoinedRoom(){
		base.OnJoinedRoom();
		Debug.Log("We are now in a room");
		photonPlayers = PhotonNetwork.PlayerList;
		playersInRoom = photonPlayers.Length;
		myNumberInRoom = playersInRoom;
		PhotonNetwork.NickName = myNumberInRoom.ToString();
		if(MultiplayerSettings.multiplayerSettings.delayStart)
		{
			Debug.Log("Players in room out of players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
			if(playersInRoom >1){
				readyToCount = true;
			}
			if(playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers){
				readyToStart = true;
				if(!PhotonNetwork.IsMasterClient)
				return;
				PhotonNetwork.CurrentRoom.IsOpen = false;
			}
		}
		else{
			StartGame();
		}
	}

	public override void OnPlayerEnteredRoom(Player newPlayer){
		base.OnPlayerEnteredRoom(newPlayer);
		Debug.Log("A new player has entered a room");
		photonPlayers = PhotonNetwork.PlayerList;
		playersInRoom++;
		if(MultiplayerSettings.multiplayerSettings.delayStart){
			Debug.Log("Players in room out of players possible (" + playersInRoom + ":" + MultiplayerSettings.multiplayerSettings.maxPlayers + ")");
			if(playersInRoom > 1){
				readyToCount = true;
			}
			if (playersInRoom == MultiplayerSettings.multiplayerSettings.maxPlayers){
				readyToStart = true;
				if(!PhotonNetwork.IsMasterClient)
				return;
				PhotonNetwork.CurrentRoom.IsOpen = false;
			}
		}
	}
	void StartGame(){
		IsGameLoaded = true;
		if(!PhotonNetwork.IsMasterClient)
		return;
		if(MultiplayerSettings.multiplayerSettings.delayStart){
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}
		PhotonNetwork.LoadLevel(MultiplayerSettings.multiplayerSettings.multiplayerScene);
	}
	void RestartTimer(){
		lessThanMaxPlayers = startingTime;
		timeToStart = startingTime;
		atMaxPlayers = 6;
		readyToCount = false;
		readyToStart = false;
	}
	void OnSceneFinishedLoading(Scene scene, LoadSceneMode mode){
		currentScene = scene.buildIndex;
		if(currentScene == MultiplayerSettings.multiplayerSettings.multiplayerScene){
			IsGameLoaded = true;

			if(MultiplayerSettings.multiplayerSettings.delayStart){
				PV.RPC("RPC_LoadedGameScene", RpcTarget.MasterClient);
			}else{
				RPC_CreatePlayer();
			}
		}
	}
	[PunRPC]
	private void RPC_LoadedGameScene(){
		playerInGame++;
		if(playerInGame == PhotonNetwork.PlayerList.Length){
			PV.RPC("RPC_CreatePlayer", RpcTarget.All);
		}
	}

	[PunRPC]
	private void RPC_CreatePlayer(){
		PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "PhotonNetworkPlayer"), transform.position, Quaternion.identity, 0);
	}
}
