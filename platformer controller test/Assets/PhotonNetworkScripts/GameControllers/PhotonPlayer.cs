﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using System.IO;

public class PhotonPlayer : MonoBehaviour {


	private PhotonView PV;
	public GameObject myAvatar;

	// Use this for initialization
	void Start () {
		PV = GetComponent<PhotonView>();
		int spawnPicker = Random.Range(0, GameSetup.GS.spawnPoints.Length);
		if(PV.IsMine){
			myAvatar = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Player"),
			 GameSetup.GS.spawnPoints[spawnPicker].position, GameSetup.GS.spawnPoints[spawnPicker].rotation, 0);

		}
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
