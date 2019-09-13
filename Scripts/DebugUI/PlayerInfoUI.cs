using TMPro;
using UnityEngine;

public class PlayerInfoUI : MonoBehaviour
{
    public TextMeshProUGUI playerPosition;
    public TextMeshProUGUI lookingAt;

    private World world;
    private Transform player;
    private BlockBreaker bb;

    private void Start() {
        world = World.instance;
        player = world.Player.transform;
        bb = world.Player.GetComponent<BlockBreaker>();
    }

    private void Update() {
        //Vector3 playerLegPosition = player.position;
        //playerLegPosition.y -= 0.75f;
        playerPosition.SetText("XYZ: " + player.transform.position.FloorToInt());
        lookingAt.SetText($"Looking at: {bb.lookingAt.name} {bb.lookingAtPos}");
    }
}
