using System;
using UnityEngine;
using UnityEngine.UI;

public class TaskStar : MonoBehaviour
{
    [Header("Star UI")]
    [SerializeField] private Image[] starImages;       // Õÿ Â‰« 3 ‰ÃÊ„ »«· — Ì»
    [SerializeField] private Sprite emptyStarSprite;   // «·‰Ã„… «·—„«œÌÂ
    [SerializeField] private Sprite filledStarSprite;  // «·‰Ã„… «·„·Ê¯‰…

    [Header("Thresholds")]
    // »⁄œ ﬂ„ ÿ»ﬁ  ‘ €· ﬂ· ‰Ã„…ø („À«·: 2 À„ 4 À„ 6)
    [SerializeField] private int[] starThresholds = { 2, 4, 6 };

    private void Start()
    {
        //  √ﬂœ √‰ ﬂ· «·‰ÃÊ„  »œ√ ›«÷Ì…
        UpdateStars(0);

        // «‘ —ﬂ „⁄ DeliveryManager
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
        }
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess -= OnRecipeSuccess;
        }
    }

    private void OnRecipeSuccess(object sender, EventArgs e)
    {
        int platesCount = DeliveryManager.Instance.GetSuccessfulRecipesDelivered();
        UpdateStars(platesCount);
    }

    private void UpdateStars(int platesCount)
    {
        for (int i = 0; i < starImages.Length; i++)
        {
            if (starImages[i] == null) continue;

            bool shouldBeFilled = (i < starThresholds.Length) && (platesCount >= starThresholds[i]);
            starImages[i].sprite = shouldBeFilled ? filledStarSprite : emptyStarSprite;
        }
    }
}
