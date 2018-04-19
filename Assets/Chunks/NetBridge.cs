using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public WorldMaker world;

}
