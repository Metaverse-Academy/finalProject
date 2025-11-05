using UnityEngine;
using UnityEngine.InputSystem; // <-- الجديد

public class ClearCounterInteraction : IInteractable, IKitchenObjectParant
{
    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    [SerializeField] private Transform CounterTopPoint;
    [SerializeField] private ClearCounterInteraction secondClearCounter;

    [Header("Input System (اختياري للاختبار)")]
    [SerializeField] private bool testing = false;
    [SerializeField] private InputActionReference testAction; // اربط Action اسمها "Test" على المفتاح T

    private KitchenObject kitchenObject;
    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.tKey.wasPressedThisFrame)
        {
            if (kitchenObject != null && secondClearCounter != null)
                kitchenObject.SetKitchenObjectParent(secondClearCounter);
        }
    }
    private void OnEnable()
    {
        if (testing && testAction != null)
        {
            testAction.action.performed += OnTestPerformed;
            testAction.action.Enable();
        }
    }

    private void OnDisable()
    {
        if (testAction != null)
        {
            testAction.action.performed -= OnTestPerformed;
            testAction.action.Disable();
        }
    }

    private void OnTestPerformed(InputAction.CallbackContext _)
    {
        if (kitchenObject != null && secondClearCounter != null)
        {
            kitchenObject.SetKitchenObjectParent(secondClearCounter);
            Debug.Log(kitchenObject.GetClearCounter());
        }
    }


    public override void Interact(PlayerMovement interactor)
    {
        // 1) الطاولة عليها عنصر؟
        if (HasKitchenObject())
        {
            // اللاعب فاضي -> خذ من الطاولة
            if (!interactor.HasKitchenObject())
            {
                GetKitchenObject().SetKitchenObjectParent(interactor);
            }
            else
            {
                // الطرفان مشغولان -> لا شيء (لا سواپ حالياً)
                // Debug.Log("Both holding; no action.");
            }
            return;
        }

        // 2) الطاولة فاضية
        if (interactor.HasKitchenObject())
        {
            // اللاعب ماسك -> حط عنصر اللاعب على الطاولة
            interactor.GetKitchenObject().SetKitchenObjectParent(this);
            return;
        }

        // 3) الطاولة فاضية + اللاعب فاضي -> اسبن أول عنصر على الطاولة
        if (kitchenObjectSO == null || kitchenObjectSO.prefab == null || CounterTopPoint == null)
        {
            Debug.LogError($"[{name}] Setup missing (SO/prefab/CounterTopPoint).");
            return;
        }

        var t = Instantiate(kitchenObjectSO.prefab, CounterTopPoint.position, CounterTopPoint.rotation);
        if (!t.TryGetComponent(out KitchenObject ko))
        {
            Debug.LogError($"[{name}] Spawned prefab has no KitchenObject component.");
            Destroy(t.gameObject);
            return;
        }

        ko.SetKitchenObjectParent(this);   // يضبط الأب والموضع ويحدّث الحالة
    }



    //public void Interact(PlayerMovement player)
    //{
    //    //if (kitchenObject == null)
    //    //{
    //    //    Transform kitchenObjectTransform = Instantiate(kitchenObjectSO.prefab, CounterTopPoint);
    //    //    kitchenObjectTransform.GetComponent<KitchenObject>().SetKitchenObjectParent(this);

    //    //    //kitchenObjectTransform.localPosition = Vector3.zero;
    //    //    //kitchenObject = kitchenObjectTransform.GetComponent<KitchenObject>();
    //    //    //kitchenObject.SetKitchenObjectParent(this);
    //    //    //Debug.Log("inside ");
    //    //}
    //    //else
    //    //{
    //    //    //Give the object to the plsyer
    //    //    kitchenObject.SetKitchenObjectParent(player);
    //    //}



    //    //if (HasKitchenObject())
    //    //{
    //    //    Debug.Log("inside "); // بس لوق للتأكيد
    //    //    return;
    //    //}

    //    //// سباون نظيف مع حمايات
    //    //if (kitchenObjectSO == null || kitchenObjectSO.prefab == null)
    //    //{
    //    //    Debug.LogError($"[{name}] KitchenObjectSO/prefab not assigned.");
    //    //    return;
    //    //}
    //    //if (CounterTopPoint == null)
    //    //{
    //    //    Debug.LogError($"[{name}] CounterTopPoint not assigned.");
    //    //    return;
    //    //}

    //    //// أنشئ بدون تعيين أب؛ التعيين سيتم عبر SetKitchenObjectParent(this)
    //    //Transform t = Instantiate(kitchenObjectSO.prefab, CounterTopPoint.position, CounterTopPoint.rotation);

    //    //var ko = t.GetComponent<KitchenObject>();
    //    //if (ko == null)
    //    //{
    //    //    Debug.LogError($"[{name}] Spawned prefab has no KitchenObject component.");
    //    //    Destroy(t.gameObject);
    //    //    return;
    //    //}

    //    //ko.SetKitchenObjectParent(this);

    //}

    public Transform GetKitchenObjectFollowTransform() => CounterTopPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject()
    {
        return kitchenObject;
    }

    public void ClearKitchenObject()
    {
        kitchenObject = null;
    }

    public bool HasKitchenObject()
    {
        return kitchenObject != null;
    }
}
