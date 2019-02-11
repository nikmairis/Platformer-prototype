using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

	public float lifeTime = 3f;
	public GameObject destroyEffect;


	// Use this for initialization
	void Start () {
		Invoke("Explode", lifeTime);
	}
	
	//Explosion function
	void Explode(){
		destroyEffect.transform.localScale += new Vector3(0.25F, 0.25F, 0.25F);
		Instantiate(destroyEffect, transform.position, Quaternion.identity);
		Destroy(gameObject);
		destroyEffect.transform.localScale -= new Vector3(0.25F, 0.25F, 0.25F);
	}
}
