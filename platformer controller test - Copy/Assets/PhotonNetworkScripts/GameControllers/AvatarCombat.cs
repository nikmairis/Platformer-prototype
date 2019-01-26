using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class AvatarCombat : MonoBehaviour {

	private PhotonView PV;
	private AvatarSetup avatarSetup;
	public Transform rayOrigin;
	public TextMesh text2;
	private LineRenderer line;
	public GameObject[] AvatarArray;
	public Vector3 MyPosition;


	// Use this for initialization
	void Start () {
		PhotonNetwork.SendRate = 30;
		PhotonNetwork.SerializationRate = 30;
		PV = GetComponent<PhotonView>();
		avatarSetup = GetComponent<AvatarSetup>();
						line = this.gameObject.AddComponent<LineRenderer>();
						line.SetWidth(0.05F, 0.05F);
						line.SetVertexCount(2);
	}
	
	// Update is called once per frame
	void Update () {
		/*
		MyPosition = this.gameObject.transform.GetChild(2).position;
            AvatarArray = GameObject.FindGameObjectsWithTag("Avatar");
			foreach (GameObject go in AvatarArray)
        {
			Debug.Log(go.transform.position);
			Debug.DrawRay(go.transform.position, MyPosition*2, Color.white);
		}
		*/


		text2.text = avatarSetup.playerHealth.ToString();
		if(!PV.IsMine){
			return;
		}else{
			//this.gameObject.GetComponent<BoxCollider2D>().enabled = false;
			if(Input.GetMouseButtonDown(0)){
				Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Shooting(MousePos);
				PV.RPC("RPC_DrawRay", RpcTarget.All, MousePos);
			}
		}
	}


	[PunRPC]
	void RPC_DrawRay(Vector3 MousePos){
		Vector3 difference = MousePos - rayOrigin.transform.position;
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, difference, 1000);
			if(hit){
				line.SetPosition(0, rayOrigin.position);
				line.SetPosition(1, MousePos);
				}
			else{
				line.SetPosition(0, rayOrigin.position);
             	line.SetPosition(1, MousePos);
				}
	}
	void Shooting(Vector3 MousePos){
		Vector3 difference = MousePos - rayOrigin.transform.position;
			 RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, difference, 1000);
			if(hit){
				Debug.DrawRay(rayOrigin.position, difference*1000, Color.yellow);
				Debug.Log("Did Hit!");
				line.SetPosition(0, rayOrigin.position);
             line.SetPosition(1, MousePos);

				if(hit.transform.tag == "Avatar"){
					PhotonView pView = hit.transform.GetComponent<PhotonView>();
					if(pView){
						pView.RPC("ApplyDamage", RpcTarget.All, avatarSetup.playerDamage);
					}
					Debug.Log("Hit Another Player");
				}
			}
			else{
				Debug.DrawRay(rayOrigin.position, difference*1000, Color.white);
				Debug.Log("Did Not Hit!");
				line.SetPosition(0, rayOrigin.position);
             line.SetPosition(1, MousePos);
			}
	}
	void Connect(){

	}
}
