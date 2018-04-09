using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectPool : NetworkBehaviour {

    public GameObject prefab;
    public int initialPoolSize = 10;
    private int poolSize;

    private void Start()
    {
        if (!isServer)
        {
            return;
        }

        poolSize = initialPoolSize;

        for(int i = 0; i < initialPoolSize; ++i)
        {
            GameObject o = Instantiate(prefab);
            o.SetActive(false);
            o.transform.parent = transform;
            NetworkServer.Spawn(o);
        }
    }

    
    public GameObject GetObject()
    {
        foreach(Transform child in transform)
        {
            if(!child.gameObject.activeInHierarchy)
            {
                return child.gameObject;
            }
        }

        for (int i = 0; i < poolSize; ++i)
        {
            GameObject o = Instantiate(prefab);
            o.SetActive(false);
            o.transform.parent = transform;
            NetworkServer.Spawn(o);
        }
        poolSize *= 2;

        return transform.GetChild(poolSize / 2).gameObject;
    }

}
