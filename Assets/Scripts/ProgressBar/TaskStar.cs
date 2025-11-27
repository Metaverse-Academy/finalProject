using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskStar : MonoBehaviour
{
    [Header("Star UI")]
    [SerializeField] private Image[] starImages;
    [SerializeField] private Sprite emptyStarSprite;
    [SerializeField] private Sprite filledStarSprite;

    [Header("Thresholds")]
    [SerializeField] private int plateThreshold = 2;
    [SerializeField] private int toyThreshold = 7;

    [Header("References")]
    [SerializeField] private ToyCollectionBox toyCollectionBox;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip starEarnedSound;

    public GameObject WinPanel;

    private bool star1Given = false;
    private bool star2Given = false;
    private bool star3Given = false;
    private int currentPlatesCount = 0;
    private ShopSystem shopSystem;

    private void Start()
    {
        ResetStars();
        shopSystem = FindObjectOfType<ShopSystem>();

        if (DeliveryManager.Instance != null)
        {
            DeliveryManager.Instance.OnRecipeSuccess += OnRecipeSuccess;
        }

        if (toyCollectionBox != null)
        {
            toyCollectionBox.OnToyCollected += OnToyCollected;
        }

        // افحص النجوم كل ثانية
        InvokeRepeating("CheckStarsRepeatedly", 1f, 1f);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            CheckStarsManually();
        }
        if (star3Given == true && star2Given == true && star1Given == true)
        {
            WinPanel.SetActive(true);
            Debug.Log("جميع النجوم مكتسبة، إلغاء التحقق المتكرر.");
        }
    }

    private void CheckStarsRepeatedly()
    {
        CheckStars();
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

        if (!star1Given && platesCount >= plateThreshold)
        {
            GiveStar(0);
            PlayStarSound();
            star1Given = true;
            Debug.Log($"⭐ النجمة الأولى - تم تسليم {platesCount} طبق!");
        }

        if (!star2Given && toysCount >= toyThreshold)
        {
            GiveStar(1);
            PlayStarSound();
            star2Given = true;
            Debug.Log($"⭐ النجمة الثانية - تم جمع {toysCount} لعبة!");
        }

        if (!star3Given && CheckForThirdStar())
        {
            GiveStar(2);
            PlayStarSound();
            star3Given = true;
            Debug.Log($"⭐ النجمة الثالثة - تم جمع جميع المكونات المطلوبة!");
        }
    }

    private bool CheckForThirdStar()
    {
        if (shopSystem == null)
        {
            shopSystem = FindObjectOfType<ShopSystem>();
            if (shopSystem == null) return false;
        }

        // جمع جميع العناصر من كلا اللاعبين
        var allItems = new List<PurchaseItem>();
        allItems.AddRange(shopSystem.GetPlayerItems(1));
        allItems.AddRange(shopSystem.GetPlayerItems(2));

        bool hasMilk = false, hasApple = false, hasAvocado = false, hasOil = false;

        Debug.Log($"🔍 عدد العناصر في الفواتير: {allItems.Count}");

        foreach (var item in allItems)
        {
            if (item.itemName == null) continue;

            string name = item.itemName.ToLower();
            Debug.Log($"🔍 فحص: {item.itemName}");

            if (name.Contains("milk"))
            {
                hasMilk = true;
                Debug.Log($"✅ وجد الحليب: {item.itemName}");
            }
            if (name.Contains("apple"))
            {
                hasApple = true;
                Debug.Log($"✅ وجد التفاح: {item.itemName}");
            }
            if (name.Contains("avocado"))
            {
                hasAvocado = true;
                Debug.Log($"✅ وجد الأفوكادو: {item.itemName}");
            }
            if (name.Contains("oil"))
            {
                hasOil = true;
                Debug.Log($"✅ وجد الزيت: {item.itemName}");
            }
        }

        Debug.Log($"🎯 النتيجة: Milk={hasMilk}, Apple={hasApple}, Avocado={hasAvocado}, Oil={hasOil}");
        Debug.Log($"🎯 النجمة الثالثة ممكن: {hasMilk && hasApple && hasAvocado && hasOil}");

        return hasMilk && hasApple && hasAvocado && hasOil;
    }

    private void GiveStar(int starIndex)
    {
        if (starIndex >= 0 && starIndex < starImages.Length && starImages[starIndex] != null)
        {
            starImages[starIndex].sprite = filledStarSprite;
            PlayStarEffect(starIndex);
        }
    }

    private void PlayStarSound()
    {
        if (audioSource != null && starEarnedSound != null)
        {
            audioSource.PlayOneShot(starEarnedSound);
        }
    }

    private void PlayStarEffect(int starIndex)
    {
        Debug.Log($"🎉 تم تعبئة النجمة {starIndex + 1}");
    }

    public void CheckStarsManually()
    {
        Debug.Log("=== فحص يدوي للنجوم ===");
        CheckStars();
    }

    public void ResetStars()
    {
        star1Given = false;
        star2Given = false;
        star3Given = false;
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

    public bool IsStar1Earned => star1Given;
    public bool IsStar2Earned => star2Given;
    public bool IsStar3Earned => star3Given;

    public int EarnedStarsCount
    {
        get
        {
            int count = 0;
            if (star1Given) count++;
            if (star2Given) count++;
            if (star3Given) count++;
            return count;
        }
    }

    public void GetTaskProgress(out int plates, out int platesRequired, out int toys, out int toysRequired)
    {
        plates = currentPlatesCount;
        platesRequired = plateThreshold;
        toys = toyCollectionBox != null ? toyCollectionBox.CollectedToysCount : 0;
        toysRequired = toyThreshold;
    }
}