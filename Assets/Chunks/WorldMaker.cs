using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{

    public Mesh cubeMesh;
    public GameObject chunkPrefab;

    public int chunkSize = 16;
    public int worldSize = 3;



    private void Start()
    {
        foreach(Vector3 v in cubeMesh.vertices)
        {
            Debug.Log(v);
        }



        bool[] blocks = new bool[chunkSize * chunkSize * chunkSize];

        Vector3 chunkPos;
        for(int x = 0; x < worldSize; ++x)
        {
            for (int y = 0; y < worldSize; ++y)
            {
                for (int z = 0; z < worldSize; ++z)
                {

                    for (int i = 0; i < blocks.Length / 2; ++i)
                    {
                        blocks[i] = true;
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

        //int blockCount = 0;
        //for (int x = 0; x < chunkSize; ++x)
        //{
        //    for (int y = 0; y < chunkSize; ++y)
        //    {
        //        for (int z = 0; z < chunkSize; ++z)
        //        {
        //            if (blocks[x + (y * chunkSize) + (z * chunkSize * chunkSize)])
        //            {
        //                ++blockCount;
        //            }
        //        }
        //    }
        //}
        
        int vertCount = cubeMesh.vertices.Length;
        int triCount = cubeMesh.triangles.Length;
        int faceCount = vertCount / 4;
        
        vertices = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        normals = new List<Vector3>();

        Vector3 blockpos;
        int blockidx = 0;
        int faces = 0;
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
                            switch(i)
                            {
                                case 0: //front
                                    if(z < chunkSize - 1
                                    && blocks[x + (y * chunkSize) + ((z+1) * chunkSize * chunkSize)])
                                    {
                                        continue;
                                    }
                                    break;
                                case 1: //back
                                    if (z > 0
                                    && blocks[x + (y * chunkSize) + ((z - 1) * chunkSize * chunkSize)])
                                    {
                                        continue;
                                    }
                                    break;
                                case 2: //top

                                    break;
                                case 3: //bot

                                    break;
                                case 4: //left

                                    break;
                                case 5: //right

                                    break;
                                default:
                                    break;
                            }

                            for(int n = 0; n < 4; ++n)
                            {
                                vertices.Add(cubeMesh.vertices[n + i * 4] + blockpos);
                                uv.Add(cubeMesh.uv[n + i * 4]);
                                normals.Add(cubeMesh.normals[n + i * 4]);
                            }
                            for(int n = 0; n < 6; ++n)
                            {
                                triangles.Add(cubeMesh.triangles[n + i * 6] + blockidx * vertCount);
                            }
                        }
                        //for (int i = 0; i < vertCount; ++i)
                        //{
                        //    vertices.Add(cubeMesh.vertices[i] + blockpos);
                        //    uv.Add(cubeMesh.uv[i]);
                        //    normals.Add(cubeMesh.normals[i]);
                        //}

                        //for (int i = 0; i < triCount; ++i)
                        //{
                        //    triangles.Add(cubeMesh.triangles[i] + blockidx * vertCount);
                        //}

                        ++blockidx;
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

}
