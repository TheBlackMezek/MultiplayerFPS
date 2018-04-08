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

    private void Kill()
    {
        Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(isServer && !hit)
        {
            hit = true;
            if (collision.transform.tag == "Player")
            {
                PlayerController p = collision.transform.GetComponent<PlayerController>();
                p.CmdDamage(damage);
            }
            Destroy(gameObject);
        }
    }

}
