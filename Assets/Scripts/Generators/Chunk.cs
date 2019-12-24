using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public const int ChunkSize = World.ChunkSize; // this should be the same value as the chunksize in the world script
    public Vector3Int ChunkPosition;

    public Block[,,] blocks; // 3d array of all the blocks in the chunk

    public bool RenderChunk = true;

    public Chunk NorthNeighbor, SouthNeighbor, EastNeighbor, WestNeighbor, TopNeighbor, BottomNeighbor;

    List<Vector3> verts;
    Dictionary<TextureGroup, List<int>> tris;
    Dictionary<TextureGroup, List<Vector2>> uvs;

    private List<Vector2> faceUV;
    private List<int> t;

    private void Awake() {
        // if chunk isn't even going to get rendered, there's no point in initializing these lists
        if (RenderChunk) {
            blocks = new Block[ChunkSize, ChunkSize, ChunkSize];
            verts = new List<Vector3>(ChunkSize * ChunkSize * ChunkSize * 24);
            tris = new Dictionary<TextureGroup, List<int>>(ChunkSize * ChunkSize * ChunkSize * 36);
            uvs = new Dictionary<TextureGroup, List<Vector2>>(ChunkSize * ChunkSize * ChunkSize * 24);
            //tris = new List<int>(ChunkSize * ChunkSize * ChunkSize * 36);
            //uvs = new List<Vector2>(ChunkSize * ChunkSize * ChunkSize * 24);

            t = new List<int>(6);

            faceUV = new List<Vector2>(4) {
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1)
            };
        }
    }

    private void Start() {
        GenerateBlocks();
        if (RenderChunk) {
            GenerateChunkMesh();
        }
    }

    void GenerateBlocks() {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();
        for (int x = 0; x < ChunkSize; x++) {
            for (int z = 0; z < ChunkSize; z++) {

                Biome _b = GetDitheredBiome(x,z,ChunkPosition);
                int _h = GetTerrainNoise(x, z, ChunkPosition);
                int dirtLevel = Random.Range(_h-4, _h-6);
                int bedrockLevel = Random.Range(0,3);

                for (int y = 0; y < ChunkSize; y++) {
                    if (y+ChunkPosition.y == _h-1) {
                        SetBlock(x, y, z, BlockList.instance.blocks[_b.surfaceBlock]);
                    } else if (y + ChunkPosition.y < _h-1 && y + ChunkPosition.y > dirtLevel) {
                        SetBlock(x, y, z, BlockList.instance.blocks[_b.subSurfaceBlock]);
                    } else if (y + ChunkPosition.y <= dirtLevel) {
                        SetBlock(x, y, z, BlockList.instance.blocks["Stone"]);
                    } else {
                        SetBlock(x, y, z, BlockList.instance.blocks["Air"]);
                    }

                    // Bedrock layer
                    if (y+ChunkPosition.y <= bedrockLevel) {
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

        for (int x = 0; x < ChunkSize; x++) {
            for (int y = 0; y < ChunkSize; y++) {
                for (int z = 0; z < ChunkSize; z++) {

                    if (blocks[x, y, z].name == "Air")
                        continue;

                    // Calculate faces that need to be rendered and generate the mesh data

                    // top
                    if (GetBlock(x, y + 1, z).translucent) {
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        AddTrisAndUVS(faceCount, GetBlock(x,y,z).GetTextureGroup(Faces.Top));
                        faceCount++;
                    }

                    // bottom
                    if (GetBlock(x, y - 1, z).translucent) {
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        verts.Add(new Vector3(x, y, z + 1));
                        verts.Add(new Vector3(x, y, z));
                        AddTrisAndUVS(faceCount, GetBlock(x, y, z).GetTextureGroup(Faces.Bottom));
                        faceCount++;
                    }

                    // left
                    if (GetBlock(x - 1, y, z).translucent) {
                        verts.Add(new Vector3(x, y, z + 1));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x, y, z));
                        AddTrisAndUVS(faceCount, GetBlock(x, y, z).GetTextureGroup(Faces.Left));
                        faceCount++;
                    }

                    // right
                    if (GetBlock(x + 1, y, z).translucent) {
                        verts.Add(new Vector3(x + 1, y, z));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        AddTrisAndUVS(faceCount, GetBlock(x, y, z).GetTextureGroup(Faces.Right));
                        faceCount++;
                    }

                    // front
                    if (GetBlock(x, y, z - 1).translucent) {
                        verts.Add(new Vector3(x, y, z));
                        verts.Add(new Vector3(x, y + 1, z));
                        verts.Add(new Vector3(x + 1, y + 1, z));
                        verts.Add(new Vector3(x + 1, y, z));
                        AddTrisAndUVS(faceCount, GetBlock(x, y, z).GetTextureGroup(Faces.Front));
                        faceCount++;
                    }

                    // back
                    if (GetBlock(x, y, z + 1).translucent) {
                        verts.Add(new Vector3(x + 1, y, z + 1));
                        verts.Add(new Vector3(x + 1, y + 1, z + 1));
                        verts.Add(new Vector3(x, y + 1, z + 1));
                        verts.Add(new Vector3(x, y, z + 1));
                        AddTrisAndUVS(faceCount, GetBlock(x, y, z).GetTextureGroup(Faces.Back));
                        faceCount++;
                    }
                }
            }
        }

        // create new mesh out of the new verts and tris and apply it to the chunk
        MeshFilter MF = GetComponent<MeshFilter>();
        MeshRenderer MR = GetComponent<MeshRenderer>();
        Mesh chunkMesh = MF.sharedMesh;
        chunkMesh.Clear(); // clear current mesh
        chunkMesh.SetVertices(verts); // set vertices

        Material[] chunkMats = new Material[tris.Count];

        int submesh = 0;
        foreach (KeyValuePair<TextureGroup, List<int>> entry in tris) {
            chunkMesh.SetTriangles(entry.Value, submesh);
            chunkMesh.SetUVs(submesh, uvs[entry.Key]);
            chunkMats[submesh] = entry.Key.mainMaterial;
            submesh++;
        }

        MR.materials = chunkMats;

        //chunkMesh.SetTriangles(tris,0); // set triangles
        //chunkMesh.SetUVs(0,uvs); // set uvs

        chunkMesh.RecalculateNormals(); // calculate normals
        Helper.CalculateMeshTangents(chunkMesh); // calculate tangents

        GetComponent<MeshCollider>().sharedMesh = chunkMesh;

        //sw.Stop();
        //UnityEngine.Debug.Log($"Generating chunk mesh took {sw.ElapsedMilliseconds}ms");
    }

    private void AddTrisAndUVS (int fc, TextureGroup tg) {
        t.Clear();
        t.Add(fc * 4);
        t.Add(fc * 4 + 1);
        t.Add(fc * 4 + 2);
        t.Add(fc * 4);
        t.Add(fc * 4 + 2);
        t.Add(fc * 4 + 3);

        if(tris.ContainsKey(tg)) {
            tris[tg].AddRange(t);
        } else {
            tris.Add(tg, t);
        }

        if(uvs.ContainsKey(tg)) {
            uvs[tg].AddRange(faceUV);
        } else {
            uvs.Add(tg, faceUV);
        }
    }

    /*
    private void AddUvs (Vector2 texture) {
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + UVBleedCompromise, TextureAtlasSize * ((1/TextureAtlasSize) - 1 - texture.y) + UVBleedCompromise)); // Top left corner
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + UVBleedCompromise, TextureAtlasSize * ((1/TextureAtlasSize) - 1 - texture.y) + TextureAtlasSize - UVBleedCompromise)); // bottom left corner
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + TextureAtlasSize - UVBleedCompromise, TextureAtlasSize * ((1/TextureAtlasSize) - 1 - texture.y) + TextureAtlasSize - UVBleedCompromise)); // bottom right corner
        uvs.Add(new Vector2(TextureAtlasSize * texture.x + TextureAtlasSize - UVBleedCompromise, TextureAtlasSize * ((1/TextureAtlasSize) - 1 - texture.y) + UVBleedCompromise)); // top right corner
    }
    */

    public static int GetTerrainNoise(int x, int y, Vector3 Offset) {
        float biomenoise = GetBiomeNoise(x,y,Offset);
        float multipliedBiomeNoise = biomenoise * BiomeList.instance.biomesArray.Length;
        Biome b1 = BiomeList.instance.biomesArray[(int)multipliedBiomeNoise];
        Biome b2 = BiomeList.instance.biomesArray[Mathf.Clamp((int)multipliedBiomeNoise+1, 0, BiomeList.instance.biomesArray.Length-1)];

        float noiseMap1 = PerlinNoise(x + (int)Offset.x,  y + (int)Offset.z, b1.scale, b1.height, b1.multiplier, World.instance.Seed)
                                    + (PerlinNoise(x + (int)Offset.x, y + (int)Offset.z, b1.scale / b1.roughnessFactor, b1.height, b1.multiplier, World.instance.Seed) * (1f / b1.roughnessFactor)
                                    + PerlinNoise(x + (int)Offset.x, y + (int)Offset.z, b1.scale / (b1.roughnessFactor * b1.roughnessFactor), b1.height, b1.multiplier, World.instance.Seed) * (1f / (b1.roughnessFactor * b1.roughnessFactor)) * b1.roughnessStrength);
        noiseMap1 /= World.instance.HeightCurve.Evaluate(noiseMap1 / b1.height);

        float noiseMap2 = PerlinNoise(x + (int)Offset.x, y + (int)Offset.z, b2.scale, b2.height, b2.multiplier, World.instance.Seed)
                                    + (PerlinNoise(x + (int)Offset.x, y + (int)Offset.z, b2.scale / b2.roughnessFactor, b2.height, b2.multiplier, World.instance.Seed) * (1f / b2.roughnessFactor)
                                    + PerlinNoise(x + (int)Offset.x, y + (int)Offset.z, b2.scale / (b2.roughnessFactor * b2.roughnessFactor), b2.height, b2.multiplier, World.instance.Seed) * (1f / (b2.roughnessFactor * b2.roughnessFactor)) * b2.roughnessStrength);
        noiseMap2 /= World.instance.HeightCurve.Evaluate(noiseMap2 / b2.height);
        
        float smoothedNoiseMap = Mathf.Lerp(noiseMap1, noiseMap2, multipliedBiomeNoise - (int)multipliedBiomeNoise);

        return (int)smoothedNoiseMap;
    }

    public static float GetBiomeNoise(int x, int y, Vector3 Offset) {
        float nm = World.instance.vore.GetNoise(x + (int)Offset.x, y + (int)Offset.z);
        return (1 + nm)/2;
    }

    public static Biome GetDitheredBiome (int x, int y, Vector3 Offset) {
        float nm = GetBiomeNoise(x,y,Offset);
        float multipliedValue = nm * BiomeList.instance.biomesArray.Length;
        int biome1Index = (int)multipliedValue;
        int biome2Index = Mathf.Clamp((int)multipliedValue+1, 0, BiomeList.instance.biomesArray.Length-1);
        Biome biome1 = BiomeList.instance.biomesArray[biome1Index];
        Biome biome2 = BiomeList.instance.biomesArray[biome2Index];
        float smoothingValue = multipliedValue - biome1Index;
        if (smoothingValue > 0.8) {
            float ditherAmount = Random.Range(0f,0.1f);
            ditherAmount += 0.8f;
            if (ditherAmount < smoothingValue) return biome2;
        }
        return biome1;
    }

    public static Biome GetBiome (int x, int y, Vector3 Offset) {
        float nm = GetBiomeNoise(x,y,Offset);
        int biomeIndex = (int)(nm * BiomeList.instance.biomesArray.Length);
        return BiomeList.instance.biomesArray[biomeIndex];
    }

    private static float PerlinNoise (int x, int y, float scale, float height, float power, int seed) {
        FastNoise fn = new FastNoise();
        fn.SetNoiseType(FastNoise.NoiseType.Perlin);
        fn.SetSeed(seed);
        fn.SetFrequency(height/2);
        float rVal = fn.GetNoise(x/scale, y/scale) + height/2;
        if (System.Math.Abs(power) > 0) {
            rVal = Mathf.Pow(rVal, power);
        }
        return rVal;
    }

    Block GetBlock (int x, int y, int z) {
        return World.instance.GetBlock((int)ChunkPosition.x + x, (int)ChunkPosition.y + y, (int)ChunkPosition.z + z);
    }

    public void UpdateChunk () {
        GenerateChunkMesh();
    }

    public void Unload () {
        if (RenderChunk)
            Destroy(GetComponent<MeshFilter>().mesh);
        Destroy(gameObject);
    }

}
