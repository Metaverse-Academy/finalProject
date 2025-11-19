using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEditor.Experimental.GraphView.GraphView;

public class PlayerJointManger : MonoBehaviour
{
    private List<PlayerInput> players = new List<PlayerInput>(); // تم تعريف players هنا
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
    public void ResetGame()
    {
        ResetAllPlayersPosition();
        ResetGameState();
    }

    // إعادة جميع اللاعبين إلى نقطة البداية
    private void ResetAllPlayersPosition()
    {
        foreach (PlayerInput player in players)
        {
            if (player != null && player.transform != null)
            {
                // إعادة الموضع
                player.transform.position = spawnPoint.position;

                // إعادة الدوران إذا needed
                player.transform.rotation = Quaternion.identity;

                // إعادة تعيين السرعة إذا كان لديك Rigidbody
                Rigidbody rb = player.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.linearVelocity = Vector3.zero;
                    rb.angularVelocity = Vector3.zero;
                }
            }
        }

        Debug.Log($"All players reset to spawn point: {spawnPoint.position}");
    }

    // إعادة حالة اللعبة
    private void ResetGameState()
    {
        // إعادة النقاط
        ResetScore();

        // إعادة الوقت إذا كان لديك timer
        ResetTimer();

        // إعادة أي عناصر أخرى في اللعبة
        ResetGameObjects();

        Debug.Log("Game state reset to initial state");
    }

    private void ResetScore()
    {
        // هنا يمكنك إعادة النقاط
        // مثال: ScoreManager.ResetScores();
    }

    private void ResetTimer()
    {
        // هنا يمكنك إعادة المؤقت
        // مثال: GameTimer.ResetTimer();
    }

    private void ResetGameObjects()
    {
        // إعادة تعيين أي كائنات في المشهد
        DeliveryCounter[] counters = FindObjectsOfType<DeliveryCounter>();
        foreach (DeliveryCounter counter in counters)
        {
            counter.ResetCounter();
        }

        // يمكنك إضافة المزيد من الكائنات التي تحتاج إعادة تعيين
    }

    // إذا كنت تريد إعادة تعيين اللعبة بعد وقت معين
    public void ResetGameWithDelay(float delay)
    {
        StartCoroutine(ResetGameCoroutine(delay));
    }

    private IEnumerator ResetGameCoroutine(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetGame();
    }
}
