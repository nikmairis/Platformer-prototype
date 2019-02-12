using System.Collections;
using System.Collections.Generic;
using testprojekts;
using UnityEngine;

public class Gun : MonoBehaviour {

	private Controller2D controller;
	public float offset;

	// Use this for initialization
	void Start () {
		 controller = GetComponent<Controller2D>();
	}
	
	// Update is called once per frame
	void Update () { 

		Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.GetChild(3).position;
		float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		transform.Find("gun").rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
		if(transform.Find("gun").rotation.eulerAngles.z >= 90 && transform.Find("gun").rotation.eulerAngles.z <= 270){
			controller.collisions.faceDir = -1;
		}
		if(transform.Find("gun").rotation.eulerAngles.z <= 89 && transform.Find("gun").rotation.eulerAngles.z >= 0){
			controller.collisions.faceDir = 1;
		}
		if(transform.Find("gun").rotation.eulerAngles.z <= 360 && transform.Find("gun").rotation.eulerAngles.z >= 271){
			controller.collisions.faceDir = 1;
		}
	}
}
//Vector3 difference = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
		//float rotZ = Mathf.Atan2(difference.y, difference.x) * Mathf.Rad2Deg;
		//transform.rotation = Quaternion.Euler(0f, 0f, rotZ + offset);
