using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjectPool : NetworkBehaviour {

    public GameObject prefab;
    public int initialPoolSize = 10;
    public Transform poolHolder;
    private int poolSize;

    private void Start()
    {
        //Debug.Log(isServer);
        //if (!isServer)
        //{
        //    return;
        //}
        //
        //poolSize = initialPoolSize;
        //
        //for(int i = 0; i < initialPoolSize; ++i)
        //{
        //    GameObject o = Instantiate(prefab);
        //    o.SetActive(false);
        //    o.transform.parent = poolHolder;
        //    NetworkServer.Spawn(o);
        //}
        
    }

    public override void OnStartServer()
    {
        poolSize = initialPoolSize;

        for (int i = 0; i < initialPoolSize; ++i)
        {
            GameObject o = Instantiate(prefab);
            o.SetActive(false);
            o.transform.parent = poolHolder;
            NetworkServer.Spawn(o);
        }
    }



    public GameObject GetObject()
    {
        foreach(Transform child in poolHolder)
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
            o.transform.parent = poolHolder;
            NetworkServer.Spawn(o);
        }
        poolSize *= 2;

        if(isServer)
        {
            RpcIncreasePoolSize();
        }

        return poolHolder.GetChild(poolSize / 2).gameObject;
    }

    [ClientRpc]
    public void RpcIncreasePoolSize()
    {
        //for (int i = 0; i < poolsize; ++i)
        //{
        //    gameobject o = instantiate(prefab);
        //    o.setactive(false);
        //    o.transform.parent = poolHolder;
        //    networkserver.spawn(o);
        //}
        if (!isServer)
        {
            poolSize *= 2;
        }

    }

}
