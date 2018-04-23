using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockBedrock : BlockType
{

    public BlockBedrock()
    {
        id = 3;
        canBeDestroyed = false;
        canBePlaced = false;
    }

}
