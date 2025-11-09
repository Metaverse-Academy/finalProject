using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJointManger : MonoBehaviour
{

    [Header("Prefabs")]
    [SerializeField] private GameObject player1Prefab;   // ááßíÈæÑÏ
    [SerializeField] private GameObject player2Prefab;   // ááÌíãÈÇÏ

    [Header("Spawn Points (size = 2)")]
    [SerializeField] private Transform[] spawnPoints;    // [0]=P1, [1]=P2

    private PlayerInputManager pim;

    private void Awake()
    {

        pim = PlayerInputManager.instance ?? FindObjectOfType<PlayerInputManager>();

        //pim = PlayerInputManager.instance ?? FindFirstObjectByType<PlayerInputManager>();
        if (pim == null)
        {
            Debug.LogError("PlayerInputManager ÛíÑ ãæÌæÏ İí ÇáãÔåÏ.");
            enabled = false;
            return;
        }
    }

    private void OnEnable()
    {
        pim.onPlayerJoined += OnPlayerJoined;
    }

    private void OnDisable()
    {
        if (pim != null) pim.onPlayerJoined -= OnPlayerJoined;
    }

    private void Start()
    {
        // Ãæá áÇÚÈ ÏÇíãğÇ ãä ÈÑíİÇÈ ÇááÇÚÈ1
        pim.playerPrefab = player1Prefab;
    }

    private void OnPlayerJoined(PlayerInput input)
    {
        int idx = input.playerIndex; // 0 ááÇÚÈ1¡ 1 ááÇÚÈ2

        // ÓÈÇæä ãÑÊÈ
        if (spawnPoints != null && idx < spawnPoints.Length && spawnPoints[idx] != null)
            input.transform.SetPositionAndRotation(spawnPoints[idx].position, spawnPoints[idx].rotation);
        else
            Debug.LogWarning("Spawn point missing or index out of range.");

        if (idx == 0)
        {
            // ÍÖøÑ ÇáÈÑíİÇÈ ááÇäÖãÇã ÇáŞÇÏã (áÇÚÈ2)
            pim.playerPrefab = player2Prefab;

            // (ÇÎÊíÇÑí) ËÈøÊ ÇáÓßíã Úáì ÇáßíÈæÑÏ ááÇÚÈ1
            input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        }
        else if (idx == 1)
        {
            // (ÇÎÊíÇÑí) ËÈøÊ ÇáÌíãÈÇÏ ÇáĞí ÇäÖãø Èå ÇááÇÚÈ2
            Gamepad pad = null;
            foreach (var d in input.devices) if (d is Gamepad g) { pad = g; break; }
            if (pad != null) input.SwitchCurrentControlScheme("Gamepad", pad);
            else Debug.LogWarning("áã íÊã ÇáÊŞÇØ Gamepad áåĞÇ ÇááÇÚÈ.");
        }
    }
}
