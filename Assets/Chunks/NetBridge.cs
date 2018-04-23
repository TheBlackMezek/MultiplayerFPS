using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class NetBridge {

    private static NetBridge instance;

    public static NetBridge Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new NetBridge();
            }
            return instance;
        }
    }



    public delegate void OnServerConnect(NetworkConnection connection);

    public OnServerConnect onServerConnect;


    public WorldMaker world;

}
