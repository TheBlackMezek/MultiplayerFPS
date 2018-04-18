using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldMaker : MonoBehaviour
{
    
    public GameObject chunkPrefab;
    
    public int worldSize = 3;
    public WorldGenAbstract generator;

    private Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    
    



    private void Start()
    {
        Vector3 chunkPos;
        for (int x = 0; x < worldSize; ++x)
        {
            for (int y = 0; y < worldSize; ++y)
            {
                for (int z = 0; z < worldSize; ++z)
                {
                    chunkPos.x = x;
                    chunkPos.y = y;
                    chunkPos.z = z;
                    Chunk chunk = BuildChunk(chunkPos);
                    generator.BuildChunk(chunk);
                }
            }
        }

    }


    private Chunk BuildChunk(Vector3 pos)
    {
        GameObject chunk = Instantiate(chunkPrefab);
        chunk.transform.position = pos * Chunk.ChunkSize;
        Chunk script = chunk.GetComponent<Chunk>();
        chunks.Add(pos, script);
        return script;
    }
    
    public int GetBlock(Vector3 pos)
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
            return -1;
        }
    }

    public void DestroyBlock(Vector3 pos)
    {
        Vector3 chunkPos = pos / Chunk.ChunkSize;
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
        Chunk chunk;
        if (!chunks.TryGetValue(chunkPos, out chunk))
        {
            chunk = BuildChunk(chunkPos).GetComponent<Chunk>();
        }
        pos = pos - chunkPos * Chunk.ChunkSize;
        chunk.DestroyBlock(pos);
    }

    public void AddBlock(Vector3 pos, int type)
    {
        Vector3 chunkPos = pos / Chunk.ChunkSize;
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
        Chunk chunk;
        if (!chunks.TryGetValue(chunkPos, out chunk))
        {
            chunk = BuildChunk(chunkPos);
        }
        pos = pos - chunkPos * Chunk.ChunkSize;
        chunk.AddBlock(pos, type);
    }



}
