using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WorldGenAbstract : ScriptableObject {

    abstract public void Init();

    abstract public void BuildChunk(Chunk chunk);

}
