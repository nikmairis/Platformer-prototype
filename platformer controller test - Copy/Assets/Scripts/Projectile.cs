using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour {

	//Projectiles speed
	public float speed;
	//Projectiles max lifetime
	public float lifeTime;
	// Refference to the particleSystem Prefab
	public GameObject destroyEffect;
	//Distance of projectiles raycast
	public float distance = 0.5f;
	//Weapons damage value
	public int myDamage;



	// STARTS THE FUNCTION, THAT WILL DESTROY THE PROJECTILE
	void Start () {
		Invoke("DestroyProjectile", lifeTime);
	}
	
	// MAKES BULLET MOVE FORWARD AND DETECTS COLLISION
	void Update () {
		RaycastHit2D hitInfo =  Physics2D.Raycast(transform.position, transform.up, distance);
		Debug.DrawRay(transform.position, transform.up, Color.yellow);
		if(hitInfo.collider != null){
			if(hitInfo.transform.tag == "Avatar"){
				PhotonView pView = hitInfo.transform.GetComponent<PhotonView>();
					if(pView){
						pView.RPC("ApplyDamage", RpcTarget.All, myDamage);
					}
				Debug.Log("Hit an enemy!");
			}
			DestroyProjectile();
		}

		transform.Translate(Vector2.up * speed * Time.deltaTime);
	}


	// DESTROYS PROJECTILE AFTER TIME OR WHEN HITS A COLLIDER
	void DestroyProjectile(){
		Instantiate(destroyEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
