using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ChunkNetManager : NetworkManager {

    public NetworkManagerHUD hud;
    public NetworkDiscovery netDiscovery;
    public InputField connectIP;
    public Toggle newWorldToggle;

    private string serverName;
    private string IPPrefName = "LastUsedIP";




    private void Start()
    {
        netDiscovery.Initialize();
        netDiscovery.StartAsClient();
        connectIP.text = PlayerPrefs.GetString(IPPrefName);
        newWorldToggle.onValueChanged.AddListener(SetNewWorldToggle);
    }

    public override void OnServerConnect(NetworkConnection conn)
    {
        if(NetBridge.Instance.onServerConnect != null)
        {
            NetBridge.Instance.onServerConnect(conn);
        }
    }

    public override void OnClientConnect(NetworkConnection conn)
    {
        hud.enabled = false;
    }

    public void HostGame(string name)
    {
        serverName = name;
        StartHost();
    }

    public void JoinGame()
    {
        networkAddress = connectIP.text;
        PlayerPrefs.SetString(IPPrefName, networkAddress);
        StartClient();
    }

    public void SetNewWorldToggle(bool b)
    {
        NetBridge.Instance.makeNewWorld = b;
    }

}
