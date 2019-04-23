using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;


namespace testprojekts{

public class WeaponManager : MonoBehaviour, IPunObservable {
	public bool HasWeapon = false;
	public bool CanDestroyWeapon = true;
	public int CurrEqWeapon = 0;
	AvatarCombat avatarCombat;
	public GameObject[] Guns;

	void Start () {
		avatarCombat = this.GetComponent<AvatarCombat>();
	}

	void Update(){
		if(Input.GetKeyUp(KeyCode.G) && HasWeapon == true){
			//drops weapon and switches to sword
			HasWeapon = false;
			CanDestroyWeapon = true;

			avatarCombat.PV.RPC("RPC_WeaponSwitch", RpcTarget.All, false);

			// Spawns the dropped weapon
			int direction = this.GetComponent<Controller2D>().FaceDirForWeapon;
			Vector3 position = this.transform.position;
			avatarCombat.PV.RPC("RPC_DropWeapon", RpcTarget.All, direction, position);
	}
	}

	[PunRPC]
	void RPC_DropWeapon(int direction, Vector3 Position, PhotonMessageInfo info){
		if(direction == 1){
			Instantiate(Guns[avatarCombat.GunID], Position + new Vector3(4, 1, 0), this.transform.rotation);
			}
			else{
			Instantiate(Guns[avatarCombat.GunID], Position + new Vector3(-4, 1, 0), this.transform.rotation);
			}
	}
	
	/////// Shooting styles:
	/////// 1 = classic 1 bullet at a time;
	/////// 2 = Shotgun cone shoot
	/////// 3 = Granade Launcher
	private void OnTriggerEnter2D(Collider2D other)
    {
		if(!HasWeapon){
			if (other.gameObject.tag == "Gun"){
				int gunID = other.gameObject.GetComponent<GunID>().ID;
				avatarCombat.PV.RPC("RPC_WeaponStyle", RpcTarget.All, gunID);
				//Destroy(other.gameObject);
				avatarCombat.PV.RPC("RPC_WeaponSwitch", RpcTarget.All, true);
			}

    }
	}

[PunRPC]
	void RPC_WeaponStyle(int gunID, PhotonMessageInfo info){
		avatarCombat.GunID = gunID;
				HasWeapon=true;
				CanDestroyWeapon = false;
					if(gunID == 0){
						avatarCombat.ShootStyle = 1;
						avatarCombat.shootFrequency = 0.1f;
					}
					if(gunID == 1){
						avatarCombat.ShootStyle = 2;
						avatarCombat.shootFrequency = 0.25f;
					}
					if(gunID == 2){
						avatarCombat.ShootStyle = 3;
						avatarCombat.shootFrequency = 0.25f;
					}
					if(gunID == 3){
						avatarCombat.ShootStyle = 1;
						avatarCombat.shootFrequency = 0.3f;
					}
	}

	///////!!!!!!!!!!! HOW T OSET UP A NEW WEAPON:  !!!!!!!!!!!!!!!!\\\\\\\\\\\
	// 1. Make another prefab of guns visuals
	// 2. add all the colliders, so it stays on the ground
	// 3. add a rigid body component
	// 4. add a trigger collider around the whole gun
	// 5. add the "Gun" tag to the prefab
	// 6. add the GunID script to the prefab
	// 7. give the gun an ID that is equal to the guns position on the players character prefab under the "Gun" Child
	// 8. drag the prefab in-to the "Guns" array of this component

        public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting){
              stream.SendNext(HasWeapon);
            }else{
							HasWeapon = (bool)stream.ReceiveNext();
            }
        }

}
}