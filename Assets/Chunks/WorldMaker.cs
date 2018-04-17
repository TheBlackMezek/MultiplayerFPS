using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{
    
    public GameObject chunkPrefab;
    
    public int worldSize = 3;

    private Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    
    



    private void Start()
    {
        int chunkSize = Chunk.ChunkSize;
        bool[] blocks;
        Vector3 chunkPos;
        for (int x = 0; x < worldSize; ++x)
        {
            for (int y = 0; y < worldSize; ++y)
            {
                for (int z = 0; z < worldSize; ++z)
                {
                    blocks = new bool[chunkSize * chunkSize * chunkSize];
                    for (int i = 0; i < blocks.Length; ++i)
                    {
                        if (Random.Range(0, 5) == 0)
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
        chunk.transform.position = pos * Chunk.ChunkSize;
        Chunk script = chunk.GetComponent<Chunk>();
        chunk.GetComponent<Chunk>().SetBlocks(blocks);
        chunks.Add(pos, script);
        return chunk;
    }
    
    public bool GetBlock(Vector3 pos)
    {
        Vector3 chunkPos = pos / Chunk.ChunkSize;
        Chunk chunk;
        if(chunks.TryGetValue(chunkPos, out chunk))
        {
            pos = pos - chunkPos * Chunk.ChunkSize;
            return chunk.GetBlock(pos);
        }
        else
        {
            return false;
        }
    }

    public void DestroyBlock(Vector3 pos)
    {
        Vector3 chunkPos = pos / Chunk.ChunkSize;
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
        Chunk chunk;
        if (chunks.TryGetValue(chunkPos, out chunk))
        {
            pos = pos - chunkPos * Chunk.ChunkSize;
            chunk.DestroyBlock(pos);
        }
    }



}
