using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WGSolidStone", menuName = "Generators/SolidStone")]
public class WGSolidStone : WorldGenAbstract {

    public override WorldGenAbstract Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new WGSolidStone();
            }
            return instance;
        }
    }

    public override void BuildChunk(Chunk chunk)
    {
        int[] blocks = new int[Chunk.ChunkSize * Chunk.ChunkSize * Chunk.ChunkSize];

        for(int i = 0; i < blocks.Length; ++i)
        {
            blocks[i] = 1;
        }

        chunk.SetBlocks(blocks);
    }

}
