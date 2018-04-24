using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class JoinLanButton : MonoBehaviour {

    public NetworkManager netManager;
    public Text myText;
    public string gameName;
    public string netAddress;
    public int netPort;



    private void Start()
    {
        myText.text = gameName;
    }

    public void JoinGame()
    {
        netManager.networkAddress = netAddress;
        netManager.networkPort = netPort;
        netManager.StartClient();
    }

}
