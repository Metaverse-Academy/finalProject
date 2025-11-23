using UnityEngine;

public class CrediteUi : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private Transform creditsBox;
    void OnEnable()
    {
        canvasGroup.alpha = 0;
        canvasGroup.LeanAlpha(1,0.5f);

        creditsBox.position = new Vector2(0, -Screen.height);
        creditsBox.LeanMoveLocal(new Vector3(0, 0, 0),0.5f).setEaseOutExpo().delay = 0.1f;
    }

    public void CloseBox()
    {
        canvasGroup.LeanAlpha(0, 0.5f);
        creditsBox.LeanMoveLocalY(-Screen.height, 0.5f).setEaseInExpo();
    }

}
