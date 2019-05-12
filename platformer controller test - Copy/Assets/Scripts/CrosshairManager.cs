using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrosshairManager : MonoBehaviour {


	
	// Update is called once per frame
	void Update () {
		Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		MousePos.z = -0.01f;
		MousePos.y += -0.1f;
		this.transform.position = Vector3.Lerp(this.transform.position, MousePos, 0.4f);
	}
}
