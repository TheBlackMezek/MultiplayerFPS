using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LanDiscovery : NetworkDiscovery {

    public delegate void onReceiveBroadcast(string fromAddress, string data);
    public onReceiveBroadcast OnReceiveBroadcast = null;



    public override void OnReceivedBroadcast(string fromAddress, string data)
    {
        Debug.Log(data);
        if(OnReceiveBroadcast != null)
        {
            OnReceiveBroadcast(fromAddress, data);
        }
        base.OnReceivedBroadcast(fromAddress, data);
    }

}
