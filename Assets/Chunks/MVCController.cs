using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}


public class MVCController : MonoBehaviour {

    public float moveSpeed;
    public float jumpForce;
    public float gravForce;
    public float camSensitivity;
    public float camPitchClamp1 = 271.0f;
    public float camPitchClamp2 = 89.0f;
    public float interactRange;


    public WorldMaker world;
    public CharacterController cc;
    public Transform avatar;
    public Transform camTrans;
    public Camera cam;

    private float yvel = 0;

    private int blockType = 1;
    private int numOfBlockTypes = 2;

    public delegate void colliderHitHack(ControllerColliderHit hit);
    public colliderHitHack ColliderHitHack;



    private void Awake()
    {
        ColliderHitHack = onControllerColliderHit;
    }

    void Update () {

        Cursor.lockState = CursorLockMode.Locked;

        MVCInput input = DoInput();



        blockType += input.blockTypeChange;
        if(blockType > numOfBlockTypes)
        {
            blockType = 1;
        }
        else if(blockType < 1)
        {
            blockType = numOfBlockTypes;
        }

        if(input.blockTypeChange != 0)
        {
            UIBridge.Instance.OnBlockSelectionChange(blockType);
        }



        if(input.destroy)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, interactRange))
            {
                Vector3 blockpos = hit.point;
                if(blockpos.x % 1.0 == 0 && ray.direction.x < 0)
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
        if(input.place)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
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



        if (!cc.isGrounded)
        {
            yvel -= gravForce * Time.deltaTime;
        }
        else if (input.jump)
        {
            yvel = jumpForce;
        }
        else
        {
            yvel = 0;
        }

        float ymove = yvel * Time.deltaTime;

        Vector3 moveVec = avatar.up * ymove;
        moveVec += avatar.right * input.moveX;
        moveVec += avatar.forward * input.moveZ;

        cc.Move(moveVec);

        avatar.eulerAngles += Vector3.up * input.rotYaw;
        Vector3 camAng = camTrans.eulerAngles;
        camAng += Vector3.right * input.rotPitch;
        if(camAng.x > 180.0f)
        {
            camAng.x = Mathf.Clamp(camAng.x, camPitchClamp1, 360.0f);
        }
        else if(camAng.x < 10.0f)
        {
            camAng.x = Mathf.Clamp(camAng.x, -10.0f, 10.0f);
        }
        else
        {
            camAng.x = Mathf.Clamp(camAng.x, 0.0f, camPitchClamp2);
        }
        camTrans.eulerAngles = camAng;

	}

    private MVCInput DoInput()
    {
        MVCInput input;
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

    private void onControllerColliderHit(ControllerColliderHit hit)
    {
        if(hit.normal.y == -1.0f && yvel > 0)
        {
            yvel = 0;
        }
    }

}
