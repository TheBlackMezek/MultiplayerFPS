using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockStone : BlockType
{

    public BlockStone()
    {
        id = 1;
        canBeDestroyed = true;
        canBePlaced = true;
    }

}
