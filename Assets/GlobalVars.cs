using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour {

    public static PlayerController localPlayer = null;
    public static ObjectPool BulletPool { get { return bulletPool; } }
    private static ObjectPool bulletPool;

    public ObjectPool bulletPoolObj;

    private void Start()
    {
        bulletPool = bulletPoolObj;
    }

}
