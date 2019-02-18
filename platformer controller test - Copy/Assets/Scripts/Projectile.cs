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
	public BoxCollider2D ShootersColider;
	public PhotonView PV;



	// STARTS THE FUNCTION, THAT WILL DESTROY THE PROJECTILE
	void Start () {
		Invoke("DestroyProjectile", lifeTime);
		//Physics2D.IgnoreCollision(this.transform.GetComponent<BoxCollider2D>(), ShootersColider);
	}
	
	// MAKES BULLET MOVE FORWARD AND DETECTS COLLISION
	void Update () {
		/*
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
		*/
		transform.Translate(Vector2.up * speed * Time.deltaTime);
	}
 void OnCollisionEnter2D()
    {
        //Debug.Log("Triggered");
		//DestroyProjectile();
    }
	void OnTriggerEnter2D(Collider2D col){
		if(col == ShootersColider){
			return;
		}
		 if(col.transform.tag == "Avatar" && col!= ShootersColider){
				PhotonView pView = col.transform.GetComponent<PhotonView>();
					if(pView && PV.IsMine){
						pView.RPC("ApplyDamage", RpcTarget.All, myDamage);
					}
				Debug.Log("Hit an enemy!");
				DestroyProjectile();
			}
			else{
				DestroyProjectile();
			}
	}


	// DESTROYS PROJECTILE AFTER TIME OR WHEN HITS A COLLIDER
	void DestroyProjectile(){
		Instantiate(destroyEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
