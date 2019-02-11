using System.Collections;
using System.Collections.Generic;
using System.IO;
using Photon.Pun;
using UnityEngine;

public class AvatarCombat : MonoBehaviour {

	private PhotonView PV;
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
	public GameObject Projectile;
	public GameObject Granade;
	//Projectile GameObject
	public GameObject myProjectile;
	//Granade GameObject
	public GameObject myGranade;
	// Granade throwing default force
	public float ThrowForce = 100f;
	private float throwTimer =0;


	// Use this for initialization
	void Start () {
		PhotonNetwork.SendRate = 30;
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
		text2.text = avatarSetup.playerHealth.ToString();
		if(!PV.IsMine){
			return;
		}else{
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

			//button for projectile shooting
			if(Input.GetMouseButtonDown(0)){
				ProjectileShoot();
			}
			if(Input.GetKey(KeyCode.G)){
				//Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				Debug.Log(throwTimer);
				throwTimer += Time.deltaTime*10;
				Vector3 meterWidth = this.transform.Find("ThrowBar").localScale;
				float scaleSize = throwTimer / 5;
				if(scaleSize > 1) scaleSize =1;
				meterWidth.x = scaleSize;
				this.transform.Find("ThrowBar").localScale = meterWidth;


				//GranadeThrow(MousePos);
			}
			if(Input.GetKeyUp(KeyCode.G)){
				Vector3 MousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
				if(throwTimer < 2) throwTimer = 2;
				if(throwTimer > 5) throwTimer = 5;
				ThrowForce = 250 * throwTimer;
				PV.RPC("RPC_ThrowBomb", RpcTarget.All, MousePos , ThrowForce);
				//GranadeThrow(MousePos);
				throwTimer = 0;
				Vector3 tempSize = this.transform.Find("ThrowBar").localScale;
				tempSize.x =0;
				this.transform.Find("ThrowBar").localScale = tempSize;
			}
		}
	}


	//Mechanic for projectile shooting
	void ProjectileShoot(){
		var ShootDirection = rayOrigin.transform.rotation.eulerAngles - new Vector3(0, 0, 90);
				myProjectile = PhotonNetwork.Instantiate(Path.Combine("PhotonPrefabs", "Projectile"), rayOrigin.position, Quaternion.Euler(ShootDirection));
				myProjectile.GetComponent<Projectile>().myDamage = avatarSetup.playerDamage;
				Debug.Log(ShootDirection);
	}
	[PunRPC]
	void RPC_ThrowBomb(Vector3 MousePos, float ThrowForceSent){
		myGranade = Instantiate(Granade, this.transform.position, this.transform.rotation);
			Physics2D.IgnoreCollision(myGranade.GetComponent<CapsuleCollider2D>(), this.gameObject.GetComponent<BoxCollider2D>());
			Rigidbody2D rb2d = myGranade.GetComponent<Rigidbody2D>();
			Vector3 SendDirection = (MousePos - rayOrigin.transform.position).normalized * ThrowForceSent;
			//PV.RPC("RPC_ThrowBomb", RpcTarget.All, SendDirection , rb2d);
			rb2d.AddForce((MousePos - rayOrigin.transform.position).normalized * ThrowForceSent);
	}

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

}
