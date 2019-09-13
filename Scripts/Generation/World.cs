//using System.Diagnostics;
using UnityEngine;
using System.Collections;

public class World : MonoBehaviour
{
    public static World instance;

    public int WorldSize = 10; // will be removed later on when implementing infinite terrain, only here while terrain generation is being added
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

    public Mesh DebugChunkMesh; // debug
    public Material DebugChunkMaterial; // debug

    Chunk[,,] chunks;

    // abra kadabra 

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }

        chunks = new Chunk[WorldSize, WorldSize, WorldSize];
        Seed = Random.Range(int.MinValue, int.MaxValue);
        StartCoroutine(GenerateChunks());
    }

    IEnumerator GenerateChunks() {
        for (int x = 0; x < WorldSize; x++) {
            for (int y = 0; y < WorldSize; y++) {
                for (int z = 0; z < WorldSize; z++) {
                    GameObject chunk = new GameObject($"Chunk_{x},{y},{z}"); // create new chunk
                    chunk.isStatic = true;
                    chunk.transform.position = new Vector3Int(x * ChunkSize, y * ChunkSize, z * ChunkSize); // position chunk 

                    chunk.AddComponent<MeshFilter>(); // add mesh to chunk
                    chunk.AddComponent<MeshRenderer>().material = DebugChunkMaterial; // add renderer to chunk so you can actually see it
                    chunk.AddComponent<MeshCollider>();
                    chunk.layer = 8;

                    // actual chunk stuff
                    chunks[x, y, z] = chunk.AddComponent<Chunk>();
                    yield return null;
                }
            }
        }
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
            if (y < parentChunk.GetTerrainNoise(x, z, Vector3.zero)) {
                return BlockList.instance.blocks["Stone"];
            } else {
                return BlockList.instance.blocks["Air"];
            }
        }

        // Get the desired block by using modulo to get the correct block position in the chunk
        Block desiredBlock = parentChunk.blocks[x % ChunkSize, y % ChunkSize, z % ChunkSize];

        return desiredBlock;
    }

    public Chunk GetChunkFromBlockCoords(int x, int y, int z) {
        int blockLen = WorldSize * ChunkSize;

        // make sure the position is even in the world
        if (x >= blockLen || x < 0 || y >= blockLen || y < 0 || z >= blockLen || z < 0) {
            return null;
        }

        return chunks[Mathf.FloorToInt((float)x / ChunkSize), Mathf.FloorToInt((float)y / ChunkSize), Mathf.FloorToInt((float)z / ChunkSize)];
    }

    public void SetBlock(int x, int y, int z, Block value) {
        Chunk _chunk = GetChunkFromBlockCoords(x, y, z);
        if (_chunk != null) {
            _chunk.SetBlock(x % ChunkSize, y % ChunkSize, z % ChunkSize, value);
        }
    }

    public void UpdateChunk (int x, int y, int z, bool updateAdjacentBlocks) {
        Chunk chunkToUpdate = GetChunkFromBlockCoords (x,y,z);
        chunkToUpdate.UpdateChunk();

        if (updateAdjacentBlocks) {
            int cx = Mathf.FloorToInt( x / 16 );
            int cy = Mathf.FloorToInt( y / 16 );
            int cz = Mathf.FloorToInt( z / 16 );

            int rx = x % 16;
            int ry = y % 16;
            int rz = z % 16;

            if (rx == 0 && cx > 0) chunks[cx-1,cy,cz].UpdateChunk();
            if (rx == 15 && cx < chunks.GetLength(0)-1) chunks[cx+1,cy,cz].UpdateChunk();
            if (ry == 0 && cy > 0) chunks[cx,cy-1,cz].UpdateChunk();
            if (ry == 15 && cy < chunks.GetLength(1)-1) chunks[cx,cy+1,cz].UpdateChunk();
            if (rz == 0 && cz > 0) chunks[cx,cy,cz-1].UpdateChunk();
            if (rz == 15 && cz < chunks.GetLength(2)-1) chunks[cx,cy,cz+1].UpdateChunk();
        }
    }

}
