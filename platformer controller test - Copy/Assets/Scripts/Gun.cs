using System.Collections;
using System.Collections.Generic;
using testprojekts;
using UnityEngine;
using Photon.Pun;


public class Gun : MonoBehaviour {

	private Controller2D controller;
	public float offset;
	public PhotonView PV;


	// Use this for initialization
	void Start () {
		 controller = GetComponent<Controller2D>();
		 PV = GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () { 
		if(PV.IsMine){
		//rotate();
		Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.GetChild(3).position;
		PV.RPC("RPC_rotate", RpcTarget.All, difference);
		}
	}
	[PunRPC]
	public void RPC_rotate(Vector3 difference){
		float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		transform.Find("gun").rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
		if(transform.Find("gun").rotation.eulerAngles.z >= 90 && transform.Find("gun").rotation.eulerAngles.z <= 270){
			controller.FaceDirForWeapon = -1;
		}
		if(transform.Find("gun").rotation.eulerAngles.z <= 89 && transform.Find("gun").rotation.eulerAngles.z >= 0){
			controller.FaceDirForWeapon = 1;
		}
		if(transform.Find("gun").rotation.eulerAngles.z <= 360 && transform.Find("gun").rotation.eulerAngles.z >= 271){
			controller.FaceDirForWeapon = 1;
		}
	}
}
//Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		//float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
