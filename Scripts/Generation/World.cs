//using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using QFSW.QC;

public class World : MonoBehaviour
{
    public static World instance;
    public int ChunkRenderRadius = 2;
    [HideInInspector]
    public const int ChunkSize = 16; // how many blocks in each direction are in a chunk, used for correctly positioning chunks

    public int Seed;
    public float BaseScale;
    public AnimationCurve HeightCurve;
    public float MaxHeight;
    public float Multiplier;
    public float RoughnessFactor;
    public float RoughnessStrength;

    public GameObject Player;
    private Vector3Int previousPlayerChunk = Vector3Int.zero;
    private Vector3Int currentPlayerChunk = Vector3Int.zero;

    public Material TerrainMaterial;

    public Vector3 WorldSpawn;
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        Seed = Random.Range(int.MinValue, int.MaxValue);

        // Set World Spawn
        int _h = Chunk.GetTerrainNoise(0,0, Vector3.zero);
        WorldSpawn = new Vector3(0,_h+1,0);

        Vector3Int RoundedPlayerPosition = Player.transform.position.RoundToInt();
        currentPlayerChunk = new Vector3Int ((int)(RoundedPlayerPosition.x / ChunkSize), (int)(RoundedPlayerPosition.y / ChunkSize), (int)(RoundedPlayerPosition.z / ChunkSize));
        LoadChunksAroundPlayer(currentPlayerChunk);
    }

    void Update () {
        Vector3Int RoundedPlayerPosition = Player.transform.position.RoundToInt();
        currentPlayerChunk = new Vector3Int ((int)(RoundedPlayerPosition.x / ChunkSize), (int)(RoundedPlayerPosition.y / ChunkSize), (int)(RoundedPlayerPosition.z / ChunkSize));

        // check if player has entered a new chunk
        if(currentPlayerChunk != previousPlayerChunk) {
            LoadChunksAroundPlayer(currentPlayerChunk);
        }

        previousPlayerChunk = currentPlayerChunk;
    }

    void LoadChunksAroundPlayer (Vector3Int playerChunkPos) {
        List<Vector3Int> chunksAroundPlayer = new List<Vector3Int>();
        for (int x = 0; x <= ChunkRenderRadius*2; x++) {
            for (int y = 0; y <= ChunkRenderRadius*2; y++) {
                for (int z = 0; z <= ChunkRenderRadius*2; z++) {
                    Vector3Int p = new Vector3Int(playerChunkPos.x-ChunkRenderRadius+x, playerChunkPos.y-ChunkRenderRadius+y, playerChunkPos.z-ChunkRenderRadius+z);
                    if (!chunksAroundPlayer.Contains(p)) {
                        chunksAroundPlayer.Add(p);
                    }
                }
            }
        }
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();
        List<Vector3Int> chunksToLoad = new List<Vector3Int>();

        for (int i = 0; i < chunksAroundPlayer.Count; i++) {
            if (!chunks.ContainsKey(chunksAroundPlayer[i])) {
                //these chunks are not currenly loaded so need to be loaded
                chunksToLoad.Add(chunksAroundPlayer[i]);
            }
        }

        for (int i = 0; i < chunks.Keys.Count; i++) {
            if (!chunksAroundPlayer.Contains(chunks.Keys.ElementAt(i))) {
                // these chunks are currently loaded but no longer need to be loaded
                chunksToUnload.Add(chunks.Keys.ElementAt(i));
            }
        }

        for (int i = 0; i < chunksToUnload.Count; i++) {
            UnloadChunk(chunksToUnload[i].x,chunksToUnload[i].y,chunksToUnload[i].z);
        }

        for (int i = 0; i < chunksToLoad.Count; i++) {
            LoadChunk(chunksToLoad[i].x,chunksToLoad[i].y,chunksToLoad[i].z);
        }

        SetAdjacentChunkReferences();

    }

    void GenerateChunk(int x, int y, int z) {
        GameObject chunk = new GameObject($"Chunk_{x},{y},{z}"); // create new chunk
        chunk.isStatic = true;
        chunk.transform.position = new Vector3Int(x * ChunkSize, y * ChunkSize, z * ChunkSize); // position chunk 

        chunk.AddComponent<MeshFilter>(); // add mesh to chunk
        chunk.AddComponent<MeshRenderer>().material = TerrainMaterial; // add renderer to chunk so you can actually see it
        chunk.AddComponent<MeshCollider>();
        chunk.layer = 8;

        // actual chunk stuff
        Vector3Int chunkPos = new Vector3Int(x,y,z);
        chunks.Add(chunkPos, chunk.AddComponent<Chunk>());
    }

    void SetAdjacentChunkReferences () {
        foreach (KeyValuePair<Vector3Int, Chunk> ch in chunks) {
            ch.Value.NorthNeighbor = GetChunk(ch.Key.x, ch.Key.y, ch.Key.z+1);
            ch.Value.SouthNeighbor = GetChunk(ch.Key.x, ch.Key.y, ch.Key.z-1);
            ch.Value.TopNeighbor = GetChunk(ch.Key.x, ch.Key.y+1, ch.Key.z);
            ch.Value.BottomNeighbor = GetChunk(ch.Key.x, ch.Key.y-1, ch.Key.z);
            ch.Value.EastNeighbor = GetChunk(ch.Key.x+1, ch.Key.y, ch.Key.z);
            ch.Value.WestNeighbor = GetChunk(ch.Key.x-1, ch.Key.y, ch.Key.z);
        }
    }

    public Chunk GetChunk (int x, int y, int z) {
        if (chunks.TryGetValue(new Vector3Int(x,y,z), out Chunk chunk)) {
            return chunk;
        }
        return null;
    }

    public Block GetBlock(int x, int y, int z) {

        // Get the chunk that the specified block is in
        Chunk parentChunk = GetChunkFromBlockCoords(x, y, z);

        // when GetChunkFromBlockCoords is supplied coordinates that are outside the world, it will return a null chunk, this is normal behavior
        if (parentChunk == null) {
            return BlockList.instance.blocks["Air"]; // this should normally be set to stone, its only set to air for debugging purposes because that allows you to see the sides of blocks that face the edge of the world
        }

        // in the event that the chunk's blocks weren't generated yet, we will check the noisemap to guess the blocks in the chunk
        if (parentChunk.blocks == null) {
            if (y < Chunk.GetTerrainNoise(x, z, Vector3.zero)) {
                return BlockList.instance.blocks["Stone"];
            } else {
                return BlockList.instance.blocks["Air"];
            }
        }

        if(parentChunk.blocks[x.Mod(ChunkSize), y.Mod(ChunkSize), z.Mod(ChunkSize)] == null) {
            if (y < Chunk.GetTerrainNoise(x, z, Vector3.zero)) {
                return BlockList.instance.blocks["Stone"];
            } else {
                return BlockList.instance.blocks["Air"];
            }
        }

        // Get the desired block by using modulo to get the correct block position in the chunk
        Block desiredBlock = parentChunk.blocks[x.Mod(ChunkSize), y.Mod(ChunkSize), z.Mod(ChunkSize)];
        return desiredBlock;
    }

    public Chunk GetChunkFromBlockCoords(int x, int y, int z) {
        int _x = x;
        int _y = y;
        int _z = z;
        if (_x < 0) _x-=ChunkSize;
        if (_y < 0) _y-=ChunkSize;
        if (_z < 0) _z-=ChunkSize;
        Vector3Int cpos = new Vector3Int((int)((float)_x / ChunkSize), (int)((float)_y / ChunkSize), (int)((float)_z / ChunkSize));

        // make sure the position is even in the world
        if(!chunks.ContainsKey(cpos)) {
            return null;
        }
        return chunks[cpos];
    }

    [Command("loadchunk")]
    public void LoadChunk (int x, int y, int z) {
        GenerateChunk(x,y,z);
    }

    [Command("unloadchunk")]
    public void UnloadChunk (int x, int y, int z) {
        Chunk chunkToUnload = GetChunk(x,y,z);
        if(chunkToUnload != null) {
            chunks.Remove(new Vector3Int(x,y,z));
            chunkToUnload.Unload();
        }
    }

    public void SetBlock(int x, int y, int z, Block value) {
        Chunk _chunk = GetChunkFromBlockCoords(x, y, z);
        if (_chunk != null) {
            _chunk.SetBlock(x.Mod(ChunkSize), y.Mod(ChunkSize), z.Mod(ChunkSize), value);
        }
    }

    [Command("setblock")]
    public void SetBlock(int x, int y, int z, string block) {
        SetBlock(x,y,z,BlockList.instance.blocks[block]);
        UpdateChunk(x,y,z,true);
    }

    public void UpdateChunk (int x, int y, int z, bool updateAdjacentBlocks) {
        Chunk chunkToUpdate = GetChunkFromBlockCoords (x,y,z);
        chunkToUpdate.UpdateChunk();

        if (updateAdjacentBlocks) {
            // convert world position to chunk position
            int cx = (int)( x / ChunkSize );
            int cy = (int)( y / ChunkSize );
            int cz = (int)( z / ChunkSize );

            Vector3Int chunkPos = new Vector3Int(cx,cy,cz);

            // convert world position to position relative to chunk
            int rx = x.Mod(ChunkSize);
            int ry = y.Mod(ChunkSize);
            int rz = z.Mod(ChunkSize);

            if (rx == 0 && chunkToUpdate.WestNeighbor != null) chunkToUpdate.WestNeighbor.UpdateChunk();
            if (rx == ChunkSize-1 && chunkToUpdate.EastNeighbor != null) chunkToUpdate.EastNeighbor.UpdateChunk();
            if (ry == 0 && chunkToUpdate.BottomNeighbor != null) chunkToUpdate.BottomNeighbor.UpdateChunk();
            if (ry == ChunkSize-1 && chunkToUpdate.TopNeighbor != null) chunkToUpdate.TopNeighbor.UpdateChunk();
            if (rz == 0 && chunkToUpdate.SouthNeighbor != null) chunkToUpdate.SouthNeighbor.UpdateChunk();
            if (rz == ChunkSize-1 && chunkToUpdate.NorthNeighbor != null) chunkToUpdate.NorthNeighbor.UpdateChunk();
        }
    }
}