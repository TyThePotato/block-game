using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int ChunkSize = World.ChunkSize; // this should be the same value as the chunksize in the world script
    public Block[,,] blocks; // 3d array of all the blocks in the chunk

    public const float TextureAtlasSize = 0.25f; //0.0625f;
    public const float UVBleedCompromise = 0.001953125f; //0.0009765625f;

    public Chunk NorthNeighbor, SouthNeighbor, EastNeighbor, WestNeighbor, TopNeighbor, BottomNeighbor;

    List<Vector3> verts;
    List<int> tris;
    List<Vector2> uvs;
    List<Vector2> coloruvs; // experimental

    private void Awake() {
        blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
        verts = new List<Vector3>(ChunkSize*ChunkSize*ChunkSize*24);
        tris = new List<int>(ChunkSize*ChunkSize*ChunkSize*36);
        uvs = new List<Vector2>(ChunkSize*ChunkSize*ChunkSize*24);
        coloruvs = new List<Vector2>(ChunkSize*ChunkSize*ChunkSize*24);
    }

    private void Start() {
        GenerateBlocks();
        GenerateChunkMesh();
    }

    void GenerateBlocks() {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();
        for (int x = 0; x < ChunkSize; x++) {
            for (int z = 0; z < ChunkSize; z++) {

                int _h = GetTerrainNoise(x, z, transform.position);
                int dirtLevel = Random.Range(_h-5, _h-6);
                int bedrockLevel = Random.Range(0,3);

                for (int y = 0; y < ChunkSize; y++) {
                    if (y+transform.position.y == _h-1) {
                        SetBlock(x, y, z, BlockList.instance.blocks["Grass"]);
                    } else if (y + transform.position.y < _h-1 && y + transform.position.y > dirtLevel) {
                        SetBlock(x, y, z, BlockList.instance.blocks["Dirt"]);
                    } else if (y + transform.position.y <= dirtLevel) {
                        SetBlock(x, y, z, BlockList.instance.blocks["Stone"]);
                    } else {
                        SetBlock(x, y, z, BlockList.instance.blocks["Air"]);
                    }

                    // Bedrock layer
                    if (y+transform.position.y <= bedrockLevel) {
                        SetBlock(x,y,z, BlockList.instance.blocks["Bedrock"]);
                    }
                }
            }
        }
        //sw.Stop();
        //UnityEngine.Debug.Log($"Populating block array took {sw.ElapsedMilliseconds}ms");
    }

    public void SetBlock(int x, int y, int z, Block value) {
        blocks[x, y, z] = value;
    }

    // TODO: greedy meshing
    public void GenerateChunkMesh() {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        int faceCount = 0;
        verts.Clear();
        tris.Clear();
        uvs.Clear();

        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkSize; y++) {
                for (int z = 0; z < ChunkSize; z++) {

                    if (blocks[x, y, z].name == "Air")
                        continue;

                    List<Faces> facesToDraw = FacesToDraw(x, y, z);

                    // top
                    if (facesToDraw.Contains(Faces.Top)) {
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        AddTris(faceCount);
                        AddUvs(blocks[x,y,z].GetTexture(Faces.Top));
                        faceCount++;
                    }

                    // bottom
                    if (facesToDraw.Contains(Faces.Bottom)) {
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        verts.Add(new Vector3(x, y, z + 1));
                        verts.Add(new Vector3(x, y, z));
                        AddTris(faceCount);
                        AddUvs(blocks[x,y,z].GetTexture(Faces.Bottom));
                        faceCount++;
                    }

                    // left
                    if (facesToDraw.Contains(Faces.Left)) {
                        verts.Add(new Vector3(x, y, z + 1));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x, y, z));
                        AddTris(faceCount);
                        AddUvs(blocks[x,y,z].GetTexture(Faces.Left));
                        faceCount++;
                    }

                    // right
                    if (facesToDraw.Contains(Faces.Right)) {
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        AddTris(faceCount);
                        AddUvs(blocks[x,y,z].GetTexture(Faces.Right));
                        faceCount++;
                    }

                    // front
                    if (facesToDraw.Contains(Faces.Front)) {
                        verts.Add(new Vector3(x, y, z));
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x + 1, y, z));
                        AddTris(faceCount);
                        AddUvs(blocks[x,y,z].GetTexture(Faces.Front));
                        faceCount++;
                    }

                    // back
                    if (facesToDraw.Contains(Faces.Back)) {
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x, y, z + 1));
                        AddTris(faceCount);
                        AddUvs(blocks[x,y,z].GetTexture(Faces.Back));
                        faceCount++;
                    }
                }
            }
        }

        // create new mesh out of the new verts and tris and apply it to the chunk
        Mesh chunkMesh = new Mesh();
        chunkMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        chunkMesh.SetVertices(verts);
        chunkMesh.SetTriangles(tris,0);
        chunkMesh.SetUVs(0,uvs);
        chunkMesh.SetUVs(1,coloruvs);

        chunkMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = chunkMesh;
        GetComponent<MeshCollider>().sharedMesh = chunkMesh;

        //sw.Stop();
        //UnityEngine.Debug.Log($"Generating chunk mesh took {sw.ElapsedMilliseconds}ms");
    }

    private void AddTris (int fc) {
        tris.Add(fc * 4);
        tris.Add(fc * 4 + 1);
        tris.Add(fc * 4 + 2);
        tris.Add(fc * 4);
        tris.Add(fc * 4 + 2);
        tris.Add(fc * 4 + 3);
    }

    private void AddUvs (Vector2 texture) {
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + UVBleedCompromise, TextureAtlasSize * texture.y + UVBleedCompromise));
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + UVBleedCompromise, TextureAtlasSize * texture.y + TextureAtlasSize - UVBleedCompromise));
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + TextureAtlasSize - UVBleedCompromise, TextureAtlasSize * texture.y + TextureAtlasSize - UVBleedCompromise));
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + TextureAtlasSize - UVBleedCompromise, TextureAtlasSize * texture.y + UVBleedCompromise));
    }

    public int GetTerrainNoise(int x, int y, Vector3 Offset) {
        float noiseMap = PerlinNoise(x + (int)Offset.x, 0, y + (int)Offset.z, World.instance.BaseScale, World.instance.BaseScale, World.instance.MaxHeight, World.instance.Multiplier, World.instance.Seed)
                                    + (PerlinNoise(x + (int)Offset.x, 0, y + (int)Offset.z, World.instance.BaseScale / World.instance.RoughnessFactor, World.instance.BaseScale, World.instance.MaxHeight, World.instance.Multiplier, World.instance.Seed) * (1f / World.instance.RoughnessFactor)
                                    + PerlinNoise(x + (int)Offset.x, 0, y + (int)Offset.z, World.instance.BaseScale / (World.instance.RoughnessFactor * World.instance.RoughnessFactor), World.instance.BaseScale, World.instance.MaxHeight, World.instance.Multiplier, World.instance.Seed) * (1f / (World.instance.RoughnessFactor * World.instance.RoughnessFactor)) * World.instance.RoughnessStrength);

        noiseMap /= World.instance.HeightCurve.Evaluate(noiseMap / World.instance.MaxHeight);
        return Mathf.FloorToInt(noiseMap);
    }

    private float PerlinNoise(int x, int y, int z, float scale, float yScale, float height, float power, int seed) {
        float rVal = Noise.Noise.GetNoise(((double)x + seed) / scale, ((double)y + seed) / yScale, ((double)z + seed) / scale);
        rVal *= height;

        if (System.Math.Abs(power) > 0) {
            rVal = Mathf.Pow(rVal, power);
        }
        return rVal;
    }

    List<Faces> FacesToDraw (int x, int y, int z) {
        List<Faces> ftd = new List<Faces>();

        // Top
        if (GetBlock(x,y+1,z).translucent) {
            ftd.Add(Faces.Top);
        }
        // Bottom
        if (GetBlock(x, y-1, z).translucent) {
            ftd.Add(Faces.Bottom);
        }
        // Left
        if (GetBlock(x-1, y, z).translucent) {
            ftd.Add(Faces.Left);
        }
        // Right
        if (GetBlock(x+1, y, z).translucent) {
            ftd.Add(Faces.Right);
        }
        // Front
        if (GetBlock(x, y, z-1).translucent) {
            ftd.Add(Faces.Front);
        }
        // Back
        if (GetBlock(x, y, z+1).translucent) {
            ftd.Add(Faces.Back);
        }

        return ftd;
    }

    Block GetBlock (int x, int y, int z) {
        return World.instance.GetBlock((int)transform.position.x + x, (int)transform.position.y + y, (int)transform.position.z + z);
    }

    public void UpdateChunk () {
        GenerateChunkMesh();
    }

    public void Unload () {
        Destroy(GetComponent<MeshFilter>().mesh);
        Destroy(gameObject);
    }

}
