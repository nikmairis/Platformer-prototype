using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class EnemyVisibility : MonoBehaviour {


	public GameObject[] ViewPointArray;
	public Vector3 MyPosition;
	private PhotonView PV;


	// Use this for initialization
	void Start () {
		PV = this.gameObject.transform.parent.gameObject.GetComponent<PhotonView>();
	}
	
	// Update is called once per frame
	void Update () {
		if(PV.IsMine){
			VisionRay();
		}
	}
	void VisionRay(){
		MyPosition = this.gameObject.transform.position;
            ViewPointArray = GameObject.FindGameObjectsWithTag("ViewPoint");
			RaycastHit2D hit;
			foreach (GameObject go in ViewPointArray)
        {
			hit = Physics2D.Raycast(MyPosition, go.transform.position - MyPosition, 1000);
			if(hit){
				if(hit.transform.tag == "Avatar"){
					Debug.Log("Hit an avatar!");
					go.transform.parent.gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = true;
					go.transform.parent.gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = true;
				}else{
					Debug.Log("Hit something else");
					go.transform.parent.gameObject.transform.GetChild(2).GetComponent<SpriteRenderer>().enabled = false;
					go.transform.parent.gameObject.transform.GetChild(4).GetComponent<SpriteRenderer>().enabled = false;
				}
			}
			Debug.Log(go.transform.position);
			Debug.DrawRay(MyPosition, go.transform.position - MyPosition, Color.red);
		}
	}
}
