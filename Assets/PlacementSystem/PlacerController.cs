using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlacerController : NetworkBehaviour {

    public float mouseSensitivity = 5.0f;
    public float moveSpeed = 50.0f;
    public float jumpForce = 10.0f;
    public float gravForce = 1.0f;
    public float worldFloor = -5.0f;

    public GameObject prefab;
    public Material ghostMat;
    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;
    public Color[] paints;

    public delegate void OnColorChange();
    public OnColorChange onColorChange;

    private GameObject ghost = null;
    private float yvel = 0;
    private Color artColor;
    private int paintIdx = 0;


	
	void Start ()
    {
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            ls.enabled = false;
            return;
        }

        GlobalVars.localPlacer = this;

        ghost = Instantiate(prefab);
        ghost.GetComponentInChildren<Renderer>().material = ghostMat;
        ghost.GetComponentInChildren<Collider>().enabled = false;

        SetArtColor(paints[paintIdx]);

        Cursor.lockState = CursorLockMode.Locked;
	}
	
	
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;


        if(Input.GetAxisRaw("Mouse ScrollWheel") > 0)
        {
            ++paintIdx;
            if(paintIdx >= paints.Length)
            {
                paintIdx = 0;
            }
            SetArtColor(paints[paintIdx]);
        }
        else if(Input.GetAxisRaw("Mouse ScrollWheel") < 0)
        {
            --paintIdx;
            if(paintIdx < 0)
            {
                paintIdx = paints.Length - 1;
            }
            SetArtColor(paints[paintIdx]);
        }


        if (!cc.isGrounded)
        {
            yvel -= gravForce * Time.deltaTime;
        }
        else
        {
            yvel = 0;
        }

        if(Input.GetAxisRaw("Jump") > 0 && cc.isGrounded)
        {
            yvel += jumpForce;
        }
        
        Vector3 movex = transform.right * Input.GetAxis("Horizontal") * moveSpeed * Time.deltaTime;
        Vector3 movez = transform.forward * Input.GetAxis("Vertical") * moveSpeed * Time.deltaTime;
        Vector3 movey = Vector3.up * yvel * Time.deltaTime;
        
        cc.Move(movex + movez + movey);

        if(transform.position.y < worldFloor)
        {
            transform.position = Vector3.zero;
        }



        float roty = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float rotx = -Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        transform.eulerAngles += Vector3.up * roty;
        cam.transform.eulerAngles += Vector3.right * rotx;
        
        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        
        if(Physics.Raycast(ray, out hit))
        {
            ghost.SetActive(true);
            
            ghost.transform.position = hit.point;
            ghost.transform.LookAt(hit.point + hit.normal);
            ghost.transform.Rotate(Vector3.right * 90);

            if(Input.GetMouseButtonDown(0))
            {
                CmdSpawnArt(hit.point, ghost.transform.rotation, paints[paintIdx]);
            }
            else if(Input.GetMouseButtonDown(1) && hit.transform.tag == "Art")
            {
                Destroy(hit.transform.parent.gameObject);
            }
            else if(Input.GetMouseButtonDown(2) && hit.transform.tag == "Art" && paints.Length > 0)
            {
                CmdPaintArt(hit.transform.parent.gameObject, paints[paintIdx]);
            }
        }
        else
        {
            ghost.SetActive(false);
        }
	}


    private void SetArtColor(Color color)
    {
        artColor = color;
        ghost.GetComponentInChildren<Renderer>().material.color = new Color(color.r, color.g, color.b, 0.5f);
        if(onColorChange != null)
        {
            onColorChange();
        }
    }

    public Color GetArtColor()
    {
        return paints[paintIdx];
    }


    [Command]
    private void CmdPaintArt(GameObject obj, Color color)
    {
        obj.GetComponent<ArtPiece>().CmdSetColor(color);
    }

    [Command]
    private void CmdSpawnArt(Vector3 pos, Quaternion rot, Color color)
    {
        GameObject art = Instantiate(prefab, pos, rot);
        art.GetComponentInChildren<Renderer>().material.color = color;
        //art.GetComponent<ArtPiece>().CmdSetColor(paints[paintIdx]);
        //CmdPaintArt(art);

        NetworkServer.Spawn(art);
    }
}
