using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Chunk : NetworkBehaviour {

    public CubeModel cubeModel;
    public MeshFilter filter;
    public MeshCollider collider;

    private static int chunkSize = 8;
    public static int ChunkSize
    {
        get
        {
            return chunkSize;
        }
    }

    public Vector3 ChunkPos
    {
        get
        {
            Vector3 pos = transform.position / chunkSize;
            pos.x = Mathf.Floor(pos.x);
            pos.y = Mathf.Floor(pos.y);
            pos.z = Mathf.Floor(pos.z);
            return pos;
        }
    }

    private int[] blocks;



    private void Awake()
    {
        blocks = new int[chunkSize * chunkSize * chunkSize];
        BuildMesh();
    }

    public void BuildMesh()
    {
        List<Vector3> vertices;
        List<int> triangles;
        List<Vector3> normals;
        List<Vector2> uv;

        int vertCount = cubeModel.cubeVertices.Length;
        int triCount = cubeModel.cubeTris.Length;
        int faceCount = vertCount / 4;

        vertices = new List<Vector3>();
        triangles = new List<int>();
        uv = new List<Vector2>();
        normals = new List<Vector3>();

        Vector3 blockpos;
        Vector3 cp = transform.position;
        WorldMaker world = NetBridge.Instance.world;
        for (int z = 0; z < chunkSize; ++z)
        {
            for (int y = 0; y < chunkSize; ++y)
            {
                for (int x = 0; x < chunkSize; ++x)
                {
                    if (blocks[x + (y * chunkSize) + (z * chunkSize * chunkSize)] > 0)
                    {
                        blockpos.x = x;
                        blockpos.y = y;
                        blockpos.z = z;

                        for (int i = 0; i < faceCount; ++i)
                        {
                            switch (i)
                            {
                                case 0: //front
                                    if (world.GetBlock(x + (int)cp.x, y + (int)cp.y, z + (int)cp.z + 1) > 0)
                                    {
                                        continue;
                                    }
                                    break;
                                case 1: //back
                                    if (world.GetBlock(x + (int)cp.x, y + (int)cp.y, z + (int)cp.z - 1) > 0)
                                    {
                                        continue;
                                    }
                                    break;
                                case 2: //top
                                    if (world.GetBlock(x + (int)cp.x, y + (int)cp.y + 1, z + (int)cp.z) > 0)
                                    {
                                        continue;
                                    }
                                    break;
                                case 3: //bot
                                    if (world.GetBlock(x + (int)cp.x, y + (int)cp.y - 1, z + (int)cp.z) > 0)
                                    {
                                        continue;
                                    }
                                    break;
                                case 4: //right
                                    if (world.GetBlock(x + (int)cp.x + 1, y + (int)cp.y, z + (int)cp.z) > 0)
                                    {
                                        continue;
                                    }
                                    break;
                                case 5: //left
                                    if (world.GetBlock(x + (int)cp.x - 1, y + (int)cp.y, z + (int)cp.z) > 0)
                                    {
                                        continue;
                                    }
                                    break;
                                default:
                                    break;
                            }

                            int blockType = GetBlock(x, y, z);
                            float atlasXIncrement = 1.0f / cubeModel.atlasSize.x;
                            float atlasYIncrement = 1.0f / cubeModel.atlasSize.y;

                            for (int n = 0; n < 4; ++n)
                            {
                                vertices.Add(cubeModel.cubeVertices[n + i * 4] + blockpos);
                                Vector2 uvpos = cubeModel.cubeUv[n + i * 4];
                                uvpos.x = uvpos.x * atlasXIncrement + ((blockType % cubeModel.atlasSize.x) + 1) * atlasXIncrement;
                                uvpos.y = uvpos.y * atlasYIncrement + (cubeModel.atlasSize.x - 1 - (int)((blockType-1) / cubeModel.atlasSize.x)) * atlasYIncrement;
                                uv.Add(uvpos);
                                normals.Add(cubeModel.cubeNormal[n + i * 4]);
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
        filter.mesh = mesh;
        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.normals = normals.ToArray();
        mesh.uv = uv.ToArray();
        collider.sharedMesh = mesh;
    }

    public int GetBlock(int x, int y, int z)
    {
        return blocks[x + y * chunkSize + z * chunkSize * chunkSize];
    }

    public int GetBlock(Vector3 pos)
    {
        return blocks[(int)pos.x + (int)pos.y * chunkSize + (int)pos.z * chunkSize * chunkSize];
    }

    public void DestroyBlock(Vector3 pos)
    {
        if (pos.x >= 0 && pos.x < chunkSize
         && pos.y >= 0 && pos.y < chunkSize
         && pos.z >= 0 && pos.z < chunkSize
         && NetBridge.Instance.blockTypeManager.IsBlockDestroyable(GetBlock(pos)))
        {
            blocks[(int)pos.x + (int)pos.y * chunkSize + (int)pos.z * chunkSize * chunkSize] = 0;
            BuildMesh();
        }
    }

    public void AddBlock(Vector3 pos, int type)
    {
        if (pos.x >= 0 && pos.x < chunkSize
         && pos.y >= 0 && pos.y < chunkSize
         && pos.z >= 0 && pos.z < chunkSize
         && GetBlock(pos) < 1)
        {
            blocks[(int)pos.x + (int)pos.y * chunkSize + (int)pos.z * chunkSize * chunkSize] = type;
            BuildMesh();
        }
    }

    public void SetBlocks(int[] b)
    {
        blocks = b;
        BuildMesh();
    }

    public int[] GetBlockArray()
    {
        return blocks;
    }

}
