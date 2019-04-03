using UnityEngine;
using System.Collections;

public class LaunchDelayDestroy : MonoBehaviour {

	void Start () {
		StartCoroutine(DestroyCo());
	}

	IEnumerator DestroyCo(){
		yield return new WaitForSeconds(7.5f);
		Destroy(gameObject);
	}
}
