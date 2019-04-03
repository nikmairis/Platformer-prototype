using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
namespace testprojekts{
public class ShootHook : MonoBehaviour {

	public GameObject ShootPoint;
	public GameObject Hook;
	[HideInInspector]
	public bool HookReady = true;
	private PhotonView PV;


	// Use this for initialization
	void Start () {
		PV = this.GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(PV.IsMine){
		if(Input.GetKeyDown(KeyCode.E) && HookReady == true){
		HookReady = false;
		Vector3 ShootDirection = this.gameObject.GetComponent<AvatarCombat>().rayOrigin.transform.rotation.eulerAngles - new Vector3(0, 0, 90);
		Vector3 SendPosition =  this.gameObject.GetComponent<AvatarCombat>().rayOrigin.transform.position;
		SendPosition.z = 0;
		PV.RPC("RPC_HookShoot", RpcTarget.All, ShootDirection, SendPosition);
		//GameObject myHook = Instantiate(Hook, SendPosition, Quaternion.Euler(ShootDirection));
		//myHook.GetComponent<HookTravel>().MyChar = this.transform.gameObject;
		}
		}
	}

	[PunRPC]
	void RPC_HookShoot(Vector3 ShootDirection, Vector3 Position, PhotonMessageInfo info){
		GameObject myHook = Instantiate(Hook, Position, Quaternion.Euler(ShootDirection));
		myHook.GetComponent<HookTravel>().MyChar = this.transform.gameObject;
	}
}
}