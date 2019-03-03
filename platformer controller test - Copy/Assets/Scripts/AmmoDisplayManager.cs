using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoDisplayManager : MonoBehaviour {

	// Use this for initialization
	public static int ammo;
	public static int granades;
	public Text ammoText;
	public Text granadeText;
	void Start () {
		ammo = 0;
		granades = 0;
	}
	
	// Update is called once per frame
	void Update () {
		ammoText.text = "Ammo: " + ammo;
		granadeText.text = "Granades: " + granades;
	}
}
