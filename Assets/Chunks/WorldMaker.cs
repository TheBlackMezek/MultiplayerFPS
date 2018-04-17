using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{

    //public Mesh cubeMesh;
    public GameObject chunkPrefab;

    public int chunkSize = 16;
    public int worldSize = 3;


    private Vector3[] cubeVertices =
    {
        new Vector3(1, 0, 1), new Vector3(0, 0, 1), new Vector3(0, 1, 1), new Vector3(1, 1, 1), //front
        new Vector3(1, 1, 0), new Vector3(0, 1, 0), new Vector3(0, 0, 0), new Vector3(1, 0, 0), //back
        new Vector3(0, 1, 0), new Vector3(1, 1, 0), new Vector3(1, 1, 1), new Vector3(0, 1, 1), //top
        new Vector3(1, 0, 0), new Vector3(0, 0, 0), new Vector3(0, 0, 1), new Vector3(1, 0, 1), //bot
        new Vector3(1, 0, 0), new Vector3(1, 0, 1), new Vector3(1, 1, 1), new Vector3(1, 1, 0), //right
        new Vector3(0, 0, 1), new Vector3(0, 0, 0), new Vector3(0, 1, 0), new Vector3(0, 1, 1)  //left
    };

    private int[] cubeTris =
    {
        0, 1, 2,       2, 3, 0,    //front
        4, 5, 6,       6, 7, 4,    //back
        8, 9, 10,      10, 11, 8,  //top
        12, 13, 14,    14, 15, 12, //bot
        16, 17, 18,    18, 19, 16, //right
        20, 21, 22,    22, 23, 20  //left
    };

    private Vector2[] cubeUv =
    {
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1),
        new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 1), new Vector2(1, 1)
    };

    private Vector3[] cubeNormal =
    {
        Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward,
        -Vector3.forward, -Vector3.forward, -Vector3.forward, -Vector3.forward,
        Vector3.up, Vector3.up, Vector3.up, Vector3.up,
        -Vector3.up, -Vector3.up, -Vector3.up, -Vector3.up,
        Vector3.right, Vector3.right, Vector3.right, Vector3.right,
        -Vector3.right, -Vector3.right, -Vector3.right, -Vector3.right
    };



    private void Start()
    {
        foreach(Vector3 v in cubeNormal)
        {
            //Debug.Log(v);
        }



        bool[] blocks;// = new bool[chunkSize * chunkSize * chunkSize];
        int counter = 0;
        Vector3 chunkPos;
        for(int x = 0; x < worldSize; ++x)
        {
            for (int y = 0; y < worldSize; ++y)
            {
                for (int z = 0; z < worldSize; ++z)
                {
                    blocks = new bool[chunkSize * chunkSize * chunkSize];
                    for (int i = 0; i < blocks.Length; ++i)
                    {
                        if (counter % 2 == 0)
                        {
                            blocks[i] = true;
                        }
                    }

                    chunkPos.x = x;
                    chunkPos.y = y;
                    chunkPos.z = z;
                    BuildChunk(chunkPos, blocks);

                }
            }
        }

    }


    private GameObject BuildChunk(Vector3 pos, bool[] blocks)
    {
        GameObject chunk = Instantiate(chunkPrefab);
        chunk.transform.position = pos * chunkSize;
        BuildMesh(chunk, blocks);
        return chunk;
    }

    public GameObject MakeChunkObj(Vector3 pos)
    {
        GameObject chunk = Instantiate(chunkPrefab);
        chunk.transform.position = pos * chunkSize;
        return chunk;
    }

    public void BuildMesh(GameObject chunkObj, bool[] blocks)
    {
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector3> normals;
        List<Vector2> uv;
        
        int vertCount = cubeVertices.Length;
        int triCount = cubeTris.Length;
        int faceCount = vertCount / 4;
        
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        normals = new List<Vector3>();

        Vector3 blockpos;
        for (int z = 0; z < chunkSize; ++z)
        {
            for (int y = 0; y < chunkSize; ++y)
            {
                for (int x = 0; x < chunkSize; ++x)
                {
                    if (blocks[x + (y * chunkSize) + (z * chunkSize * chunkSize)])
                    {
                        blockpos.x = x;
                        blockpos.y = y;
                        blockpos.z = z;
                        
                        for(int i = 0; i < faceCount; ++i)
                        {
                            switch (i)
                            {
                                case 0: //front
                                    if(z < chunkSize - 1 && GetBlock(x, y, z + 1, blocks))
                                    {
                                        continue;
                                    }
                                    break;
                                case 1: //back
                                    if (z > 0 && GetBlock(x, y, z - 1, blocks))
                                    {
                                        continue;
                                    }
                                    break;
                                case 2: //top
                                    if (y < chunkSize - 1 && GetBlock(x, y + 1, z, blocks))
                                    {
                                        continue;
                                    }
                                    break;
                                case 3: //bot
                                    if (y > 0 && GetBlock(x, y - 1, z, blocks))
                                    {
                                        continue;
                                    }
                                    break;
                                case 4: //right
                                    if (x < chunkSize - 1 && GetBlock(x + 1, y, z, blocks))
                                    {
                                        continue;
                                    }
                                    break;
                                case 5: //left
                                    if (x > 0 && GetBlock(x - 1, y, z, blocks))
                                    {
                                        continue;
                                    }
                                    break;
                                default:
                                    break;
                            }
                            
                            for(int n = 0; n < 4; ++n)
                            {
                                vertices.Add(cubeVertices[n + i * 4] + blockpos);
                                uv.Add(cubeUv[n + i * 4]);
                                normals.Add(cubeNormal[n + i * 4]);
                            }

                            triangles.Add(vertices.Count - 2);
                            triangles.Add(vertices.Count - 3);
                            triangles.Add(vertices.Count - 4);
                            triangles.Add(vertices.Count - 4);
                            triangles.Add(vertices.Count - 1);
                            triangles.Add(vertices.Count - 2);
                            
                        }
                    }
                }
            }
        }


        Mesh mesh = new Mesh();
        chunkObj.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        chunkObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

    public bool GetBlock(int x, int y, int z, bool[] blocks)
    {
        return blocks[x + y * chunkSize + z * chunkSize * chunkSize];
    }

}
