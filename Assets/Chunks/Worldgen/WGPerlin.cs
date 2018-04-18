using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WGPerlin", menuName = "Generators/Perlin")]
public class WGPerlin : WorldGenAbstract
{

    public int groundStart;
    public float heightScale;
    public float frequency;



    public override WorldGenAbstract Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new WGPerlin();
            }
            return instance;
        }
    }

    public override void BuildChunk(Chunk chunk)
    {
        int[] blocks = new int[Chunk.ChunkSize * Chunk.ChunkSize * Chunk.ChunkSize];
        
        for(int x = 0; x < Chunk.ChunkSize; ++x)
        {
            for (int z = 0; z < Chunk.ChunkSize; ++z)
            {
                Vector2 blockpos = new Vector2(chunk.transform.position.x + x,
                                               chunk.transform.position.z + z);
                blockpos /= frequency;
                float height = Mathf.PerlinNoise(blockpos.x, blockpos.y) * heightScale;
                for (int y = 0; y < Chunk.ChunkSize; ++y)
                {
                    if(y + chunk.transform.position.y - groundStart <= height)
                    {
                        blocks[x + y * Chunk.ChunkSize + z * Chunk.ChunkSize * Chunk.ChunkSize] = 1;
                    }
                    else
                    {
                        blocks[x + y * Chunk.ChunkSize + z * Chunk.ChunkSize * Chunk.ChunkSize] = 0;
                    }
                }
            }
        }

        chunk.SetBlocks(blocks);
    }

}
