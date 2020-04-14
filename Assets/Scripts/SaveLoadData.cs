using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using QFSW.QC;

public class SaveLoadData : MonoBehaviour
{
    public static SaveLoadData instance;

    public GameObject Player;

    public static Settings settings;

    void Awake () {
        if (instance == null) {
            instance = this;
        } else {
            Destroy(this);
        }
    }

    void Start () {
        LoadSettings();
    }

    void OnApplicationQuit () {
        SaveSettings();
    }

    // save world data
    public void SaveWorld(Dictionary<Vector3Int, Chunk> chunks) {
        string worldName = World.instance.Name;
        string worldFolder = Helper.GameFolder(@"Worlds\" + worldName + @"\");
        string chunksFolder = Helper.GameFolder(@"Worlds\" + worldName + @"\Chunks\");

        // save world info
        WorldData wd = new WorldData();
        wd.Name = "Untitled World";
        wd.Time = DayCycle.instance.Hours;

        string wdString = JsonUtility.ToJson(wd, true);
        File.WriteAllText(worldFolder + "worldinfo.json", wdString);

        // save player info
        SavePlayerData();

        // save chunks
        foreach (KeyValuePair<Vector3Int, Chunk> kvp in chunks) {
            Chunk currentChunk = kvp.Value;
            string _chunkData = "";
            for (int x = 0; x < World.ChunkSize; x++) {
                for (int y = 0; y < World.ChunkSize; y++) {
                    for (int z = 0; z < World.ChunkSize; z++) {
                        _chunkData += currentChunk.blocks[x, y, z].name + ";";
                    }
                }
            }
            File.WriteAllText(chunksFolder + $"{kvp.Key.x} {kvp.Key.y} {kvp.Key.z}.chunk", _chunkData);
        }
        Debug.Log("Saved world!");
    }

    [Command("loadworld")]
    public void LoadWorld (string WorldName) {
        string worldsFolder = Helper.GameFolder(@"Worlds\");
        if (Directory.Exists(worldsFolder + WorldName + @"\") && File.Exists(worldsFolder + WorldName + @"\worldinfo.json")) {
            string json = File.ReadAllText(worldsFolder + WorldName + @"\worldinfo.json");
            WorldData wd = JsonUtility.FromJson<WorldData>(json);

            // load world info
            World w = World.instance; // WARIO
            w.name = wd.Name;
            w.Seed = wd.Seed;
            w.WorldSpawn = wd.WorldSpawn;
            DayCycle.instance.Hours = wd.Time;

            // load player data
            LoadPlayerData();

            // load chunk data eek
            string chunksFolder = Helper.GameFolder(@"Worlds\" + WorldName + @"\Chunks\");
            string[] chunkFiles = Directory.GetFiles(chunksFolder);
            w.DeleteAllChunks();
            for (int i = 0; i < chunkFiles.Length; i++) {
                string fileName = Path.GetFileName(chunkFiles[i]).Substring(0, chunkFiles[i].Length-6);
                string[] chunkPosString = fileName.Split(' ');
                Vector3Int chunkPos = new Vector3Int(int.Parse(chunkPosString[0]), int.Parse(chunkPosString[1]), int.Parse(chunkPosString[2]));
            }
        } else {
            Debug.LogError($"World '{WorldName}' doesn't exist!");
            return;
        }
    }

    // save player data
    public void SavePlayerData () {
        string worldName = World.instance.Name;
        string worldFolder = Helper.GameFolder(@"Worlds\" + worldName + @"\");
        PlayerData pd = new PlayerData();
        pd.ImportPlayer(Player);

        string pdString = JsonUtility.ToJson(pd);
        File.WriteAllText(worldFolder + "playerdata.json", pdString);
        Debug.Log("Saved Player Data");
    }

    // load player data
    public void LoadPlayerData() {
        string worldName = World.instance.Name;
        string worldFolder = Helper.GameFolder(@"Worlds\" + worldName + @"\");
        if (File.Exists(worldFolder + "playerdata.json")) {
            string json = File.ReadAllText(worldFolder + "playerdata.json");
            PlayerData pd = JsonUtility.FromJson<PlayerData>(json);
            pd.ExportToPlayer(Player, true);
        }
    }

    // save settings to a file
    public void SaveSettings() {
        string gameFolder = Helper.GameFolder();
        string settingsJson = JsonUtility.ToJson(settings, true);
        File.WriteAllText(gameFolder + "settings.json", settingsJson);
        Debug.Log("Saved settings!");
    }

    // load settings from a file or create new settings file if one doesnt exist
    public void LoadSettings() {
        string gameFolder = Helper.GameFolder();
        if (File.Exists(gameFolder + "settings.json")) {
            // load json
            string json = File.ReadAllText(gameFolder + "settings.json");
            settings = JsonUtility.FromJson<Settings>(json);
        } else {
            // create new settings instance
            settings = new Settings();
            SaveSettings();
            Debug.Log("No settings file, created new one.");
        }
        // actually load the settings now
        Player.GetComponent<PlayerControl>().FOV = settings.fov;

        Debug.Log("Loaded Settings!");
    }
}

public class PlayerData
{
    public int Health;
    public Vector3 Position;
    public Vector2 Rotation;
    public int HotbarIndex;
    public string[] Inventory; 

    public void ImportPlayer (GameObject playerGO) {
        PlayerInfo pi = playerGO.GetComponent<PlayerInfo>();
        Health = pi.Health;
        HotbarIndex = pi.HotbarIndex;
        Inventory = new string[10];
        for (int i = 0; i < 10; i++) {
            Inventory[i] = pi.HotbarItems[i].name;
        }

        Position = playerGO.transform.position;
        Rotation = playerGO.GetComponent<PlayerControl>()._camTransform.localEulerAngles;
    }

    public void ExportToPlayer (GameObject playerGO, bool includeTransformInfo) {
        PlayerInfo pi = playerGO.GetComponent<PlayerInfo>();
        pi.Health = Health;
        pi.HotbarIndex = HotbarIndex;
        // todo: load inventory
        if (includeTransformInfo) {
            playerGO.transform.position = Position;
            playerGO.GetComponent<PlayerControl>()._camTransform.localEulerAngles = Rotation;
        }
    }
}

public class WorldData
{
    public string Name;
    public int Seed;
    public Vector3 WorldSpawn;
    public float Time;
}

public class Settings
{
    public float fov = 70;
}
