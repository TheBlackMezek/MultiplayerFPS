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
    public float maxHp = 100.0f;
    public float bulletSpeed = 10.0f;

    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;
    public GameObject bulletPrefab;

    private PlayerState lastVerifiedState;
    private List<PlayerInput> inputs = new List<PlayerInput>();
    private List<InputPacket> packets = new List<InputPacket>();
    private int inputId = 0;
    private int inputIdMax = 10000;

    [SyncVar]
    private float hp;



    private void Start()
    {
        lastVerifiedState.id = 0;
        lastVerifiedState.position = transform.position;
        lastVerifiedState.euler = transform.eulerAngles;

        hp = maxHp;

        if(!isLocalPlayer)
        {
            cam.enabled = false;
            ls.enabled = false;
        }
        else
        {
            Cursor.lockState = CursorLockMode.Locked;
            GlobalVars.localPlayer = this;
        }
    }

    private void Update()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        PlayerInput input;

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

        if(Input.GetMouseButtonDown(0))
        {
            CmdShoot();
        }
    }

    private void FixedUpdate()
    {
        if(!isLocalPlayer || inputs.Count <= 0)
        {
            return;
        }

        if(isServer)
        {
            PlayerState state;
            state.id = inputId;
            state.position = transform.position;
            state.euler = transform.eulerAngles;
            RpcVerifyState(state);
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
        if(isServer)
        {
            return;
        }
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

    [Command]
    public void CmdShoot()
    {
        GameObject b = Instantiate(bulletPrefab);
        b.transform.position = transform.position + transform.forward * 2;
        b.transform.eulerAngles = transform.eulerAngles;
        Rigidbody bbody = b.GetComponent<Rigidbody>();
        bbody.AddForce(transform.forward * bulletSpeed, ForceMode.Impulse);

        NetworkServer.Spawn(b);
    }

    [Command]
    public void CmdDamage(float amt)
    {
        hp -= amt;
        
        if(hp <= 0)
        {
            CmdKill();
        }
    }

    [Command]
    public void CmdKill()
    {
        hp = maxHp;
        transform.position = new Vector3(0, 10, 0);
    }

    public float GetHealth()
    {
        return hp;
    }

}
