using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public abstract class BlockType
{

    protected int id;

    protected bool canBePlaced;
    protected bool canBeDestroyed;



    public int Id
    {
        get
        {
            return id;
        }
    }

    public bool CanBePlaced
    {
        get
        {
            return canBePlaced;
        }
    }

    public bool CanBeDestroyed
    {
        get
        {
            return canBeDestroyed;
        }
    }

}
