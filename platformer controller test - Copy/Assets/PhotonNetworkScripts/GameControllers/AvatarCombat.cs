using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

namespace testprojekts{

public class AvatarCombat : MonoBehaviour {

	public PhotonView PV;
	//singleton
	private AvatarSetup avatarSetup;
	// Tip of a weapon transform
	public Transform rayOrigin;
	// Health text object
	public TextMesh text2;
	//Linerenderer for testing raycast shooting
	private LineRenderer line;
	//Array of all ingame avatars
	public GameObject[] AvatarArray;
	// Vector3 of my position
	public Vector3 MyPosition;
	// Refference to the projectile Prefab
	public GameObject Projectile;
	// Refference to the Granade Prefab
	public GameObject Granade;
	//Projectile GameObject
	[HideInInspector]
	public GameObject myProjectile;
	//Granade GameObject
	[HideInInspector]
	public GameObject myGranade;
	public GameObject LaunchableGranade;
	// Granade throwing default force
	public float ThrowForce = 100f;
	//Granade strength channeling timer initialization
	private float throwTimer =0;
	private int bulletCount;
	private int granadeCount;
	private float shootTimer = 0;
	public float shootFrequency = 0.1f;
	private float SlashTimer = 0;
	private int WeaponEquipped = 0;
	private bool SwordSlot = true;
	public Transform AttackPos;
	public float swordAttackRadius;
	private bool IsSwinging;
	public int GunID = 0;
	public int ShootStyle = 1;
	TrajectoryPredictor tp;
	WeaponManager weaponManager;



	// Start function for initialization
	void Start () {
		weaponManager = this.GetComponent<WeaponManager>();
		//////////////////////////////
		tp = gameObject.AddComponent<TrajectoryPredictor>();
		tp.predictionType = TrajectoryPredictor.predictionMode.Prediction2D;
		tp.drawDebugOnPrediction = true;
		tp.accuracy = 0.99f;
		tp.lineWidth = 0.2f;
		tp.iterationLimit = 250;
		//////////////////////////////
		bulletCount = 30;
		granadeCount = 5;
		PhotonNetwork.SendRate = 30	;
		PhotonNetwork.SerializationRate = 30;
		PV = GetComponent<PhotonView>();
		avatarSetup = GetComponent<AvatarSetup>();
		//Linerenderer Debuggingam testējot raycast shooting mechanic
			line = this.gameObject.AddComponent<LineRenderer>();
			line.startWidth = 0.2f;
			line.endWidth = 0.1f;
			line.positionCount = 2;
			line.sortingOrder = 1;
			line.material = new Material (Shader.Find ("Sprites/Default"));
			line.material.color = Color.red; 
	}


