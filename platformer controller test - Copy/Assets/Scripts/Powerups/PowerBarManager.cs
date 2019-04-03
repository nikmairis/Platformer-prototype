using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
namespace testprojekts{
public class PowerBarManager : MonoBehaviour {
	private AvatarSetup avatarSetup;
	private PhotonView PV;
	private Vector3 fullSize;
	private float xSize;
	public static int DamageDealt;
	void Start () {

	}
	
	void Update () {
		Debug.Log(DamageDealt);
			if(DamageDealt <=100){
				float percentDivider = DamageDealt;
				float percent = percentDivider/100f;
				Debug.Log(percent);
				this.transform.localScale = new Vector3(percent, 1, 1);
			}
			else{
				this.transform.localScale = new Vector3(1, 1, 1);
			}
	}
}
}