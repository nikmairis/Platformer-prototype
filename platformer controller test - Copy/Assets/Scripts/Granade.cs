using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


public class Granade : MonoBehaviour {

	//TIME TILL EXPLOSION
	public float lifeTime = 3f;
	public float radius = 2f;
	public int myDamage = 15;
	//refferenc to the particleSystem Prefab
	public GameObject destroyEffect;
	public PhotonView PV;


	// TRIGGERS THE GRANADE GAMEOBJECT DESTRUCTION FUNCTION
	void Start () {
		Invoke("Explode", lifeTime);
	}
	
	//Explosion function
	void Explode(){
		destroyEffect.transform.localScale += new Vector3(0.25F, 0.25F, 0.25F);
		Instantiate(destroyEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
		destroyEffect.transform.localScale -= new Vector3(0.25F, 0.25F, 0.25F);
		if(PV.IsMine){
			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
			foreach(Collider2D nearbyObject in colliders){
				if(nearbyObject.transform.tag == "Avatar"){
					PhotonView pView = nearbyObject.transform.GetComponent<PhotonView>();
					if(pView != null){
						pView.RPC("ApplyDamage", RpcTarget.All, myDamage);
					}
				}
			}
		}
	} 
}
