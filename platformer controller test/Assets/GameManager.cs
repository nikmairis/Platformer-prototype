using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace testprojekts{
public class GameManager : MonoBehaviourPunCallbacks {
	[Header("Platformer Prototype Game Manager")]
	public Player PlayerPrefab;
	//[HideInInspector]
	public Player LocalPlayer;
public Text text;
	public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer){
		base.OnPlayerEnteredRoom(newPlayer);
		Player.RefreshInstance(ref LocalPlayer, PlayerPrefab);
	}
	private void Awake(){
		if(!PhotonNetwork.IsConnected){
			SceneManager.LoadScene("MainMenu");
			return;
		}
	}
	// Use this for initialization
	void Start () {
		Player.RefreshInstance(ref LocalPlayer, PlayerPrefab);
	}
	
	// Update is called once per frame
	void Update () {
		text.text = PhotonNetwork.CurrentRoom.PlayerCount.ToString();
	}
}
}