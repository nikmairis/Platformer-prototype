using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class CameraMover : MonoBehaviour, IPunObservable{

	public float Timer;
	public Vector3 StartPos;
	public Vector3 StartPosLava;
	public float MoveAmount;
	void Start(){
		StartPos = Camera.main.transform.position;
		StartPosLava = Camera.main.transform.GetChild(0).transform.position;
	}

	// Update is called once per frame
	void Update () {
		if(PhotonNetwork.IsMasterClient){
		Timer += Time.deltaTime;
		MoveAmount = Timer/4;
		Camera.main.transform.position = StartPos + new Vector3(0,MoveAmount, 0);
		Camera.main.transform.GetChild(0).transform.position = StartPosLava + new Vector3(0,MoveAmount*2, 0);
		}else{
			Camera.main.transform.position = StartPos + new Vector3(0,MoveAmount, 0);
			Camera.main.transform.GetChild(0).transform.position = StartPosLava + new Vector3(0,MoveAmount*2, 0);
		}

	}

	 public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
        {
            if(stream.IsWriting && PhotonNetwork.IsMasterClient){
                stream.SendNext(MoveAmount);
            }else{
                MoveAmount = (float)stream.ReceiveNext();
            }
        }
}
