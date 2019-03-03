using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class AmmoSpawnManager : MonoBehaviour {

	public GameObject Ammo;
	public GameObject TNT;
	[HideInInspector]
	public GameObject MyAmo;
	[HideInInspector]
	public GameObject MyTNT;
	public float Timer = 0;
	public Vector3[] Spawnpoints;
	// Update is called once per frame
	void Update () {
		Timer += Time.deltaTime;
		if(Timer >= 5f){
			Timer = 0;
			int randomn = Random.Range(0, 5);
			int DropPicker = Random.Range(0,3);
			if(PhotonNetwork.IsMasterClient){
				if(DropPicker <=1){
					MyAmo = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "amo"),Spawnpoints[randomn] ,this.transform.rotation , 0);
				}else{
					MyTNT = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "tnt"),Spawnpoints[randomn] ,this.transform.rotation , 0);
				}
			}
		}
}
}
