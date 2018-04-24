using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class FindGameUI : MonoBehaviour {

    public NetworkManager netManager;
    public InputField serverNameField;
    public LanDiscovery netDiscovery;

    public RectTransform buttonList;

    public GameObject connectButtonPrefab;

    private string serverName;



    private void Awake()
    {
        netDiscovery.OnReceiveBroadcast += ReceiveBroadcast;
    }

    public void HostGame()
    {
        serverName = serverNameField.text;
        serverNameField.text = "";

        //netDiscovery.Initialize();
        netDiscovery.StopBroadcast();
        netDiscovery.broadcastData = serverName;
        netDiscovery.StartAsServer();

        netManager.StartHost();
    }
    
    private void ReceiveBroadcast(string fromAdress, string data)
    {
        GameObject button = Instantiate(connectButtonPrefab);
        button.transform.parent = buttonList;
        JoinLanButton script = button.GetComponent<JoinLanButton>();
        script.netManager = netManager;
        script.gameName = data;
    }

}
