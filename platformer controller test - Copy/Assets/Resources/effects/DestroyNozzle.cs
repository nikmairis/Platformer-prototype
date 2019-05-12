using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyNozzle : MonoBehaviour {

	// Use this for initialization
	void Start () {
		Invoke("DestroyNozzles", 0.1f);
	}
	
	void DestroyNozzles(){
		Destroy(this.gameObject);
	}
}
