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

    public GameObject[] prefabs;
    public Color[] paints;
    public Material ghostMat;
    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;

    public delegate void OnColorChange();
    public OnColorChange onColorChange;

    private GameObject ghost = null;
    private float yvel = 0;
    private Color artColor;
    private int prefabIdx = 0;
    private int paintIdx = 0;
    private bool snapOn = false;
    private float snapSize = 1.0f;
    private float pieceHeight;


	
	void Start ()
    {
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            ls.enabled = false;
            return;
        }

        GlobalVars.localPlacer = this;

        MakeGhost();
        pieceHeight = prefabs[prefabIdx].GetComponent<ArtPiece>().height;

        Cursor.lockState = CursorLockMode.Locked;
	}
	
	
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;



        if(Input.GetKeyDown(KeyCode.E))
        {
            ++prefabIdx;
            if (prefabIdx >= prefabs.Length)
            {
                prefabIdx = 0;
            }
            pieceHeight = prefabs[prefabIdx].GetComponent<ArtPiece>().height;
            MakeGhost();
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            --prefabIdx;
            if (prefabIdx < 0)
            {
                prefabIdx = prefabs.Length - 1;
            }
            pieceHeight = prefabs[prefabIdx].GetComponent<ArtPiece>().height;
            MakeGhost();
        }

        if (Input.GetAxisRaw("Mouse ScrollWheel") > 0)
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



        if(Input.GetKey(KeyCode.LeftShift))
        {
            snapOn = true;
        }
        else
        {
            snapOn = false;
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
            ghost.transform.position += ghost.transform.up * (pieceHeight / 2.0f);
            if (snapOn)
            {
                Vector3 pos = ghost.transform.position;
                ghost.transform.position = new Vector3(Mathf.Round(pos.x / snapSize) * snapSize,
                                                       Mathf.Round(pos.y / snapSize) * snapSize,
                                                       Mathf.Round(pos.z / snapSize) * snapSize);
            }

            if(Input.GetMouseButtonDown(0))
            {
                CmdSpawnArt(prefabIdx, ghost.transform.position, ghost.transform.rotation, paints[paintIdx]);
            }
            else if(Input.GetMouseButtonDown(1) && hit.transform.tag == "Art")
            {
                if (hit.transform.GetComponent<PlacerController>() == null)
                {
                    CmdDestroyArt(hit.transform.parent.gameObject);
                }
            }
            else if(Input.GetMouseButtonDown(2) && hit.transform.tag == "Art" && paints.Length > 0)
            {
                if(hit.transform.GetComponent<PlacerController>() != null)
                {
                    CmdPaintArt(hit.transform.gameObject, paints[paintIdx]);
                }
                else
                {
                    CmdPaintArt(hit.transform.parent.gameObject, paints[paintIdx]);
                }
            }
        }
        else
        {
            ghost.SetActive(false);
        }
	}


    private void MakeGhost()
    {
        if(ghost != null)
        {
            Destroy(ghost);
        }
        ghost = Instantiate(prefabs[prefabIdx]);
        ghost.GetComponentInChildren<Renderer>().material = ghostMat;
        ghost.GetComponentInChildren<Collider>().enabled = false;

        SetArtColor(paints[paintIdx]);
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
    private void CmdDestroyArt(GameObject art)
    {
        NetworkServer.Destroy(art);
    }
    
    [Command]
    private void CmdPaintArt(GameObject obj, Color color)
    {
        obj.GetComponent<ArtPiece>().CmdSetColor(color);
    }

    [Command]
    private void CmdSpawnArt(int pfidx, Vector3 pos, Quaternion rot, Color color)
    {
        GameObject art = Instantiate(prefabs[pfidx], pos, rot);
        art.GetComponentInChildren<Renderer>().material.color = color;

        NetworkServer.Spawn(art);
    }
}
