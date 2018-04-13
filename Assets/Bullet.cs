using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public float damage = 10.0f;
    public float lifetime = 5.0f;

    private bool hit = false;




    private void Start()
    {
        Invoke("Kill", lifetime);
    }

    public override void OnStartServer()
    {
        RpcSyncParent(transform.parent.gameObject);
    }

    private void Kill()
    {
        gameObject.SetActive(false);
        hit = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!hit)
        {
            hit = true;
            if (isServer && collision.transform.tag == "Player")
            {
                PlayerController p = collision.transform.parent.GetComponent<PlayerController>();
                p.CmdDamage(damage);
            }
            
            Kill();
        }
    }



    [ClientRpc]
    public void RpcSyncParent(GameObject p)
    {
        transform.parent = p.transform;
    }

}
