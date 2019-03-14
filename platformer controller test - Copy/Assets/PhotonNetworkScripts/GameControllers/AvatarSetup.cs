using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;
using System.IO;
namespace testprojekts{

public class AvatarSetup : MonoBehaviour, IPunObservable {

	private PhotonView PV;
	//Players gameobject
	public GameObject myCharacter;
	//Character selection value
	public int characterValue;
	//Players current health
	public int playerHealth;
	private int DefaultHealth;
	//Players default damage
	public int playerDamage;
	public Vector3 LocalPositionTemp;

	/*
	-->>> Possible end game results could be stored here <<<--
			public int playerDamageTaken = 0;
			public int playerHealthLeft;
			public int playerDamageDealt;
	*/
	//refference to AvatarSetup singleton
	public static AvatarSetup avatarSetup;
	//random first name
	private string[] FirstName = new string[] { "First1", "First2", "First3", "First4", "First5" };	
	//random last name
	private string[] LastName = new string[] { "Last1", "Last2", "Last3", "Last4", "Last5" };	
	// Name text object
	public  TextMesh NameText;
	public float DeathTimer = 0;
	public GameObject DeathParticle;


	// Use this for initialization
	void Start () {
		DefaultHealth = playerHealth;
		if(AvatarSetup.avatarSetup == null){
			AvatarSetup.avatarSetup = this;
		}
		PV = GetComponent<PhotonView>();
		int NamePicker = Random.Range(0, FirstName.Length);
		int Name2Picker = Random.Range(0, LastName.Length);
		if(PV.IsMine){
			PV.Owner.NickName = FirstName[NamePicker]+ " " + LastName[Name2Picker];
			PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
		}
		NameText.text = PV.Owner.NickName;
		LocalPositionTemp =  this.transform.position;
	}
	

	// SETS UP SELECTED CHARACTER AND IT'S PARAMETERS.
	[PunRPC]
	void RPC_AddCharacter(int whichCharacter){
		characterValue = whichCharacter;
		myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, this.transform);
		if(characterValue == 1){
			playerHealth = 200;
			this.gameObject.GetComponent<Player>().moveSpeed = 7f;
			this.gameObject.GetComponent<Player>().DashSpeedx = 25f;
		}
		else{
			playerHealth = 100;
			this.gameObject.GetComponent<Player>().moveSpeed = 14f;
			this.gameObject.GetComponent<Player>().DashSpeedx = 35f;

		}
	}
	void Update(){
		if(playerHealth <= 0){
			if(DeathTimer == 0)
			PV.RPC("RPC_BloodEffect", RpcTarget.All, this.transform.position);
			this.transform.position = LocalPositionTemp + new Vector3(0,0,-100);
			GetComponent<PlayerInput>().enabled = false;
			GetComponent<AvatarCombat>().enabled = false;
			DeathTimer += Time.deltaTime;
			if(DeathTimer >=4){
				GetComponent<PlayerInput>().enabled = true;
				GetComponent<AvatarCombat>().enabled = true;
				DeathTimer = 0;
				playerHealth = DefaultHealth;
				this.transform.position = LocalPositionTemp;
			}
		}
	}
	[PunRPC]
	void RPC_BloodEffect(Vector3 position){
		Instantiate(DeathParticle, position, this.transform.rotation);
	}

	//!!!!!!!!DAMAGE APLYING RPC!!!!!!!!\\
	[PunRPC]
	public void ApplyDamage(int Damage){
		playerHealth -= Damage;
	}
	//!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!\\

	// HEALTH SYNC
	 public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting && PV.IsMine){
                stream.SendNext(playerHealth);
            }else{
                playerHealth = (int)stream.ReceiveNext();
            }
        }
}
}