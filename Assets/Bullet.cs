using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Bullet : NetworkBehaviour {

    public float damage = 10.0f;
    private bool hit = false;

    private void OnCollisionEnter(Collision collision)
    {
        if(isServer && !hit && collision.transform.tag == "Player")
        {
            hit = true;
            PlayerController p = collision.transform.GetComponent<PlayerController>();
            p.CmdDamage(damage);
            Destroy(gameObject);
        }
    }

}
