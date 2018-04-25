using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChunkNetManager : NetworkManager {

    public NetworkManagerHUD hud;
    public NetworkDiscovery netDiscovery;

    private string serverName;




    private void Start()
    {
        netDiscovery.Initialize();
        netDiscovery.StartAsClient();
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

    

}
