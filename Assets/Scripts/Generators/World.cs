//using System.Diagnostics;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using QFSW.QC;
using LibNoise.Unity.Generator;

public class World : MonoBehaviour
{
    public static World instance;
    public int ChunkRenderRadius = 2;
    [HideInInspector]
    public const int ChunkSize = 16; // how many blocks in each direction are in a chunk, used for correctly positioning chunks

    public string Name;
    public int Seed;
    public bool RandomSeed = true;
    public AnimationCurve HeightCurve;

    public FastNoise vore = new FastNoise(); // i hate this

    public GameObject Player;
    private Vector3Int previousPlayerChunk = Vector3Int.zero;
    private Vector3Int currentPlayerChunk = Vector3Int.zero;

    public Material TerrainMaterial;
    public Material DebugMaterial;

    public Vector3 WorldSpawn;
    [HideInInspector]
    public Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        if(RandomSeed)
            Seed = Random.Range(int.MinValue, int.MaxValue);

        WorldSpawn = new Vector3(0.5f,300,0.5f);

        vore.SetNoiseType(FastNoise.NoiseType.Perlin);
        vore.SetCellularJitter(0f);
        vore.SetSeed(Seed);
        vore.SetFrequency(0.05f);

        Vector3Int RoundedPlayerPosition = Player.transform.position.RoundToInt();
        currentPlayerChunk = new Vector3Int ((int)(RoundedPlayerPosition.x / ChunkSize), (int)(RoundedPlayerPosition.y / ChunkSize), (int)(RoundedPlayerPosition.z / ChunkSize));
        LoadChunksAroundPlayer(currentPlayerChunk);
    }

    private void Start() {
        // Set World Spawn
        int _h = Chunk.GetTerrainNoise(0,0, Vector3.zero);
        WorldSpawn = new Vector3(0.5f,_h+10,0.5f);
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
        // get all chunks around player
        List<Vector3Int> chunksAroundPlayer = new List<Vector3Int>();
        int crr = ChunkRenderRadius+1;
        for (int x = -crr; x <= crr; x++) {
            for (int y = -crr; y <= crr; y++) {
                for (int z = -crr; z <= crr; z++) {
                    Vector3Int p = new Vector3Int(playerChunkPos.x+x, playerChunkPos.y+y, playerChunkPos.z+z);
                    // dont generate chunks under y 0, may be changed at some point
                    if (p.y < 0) continue; 

                    if (!chunksAroundPlayer.Contains(p)) {
                        chunksAroundPlayer.Add(p);
                    }
                }
            }
        }

        // figure out what do do with the chunks around player
        List<Vector3Int> chunksToUnload = new List<Vector3Int>();
        List<Vector3Int> chunksToLoad = new List<Vector3Int>();
        List<Vector3Int> edgeChunks = new List<Vector3Int>();

        for (int i = 0; i < chunksAroundPlayer.Count; i++) {
            Vector3Int relativeChunkPos = chunksAroundPlayer[i] - playerChunkPos;

            if (!chunks.ContainsKey(chunksAroundPlayer[i])) {
                if (Mathf.Abs(relativeChunkPos.x) == crr || Mathf.Abs(relativeChunkPos.y) == crr || Mathf.Abs(relativeChunkPos.z) == crr) {
                    edgeChunks.Add(chunksAroundPlayer[i]);
                    continue;
                }
                //these chunks are not currenly loaded so need to be loaded
                chunksToLoad.Add(chunksAroundPlayer[i]);
            } else {
                // check if chunk is an invisible chunk
                if (!chunks[chunksAroundPlayer[i]].RenderChunk) {
                    UnloadChunk(chunksAroundPlayer[i].x, chunksAroundPlayer[i].y, chunksAroundPlayer[i].z);
                    LoadChunk(chunksAroundPlayer[i].x, chunksAroundPlayer[i].y, chunksAroundPlayer[i].z);
                }
            }
        }

        for (int i = 0; i < chunks.Keys.Count; i++) {
            if (!chunksAroundPlayer.Contains(chunks.Keys.ElementAt(i))) {
                // these chunks are currently loaded but no longer need to be loaded
                chunksToUnload.Add(chunks.Keys.ElementAt(i));
            }
        }

        for (int i = 0; i < chunksToUnload.Count; i++) {
            UnloadChunk(chunksToUnload[i].x, chunksToUnload[i].y, chunksToUnload[i].z);
        }

        for (int i = 0; i < chunksToLoad.Count; i++) {
            LoadChunk(chunksToLoad[i].x, chunksToLoad[i].y, chunksToLoad[i].z);
        }

        for (int i = 0; i < edgeChunks.Count; i++) {
            LoadChunk(edgeChunks[i].x, edgeChunks[i].y, edgeChunks[i].z, true);
        }

        SetAdjacentChunkReferences();
    }

    void GenerateChunk(int x, int y, int z, bool invisible) {
        GameObject chunk = new GameObject($"Chunk_{x},{y},{z}"); // create new chunk
        chunk.isStatic = true;

        Vector3Int chunkPos = new Vector3Int(x, y, z);
        Vector3Int worldPos = chunkPos * ChunkSize;
        chunk.transform.position = worldPos; // position chunk 

        if (!invisible) {
            Mesh chunkMesh = new Mesh(); // create chunk mesh
            chunkMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32; // set chunk mesh to use 32 bit indices as chunk vertices can possibly be > 65565 (16 bit index format)
            chunk.AddComponent<MeshFilter>().sharedMesh = chunkMesh; // add mesh to chunk
            chunk.AddComponent<MeshRenderer>().material = TerrainMaterial; // add renderer to chunk so you can actually see it
            chunk.AddComponent<MeshCollider>(); // eventually when some blocks are able to have different collision meshes or no collision this will have to be its own mesh
        }
        chunk.layer = 8;

        // actual chunk stuff
        Chunk c = chunk.AddComponent<Chunk>();
        c.ChunkPosition = worldPos;
        c.RenderChunk = !invisible;
        if (chunks.TryGetValue(chunkPos, out Chunk value)) {
            Debug.Log(value.RenderChunk);
        }
        chunks.Add(chunkPos, c);
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
            return BlockList.instance.blocks["Stone"]; // this should normally be set to stone, its only set to air for debugging purposes because that allows you to see the sides of blocks that face the edge of the world
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
        Vector3Int cpos = BlockToChunkSpace(x, y, z);

        // make sure the position is even in the world
        if (!chunks.ContainsKey(cpos)) {
            return null;
        }

        return chunks[cpos];
    }

    [Command("loadchunk")]
    public void LoadChunk (int x, int y, int z, bool invisible = false) {
        GenerateChunk(x,y,z, invisible);
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

    [Command("fill")]
    public void Fill (int x1, int y1, int z1, int x2, int y2, int z2, string block) {
        int _x1, _x2, _y1, _y2, _z1, _z2;

        if (x1 > x2) {
            _x1 = x2;
            _x2 = x1;
        } else {
            _x1 = x1;
            _x2 = x2;
        }

        if (y1 > y2) {
            _y1 = y2;
            _y2 = y1;
        } else {
            _y1 = y1;
            _y2 = y2;
        }

        if (z1 > z2) {
            _z1 = z2;
            _z2 = z1;
        } else {
            _z1 = z1;
            _z2 = z2;
        }

        for (int x = _x1; x <= _x2; x++) {
            for (int y = _y1; y <= _y2; y++) {
                for (int z = _z1; z <= _z2; z++) {
                    SetBlock(x, y, z, block);
                }
            }
        }
    }

    [Command("getblock")]
    public string GetBlockCmd (int x, int y, int z) {
        return (GetBlock(x, y, z).name);
    }

    [Command("renderdistance")]
    public void SetRenderDistance(int distance) {
        ChunkRenderRadius = distance;
    }

    public void UpdateChunk (int x, int y, int z, bool updateAdjacentBlocks) {
        Chunk chunkToUpdate = GetChunkFromBlockCoords (x,y,z);
        chunkToUpdate.update = true;

        if (updateAdjacentBlocks) {
            // convert world position to chunk position
            Vector3Int chunkPos = BlockToChunkSpace(x,y,z);

            // convert world position to position relative to chunk
            int rx = x.Mod(ChunkSize);
            int ry = y.Mod(ChunkSize);
            int rz = z.Mod(ChunkSize);

            if (rx == 0 && chunkToUpdate.WestNeighbor != null) chunkToUpdate.WestNeighbor.update = true;
            if (rx == ChunkSize-1 && chunkToUpdate.EastNeighbor != null) chunkToUpdate.EastNeighbor.update = true;
            if (ry == 0 && chunkToUpdate.BottomNeighbor != null) chunkToUpdate.BottomNeighbor.update = true;
            if (ry == ChunkSize-1 && chunkToUpdate.TopNeighbor != null) chunkToUpdate.TopNeighbor.update = true;
            if (rz == 0 && chunkToUpdate.SouthNeighbor != null) chunkToUpdate.SouthNeighbor.update = true;
            if (rz == ChunkSize-1 && chunkToUpdate.NorthNeighbor != null) chunkToUpdate.NorthNeighbor.update = true;
        }
    }

    public static Vector3Int BlockToChunkSpace (int x, int y, int z) {
        Vector3 p = new Vector3(x, y, z);
        return Vector3Int.FloorToInt(p / ChunkSize);
    }

    [Command("saveworld")]
    public void Save () {
        SaveLoadData.instance.SaveWorld(chunks);
    }

    // unloads all chunks and wipes the chunk dictionary
    public void DeleteAllChunks () {
        foreach (KeyValuePair<Vector3Int, Chunk> kvp in chunks) {
            kvp.Value.Unload();
        }
        chunks.Clear();
    }
}