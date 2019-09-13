using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPreview : MonoBehaviour
{
    public BlockBreaker bb;
    Block lastBlock;
    float TextureAtlasSize = Chunk.TextureAtlasSize;
    float UVBleedCompromise = Chunk.UVBleedCompromise;
    List<Vector3> verts;
    List<int> tris;
    List<Vector2> uvs;

    private void Start() {
        lastBlock = bb.currentlySelectedBlock;
        GenerateBlockMesh();
    }

    private void Update() {
        if (lastBlock != bb.currentlySelectedBlock) {
            lastBlock = bb.currentlySelectedBlock;
            GenerateBlockMesh();
        }
    }

    public void GenerateBlockMesh()
    {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        int faceCount = 0;
        verts = new List<Vector3>(98304);
        tris = new List<int>(147456);
        uvs = new List<Vector2>(98304);

        // top

        verts.Add(new Vector3(0, 0 + 1, 0));
        verts.Add(new Vector3(0, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0));
        AddTris(faceCount);
        AddUvs(bb.currentlySelectedBlock.GetTexture(Faces.Top));
        faceCount++;


        // bottom

        verts.Add(new Vector3(0 + 1, 0, 0));
        verts.Add(new Vector3(0 + 1, 0, 0 + 1));
        verts.Add(new Vector3(0, 0, 0 + 1));
        verts.Add(new Vector3(0, 0, 0));
        AddTris(faceCount);
        AddUvs(bb.currentlySelectedBlock.GetTexture(Faces.Bottom));
        faceCount++;


        // left

        verts.Add(new Vector3(0, 0, 0 + 1));
        verts.Add(new Vector3(0, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0, 0 + 1, 0));
        verts.Add(new Vector3(0, 0, 0));
        AddTris(faceCount);
        AddUvs(bb.currentlySelectedBlock.GetTexture(Faces.Left));
        faceCount++;


        // right

        verts.Add(new Vector3(0 + 1, 0, 0));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0, 0 + 1));
        AddTris(faceCount);
        AddUvs(bb.currentlySelectedBlock.GetTexture(Faces.Right));
        faceCount++;


        // front

        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0, 0 + 1, 0));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0));
        verts.Add(new Vector3(0 + 1, 0, 0));
        AddTris(faceCount);
        AddUvs(bb.currentlySelectedBlock.GetTexture(Faces.Front));
        faceCount++;


        // back

        verts.Add(new Vector3(0 + 1, 0, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0, 0, 0 + 1));
        AddTris(faceCount);
        AddUvs(bb.currentlySelectedBlock.GetTexture(Faces.Back));
        faceCount++;


        // create new mesh out of the new verts and tris and apply it to the chunk
        Mesh chunkMesh = new Mesh();
        chunkMesh.SetVertices(verts);
        chunkMesh.SetTriangles(tris, 0);
        chunkMesh.SetUVs(0, uvs);

        chunkMesh.RecalculateNormals();
        GetComponent<MeshFilter>().mesh = chunkMesh;
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
}
