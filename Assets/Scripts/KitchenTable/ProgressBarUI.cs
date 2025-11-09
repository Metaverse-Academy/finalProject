using UnityEngine;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    [SerializeField] private GameObject hasprogressBarGameObject;
    [SerializeField] private Image barImage;

    private IHasProgress hasProgress;


    private void Start()
    {
        hasProgress = hasprogressBarGameObject.GetComponentInParent<IHasProgress>();

        if (hasProgress != null)
        {
            hasProgress.OnProgressChange += HasProgress_OnProgressChange;
        }
        else
        {
            Debug.LogError("No IHasProgress component found in parent for ProgressBarUI");
        }
        barImage.fillAmount = 0f;
        Hide();
    }

    private void HasProgress_OnProgressChange(object sender, IHasProgress.OnProgressChangeEventArgs e)
    {
        barImage.fillAmount = e.progressNormalized;

        if (e.progressNormalized == 1f || e.progressNormalized == 0f)
        {
            Hide();
        }
        else
        {
            Show();
        }
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
    private void Hide()
    {
        gameObject.SetActive(false);
    }
}