	void Update () {
		Debug.Log(text2.text);
		try{
			Debug.Log(avatarSetup.playerHealth.ToString());
		text2.text = avatarSetup.playerHealth.ToString();
		}
		catch(Exception e){
			
		}
		
		//transform.Find("Sword").GetChild(0).transform.GetComponent<Animator>().SetBool("IsSwinging", IsSwinging);
		if(!PV.IsMine){
			return;
		}else{
			AmmoDisplayManager.ammo = bulletCount;
			AmmoDisplayManager.granades = granadeCount;
			PowerBarManager.DamageDealt = avatarSetup.playerDamageDealt;
			PowerUps();
			/*
							****************************************************
							!!!!!!!   Mechanic for shooting with Raycasts   !!!!
							****************************************************

			if(Input.GetMouseButtonDown(0)){
				Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Shooting(MousePos);
				PV.RPC("RPC_DrawRay", RpcTarget.All, MousePos);
			}
			*/

			if(Input.GetKeyDown(KeyCode.Q) && weaponManager.HasWeapon == true){
				PV.RPC("RPC_WeaponSwitch", RpcTarget.All, SwordSlot);
			}
			
			//button for projectile shooting
			if(Input.GetMouseButton(0)){
				if(!SwordSlot){
				ProjectileShoot();
				}else{
				SwordSlash();
				}				
			}
			SlashTimer +=Time.deltaTime;
			if(Input.GetMouseButtonUp(0)){
				IsSwinging = false;
				PV.RPC("RPC_SwingSword", RpcTarget.All, IsSwinging);
			}
		if(granadeCount >=1){
			// Granade throw ChargeUp
			if(Input.GetMouseButton(1)){
				//Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				throwTimer += Time.deltaTime*8;
				Vector3 meterWidth = this.transform.Find("ThrowBar").localScale;
				float scaleSize = throwTimer / 5;
				if(scaleSize > 1) scaleSize =1;
				meterWidth.x = scaleSize;
				this.transform.Find("ThrowBar").localScale = meterWidth;
				Vector3 test = this.transform.GetComponent<Player>().velocity.normalized/4;
				Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if(throwTimer < 2) throwTimer = 2;
				if(throwTimer > 5) throwTimer = 5;
				ThrowForce = 7 * throwTimer;
				Vector3 SendDirection = ((MousePos - rayOrigin.transform.position).normalized+ test) * ThrowForce;
				Vector3 SendPosition = this.transform.position;
				tp.debugLineDuration = Time.unscaledDeltaTime;
				tp.Predict2D(SendPosition , SendDirection, Physics2D.gravity);

				//GranadeThrow(MousePos);
			}


			//Granade throw release
			if(Input.GetMouseButtonUp(1)){
				Vector3 test = this.transform.GetComponent<Player>().velocity.normalized/4;
				Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if(throwTimer < 2) throwTimer = 2;
				if(throwTimer > 5) throwTimer = 5;
				ThrowForce = 7 * throwTimer;
				Vector3 SendDirection = ((MousePos - rayOrigin.transform.position).normalized+ test) * ThrowForce;
				Vector3 SendPosition = this.transform.position;
				PV.RPC("RPC_ThrowBomb", RpcTarget.All, SendDirection , SendPosition);
				throwTimer = 0;
				Vector3 tempSize = this.transform.Find("ThrowBar").localScale;
				tempSize.x =0;
				this.transform.Find("ThrowBar").localScale = tempSize;
				granadeCount--;
			}
		}
		}
	}

    //!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\
	//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\
	void PowerUps(){
		if(avatarSetup.playerDamageDealt >= 100){
			if(avatarSetup.characterValue == 1){
				avatarSetup.playerHealth += 100;
				avatarSetup.playerDamageDealt = 0;
			}
			if(avatarSetup.characterValue == 0){
				avatarSetup.playerDamage = 10;
				Invoke("ResetDamage", 5f);
				avatarSetup.playerDamageDealt = 0;
			}
		}
	}
	void ResetDamage(){
		avatarSetup.playerDamage = 5;
	}
	//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\
	//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\//!!!!!!!!!!!!POWERUPS!!!!!!!!!!!!\\



	//Mechanic for projectile shooting
	void ProjectileShoot(){
		shootTimer += Time.deltaTime;
			Vector3 ShootDirection = rayOrigin.transform.rotation.eulerAngles - new Vector3(0, 0, 90);
			Vector3 SendPosition = rayOrigin.transform.position;
			if(bulletCount >=1 && shootTimer >=shootFrequency){
			shootTimer = 0f;
			PV.RPC("RPC_ProjectileShoot", RpcTarget.All, ShootDirection, SendPosition );
			bulletCount--;
			}
	}
	void SwordSlash(){
		if(SlashTimer >= 0.15f){
		IsSwinging = true;
		PV.RPC("RPC_SwingSword", RpcTarget.All, IsSwinging);
		Collider2D[] enemiesToDamage = Physics2D.OverlapCircleAll(AttackPos.position, swordAttackRadius);
		foreach(Collider2D EnemyToDamage in enemiesToDamage){
				if(EnemyToDamage.transform.tag == "Avatar"){
					PhotonView pView = EnemyToDamage.transform.GetComponent<PhotonView>();
					if(pView != null && !pView.IsMine){
						pView.RPC("ApplyDamage", RpcTarget.All, avatarSetup.playerDamage);
					}
				}
			}
			SlashTimer = 0;
		}
	}

