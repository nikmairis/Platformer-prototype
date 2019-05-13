using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace testprojekts{
public class HookTravel : MonoBehaviour {

	public GameObject MyChar;
	private LineRenderer lr;
	public float speed = 50f;
	private bool destroyable = false;
	private float DestroyTimer = 2f;
	// Use this for initialization
	void Start () {
		lr = this.GetComponent<LineRenderer>();
	}
	
	// Update is called once per frame
	void Update () {
		DestroyTimer -= Time.deltaTime;
		transform.Translate(Vector2.up * speed * Time.deltaTime);
		lr.SetPosition(0, MyChar.transform.position);
		lr.SetPosition(1, this.transform.position);
		if(DestroyTimer <= 0 && destroyable == false){
			MyChar.GetComponent<ShootHook>().HookReady = true;
			Destroy(this.gameObject);
		}
		if(Vector3.Distance(MyChar.transform.position, this.transform.position) < 3f && destroyable == true){
			MyChar.GetComponent<ShootHook>().HookReady = true;
			Destroy(this.gameObject);
		}
		if(DestroyTimer <= -10f && destroyable == true){
			MyChar.GetComponent<ShootHook>().HookReady = true;
			Destroy(this.gameObject);
		}
		
	}
	void OnTriggerEnter2D(Collider2D col){
		if(col != MyChar.GetComponent<BoxCollider2D>() && col.gameObject.tag != "DeathZone"){
		speed = 0f;
		MyChar.GetComponent<Player>().grappling = true;
		MyChar.GetComponent<Player>().GrapplePos = this.transform.position;
		destroyable = true;
		}
	}
}
}