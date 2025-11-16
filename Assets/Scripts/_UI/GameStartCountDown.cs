using TMPro;
using UnityEditor.Search;
using UnityEngine;

public class GameStartCountDown : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI countdownText;


    private void Start()
    {
        GameMangarI.Instance.OnStateChanged += GameMangarI_OnStateChanged;
        Hide();
    }
    private void Update()
    {
        countdownText.text = Mathf.CeilToInt(GameMangarI.Instance.GetCountdownToStartTimer()).ToString();
    }
    private void GameMangarI_OnStateChanged(object sender, System.EventArgs e)
    {
        if (GameMangarI.Instance.IsCountdownToStartActive())
        {
            Show();
        }else
        {
             Hide();
        }

    }

    private void Show()
    {
        countdownText.gameObject.SetActive(true);
    }
    private void Hide()
    {
        countdownText.gameObject.SetActive(false);
    }

}
