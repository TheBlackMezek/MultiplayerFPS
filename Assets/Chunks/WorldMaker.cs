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
                ++queuedChunk;
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
        pos = pos - chunkPos * Chunk.ChunkSize;
        chunk.DestroyBlock(pos);
        
        if(isServer)
        {
            foreach (ClientConnection client in clients)
            {
                client.chunksToBuild.Enqueue(chunkPos);
            }
        }
    }

    [Command]
    public void CmdDestroyBlock(Vector3 pos)
    {
        DestroyBlock(pos);
    }

    public void AddBlock(Vector3 pos, int type)
    {
        Vector3 chunkPos = pos / Chunk.ChunkSize;
        chunkPos.x = Mathf.Floor(chunkPos.x);
        chunkPos.y = Mathf.Floor(chunkPos.y);
        chunkPos.z = Mathf.Floor(chunkPos.z);
        Chunk chunk;
        if (!chunks.TryGetValue(chunkPos, out chunk))
        {
            chunk = BuildChunk(chunkPos);
        }
        pos = pos - chunkPos * Chunk.ChunkSize;
        chunk.AddBlock(pos, type);

        if (isServer)
        {
            foreach (ClientConnection client in clients)
            {
                client.chunksToBuild.Enqueue(chunkPos);
            }
        }
    }

    [Command]
    public void CmdAddBlock(Vector3 pos, int type)
    {
        AddBlock(pos, type);
    }

    public Vector3 GetPlayerSpawn()
    {
        return new Vector3(0, (worldSize+1) * Chunk.ChunkSize, 0);
    }



}
