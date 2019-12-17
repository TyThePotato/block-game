using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    public TextMeshProUGUI playerPosition;
    public TextMeshProUGUI lookingAt;
    public TextMeshProUGUI biome;

    private World world;
    private Transform player;
    private BlockBreaker bb;

    private void Start() {
        world = World.instance;
        player = world.Player.transform;
        bb = world.Player.GetComponent<BlockBreaker>();
    }

    private void Update() {
        playerPosition.SetText("XYZ: " + player.position.FloorToInt());
        lookingAt.SetText($"Looking at: {bb.lookingAt.name} {bb.lookingAtPos}");
        biome.SetText($"Biome: {Chunk.GetBiome((int)player.position.x, (int)player.position.z, Vector3.zero).name}");
    }
}
