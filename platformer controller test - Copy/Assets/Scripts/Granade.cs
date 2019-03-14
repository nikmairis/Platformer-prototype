using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using EZCameraShake;


public class Granade : MonoBehaviour {

	//TIME TILL EXPLOSION
	public float lifeTime = 3f;
	public float radius = 2f;
	public int myDamage = 15;
	//refferenc to the particleSystem Prefab
	public GameObject destroyEffect;
	public PhotonView PV;
	public float force = 10f;



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
		
			Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, radius);
			foreach(Collider2D nearbyObject in colliders){
				Debug.Log(nearbyObject);
				if(nearbyObject.transform.tag == "Avatar"){
					PhotonView pView = nearbyObject.transform.GetComponent<PhotonView>();
					if(pView != null){
						if(PV.IsMine){
						pView.RPC("ApplyDamage", RpcTarget.All, myDamage);
						}
					}
				}
				Rigidbody2D rb = nearbyObject.GetComponent<Rigidbody2D>();
				if(rb != null){
					float distance = 3 - Vector3.Distance(rb.gameObject.transform.position , this.transform.position);
					Vector3 direction = (rb.gameObject.transform.position - this.transform.position).normalized;
					rb.AddForce(direction * distance * 500f);
				}
			}
		
		CameraShaker.Instance.ShakeOnce(4f, 2f,.2f,1f);
	} 
}
