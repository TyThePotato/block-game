using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockPreview : MonoBehaviour
{
    List<Vector3> verts;
    Dictionary<TextureGroup, List<int>> tris;
    Dictionary<TextureGroup, List<Vector2>> uvs;

    private List<Vector2> faceUV;
    private List<int> t;

    private void Awake () {
        verts = new List<Vector3>(24);
        tris = new Dictionary<TextureGroup, List<int>>(36);
        uvs = new Dictionary<TextureGroup, List<Vector2>>(24);

        t = new List<int>(6);

        faceUV = new List<Vector2>(4) {
                new Vector2(0,1),
                new Vector2(0,0),
                new Vector2(1,0),
                new Vector2(1,1)
         };
    }

    public GameObject GenerateBlockMesh(Block block)
    {
        //Stopwatch sw = new Stopwatch();
        //sw.Start();

        int faceCount = 0;
        verts.Clear();
        tris.Clear();

        // top

        verts.Add(new Vector3(0, 0 + 1, 0));
        verts.Add(new Vector3(0, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0));
        AddTrisAndUVS(faceCount, block.GetTextureGroup(Faces.Top));
        faceCount++;


        // bottom

        verts.Add(new Vector3(0 + 1, 0, 0));
        verts.Add(new Vector3(0 + 1, 0, 0 + 1));
        verts.Add(new Vector3(0, 0, 0 + 1));
        verts.Add(new Vector3(0, 0, 0));
        AddTrisAndUVS(faceCount, block.GetTextureGroup(Faces.Bottom));
        faceCount++;


        // left

        verts.Add(new Vector3(0, 0, 0 + 1));
        verts.Add(new Vector3(0, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0, 0 + 1, 0));
        verts.Add(new Vector3(0, 0, 0));
        AddTrisAndUVS(faceCount, block.GetTextureGroup(Faces.Left));
        faceCount++;


        // right

        verts.Add(new Vector3(0 + 1, 0, 0));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0, 0 + 1));
        AddTrisAndUVS(faceCount, block.GetTextureGroup(Faces.Right));
        faceCount++;


        // front

        verts.Add(new Vector3(0, 0, 0));
        verts.Add(new Vector3(0, 0 + 1, 0));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0));
        verts.Add(new Vector3(0 + 1, 0, 0));
        AddTrisAndUVS(faceCount, block.GetTextureGroup(Faces.Front));
        faceCount++;


        // back

        verts.Add(new Vector3(0 + 1, 0, 0 + 1));
        verts.Add(new Vector3(0 + 1, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0, 0 + 1, 0 + 1));
        verts.Add(new Vector3(0, 0, 0 + 1));
        AddTrisAndUVS(faceCount, block.GetTextureGroup(Faces.Back));
        faceCount++;


        // create new mesh out of the new verts and tris and apply it to the chunk
        Mesh chunkMesh = new Mesh();
        chunkMesh.SetVertices(verts);

        Material[] blockMats = new Material[tris.Count];

        int submesh = 0;
        foreach (KeyValuePair<TextureGroup, List<int>> entry in tris) {
            chunkMesh.SetTriangles(entry.Value, submesh);
            chunkMesh.SetUVs(submesh, uvs[entry.Key]);
            blockMats[submesh] = entry.Key.mainMaterial;
            submesh++;
        }

        chunkMesh.RecalculateNormals();

        GameObject go = new GameObject("mesh");
        go.AddComponent<MeshFilter>().mesh = chunkMesh;
        go.AddComponent<MeshRenderer>().materials = blockMats;
        go.GetComponent<MeshRenderer>().shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;
        go.transform.position = new Vector3(-0.5f, -0.5f, -0.5f);
        GameObject go2 = new GameObject(block.name);
        go.transform.SetParent(go2.transform);
        return go2;
    }

    private void AddTrisAndUVS(int fc, TextureGroup tg) {
        t.Clear();
        t.Add(fc * 4);
        t.Add(fc * 4 + 1);
        t.Add(fc * 4 + 2);
        t.Add(fc * 4);
        t.Add(fc * 4 + 2);
        t.Add(fc * 4 + 3);

        if (tris.ContainsKey(tg)) {
            tris[tg].AddRange(t);
        } else {
            tris.Add(tg, t);
        }

        if (uvs.ContainsKey(tg)) {
            uvs[tg].AddRange(faceUV);
        } else {
            uvs.Add(tg, faceUV);
        }
    }
}
