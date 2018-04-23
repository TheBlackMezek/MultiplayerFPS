using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ChunkNetManager : NetworkManager {

    public override void OnServerConnect(NetworkConnection conn)
    {
        //base.OnClientConnect(conn);
        Debug.Log("CLIENT HAS CONNECTED");
        if(NetBridge.Instance.onServerConnect != null)
        {
            Debug.Log("ONCLIENTCONENCT EXISTS");
            NetBridge.Instance.onServerConnect(conn);
        }
    }

}
