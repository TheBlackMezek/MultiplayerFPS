using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class UIManager : MonoBehaviour {

    public Text healthText;

    //private PlayerController player;
    //
	//
	//void Start () {
    //    player = NetworkManager.singleton.client.connection.playerControllers[0].gameObject.GetComponent<PlayerController>();
	//}
	
	
	void Update () {
        if(GlobalVars.localPlayer != null)
        {
            healthText.text = ((int)GlobalVars.localPlayer.GetHealth()).ToString();
        }
	}
}
