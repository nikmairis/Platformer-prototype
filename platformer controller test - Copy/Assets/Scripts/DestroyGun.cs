using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace testprojekts{

public class DestroyGun : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Avatar")
        {
            //Re-enable the if statement, if desired for guns to be obtainable only when no gun equipped
            //if(other.gameObject.transform.GetComponent<WeaponManager>().HasWeapon == false)
           Destroy(this.gameObject);
            
        }
    }
}
}