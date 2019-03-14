using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Lava : MonoBehaviour {

	public int LavaDamage = 10;
	public float BurnInterval = 0.3f;
	public float BurnTimer = 0;

	void Update(){
		BurnTimer += Time.deltaTime;
	}
	void OnTriggerStay2D(Collider2D other)
    {
		
        if(other.transform.tag == "Avatar"){
			if(BurnTimer > BurnInterval){
				PhotonView pView = other.transform.GetComponent<PhotonView>();
					if(pView && pView.IsMine){
						pView.RPC("ApplyDamage", RpcTarget.All, LavaDamage);
					}
					BurnTimer = 0;
			}
    }
	}
}
