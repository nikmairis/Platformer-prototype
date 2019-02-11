using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using Photon.Realtime;

public class AvatarSetup : MonoBehaviour, IPunObservable {

	private PhotonView PV;
	public GameObject myCharacter;
	public int characterValue;
	public int playerHealth;
	public int playerDamage;
	public int playerDamageTaken = 0;
	public int playerHealthLeft;
	public static AvatarSetup avatarSetup;
	// Use this for initialization
	void Start () {
		if(AvatarSetup.avatarSetup == null){
			AvatarSetup.avatarSetup = this;
		}
		PV = GetComponent<PhotonView>();
		if(PV.IsMine){
			PV.RPC("RPC_AddCharacter", RpcTarget.AllBuffered, PlayerInfo.PI.mySelectedCharacter);
		}
	}
	
	[PunRPC]
	void RPC_AddCharacter(int whichCharacter){
		characterValue = whichCharacter;
		myCharacter = Instantiate(PlayerInfo.PI.allCharacters[whichCharacter], transform.position, transform.rotation, this.transform);
		if(characterValue == 1){
			playerHealth = 200;
		}
		else{
			playerHealth = 100;
		}
	}
	[PunRPC]
	public void ApplyDamage(int Damage){
		playerHealth -= Damage;
	}
	 public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting){
                stream.SendNext(playerHealth);
            }else{
                playerHealth = (int)stream.ReceiveNext();
            }
        }
}
