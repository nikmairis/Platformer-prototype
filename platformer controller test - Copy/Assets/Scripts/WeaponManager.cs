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
		Instantiate(Guns[avatarCombat.GunID], new Vector3(0,0,0), this.transform.rotation);
	}

	void Update(){
		if(Input.GetKeyUp(KeyCode.G) && HasWeapon == true){
			WeaponDropHandle();
	}
	}


	public void WeaponDropHandle(){
		int direction = this.GetComponent<Controller2D>().FaceDirForWeapon;
		bool CanBeDropped = true;
		bool NeedsToContinue = true;
			if(CanBeDropped){
				Collider2D[] WallCheck = Physics2D.OverlapCircleAll(this.transform.position+ new Vector3(0, 4, 0), 0.1f);
					foreach(Collider2D nearbyObject in WallCheck){
						if(nearbyObject.transform.tag == "Tiles")
						CanBeDropped = false;
					}
					if(CanBeDropped){
						NeedsToContinue = false;
						WeaponDropCaller(this.transform.position+ new Vector3(0, 4, 0));
					}
			}
			if(NeedsToContinue){
				CanBeDropped = true;
				Collider2D[] WallCheck = Physics2D.OverlapCircleAll(this.transform.position+ new Vector3(4*direction, 1, 0), 0.1f);
					foreach(Collider2D nearbyObject in WallCheck){
						if(nearbyObject.transform.tag == "Tiles")
						CanBeDropped = false;
					}
					if(CanBeDropped){
						NeedsToContinue = false;
						WeaponDropCaller(this.transform.position+ new Vector3(4*direction, 1, 0));
					}
			}
			if(NeedsToContinue){
				CanBeDropped = true;
				Collider2D[] WallCheck = Physics2D.OverlapCircleAll(this.transform.position+ new Vector3(-4*direction, 1, 0), 0.1f);
					foreach(Collider2D nearbyObject in WallCheck){
						if(nearbyObject.transform.tag == "Tiles")
						CanBeDropped = false;
					}
					if(CanBeDropped){
						NeedsToContinue = false;
						WeaponDropCaller(this.transform.position+ new Vector3(-4*direction, 1, 0));
					}
			}
			if(NeedsToContinue){
				CanBeDropped = true;
				Collider2D[] WallCheck = Physics2D.OverlapCircleAll(this.transform.position+ new Vector3(0, -4, 0), 0.1f);
					foreach(Collider2D nearbyObject in WallCheck){
						if(nearbyObject.transform.tag == "Tiles")
						CanBeDropped = false;
					}
					if(CanBeDropped){
						NeedsToContinue = false;
						WeaponDropCaller(this.transform.position+ new Vector3(0, -4, 0));
					}
			}
	}

	public void WeaponDropCaller(Vector3 Position){
			//drops weapon and switches to sword
			HasWeapon = false;
			CanDestroyWeapon = true;

			avatarCombat.PV.RPC("RPC_WeaponSwitch", RpcTarget.All, false);

			// Spawns the dropped weapon
			Vector3 position = Position;
			avatarCombat.PV.RPC("RPC_DropWeapon", RpcTarget.All, position);
	}


	[PunRPC]
	void RPC_DropWeapon(Vector3 Position, PhotonMessageInfo info){
			Instantiate(Guns[avatarCombat.GunID], Position, this.transform.rotation);
	}
	
	/////// Shooting styles:
	/////// 1 = classic 1 bullet at a time;
	/////// 2 = Shotgun cone shoot
	/////// 3 = Granade Launcher
	private void OnTriggerEnter2D(Collider2D other)
    {
			//Re-enable the if statement, if desire weapons to be obtainable only while no weapon equipped
	//	if(!HasWeapon){
			if (other.gameObject.tag == "Gun"){
				int gunID = other.gameObject.GetComponent<GunID>().ID;
				avatarCombat.PV.RPC("RPC_WeaponStyle", RpcTarget.All, gunID);
				//Destroy(other.gameObject);
				avatarCombat.PV.RPC("RPC_WeaponSwitch", RpcTarget.All, true);
			}

   // }
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