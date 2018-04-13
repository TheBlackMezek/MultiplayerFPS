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
    public float jumpForce = 100.0f;
    public float gravForce = 15.0f;
    public float fireCooldown = 0.25f;
    public float maxPitch = 89.0f;
    public float minPitch = -89.0f;

    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;
    public GameObject avatar;
    public ObjectPool bulletPool;

    private PlayerState lastVerifiedState;
    private List<PlayerInput> inputs = new List<PlayerInput>();
    private List<InputPacket> packets = new List<InputPacket>();
    private int inputId = 0;
    private int inputIdMax = 10000;
    private float yVel = 0;
    private float gunHeat = 0;

    [SyncVar]
    private float hp;



    private void Start()
    {
        lastVerifiedState.id = 0;
        lastVerifiedState.position = avatar.transform.position;
        lastVerifiedState.euler = avatar.transform.eulerAngles;

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

        if(!cc.isGrounded)
        {
            yVel -= Time.deltaTime * gravForce;
        }
        else
        {
            yVel = 0;
        }

        PlayerInput input;

        float x = Input.GetAxisRaw("Horizontal") * Time.deltaTime * moveSpeed;
        float z = Input.GetAxisRaw("Vertical") * Time.deltaTime * moveSpeed;
        Vector3 move = avatar.transform.right * x + avatar.transform.forward * z;

        if(Input.GetAxisRaw("Jump") > 0 && cc.isGrounded)
        {
            yVel = jumpForce;
        }
        move += Vector3.up * Time.deltaTime * yVel;

        float yaw = Input.GetAxis("Mouse X") * Time.deltaTime * mouseSensitivity;
        float pitch = -Input.GetAxis("Mouse Y") * Time.deltaTime * mouseSensitivity;
        Vector3 turn = new Vector3(pitch, yaw, 0);
        
        input.movement = move;
        input.rotation = turn;

        ProcessInput(input);
        
        inputs.Add(input);


        if(gunHeat > 0)
        {
            gunHeat -= Time.deltaTime;
        }

        if(Input.GetMouseButton(0) && gunHeat <= 0)
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
            state.position = avatar.transform.position;
            state.euler = avatar.transform.eulerAngles;
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
        cc.Move(input.movement);
        avatar.transform.eulerAngles += input.rotation;
        
        if(avatar.transform.eulerAngles.x < maxPitch && avatar.transform.eulerAngles.x > minPitch)
        {
            if (input.rotation.x < 0)
            {
                avatar.transform.eulerAngles = new Vector3(maxPitch, avatar.transform.eulerAngles.y, avatar.transform.eulerAngles.z);
            }
            else
            {
                avatar.transform.eulerAngles = new Vector3(minPitch, avatar.transform.eulerAngles.y, avatar.transform.eulerAngles.z);
            }
        }
    }

    [Command]
    public void CmdTakeInputPacket(InputPacket packet)
    {
        if(packet.inputs.Length > 0)
        {
            ProcessInputPacket(packet);

            PlayerState state;
            state.euler = avatar.transform.eulerAngles;
            state.position = avatar.transform.position;
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

        avatar.transform.position = lastVerifiedState.position;
        avatar.transform.eulerAngles = lastVerifiedState.euler;

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
        gunHeat = fireCooldown;

        //GameObject b = Instantiate(bulletPrefab);
        //GameObject b = GlobalVars.BulletPool.GetObject();
        GameObject b = bulletPool.GetObject();
        b.transform.position = avatar.transform.position + avatar.transform.forward * 2;
        b.transform.eulerAngles = avatar.transform.eulerAngles;
        Rigidbody bbody = b.GetComponent<Rigidbody>();
        bbody.velocity = Vector3.zero;
        b.SetActive(true);
        bbody.AddForce(avatar.transform.forward * bulletSpeed, ForceMode.Impulse);

        //RpcShoot(b);
        //NetworkServer.Spawn(b);
    }

    [ClientRpc]
    public void RpcShoot(GameObject b)
    {
        if(isServer)
        {
            return;
        }
        b.transform.position = avatar.transform.position + avatar.transform.forward * 2;
        b.transform.eulerAngles = avatar.transform.eulerAngles;
        Rigidbody bbody = b.GetComponent<Rigidbody>();
        bbody.velocity = Vector3.zero;
        b.SetActive(true);
        bbody.AddForce(avatar.transform.forward * bulletSpeed, ForceMode.Impulse);
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
        avatar.transform.position = new Vector3(0, 10, 0);
    }

    public float GetHealth()
    {
        return hp;
    }

}
