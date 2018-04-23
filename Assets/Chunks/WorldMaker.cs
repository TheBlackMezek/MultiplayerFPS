using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class WorldMaker : NetworkBehaviour
{

    private class ClientConnection
    {
        public NetworkConnection conn;
        public Vector3[] chunksToBuild;
        public int queuedChunk;
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
            if (client.queuedChunk < client.chunksToBuild.Length)
            {
                for (int n = 0; n < chunksSentPerFrame && client.queuedChunk < client.chunksToBuild.Length; ++n)
                {
                    chunkPos = client.chunksToBuild[client.queuedChunk];
                    Debug.Log("SENDING DATA FOR " + chunkPos);
                    chunks.TryGetValue(client.chunksToBuild[client.queuedChunk], out chunk);
                    TargetReceiveChunkData(client.conn, chunkPos, chunk.GetBlockArray());
                    ++client.queuedChunk;
                    Debug.Log(client.queuedChunk);
                }
            }
        }
    }

    //private void FixedUpdate()
    //{
    //    
    //}

    public void OnClientConnect(NetworkConnection connection)
    {
        Debug.Log(connection.address);
        ClientConnection clcn = new ClientConnection();
        clcn.conn = connection;
        clcn.queuedChunk = 0;
        clcn.chunksToBuild = new Vector3[chunks.Count];
        int iterator = 0;
        foreach(KeyValuePair<Vector3, Chunk> entry in chunks)
        {
            clcn.chunksToBuild[iterator] = entry.Key;
            ++iterator;
        }
        clients.Add(clcn);
    }
    


    [TargetRpc]
    private void TargetReceiveChunkData(NetworkConnection target, Vector3 pos, int[] blocks)
    {
        Debug.Log("RECEIVED DATA FOR " + pos);
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
    }

    [Command]
    public void CmdDestroyBlock(Vector3 pos)
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
        RpcDestroyBlock(pos);
    }

    [ClientRpc]
    public void RpcDestroyBlock(Vector3 pos)
    {
        if(isServer)
        {
            return;
        }
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
    }

    public Vector3 GetPlayerSpawn()
    {
        return new Vector3(0, worldSize * Chunk.ChunkSize, 0);
    }



}
