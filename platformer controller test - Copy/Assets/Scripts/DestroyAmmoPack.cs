using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyAmmoPack : MonoBehaviour {

	private int projectileDamage = 0;
	private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Avatar")
        {
            Destroy(this.gameObject);
        }
		if (other.gameObject.tag == "Projectile"){
			projectileDamage++;
			if(projectileDamage >= 3){
				Destroy(this.gameObject);
			}
		}
    }
	
}
