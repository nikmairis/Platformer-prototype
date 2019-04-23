using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class TouchExplode : MonoBehaviour {
	public PhotonView PV;
	public GameObject ExplosionEffect;
	void OnCollisionEnter2D()
    {
        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 2f);
			foreach(Collider2D nearbyObject in colliders){
				Debug.Log(nearbyObject);
				if(nearbyObject.transform.tag == "Avatar"){
					PhotonView pView = nearbyObject.transform.GetComponent<PhotonView>();
					if(pView != null){
						if(PV.IsMine){
						pView.RPC("ApplyDamage", RpcTarget.All, 10);
						PV.RPC("IncreaseDamageDealth", RpcTarget.All, 10);
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
			Instantiate(ExplosionEffect, this.transform.position, this.transform.rotation);
			Destroy(this.gameObject);
    }
}
