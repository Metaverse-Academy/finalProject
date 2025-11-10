using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJointManger : MonoBehaviour
{
    [Header("Spawn Points (size = 2)")]
    [SerializeField] private Transform spawnPoint;    // [0]=P1, [1]=P2

    public void OnPlayerJoined(PlayerInput input)
    {
        // int idx = input.playerIndex; // 0 �����1� 1 �����2

        // // ����� ����
        // if (spawnPoints != null && idx < spawnPoints.Length && spawnPoints[idx] != null)
        //     input.transform.SetPositionAndRotation(spawnPoints[idx].position, spawnPoints[idx].rotation);
        // else
        //     Debug.LogWarning("Spawn point missing or index out of range.");

        // if (idx == 0)
        // {
        //     // ���� �������� �������� ������ (����2)
        //     pim.playerPrefab = player2Prefab;

        //     // (�������) ���� ������ ��� �������� �����1
        //     input.SwitchCurrentControlScheme("Keyboard&Mouse", Keyboard.current, Mouse.current);
        // }
        // else if (idx == 1)
        // {
        //     // (�������) ���� �������� ���� ����� �� ������2
        //     Gamepad pad = null;
        //     foreach (var d in input.devices) if (d is Gamepad g) { pad = g; break; }
        //     if (pad != null) input.SwitchCurrentControlScheme("Gamepad", pad);
        //     else Debug.LogWarning("�� ��� ������ Gamepad ���� ������.");
        // }

        StartCoroutine(SetPlayerPos(input.transform));


    }

    private IEnumerator SetPlayerPos(Transform transformPos)
    {
        yield return new WaitForSeconds(0.25f);
        transformPos.position = spawnPoint.position;
        Debug.Log($"Move Player TO {spawnPoint.position}");
    }
}
