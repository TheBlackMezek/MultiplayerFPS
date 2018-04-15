using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PlacerController : NetworkBehaviour {

    public float mouseSensitivity = 5.0f;
    public float moveSpeed = 50.0f;
    public float jumpForce = 10.0f;
    public float gravForce = 1.0f;

    public GameObject prefab;
    public Material ghostMat;
    public Camera cam;
    public AudioListener ls;
    public CharacterController cc;
    public Material[] paintMats;

    private GameObject ghost = null;
    private float yvel = 0;


	
	void Start ()
    {
        if (!isLocalPlayer)
        {
            cam.enabled = false;
            ls.enabled = false;
            return;
        }
        ghost = Instantiate(prefab);
        ghost.GetComponentInChildren<Renderer>().material = ghostMat;
        ghost.GetComponentInChildren<Collider>().enabled = false;

        Cursor.lockState = CursorLockMode.Locked;
	}
	
	
	void Update () {
        if(!isLocalPlayer)
        {
            return;
        }

        if(!cc.isGrounded)
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
                CmdSpawnArt(hit.point, ghost.transform.rotation);
                //GameObject o = Instantiate(prefab);
                //
                //o.transform.position = hit.point;
                //o.transform.rotation = ghost.transform.rotation;
                //
                //NetworkServer.Spawn(o);
            }
            else if(Input.GetMouseButtonDown(1) && hit.transform.tag == "Art")
            {
                Destroy(hit.transform.parent.gameObject);
            }
            else if(Input.GetMouseButtonDown(2) && hit.transform.tag == "Art" && paintMats.Length > 0)
            {
                CmdModArt(hit.transform.parent.gameObject);
                //hit.transform.GetComponent<Renderer>().material = paintMats[Random.Range(0, paintMats.Length)];
                //hit.transform.parent.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
                //hit.transform.parent.GetComponent<ArtPiece>().CmdSetColor(paintMats[Random.Range(0, paintMats.Length)].color);
                //hit.transform.parent.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
            }
        }
        else
        {
            ghost.SetActive(false);
        }
	}

    [Command]
    private void CmdModArt(GameObject obj)
    {
        //obj.GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        obj.GetComponent<ArtPiece>().CmdSetColor(paintMats[Random.Range(0, paintMats.Length)].color);
        //obj.GetComponent<NetworkIdentity>().RemoveClientAuthority(connectionToClient);
    }

    [Command]
    private void CmdSpawnArt(Vector3 pos, Quaternion rot)
    {
        GameObject art = Instantiate(prefab, pos, rot);

        NetworkServer.Spawn(art);
    }
}