	[PunRPC]
	void RPC_ProjectileShoot(Vector3 ShootDirection, Vector3 Position, PhotonMessageInfo info){
		if(ShootStyle == 1)
		Regular(ShootDirection, Position, info.photonView.gameObject.transform.GetComponent<BoxCollider2D>());
		if(ShootStyle == 2)
		ConeSpray(ShootDirection, Position, info.photonView.gameObject.transform.GetComponent<BoxCollider2D>());
		if(ShootStyle == 3)
		GranadeLauncher(ShootDirection, Position, info.photonView.gameObject.transform.GetComponent<BoxCollider2D>());
	}
	[PunRPC]
	void RPC_WeaponSwitch(bool current, PhotonMessageInfo info){
		if(current){
			transform.Find("gun").GetChild(GunID).transform.GetComponent<SpriteRenderer>().enabled = true;
			transform.Find("Sword").GetChild(0).transform.GetComponent<SpriteRenderer>().enabled = false;
			SwordSlot = false;
		}else{
			transform.Find("gun").GetChild(GunID).transform.GetComponent<SpriteRenderer>().enabled = false;
			transform.Find("Sword").GetChild(0).transform.GetComponent<SpriteRenderer>().enabled = true;
			SwordSlot = true;
		}
	}

	[PunRPC]
	void RPC_SwingSword(bool IsSwinging, PhotonMessageInfo info){
		transform.Find("Sword").GetChild(0).transform.GetComponent<Animator>().SetBool("IsSwinging", IsSwinging);
	}


	// RPC FOR THROWING GRANADE ON ALL CLIENTS
	[PunRPC]
	void RPC_ThrowBomb(Vector3 Direction, Vector3 Position){
		myGranade = Instantiate(Granade, Position, this.transform.rotation);
			Physics2D.IgnoreCollision(myGranade.GetComponent<CapsuleCollider2D>(), this.gameObject.GetComponent<BoxCollider2D>());
			Rigidbody2D rb2d = myGranade.GetComponent<Rigidbody2D>();
			//rb2d.AddForce(Direction);
			rb2d.velocity = Direction;
			rb2d.AddTorque(5f);
			myGranade.GetComponent<Granade>().PV = PV;
	}


	// RPC FOR TEST PURPOUSES
	// DRAWING RAYS WHEN USING RAYCAST SHOOTING
	[PunRPC]
	void RPC_DrawRay(Vector3 MousePos){
		Vector3 difference = MousePos - rayOrigin.transform.position;
				RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, difference, 1000);
			if(hit){
				line.SetPosition(0, rayOrigin.position);
				line.SetPosition(1, MousePos);
				Debug.Log(rayOrigin.position + " + " + MousePos);
				}
			else{
				line.SetPosition(0, rayOrigin.position);
             	line.SetPosition(1, MousePos);
				}
	}

	
	// LOCAL FUNCTION FOR SHOOTING USING RAYCASTS. CALLS AN RPC WHEN HITS A PLAYER.
	void Shooting(Vector3 MousePos){
		Vector3 difference = MousePos - rayOrigin.transform.position;
			 RaycastHit2D hit = Physics2D.Raycast(rayOrigin.position, difference, 1000);
			if(hit){
				Debug.DrawRay(rayOrigin.position, difference*1000, Color.yellow);
				Debug.Log("Did Hit!");
				line.SetPosition(0, rayOrigin.position);
             line.SetPosition(1, MousePos);

				if(hit.transform.tag == "Avatar"){
					PhotonView pView = hit.transform.GetComponent<PhotonView>();
					if(pView){
						pView.RPC("ApplyDamage", RpcTarget.All, avatarSetup.playerDamage);
					}
					Debug.Log("Hit Another Player");
				}
			}
			else{
				Debug.DrawRay(rayOrigin.position, difference*1000, Color.white);
				Debug.Log("Did Not Hit!");
				line.SetPosition(0, rayOrigin.position);
             line.SetPosition(1, MousePos);
			}
	}

