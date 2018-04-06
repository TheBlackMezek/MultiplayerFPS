using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public struct PlayerInput
{
    public int id;
    public Vector3 movement;
}

public struct InputPacket
{
    PlayerInput[] inputs;
}

public struct PlayerState
{
    public int id;
    public Vector3 position;
    public Vector3 euler;
}


public class PlayerController : NetworkBehaviour {

    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;

    private PlayerState lastVerifiedState;
    private List<PlayerInput> inputs;
    private int ksadhaljskd;



    private void Start()
    {
        lastVerifiedState.id = 0;
        lastVerifiedState.position = transform.position;
        lastVerifiedState.euler = transform.eulerAngles;

        if(!isLocalPlayer)
        {
            cam.enabled = false;
            ls.enabled = false;
        }
    }

    private void Update()
    {
        
    }

}
