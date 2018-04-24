using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WorldMaker : NetworkBehaviour
{

    private class ClientConnection
    {
        public NetworkConnection conn;
        public Queue<Vector3> chunksToBuild = new Queue<Vector3>();
    }



    
    public GameObject chunkPrefab;
    
    public int worldSize = 3;
    public int chunksGennedPerFrame;
    public int chunksSentPerFrame = 1;
    public WorldGenAbstract generator;

    private Dictionary<Vector3, Chunk> chunks = new Dictionary<Vector3, Chunk>();
    private Vector3[] chunksToBuild;
    private int queuedChunk = 0;

    private List<ClientConnection> clients = new List<ClientConnection>();





    private void Awake()
    {
        NetBridge.Instance.world = this;
        NetBridge.Instance.onServerConnect += OnClientConnect;

        generator.Init();
    }

    private void Start()
    {
        if(!isServer)
        {
            return;
        }



        chunksToBuild = new Vector3[worldSize * worldSize * worldSize];

        int iterator = 0;
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
                    chunksToBuild[iterator] = chunkPos;
                    ++iterator;
                }
            }
        }

    }

    private void Update()
    {
        if(!isServer)
        {
            return;
        }

        Chunk chunk;
        if(queuedChunk < chunksToBuild.Length)
        {
            for(int i = 0; i < chunksGennedPerFrame && queuedChunk < chunksToBuild.Length; ++i)
            {
                chunk = BuildChunk(chunksToBuild[queuedChunk]);
                generator.BuildChunk(chunk);
                foreach (ClientConnection c in clients)
                {
                    c.chunksToBuild.Enqueue(chunksToBuild[queuedChunk]);
                }
                ++queuedChunk;
                UpdateSurroundingChunks(chunk.ChunkPos);
            }
        }

        Vector3 chunkPos;
        ClientConnection client;
        for(int i = 0; i < clients.Count; ++i)
        {
            client = clients[i];
            if (client.conn.isReady && client.chunksToBuild.Count > 0)
            {
                for (int n = 0; n < chunksSentPerFrame && client.chunksToBuild.Count > 0; ++n)
                {
                    chunkPos = client.chunksToBuild.Dequeue();
                    chunks.TryGetValue(chunkPos, out chunk);
                    TargetReceiveChunkData(client.conn, chunkPos, chunk.GetBlockArray());
                }
            }
        }
    }

    public void OnClientConnect(NetworkConnection connection)
    {
        ClientConnection clcn = new ClientConnection();
        clcn.conn = connection;
        int iterator = 0;
        foreach(KeyValuePair<Vector3, Chunk> entry in chunks)
        {
            clcn.chunksToBuild.Enqueue(entry.Key);
            ++iterator;
        }
        clients.Add(clcn);
    }
    


    private void UpdateSurroundingChunks(Vector3 chunkpos)
    {
        Chunk chunk;

        if(chunks.TryGetValue(chunkpos + Vector3.forward, out chunk))
        {
            chunk.BuildMesh();
            foreach (ClientConnection c in clients)
            {
                c.chunksToBuild.Enqueue(chunkpos + Vector3.forward);
            }
        }
        if (chunks.TryGetValue(chunkpos - Vector3.forward, out chunk))
        {
            chunk.BuildMesh();
            foreach (ClientConnection c in clients)
            {
                c.chunksToBuild.Enqueue(chunkpos - Vector3.forward);
            }
        }

        if (chunks.TryGetValue(chunkpos + Vector3.up, out chunk))
        {
            chunk.BuildMesh();
            foreach (ClientConnection c in clients)
            {
                c.chunksToBuild.Enqueue(chunkpos + Vector3.up);
            }
        }
        if (chunks.TryGetValue(chunkpos - Vector3.up, out chunk))
        {
            chunk.BuildMesh();
            foreach (ClientConnection c in clients)
            {
                c.chunksToBuild.Enqueue(chunkpos - Vector3.up);
            }
        }

        if (chunks.TryGetValue(chunkpos + Vector3.right, out chunk))
        {
            chunk.BuildMesh();
            foreach (ClientConnection c in clients)
            {
                c.chunksToBuild.Enqueue(chunkpos + Vector3.right);
            }
        }
        if (chunks.TryGetValue(chunkpos - Vector3.right, out chunk))
        {
            chunk.BuildMesh();
            foreach (ClientConnection c in clients)
            {
                c.chunksToBuild.Enqueue(chunkpos - Vector3.right);
            }
        }
    }

    private void UpdateAdjacentChunk(Vector3 blockpos)
    {
        Chunk chunk;
        Vector3 chunkPos;

        if(blockpos.x >= 0 && blockpos.x % Chunk.ChunkSize == 0)
        {
            chunkPos = GetChunkIn(blockpos - Vector3.right);
            if(chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if(blockpos.x < 0 && (blockpos.x + 1) % Chunk.ChunkSize == 0)
        {
            chunkPos = GetChunkIn(blockpos + Vector3.right);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if(blockpos.x >= 0 && blockpos.x % Chunk.ChunkSize == Chunk.ChunkSize - 1)
        {
            chunkPos = GetChunkIn(blockpos + Vector3.right);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.x < 0 && (blockpos.x + 1) % Chunk.ChunkSize == Chunk.ChunkSize - 1)
        {
            chunkPos = GetChunkIn(blockpos - Vector3.right);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }

        if (blockpos.y >= 0 && blockpos.y % Chunk.ChunkSize == 0)
        {
            chunkPos = GetChunkIn(blockpos - Vector3.up);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.y < 0 && (blockpos.y + 1) % Chunk.ChunkSize == 0)
        {
            chunkPos = GetChunkIn(blockpos + Vector3.up);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.y >= 0 && blockpos.y % Chunk.ChunkSize == Chunk.ChunkSize - 1)
        {
            chunkPos = GetChunkIn(blockpos + Vector3.up);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.y < 0 && (blockpos.y + 1) % Chunk.ChunkSize == Chunk.ChunkSize - 1)
        {
            chunkPos = GetChunkIn(blockpos - Vector3.up);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }

        if (blockpos.z >= 0 && blockpos.z % Chunk.ChunkSize == 0)
        {
            chunkPos = GetChunkIn(blockpos - Vector3.forward);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.z < 0 && (blockpos.z + 1) % Chunk.ChunkSize == 0)
        {
            chunkPos = GetChunkIn(blockpos + Vector3.forward);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.z >= 0 && blockpos.z % Chunk.ChunkSize == Chunk.ChunkSize - 1)
        {
            chunkPos = GetChunkIn(blockpos + Vector3.forward);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
        else if (blockpos.z < 0 && (blockpos.z + 1) % Chunk.ChunkSize == Chunk.ChunkSize - 1)
        {
            chunkPos = GetChunkIn(blockpos - Vector3.forward);
            if (chunks.TryGetValue(chunkPos, out chunk))
            {
                chunk.BuildMesh();
                if (isServer)
                {
                    foreach (ClientConnection client in clients)
                    {
                        client.chunksToBuild.Enqueue(chunkPos);
                    }
                }
            }
        }
    }

    [TargetRpc]
    private void TargetReceiveChunkData(NetworkConnection target, Vector3 pos, int[] blocks)
    {
        Chunk chunk;
        chunks.TryGetValue(pos, out chunk);

        if (chunk == null)
        {
            chunk = BuildChunk(pos);
        }

        chunk.SetBlocks(blocks);

        UpdateSurroundingChunks(pos);
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
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
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

    public int GetBlock(int x, int y, int z)
    {
        return GetBlock(new Vector3(x, y, z));
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

        Vector3 posInChunk = pos - chunkPos * Chunk.ChunkSize;
        chunk.DestroyBlock(posInChunk);

        UpdateAdjacentChunk(pos);

        if (isServer)
        {
            foreach (ClientConnection client in clients)
            {
                client.chunksToBuild.Enqueue(chunkPos);
            }
        }
    }

    public void AddBlock(Vector3 pos, int type)
    {
        Vector3 chunkPos = pos / Chunk.ChunkSize;
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
        
        if (Physics.CheckBox(pos + Vector3.one / 2.0f, Vector3.one / 2.0f, Quaternion.identity, ~LayerMask.GetMask("Chunk")))
        {
            if (isServer)
            {
                UpdateAdjacentChunk(pos);
                foreach (ClientConnection client in clients)
                {
                    client.chunksToBuild.Enqueue(chunkPos);
                }
            }
            return;
        }



        Chunk chunk;
        if (!chunks.TryGetValue(chunkPos, out chunk))
        {
            chunk = BuildChunk(chunkPos);
        }
        Vector3 posInChunk = pos - chunkPos * Chunk.ChunkSize;
        chunk.AddBlock(posInChunk, type);

        UpdateAdjacentChunk(pos);

        if (isServer)
        {
            foreach (ClientConnection client in clients)
            {
                client.chunksToBuild.Enqueue(chunkPos);
            }
        }
    }

    public Vector3 GetPlayerSpawn()
    {
        return new Vector3(0, (worldSize+1) * Chunk.ChunkSize, 0);
    }

    private Vector3 GetChunkIn(Vector3 blockpos)
    {
        Vector3 chunkPos = blockpos / Chunk.ChunkSize;
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
        return chunkPos;
    }


}
