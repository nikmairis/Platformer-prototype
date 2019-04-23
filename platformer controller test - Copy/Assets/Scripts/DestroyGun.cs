using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace testprojekts{

public class DestroyGun : MonoBehaviour {

	private void OnTriggerEnter2D(Collider2D other)
    {

        if (other.gameObject.tag == "Avatar")
        {
           Destroy(this.gameObject);
            
        }
    }
}
}