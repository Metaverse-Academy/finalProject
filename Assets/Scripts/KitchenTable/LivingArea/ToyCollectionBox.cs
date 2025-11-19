using UnityEngine;
using System.Collections.Generic;

public class ToyCollectionBox : MonoBehaviour
{
    [Header("Toy Collection Settings")]
    [SerializeField] private string toyTag = "Toy";
    [SerializeField] private int totalToysRequired = 7;
    [SerializeField] private GameObject collectionEffect;

    [Header("UI Reference")]
    [SerializeField] private UnityEngine.UI.Text toysCountText;

    [Header("Trigger Events")]
    [SerializeField] private GameObject triggerObject;
    [SerializeField] private bool activateOnComplete = true;

    private int collectedToysCount = 0;
    private List<GameObject> collectedToys = new List<GameObject>();
    private bool triggerActivated = false;

    public System.Action<bool> OnToyThresholdReached;
    public System.Action OnSevenToysCollected;
    public System.Action OnNotSevenToys;
    public System.Action<int> OnToyCollected;
    public System.Action OnAllToysCollected;

    private void Start()
    {
        UpdateUI();
        CheckInitialTriggerState();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(toyTag))
        {
            CollectToy(other.gameObject);
        }
    }

    private void CollectToy(GameObject toy)
    {
        if (collectedToys.Contains(toy)) return;

        collectedToysCount++;
        collectedToys.Add(toy);

        // لا نخفي اللعبة - تبقى ظاهرة دائماً
        // toy.SetActive(false);  // محذوف

        // تأثير الجمع فقط
        if (collectionEffect != null)
        {
            Instantiate(collectionEffect, toy.transform.position, Quaternion.identity);
        }

        UpdateUI();
        OnToyCollected?.Invoke(collectedToysCount);
        Debug.Log($"🎮 لعبة مجمعة! ({collectedToysCount}/{totalToysRequired}) - {toy.name}");

        CheckToyCountTrigger();

        if (collectedToysCount >= totalToysRequired)
        {
            AllToysCollected();
        }
    }

    private void CheckToyCountTrigger()
    {
        bool reachedSeven = collectedToysCount == 7;

        if (reachedSeven && !triggerActivated)
        {
            triggerActivated = true;
            ActivateTrigger(true);
            OnSevenToysCollected?.Invoke();
            OnToyThresholdReached?.Invoke(true);
            Debug.Log("✅ تم جمع 7 ألعاب - التريغر مفعل!");
        }
        else if (!reachedSeven && triggerActivated)
        {
            triggerActivated = false;
            ActivateTrigger(false);
            OnNotSevenToys?.Invoke();
            OnToyThresholdReached?.Invoke(false);
            Debug.Log("❌ العدد لم يعد 7 - التريغر معطل!");
        }
    }

    private void ActivateTrigger(bool activate)
    {
        if (triggerObject != null)
        {
            if (activateOnComplete)
            {
                triggerObject.SetActive(activate);
            }
            else
            {
                triggerObject.SetActive(!activate);
            }
        }
    }

    private void CheckInitialTriggerState()
    {
        if (collectedToysCount == 7)
        {
            triggerActivated = true;
            ActivateTrigger(true);
        }
        else
        {
            triggerActivated = false;
            ActivateTrigger(false);
        }
    }

    private void AllToysCollected()
    {
        Debug.Log("🎉 مبروك! جمعت كل الألعاب في الصندوق!");
        OnAllToysCollected?.Invoke();

        if (collectedToysCount == 7)
        {
            CheckToyCountTrigger();
        }
    }

    private void UpdateUI()
    {
        if (toysCountText != null)
        {
            toysCountText.text = $"{collectedToysCount}/{totalToysRequired}";
            toysCountText.color = (collectedToysCount == 7) ? Color.green : Color.white;
        }
    }

    // دالة لإعادة تعيين الصندوق
    public void ResetCollection()
    {
        // لا داعي لإعادة إظهار الألعاب لأنها لم تختفي أصلاً
        collectedToysCount = 0;
        collectedToys.Clear();
        triggerActivated = false;
        ActivateTrigger(false);
        UpdateUI();
    }

    // دالة لإضافة لعبة يدوياً (للتستينغ)
    public void AddToyManually()
    {
        if (collectedToysCount < totalToysRequired)
        {
            collectedToysCount++;
            UpdateUI();
            CheckToyCountTrigger();

            if (collectedToysCount >= totalToysRequired)
            {
                AllToysCollected();
            }
        }
    }

    // دالة لإزالة لعبة يدوياً (للتستينغ)
    public void RemoveToyManually()
    {
        if (collectedToysCount > 0)
        {
            collectedToysCount--;
            UpdateUI();
            CheckToyCountTrigger();
        }
    }

    // دالة للتحقق إذا العدد 7 أم لا
    public bool HasSevenToys()
    {
        return collectedToysCount == 7;
    }

    // خصائص للوصول إلى المعلومات
    public int CollectedToysCount => collectedToysCount;
    public int TotalToysRequired => totalToysRequired;
    public bool IsCompleted => collectedToysCount >= totalToysRequired;
    public bool IsTriggerActive => triggerActivated;
}