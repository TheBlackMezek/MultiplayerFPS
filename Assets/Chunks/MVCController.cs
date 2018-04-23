using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public struct MVCInput
{
    public float moveX;
    public float moveZ;

    public float rotPitch;
    public float rotYaw;

    public int blockTypeChange;

    public bool jump;
    public bool destroy;
    public bool place;

    public float dt;
}

public struct MVCState
{
    public int id;
    public Vector3 pos;
    public Vector3 eulerAvatar;
    public Vector3 eulerCam;
    public float yvel;
    public int blockType;
}

public struct MVCInputPacket
{
    public int id;
    public MVCInput[] inputs;
}


public class MVCController : NetworkBehaviour {

    public float moveSpeed;
    public float jumpForce;
    public float gravForce;
    public float camSensitivity;
    public float camPitchClamp1 = 271.0f;
    public float camPitchClamp2 = 89.0f;
    public float interactRange;
    public float worldBottom = 0;
    
    public CharacterController cc;
    public Transform avatar;
    public Transform camTrans;
    public Camera cam;
    public AudioListener ls;


    private float yvel = 0;
    private int blockType = 1;
    private int numOfBlockTypes = 2;

    private WorldMaker world;

    private MVCState lastVerifiedState;
    private List<MVCInput> inputs = new List<MVCInput>();
    private int packetId = 0;
    private int packetIdCap = 10000;
    private List<MVCInputPacket> sentPackets = new List<MVCInputPacket>();

    public delegate void colliderHitHack(ControllerColliderHit hit);
    public colliderHitHack ColliderHitHack;



    private void Awake()
    {
        ColliderHitHack = onControllerColliderHit;
    }

    private void Start()
    {
        world = NetBridge.Instance.world;
        transform.position = world.GetPlayerSpawn();

        if (!isLocalPlayer)
        {
            cam.enabled = false;
            ls.enabled = false;
        }
        else
        {
            lastVerifiedState.pos = transform.position;
            lastVerifiedState.eulerAvatar = avatar.eulerAngles;
            lastVerifiedState.eulerCam = camTrans.eulerAngles;
            lastVerifiedState.yvel = yvel;
            lastVerifiedState.id = packetId;
            lastVerifiedState.blockType = blockType;
        }
    }

    void Update () {
        if(!isLocalPlayer)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;

        MVCInput input = GetInput();



        ProcessInputAndMotion(input);
        if(!isServer)
        {
            inputs.Add(input);
        }


        //ProcessInteractionInput(input);
        
	}

    private void FixedUpdate()
    {
        if(!isLocalPlayer)
        {
            return;
        }

        if(isServer)
        {
            MVCState state;
            state.id = packetId;
            state.pos = avatar.position;
            state.eulerAvatar = avatar.eulerAngles;
            state.eulerCam = camTrans.eulerAngles;
            state.yvel = yvel;
            state.blockType = blockType;
            RpcReconcileState(state);
            return;
        }

        MVCInputPacket packet;
        packet.id = packetId;
        packet.inputs = inputs.ToArray();
        inputs.Clear();

        CmdVerifyInputs(packet);
        sentPackets.Add(packet);
        
        ++packetId;
        if(packetId > packetIdCap)
        {
            packetId = 0;
        }
    }

    private MVCInput GetInput()
    {
        MVCInput input;

        input.dt = Time.deltaTime;

        input.moveX = moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        input.moveZ = moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");

        input.rotPitch = camSensitivity * -Input.GetAxis("Mouse Y");
        input.rotYaw = camSensitivity * Input.GetAxis("Mouse X");

        input.blockTypeChange = 0;
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll > 0)
        {
            input.blockTypeChange = 1;
        }
        else if(scroll < 0)
        {
            input.blockTypeChange = -1;
        }
        
