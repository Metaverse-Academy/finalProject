using TMPro;
using UnityEngine;

public class GameOverUI : MonoBehaviour
{
    [SerializeField] private GameObject dayOverPanel;


    private void Start()
    {
        GameMangarI.Instance.OnStateChanged += GameMangarI_OnStateChanged;
        Hide();
    }
    private void Update()
    {
    }
    private void GameMangarI_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameMangarI.Instance.IsGameOver())
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
        dayOverPanel.gameObject.SetActive(true);
    }
    private void Hide()
    {
        dayOverPanel.gameObject.SetActive(false);
    }

}
