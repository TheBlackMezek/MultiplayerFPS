using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockTypeManager : MonoBehaviour {

    private List<BlockType> blockTypes = new List<BlockType>();



    private void Awake()
    {
        NetBridge.Instance.blockTypeManager = this;

        RegisterType(new BlockAir());
        RegisterType(new BlockStone());
        RegisterType(new BlockGrass());
        RegisterType(new BlockBedrock());
    }

    public void RegisterType(BlockType type)
    {
        if(blockTypes.Count <= type.Id)
        {
            blockTypes.AddRange(new BlockAir[type.Id + 1 - blockTypes.Count]);
        }
        blockTypes[type.Id] = type;
    }



    public bool IsBlockPlacable(int type)
    {
        return blockTypes[type].CanBePlaced;
    }

    public bool IsBlockDestroyable(int type)
    {
        return blockTypes[type].CanBeDestroyed;
    }
    
}
