using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Cube Model")]
public class CubeModel : ScriptableObject {

    public Vector3[] cubeVertices =
    {
        new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1), //front
        new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0), //back
        new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1), //top
        new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1), //bot
        new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), //right
        new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1)  //left
    };

    public int[] cubeTris =
    {
        0, 1, 2,       2, 3, 0,    //front
        4, 5, 6,       6, 7, 4,    //back
        8, 9, 10,      10, 11, 8,  //top
        12, 13, 14,    14, 15, 12, //bot
        16, 17, 18,    18, 19, 16, //right
        20, 21, 22,    22, 23, 20  //left
    };

    public Vector2[] cubeUv =
    {
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(0, 1)
    };

    public Vector3[] cubeNormal =
    {
        Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
        -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
        Vector3.up, Vector3.up, Vector3.up, Vector3.up,
        -Vector3.up, -Vector3.up, -Vector3.up, -Vector3.up,
        Vector3.right, Vector3.right, Vector3.right, Vector3.right,
        -Vector3.right, -Vector3.right, -Vector3.right, -Vector3.right
    };

    public Texture blockAtlas;
    public Vector2 atlasSize;

}