	void ConeSpray(Vector3 ShootDirection, Vector3 Position, BoxCollider2D Col){
		Vector3 ConeDirection = ShootDirection - new Vector3(0, 0, 10);
		Vector3 ConeDirection2 = ShootDirection - new Vector3(0, 0, -10);
		myProjectile = Instantiate(Projectile, Position, Quaternion.Euler(ShootDirection));
		GameObject myProjectile2 = Instantiate(Projectile, Position, Quaternion.Euler(ConeDirection));
		GameObject myProjectile3 = Instantiate(Projectile, Position, Quaternion.Euler(ConeDirection2));
		
		Physics2D.IgnoreCollision(Col, myProjectile.GetComponent<BoxCollider2D>());
		Physics2D.IgnoreCollision(Col, myProjectile2.GetComponent<BoxCollider2D>());
		Physics2D.IgnoreCollision(Col, myProjectile3.GetComponent<BoxCollider2D>());
		Physics2D.IgnoreCollision(myProjectile.GetComponent<BoxCollider2D>(), myProjectile2.GetComponent<BoxCollider2D>());
		Physics2D.IgnoreCollision(myProjectile.GetComponent<BoxCollider2D>(), myProjectile3.GetComponent<BoxCollider2D>());
		Physics2D.IgnoreCollision(myProjectile3.GetComponent<BoxCollider2D>(), myProjectile2.GetComponent<BoxCollider2D>());


		myProjectile.GetComponent<Projectile>().myDamage = avatarSetup.playerDamage;
		myProjectile.GetComponent<Projectile>().ShootersColider = this.GetComponent<BoxCollider2D>();
		myProjectile.GetComponent<Projectile>().PV = PV;

		myProjectile2.GetComponent<Projectile>().myDamage = avatarSetup.playerDamage;
		myProjectile2.GetComponent<Projectile>().ShootersColider = this.GetComponent<BoxCollider2D>();
		myProjectile2.GetComponent<Projectile>().PV = PV;

		myProjectile3.GetComponent<Projectile>().myDamage = avatarSetup.playerDamage;
		myProjectile3.GetComponent<Projectile>().ShootersColider = this.GetComponent<BoxCollider2D>();
		myProjectile3.GetComponent<Projectile>().PV = PV;
	}
	void Regular(Vector3 ShootDirection, Vector3 Position, BoxCollider2D Col){
		myProjectile = Instantiate(Projectile, Position, Quaternion.Euler(ShootDirection));

		Physics2D.IgnoreCollision(Col, myProjectile.GetComponent<BoxCollider2D>());

		myProjectile.GetComponent<Projectile>().myDamage = avatarSetup.playerDamage;
		myProjectile.GetComponent<Projectile>().ShootersColider = this.GetComponent<BoxCollider2D>();
		myProjectile.GetComponent<Projectile>().PV = PV;	
	}
	void GranadeLauncher(Vector3 ShootDirection, Vector3 Position, BoxCollider2D Col){
		Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
		Vector3 LaunchDirection = (MousePos- this.transform.position).normalized;
		float LaunchPower = Vector3.Distance(MousePos, this.transform.position);

		if(LaunchPower <10)
		LaunchPower =10;
		if(LaunchPower >32)
		LaunchPower =32;

		myProjectile = Instantiate(LaunchableGranade, Position, Quaternion.Euler(ShootDirection));
		Physics2D.IgnoreCollision(Col, myProjectile.GetComponent<CircleCollider2D>());
		Rigidbody2D rb2d = myProjectile.GetComponent<Rigidbody2D>();
		myProjectile.GetComponent<TouchExplode>().PV = PV;
		rb2d.velocity = LaunchDirection * LaunchPower;
	}
	 private void OnTriggerEnter2D(Collider2D other)
    {
		Debug.Log(other.gameObject.tag);
        if (other.gameObject.tag == "AmmoBox")
        {
            bulletCount += 20;
        }
		if (other.gameObject.tag == "TNTBox")
        {
            granadeCount += 5;
        }
    }


	void OnDrawGizmosSelected(){
		Gizmos.color = Color.red;
		Gizmos.DrawWireSphere(AttackPos.position, swordAttackRadius);
	}
    }
}