using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ArtPiece : NetworkBehaviour {

    public Renderer renderer;
    [SyncVar]
    private Color matCol;



    public override void OnStartClient()
    {
        if (matCol != null)
        {
            renderer.material.color = matCol;
        }
    }

    public override void OnStartServer()
    {
        matCol = renderer.material.color;
        //CmdSetColorsOnStart();
    }

    [Command]
    public void CmdSetColorsOnStart()
    {
        RpcSetColor(renderer.material.color);
    }

    [Command]
    public void CmdSetColor(Color c)
    {
        renderer.material.color = c;
        matCol = renderer.material.color;
        RpcSetColor(renderer.material.color);
    }

    [ClientRpc]
    public void RpcSetColor(Color c)
    {
        renderer.material.color = c;
    }

}
