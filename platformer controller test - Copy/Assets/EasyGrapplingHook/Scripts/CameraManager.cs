using UnityEngine;
using System.Collections;

public class CameraManager : MonoBehaviour {

	public GameObject player;
	public float zPosition = -10f;
	// Update is called once per frame
	void Start()
	{
		if(player ==null)
			player = GameObject.FindGameObjectWithTag("Player");
	}
	void Update () {
		this.gameObject.transform.position = new Vector3( player.gameObject.transform.position.x
		                                                 , player.gameObject.transform.position.y  ,
		                                   zPosition);
	}
}
