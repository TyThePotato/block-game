﻿using System;
using System.Collections.Generic;
using UnityEngine;
using QFSW.QC;

public class BiomeList : MonoBehaviour
{
    public static BiomeList instance;

    public TextAsset BiomeListText;
    public Dictionary<string, Biome> biomes = new Dictionary<string, Biome>();
    public Biome[] biomesArray;

    private void Awake() {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
        RegisterBiomes(BiomeListText.text);
    }

    private void RegisterBiomes(string biomeJSON) {
        _BiL bil = JsonUtility.FromJson<_BiL>(biomeJSON);
        for (int i = 0; i < bil.biomes.Length; i++) {
            biomes.Add(bil.biomes[i].name, bil.biomes[i]);
            bil.biomes[i].FillBlankValues();
        }
        biomesArray = new Biome[biomes.Count];
        biomes.Values.CopyTo(biomesArray, 0);
        Debug.Log($"Registered {biomes.Count} Biomes");
    }

    public Biome[] GetBiomeArray () {
        Biome[] b = new Biome[biomes.Count];
        biomes.Values.CopyTo(b, 0);
        return b;
    }

    [Command("listloadedbiomes")]
    void ListLoadedBlocks() {
        string l = "";
        foreach (string key in biomes.Keys) {
            l += key + Environment.NewLine;
        }
        QuantumConsole.print(l);
    }
}

[Serializable]
public class _BiL
{
    public Biome[] biomes;
}
