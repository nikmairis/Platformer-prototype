using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour {
	public Text teksts;

	public void OnClickCharacterPick(int whichCharacter){
		if(PlayerInfo.PI != null){
			if(whichCharacter == 0){
				teksts.text = "You have chosen: Brown";
			}
			if(whichCharacter == 1){
				teksts.text = "You have chosen: Black";
			}
			PlayerInfo.PI.mySelectedCharacter = whichCharacter;
			PlayerPrefs.SetInt("MyCharacter", whichCharacter);
		}
	}
}
