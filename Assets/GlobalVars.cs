using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GlobalVars : MonoBehaviour {

    //placer stuff
    public static PlacerController localPlacer = null;

    //FPS stuff
    public static PlayerController localPlayer = null;
    public static ObjectPool BulletPool { get { return bulletPool; } }
    private static ObjectPool bulletPool;

    public ObjectPool bulletPoolObj;

    private void Start()
    {
        bulletPool = bulletPoolObj;
    }

}
