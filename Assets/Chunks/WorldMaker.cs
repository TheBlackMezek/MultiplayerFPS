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
        Vector3[] vertices;
        int[] triangles;
        Vector3[] normals;
        Vector2[] uv;

        int blockCount = 0;
        for (int x = 0; x < chunkSize; ++x)
        {
            for (int y = 0; y < chunkSize; ++y)
            {
                for (int z = 0; z < chunkSize; ++z)
                {
                    if (blocks[x + (y * chunkSize) + (z * chunkSize * chunkSize)])
                    {
                        ++blockCount;
                    }
                }
            }
        }
        Debug.Log(blockCount);
        int vertCont = cubeMesh.vertices.Length;
        int triCount = cubeMesh.triangles.Length;
        int uvCount = cubeMesh.uv.Length;
        int normCount = cubeMesh.normals.Length;

        vertices = new Vector3[vertCont * blockCount];
        triangles = new int[triCount * blockCount];
        uv = new Vector2[uvCount * blockCount];
        normals = new Vector3[normCount * blockCount];

        Vector3 blockpos;
        int blockidx = 0;
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

                        for (int i = 0; i < vertCont; ++i)
                        {
                            vertices[i + vertCont * blockidx] = cubeMesh.vertices[i] + blockpos;
                        }

                        for (int i = 0; i < triCount; ++i)
                        {
                            triangles[i + blockidx * triCount] = cubeMesh.triangles[i] + blockidx * vertCont;
                        }

                        for (int i = 0; i < uvCount; ++i)
                        {
                            uv[i + uvCount * blockidx] = cubeMesh.uv[i];
                        }

                        for (int i = 0; i < normCount; ++i)
                        {
                            normals[i + normCount * blockidx] = cubeMesh.normals[i];
                        }

                        ++blockidx;
                    }
                }
            }
        }


        Mesh mesh = new Mesh();
        chunkObj.GetComponent<MeshFilter>().mesh = mesh;
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.normals = normals;
        mesh.uv = uv;
        chunkObj.GetComponent<MeshCollider>().sharedMesh = mesh;
    }

}
