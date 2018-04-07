using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;


public struct PlayerInput
{
    public Vector3 movement;
    public Vector3 rotation;
}

public struct InputPacket
{
    public int id;
    public PlayerInput[] inputs;
}

public struct PlayerState
{
    public int id;
    public Vector3 position;
    public Vector3 euler;
}


public class PlayerController : NetworkBehaviour {

    public float moveSpeed = 1.0f;
    public float mouseSensitivity = 10.0f;

    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;

    private PlayerState lastVerifiedState;
    private List<PlayerInput> inputs = new List<PlayerInput>();
    private List<InputPacket> packets = new List<InputPacket>();
    private int inputId = 0;
    private int inputIdMax = 10000;



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
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        PlayerInput input;

        //input.id = inputId;

        //++inputId;
        //if(inputId > inputIdMax)
        //{
        //    inputId = 0;
        //}

        float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
        float z = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
        Vector3 move = transform.right * x + transform.forward * z;

        float yaw = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float pitch = -Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
        Vector3 turn = new Vector3(pitch, yaw, 0);
        
        input.movement = move;
        input.rotation = turn;

        ProcessInput(input);

        inputs.Add(input);
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(inputs.Count <= 0)
        {
            return;
        }

        InputPacket packet;
        packet.inputs = new PlayerInput[inputs.Count];

        for(int i = 0; i < inputs.Count; ++i)
        {
            packet.inputs[i] = inputs[i];
        }

        inputs.Clear();


        packet.id = inputId;
        packets.Add(packet);

        ++inputId;
        if (inputId > inputIdMax)
        {
            inputId = 0;
        }

        CmdTakeInputPacket(packet);
    }

    private void ProcessInputPacket(InputPacket packet)
    {
        foreach (PlayerInput i in packet.inputs)
        {
            ProcessInput(i);
        }
    }

    private void ProcessInput(PlayerInput input)
    {
        cc.SimpleMove(input.movement);
        transform.eulerAngles += input.rotation;
    }

    [Command]
    public void CmdTakeInputPacket(InputPacket packet)
    {
        if(packet.inputs.Length > 0)
        {
            ProcessInputPacket(packet);

            PlayerState state;
            state.euler = transform.eulerAngles;
            state.position = transform.position;
            state.id = packet.id;

            RpcVerifyState(state);
        }
    }
    
    [ClientRpc]
    public void RpcVerifyState(PlayerState state)
    {
        lastVerifiedState = state;

        transform.position = lastVerifiedState.position;
        transform.eulerAngles = lastVerifiedState.euler;

        for (int i = 0; i < packets.Count; ++i)
        {
            if(packets[i].id == state.id)
            {
                packets.RemoveRange(0, i + 1);
                break;
            }
        }

        foreach(InputPacket p in packets)
        {
            ProcessInputPacket(p);
        }

    }

}
