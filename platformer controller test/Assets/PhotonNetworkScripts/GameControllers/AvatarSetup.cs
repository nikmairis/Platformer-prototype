using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AvatarSetup : MonoBehaviour {

	private PhotonView PV;
	public GameObject myCharacter;
	public int characterValue;
	// Use this for initialization
	void Start () {
		PV = GetComponent<PhotonView>();
		if(PV.IsMine){
			PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
		}
	}
	
	[PunRPC]
	void RPC_AddCharacter(int whichCharacter){
		characterValue = whichCharacter;
		myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, this.transform);
	}
}
