using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WGPerlin", menuName = "Generators/Perlin")]
public class WGPerlin : WorldGenAbstract
{

    public int groundStart;
    public float heightScale;
    public float baseFrequency;

    public int octaves = 1;
    public float persistence;
    public float lacunarity;

    public int seed;

    private Vector3[] offsets;
    private float offsetRange = 10000;


    

    public override void Init()
    {
        offsets = new Vector3[octaves];
        Random.InitState(seed);
        for (int i = 0; i < octaves; ++i)
        {
            offsets[i] = new Vector3(Random.Range(-offsetRange, offsetRange),
                                     Random.Range(-offsetRange, offsetRange),
                                     Random.Range(-offsetRange, offsetRange));
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
                float height = UsePerlin(blockpos);
                for (int y = 0; y < Chunk.ChunkSize; ++y)
                {
                    if(y + chunk.transform.position.y == 0)
                    {
                        blocks[x + y * Chunk.ChunkSize + z * Chunk.ChunkSize * Chunk.ChunkSize] = 3;
                    }
                    else if(y + chunk.transform.position.y - groundStart <= height-1)
                    {
                        blocks[x + y * Chunk.ChunkSize + z * Chunk.ChunkSize * Chunk.ChunkSize] = 1;
                    }
                    else if (y + chunk.transform.position.y - groundStart <= height)
                    {
                        blocks[x + y * Chunk.ChunkSize + z * Chunk.ChunkSize * Chunk.ChunkSize] = 2;
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

    private float UsePerlin(Vector2 blockpos)
    {
        float height = 0;

        float frequency = baseFrequency;
        float amplitude = heightScale;

        Vector3 off;

        for(int i = 0; i < octaves; ++i)
        {
            off = offsets[i];

            height += Mathf.PerlinNoise((blockpos.x + off.x) * frequency, (blockpos.y + off.y) * frequency) * amplitude;

            amplitude *= persistence;
            frequency *= lacunarity;
        }

        return height;
    }

}
