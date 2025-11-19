using System;
using UnityEngine;
using UnityEngine.UI;

public class TaskStar : MonoBehaviour
{
    [Header("Star UI")]
    [SerializeField] private Image[] starImages;       // حط هنا 3 نجوم بالترتيب
    [SerializeField] private Sprite emptyStarSprite;   // النجمة الرماديه
    [SerializeField] private Sprite filledStarSprite;  // النجمة الملوّنة

    [Header("Thresholds")]
    [SerializeField] private int plateThreshold = 2;   // عتبة الأطباق للنجمة الأولى
    [SerializeField] private int toyThreshold = 7;     // عتبة الألعاب للنجمة الثانية

    [Header("References")]
    [SerializeField] private ToyCollectionBox toyCollectionBox;

    private bool star1Given = false; // هل أعطيت النجمة الأولى للأطباق؟
    private bool star2Given = false; // هل أعطيت النجمة الثانية للألعاب؟
    private int currentPlatesCount = 0;

    private void Start()
    {
        // تأكد أن كل النجوم تبدأ فاضية
        ResetStars();

        // اشترك مع DeliveryManager
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
        }

        // اشترك مع حدث جمع الألعاب
        if (toyCollectionBox != null)
        {
            toyCollectionBox.OnToyCollected += OnToyCollected;
        }
    }

    private void OnDestroy()
    {
        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess -= OnRecipeSuccess;
        }

        if (toyCollectionBox != null)
        {
            toyCollectionBox.OnToyCollected -= OnToyCollected;
        }
    }

    private void OnRecipeSuccess(object sender, EventArgs e)
    {
        currentPlatesCount = DeliveryManager.Instance.GetSuccessfulRecipesDelivered();
        CheckStars();
    }

    private void OnToyCollected(int toysCount)
    {
        CheckStars();
    }

    private void CheckStars()
    {
        int platesCount = currentPlatesCount;
        int toysCount = toyCollectionBox != null ? toyCollectionBox.CollectedToysCount : 0;

        // التحقق من النجمة الأولى (الأطباق)
        if (!star1Given && platesCount >= plateThreshold)
        {
            GiveStar(0); // النجمة الأولى
            star1Given = true;
            Debug.Log($"⭐ النجمة الأولى - تم تسليم {platesCount} طبق!");
        }

        // التحقق من النجمة الثانية (الألعاب)
        if (!star2Given && toysCount >= toyThreshold)
        {
            GiveStar(1); // النجمة الثانية
            star2Given = true;
            Debug.Log($"⭐ النجمة الثانية - تم جمع {toysCount} لعبة!");
        }

        // إذا كانت هناك نجمة ثالثة، يمكنك إضافة شرط آخر هنا
        //if (starImages.Length > 2)
        //{
        //    // مثال: إذا اكتملت المهمتان معاً
        //    if (!IsStarFilled(2) && star1Given && star2Given)
        //    {
        //        GiveStar(2); // النجمة الثالثة (مكافأة)
        //        Debug.Log("⭐ النجمة الثالثة - مبروك إكمال المهمتين!");
        //    }
        //}
    }

    private void GiveStar(int starIndex)
    {
        if (starIndex >= 0 && starIndex < starImages.Length && starImages[starIndex] != null)
        {
            starImages[starIndex].sprite = filledStarSprite;

            // إضافة تأثيرات إضافية
            PlayStarEffect(starIndex);
        }
    }

    private bool IsStarFilled(int starIndex)
    {
        if (starIndex >= 0 && starIndex < starImages.Length && starImages[starIndex] != null)
        {
            return starImages[starIndex].sprite == filledStarSprite;
        }
        return false;
    }

    private void PlayStarEffect(int starIndex)
    {
        // يمكنك إضافة تأثيرات هنا مثل:
        // - صوت
        // - أنيميشن
        // - particles

        Debug.Log($"🎉 تم تعبئة النجمة {starIndex + 1}");
    }

    // دالة للتحقق يدوياً من النجوم
    public void CheckStarsManually()
    {
        CheckStars();
    }

    // دالة لإعادة تعيين النجوم
    public void ResetStars()
    {
        star1Given = false;
        star2Given = false;
        currentPlatesCount = 0;

        foreach (Image star in starImages)
        {
            if (star != null)
            {
                star.sprite = emptyStarSprite;
            }
        }

        Debug.Log("🔄 تم إعادة تعيين النجوم");
    }

    // خصائص للوصول إلى المعلومات
    public bool IsStar1Earned => star1Given;
    public bool IsStar2Earned => star2Given;
    public int EarnedStarsCount
    {
        get
        {
            int count = 0;
            if (star1Given) count++;
            if (star2Given) count++;
            // تحقق من النجوم الإضافية
            for (int i = 2; i < starImages.Length; i++)
            {
                if (IsStarFilled(i)) count++;
            }
            return count;
        }
    }

    // دالة للحصول على تقدم المهمة
    public void GetTaskProgress(out int plates, out int platesRequired, out int toys, out int toysRequired)
    {
        plates = currentPlatesCount;
        platesRequired = plateThreshold;
        toys = toyCollectionBox != null ? toyCollectionBox.CollectedToysCount : 0;
        toysRequired = toyThreshold;
    }
}