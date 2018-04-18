using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldGenAbstract : ScriptableObject {

    protected static WorldGenAbstract instance;

    public abstract WorldGenAbstract Instance
    {
        get;
    }

    abstract public void BuildChunk(Chunk chunk);

}