        input.jump = Input.GetAxis("Jump") > 0;
        input.destroy = Input.GetMouseButtonDown(0);
        input.place = Input.GetMouseButtonDown(1);
        return input;
    }

    private void ProcessInputAndMotion(MVCInput input, bool doInteractionInput = true)
    {
        if (!cc.isGrounded)
        {
            yvel -= gravForce * input.dt;
        }
        else if (input.jump)
        {
            yvel = jumpForce;
        }
        else
        {
            yvel = 0;
        }

        Vector3 move = avatar.up * yvel * input.dt;
        move += avatar.forward * input.moveZ;
        move += avatar.right * input.moveX;
        cc.Move(move);

        if(avatar.position.y < worldBottom)
        {
            avatar.position = world.GetPlayerSpawn();
        }



        avatar.eulerAngles += Vector3.up * input.rotYaw;
        Vector3 camAng = camTrans.eulerAngles;
        camAng += Vector3.right * input.rotPitch;
        if (camAng.x > 180.0f)
        {
            camAng.x = Mathf.Clamp(camAng.x, camPitchClamp1, 360.0f);
        }
        else if (camAng.x < 10.0f)
        {
            camAng.x = Mathf.Clamp(camAng.x, -10.0f, 10.0f);
        }
        else
        {
            camAng.x = Mathf.Clamp(camAng.x, 0.0f, camPitchClamp2);
        }
        camTrans.eulerAngles = camAng;



        blockType += input.blockTypeChange;
        if (blockType > numOfBlockTypes)
        {
            blockType = 1;
        }
        else if (blockType < 1)
        {
            blockType = numOfBlockTypes;
        }

        if (isLocalPlayer && input.blockTypeChange != 0)
        {
            UIBridge.Instance.OnBlockSelectionChange(blockType);
        }


        if (doInteractionInput)
        {
            ProcessInteractionInput(input);
        }
    }

    private void ProcessInteractionInput(MVCInput input)
    {
        if (input.destroy)
        {
            RaycastHit hit;
            Ray ray = new Ray(camTrans.position, camTrans.forward);
            if (Physics.Raycast(ray, out hit, interactRange))
            {
                Vector3 blockpos = hit.point;
                if (blockpos.x % 1.0 == 0 && ray.direction.x < 0)
                {
                    --blockpos.x;
                }
                if (blockpos.y % 1.0 == 0 && ray.direction.y < 0)
                {
                    --blockpos.y;
                }
                if (blockpos.z % 1.0 == 0 && ray.direction.z < 0)
                {
                    --blockpos.z;
                }
                blockpos.x = Mathf.Floor(blockpos.x);
                blockpos.y = Mathf.Floor(blockpos.y);
                blockpos.z = Mathf.Floor(blockpos.z);
                world.DestroyBlock(blockpos);
            }
        }
        if (input.place)
        {
            RaycastHit hit;
            Ray ray = new Ray(camTrans.position, camTrans.forward);
            if (Physics.Raycast(ray, out hit, interactRange))
            {
                Vector3 blockpos = hit.point;
                if (blockpos.x % 1.0 == 0 && ray.direction.x > 0)
                {
                    --blockpos.x;
                }
                if (blockpos.y % 1.0 == 0 && ray.direction.y > 0)
                {
                    --blockpos.y;
                }
                if (blockpos.z % 1.0 == 0 && ray.direction.z > 0)
                {
                    --blockpos.z;
                }
                blockpos.x = Mathf.Floor(blockpos.x);
                blockpos.y = Mathf.Floor(blockpos.y);
                blockpos.z = Mathf.Floor(blockpos.z);
                world.AddBlock(blockpos, blockType);
            }
        }
    }

    [Command]
    private void CmdVerifyInputs(MVCInputPacket packet)
    {
        foreach (MVCInput input in packet.inputs)
        {
            ProcessInputAndMotion(input);
        }
        MVCState state;
        state.id = packet.id;
        state.pos = avatar.position;
        state.eulerAvatar = avatar.eulerAngles;
        state.eulerCam = camTrans.eulerAngles;
        state.yvel = yvel;
        state.blockType = blockType;
        RpcReconcileState(state);
    }

    [ClientRpc]
    private void RpcReconcileState(MVCState state)
    {
        if(isServer)
        {
            return;
        }

        for(int i = 0; i < sentPackets.Count; ++i)
        {
            if(sentPackets[i].id == state.id)
            {
                sentPackets.RemoveRange(0, i + 1);
                break;
            }
        }

        lastVerifiedState = state;

        avatar.position = state.pos;
        avatar.eulerAngles = state.eulerAvatar;
        camTrans.eulerAngles = state.eulerCam;
        yvel = state.yvel;
        blockType = state.blockType;
        
        foreach(MVCInputPacket packet in sentPackets)
        {
            foreach(MVCInput input in packet.inputs)
            {
                ProcessInputAndMotion(input, false);
            }
        }
    }

    private void onControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.normal.y == -1.0f && yvel > 0)
        {
            yvel = 0;
        }
    }

}
