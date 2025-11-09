using UnityEngine;

public class SlectedCounterVisual : MonoBehaviour
{
    [SerializeField] private IInteractable baseCounter;
    [SerializeField] private GameObject[] visualGameObjectArray;

    private void Start()
    {
        PlayerMovement playerMovement = FindFirstObjectByType<PlayerMovement>();
        if (playerMovement != null)
        {
            playerMovement.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
        }
    }

    private void Player_OnSelectedCounterChanged(object sender, PlayerMovement.SelectedCounterChangedEventArgs e)
    {
        if (e.selectedCounter == baseCounter)
        {
            Show();
        }
        else
        {
            Hide();
        }
    }

    private void Show()
    {
        foreach(GameObject visualGameObject in visualGameObjectArray)
            visualGameObject.SetActive(true);
    }

    private void Hide()
    {
        foreach (GameObject visualGameObject in visualGameObjectArray)
            visualGameObject.SetActive(false);
    }
}
