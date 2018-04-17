using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct MVCInput
{
    public float moveX;
    public float moveZ;

    public float rotPitch;
    public float rotYaw;

    public bool jump;
    public bool destroy;
    public bool place;
}


public class MVCController : MonoBehaviour {

    public float moveSpeed;
    public float jumpForce;
    public float gravForce;
    public float camSensitivity;
    public float interactRange;


    public WorldMaker world;
    public CharacterController cc;
    public Transform avatar;
    public Transform camTrans;
    public Camera cam;

    private float yvel = 0;

    
	
	void Update () {

        Cursor.lockState = CursorLockMode.Locked;

        MVCInput input = DoInput();



        



        if(input.destroy)
        {
            RaycastHit hit;
            Ray ray = cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
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
        camTrans.eulerAngles += Vector3.right * input.rotPitch;

	}

    private MVCInput DoInput()
    {
        MVCInput input;
        input.moveX = moveSpeed * Time.deltaTime * Input.GetAxis("Horizontal");
        input.moveZ = moveSpeed * Time.deltaTime * Input.GetAxis("Vertical");
        input.rotPitch = camSensitivity * Time.deltaTime * -Input.GetAxis("Mouse Y");
        input.rotYaw = camSensitivity * Time.deltaTime * Input.GetAxis("Mouse X");
        input.jump = Input.GetAxis("Jump") > 0;
        input.destroy = Input.GetMouseButtonDown(0);
        input.place = Input.GetMouseButtonDown(1);
        return input;
    }

}
