using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour, IKitchenObjectParant
{
    private Vector2 m_moveAmt;
    private Vector2 m_lookAmt;
    private bool m_jumpPressed;

    [Header("الإعدادات")]
    public float moveSpeed = 5f;
    public float jumpForce = 8f;
    public float lookSensitivity = 2f;

    private CharacterController characterController;
    private Camera playerCamera;
    private float rotationX = 0f;
    private float verticalVelocity = 0f;

    [SerializeField] private float interactRange = 4f;
    private Vector3 lastIntaractinDir;
    [SerializeField] private LayerMask interactLayerMask;

    public static event EventHandler OnPickupSomething;
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public IInteractable selectedCounter;
    }

    private KitchenObject kitchenObject;
    [SerializeField] private Transform holdPoint;
    private IInteractable selectedCounter;
    private Vector2 moveInput;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
    }

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        // الحركة
        Vector3 move = transform.right * m_moveAmt.x + transform.forward * m_moveAmt.y;

        // الجاذبية والقفز
        if (characterController.isGrounded)
        {
            verticalVelocity = -2f;
            if (m_jumpPressed)
            {
                verticalVelocity = jumpForce;
                m_jumpPressed = false;
            }
        }
        else
        {
            verticalVelocity += -20f * Time.deltaTime;
        }

        move.y = verticalVelocity;
        characterController.Move(move * moveSpeed * Time.deltaTime);

        // النظر
        transform.Rotate(Vector3.up * m_lookAmt.x * lookSensitivity);
        rotationX -= m_lookAmt.y * lookSensitivity;
        rotationX = Mathf.Clamp(rotationX, -90f, 90f);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        Ray centerRay = GetCenterRay();
        Debug.DrawRay(centerRay.origin, centerRay.direction * interactRange, Color.green);

        // اختيار يدوي بالضغط على F
        if (Keyboard.current.fKey.wasPressedThisFrame)
        {
            Debug.Log("Manual Interact Alternate Triggered with F Key");
            HandelInteractAlternate();
        }
    }

    private void HandelInteraction()
    {
        Debug.Log("HandelInteraction called");

        Ray centerRay = GetCenterRay();
        Vector3 startPoint = centerRay.origin;
        Vector3 direction = centerRay.direction;

        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, interactRange, interactLayerMask, QueryTriggerInteraction.Ignore))
        {
            HandleHit(hit);
            return;
        }

        Debug.Log("No interactables found with any method");
        SetSelectedCounter(null);
    }

    private void HandleHit(RaycastHit hit)
    {
        Debug.Log("HIT: " + hit.transform.name + " at distance " + hit.distance);

        if (hit.transform.TryGetComponent(out IInteractable interactableCounter))
        {
            Debug.Log("Found interactable: " + interactableCounter);
            SetSelectedCounter(interactableCounter);
            interactableCounter.Interact(this);
        }
        else
        {
            Debug.Log("No IInteractable on " + hit.transform.name);
            SetSelectedCounter(null);
        }
    }

    private void HandelInteractAlternate()
    {
        Debug.Log("HandelInteractAlternate called");

        Ray centerRay = GetCenterRay();
        Vector3 startPoint = centerRay.origin;
        Vector3 direction = centerRay.direction;

        Debug.DrawRay(startPoint, direction * interactRange, Color.yellow, 2f);
        Debug.Log("Interact Alternate raycast from center: " + startPoint + ", dir: " + direction);

        if (Physics.Raycast(startPoint, direction, out RaycastHit raycastHit, interactRange, interactLayerMask, QueryTriggerInteraction.Ignore))
        {
            Debug.Log("Interact Alternate Hit: " + raycastHit.transform.name + " at distance: " + raycastHit.distance);

            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                Debug.Log("Interact Alternate with: " + raycastHit.transform.name);
                interactableCounter.InteractAlternate(this);
            }
            else
            {
                Debug.Log("Hit object " + raycastHit.transform.name + " but no IInteractable component for alternate");
            }
        }
        else
        {
            Debug.Log("No raycast hit in Interact Alternate");
        }
    }

    #region Input System Callbacks

    public void OnMove(InputAction.CallbackContext value)
    {
        m_moveAmt = value.ReadValue<Vector2>();
    }

    public void OnLook2(InputAction.CallbackContext value)
    {
        m_lookAmt = value.ReadValue<Vector2>();
    }

    public void OnJump(InputAction.CallbackContext value)
    {
        if (value.performed)
            m_jumpPressed = true;
        else
            m_jumpPressed = false;
    }

    [SerializeField] private float interactCooldown = 0.15f;
    private float nextInteractTime = 0f;

    public static object Instance { get; internal set; }

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;
        nextInteractTime = Time.time + interactCooldown;

        HandelInteraction();
    }

    public void OnInteractAlternate(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interact Alternate Input Received: " + ctx.phase);

        if (ctx.performed)
        {
            if (Time.time < nextInteractTime) return;
            nextInteractTime = Time.time + interactCooldown;

            Debug.Log("Handling Interact Alternate");
            HandelInteractAlternate();
        }
    }

    #endregion

    private void SetSelectedCounter(IInteractable selectedCounter)
    {
        this.selectedCounter = selectedCounter;

        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs
        {
            selectedCounter = this.selectedCounter
        });
    }

    public Transform GetKitchenObjectFollowTransform() => holdPoint;

    public void SetKitchenObject(KitchenObject kitchenObject)
    {
        this.kitchenObject = kitchenObject;

        if (kitchenObject != null)
        {
            OnPickupSomething?.Invoke(this, EventArgs.Empty);
        }
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

    public void SwitchOnCamera()
    {
        if (playerCamera != null)
            playerCamera.enabled = true;
    }

    private Ray GetCenterRay()
    {
        return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    private void DebugDrawSphere(Vector3 center, float radius, Color color, float duration = 0.05f)
    {
        int segments = 16;
        float step = 2f * Mathf.PI / segments;

        for (int axis = 0; axis < 3; axis++)
        {
            Vector3 prev = Vector3.zero;
            for (int i = 0; i <= segments; i++)
            {
                float a = i * step;
                Vector3 p = axis == 0
                    ? center + new Vector3(Mathf.Cos(a) * radius, Mathf.Sin(a) * radius, 0f)
                    : axis == 1
                        ? center + new Vector3(Mathf.Cos(a) * radius, 0f, Mathf.Sin(a) * radius)
                        : center + new Vector3(0f, Mathf.Cos(a) * radius, Mathf.Sin(a) * radius);

                if (i > 0) Debug.DrawLine(prev, p, color, duration);
                prev = p;
            }
        }
    }
}