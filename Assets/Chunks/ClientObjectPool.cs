using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientObjectPool : MonoBehaviour {

    public GameObject prefab;
    public int initialPoolSize = 10;

    private List<GameObject> pool = new List<GameObject>();
    private int poolSize;



    private void Start()
    {
        poolSize = initialPoolSize;

        for (int i = 0; i < initialPoolSize; ++i)
        {
            GameObject o = Instantiate(prefab);
            o.SetActive(false);
            pool.Add(o);
        }
    }

    public GameObject GetObject()
    {
        foreach (GameObject child in pool)
        {
            if (!child.activeInHierarchy)
            {
                return child.gameObject;
            }
        }

        for (int i = 0; i < poolSize; ++i)
        {
            GameObject o = Instantiate(prefab);
            o.SetActive(false);
            pool.Add(o);
        }

        poolSize *= 2;
        
        return pool[pool.Capacity / 2];
    }

}
