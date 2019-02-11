using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class Projectile : MonoBehaviour {

	public float speed;
	public float lifeTime;
	public GameObject destroyEffect;
	public float distance = 0.5f;
	public int myDamage;
	// Use this for initialization
	void Start () {
		Invoke("DestroyProjectile", lifeTime);
	}
	
	// Update is called once per frame
	void Update () {
		RaycastHit2D hitInfo =  Physics2D.Raycast(transform.position, transform.up, distance);
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
	void DestroyProjectile(){
		Instantiate(destroyEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
