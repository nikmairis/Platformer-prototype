using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticle : MonoBehaviour {
	public float lifeTime = 3f;
	// Use this for initialization
	void Start () {
		Invoke("DestroyParticleFunc", lifeTime);
	}
	
	void DestroyParticleFunc(){
		Destroy(gameObject);
	}
}
