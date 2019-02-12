using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Granade : MonoBehaviour {

	//TIME TILL EXPLOSION
	public float lifeTime = 3f;
	//refferenc to the particleSystem Prefab
	public GameObject destroyEffect;


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
	} 
}
