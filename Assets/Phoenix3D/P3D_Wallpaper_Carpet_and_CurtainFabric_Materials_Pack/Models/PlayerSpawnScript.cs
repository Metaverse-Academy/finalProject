using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlayerSpawnScript : MonoBehaviour
{
    public Transform SpawnPoint1, SpawnPoint2;
    public GameObject Player1, Player2;

    private GameObject player1Instance, player2Instance;

    private void Awake()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        // ����� �������� ������ ������ ���
        player1Instance = Instantiate(Player1, SpawnPoint1.position, SpawnPoint1.rotation);
        player2Instance = Instantiate(Player2, SpawnPoint2.position, SpawnPoint2.rotation);
    }

    // ���� ������ ����� ����� ��������
    public void ResetPlayersPosition()
    {
        if (player1Instance != null)
        {
            player1Instance.transform.position = SpawnPoint1.position;
            player1Instance.transform.rotation = SpawnPoint1.rotation;
            ResetPlayerVelocity(player1Instance);
        }

        if (player2Instance != null)
        {
            player2Instance.transform.position = SpawnPoint2.position;
            player2Instance.transform.rotation = SpawnPoint2.rotation;
            ResetPlayerVelocity(player2Instance);
        }

        Debug.Log("Players reset to spawn positions");
    }

    // ���� ������ ������ �����
    public void ResetCompleteGame()
    {
        // 1. ����� ����� ����� ��������
        ResetPlayersPosition();

        // 2. ����� ����� ���� ���������� ��������
        ResetAllCounters();

        // 3. ����� ����� ������ ������
        ResetScoreAndTimer();

        // 4. ����� ����� ���� ��������
        ResetPlayersState();

        // 5. ����� �� ����� ������
        ResetDeliveryManager();

        Debug.Log("Complete game reset!");
    }

    // ����� ����� ���� ����������
    private void ResetAllCounters()
    {
        // ����� ����� �������� �������
        DeliveryCounter[] deliveryCounters = FindObjectsOfType<DeliveryCounter>();
        foreach (DeliveryCounter counter in deliveryCounters)
        {
            if (counter != null)
            {
                counter.ResetCounter();
            }
        }

        // ����� ����� �������� ������
        BaseCounter[] allCounters = FindObjectsOfType<BaseCounter>();
        foreach (BaseCounter counter in allCounters)
        {
            if (counter != null && counter.HasKitchenObject())
            {
                KitchenObject ko = counter.GetKitchenObject();
                if (ko != null)
                {
                    Destroy(ko.gameObject);
                }
            }
        }

        Debug.Log("All counters reset");
    }

    // ����� ����� ������ ������
    private void ResetScoreAndTimer()
    {
        // ����� ������
        DeliveryManager deliveryManager = FindObjectOfType<DeliveryManager>();
        if (deliveryManager != null)
        {
            // ��� ��� ���� ���� ������ ������ �� DeliveryManager
            // deliveryManager.ResetScore();
        }

        // ����� �����
        Timer gameTimer = FindObjectOfType<Timer>();
        if (gameTimer != null)
        {
            // ��� ��� ���� ���� ������ �����
            // gameTimer.ResetTimer();
        }

        // ����� �� ���� ���� ���
        TaskStar scoreManager = FindObjectOfType<TaskStar>();
        if (scoreManager != null)
        {
            // scoreManager.ResetScores();
        }

        Debug.Log("Score and timer reset");
    }

    // ����� ����� ���� ��������
    private void ResetPlayersState()
    {
        // ����� ����� ���� ������ 1
        if (player1Instance != null)
        {
            ResetSinglePlayerState(player1Instance);
        }

        // ����� ����� ���� ������ 2
        if (player2Instance != null)
        {
            ResetSinglePlayerState(player2Instance);
        }

        Debug.Log("Players state reset");
    }

    // ����� ����� ���� ���� ����
    private void ResetSinglePlayerState(GameObject player)
    {
        // ����� �� ����� ������ ������
        PlayerMovement playerMovement = player.GetComponent<PlayerMovement>();
        if (playerMovement != null && playerMovement.HasKitchenObject())
        {
            KitchenObject ko = playerMovement.GetKitchenObject();
            if (ko != null)
            {
                Destroy(ko.gameObject);
            }
        }

      
    }

    // ����� ����� ���� �������
    private void ResetDeliveryManager()
    {
        DeliveryManager deliveryManager = FindObjectOfType<DeliveryManager>();
        if (deliveryManager != null)
        {
            // ��� ��� ���� ���� ������ ����� ������� ��������
            // deliveryManager.ClearDeliveredOrders();
        }

        // ����� �� ����� ������ �� ������
        PlateKitchenObject[] plates = FindObjectsOfType<PlateKitchenObject>();
        foreach (PlateKitchenObject plate in plates)
        {
            if (plate != null)
            {
                Destroy(plate.gameObject);
            }
        }

        Debug.Log("Delivery system reset");
    }

    // ���� ������ ������ ����� ������ ���������
    private void ResetPlayerVelocity(GameObject player)
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            // ����� ����� CharacterController ��� needed
        }
    }

    // ���� ������ ������� ������ �� �����
    public void ResetCompleteGameWithDelay(float delay)
    {
        StartCoroutine(ResetCompleteAfterDelay(delay));
    }

    private IEnumerator ResetCompleteAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        ResetCompleteGame();
    }

    // ���� ������ ����� ������ ���� (���� �����)
    public void HardResetGame()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene().name);
    }

    // ���� ������ ��� instances ��������
    public GameObject GetPlayer1Instance()
    {
        return player1Instance;
    }

    public GameObject GetPlayer2Instance()
    {
        return player2Instance;
    }
}