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
    [SerializeField] private LayerMask interactLayerMask = -1;

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
    private Animator anim;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerCamera = GetComponentInChildren<Camera>();
        anim = GetComponentInChildren<Animator>();

        // Debug all components
        Debug.Log($"PlayerMovement Awake - CharacterController: {characterController != null}, Camera: {playerCamera != null}, Animator: {anim != null}");
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
        if (playerCamera != null)
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0f, 0f);

        // Continuous interaction detection
        HandleContinuousInteractionDetection();

        // Manual interaction with F key
        if (Keyboard.current != null && Keyboard.current.fKey.wasPressedThisFrame)
        {
            if (GameMangarI.Instance != null && GameMangarI.Instance.CanPlayerInteract())
            {
                Debug.Log("Manual Interact Alternate Triggered with F Key");
                HandelInteractAlternate();
            }
        }

        // Animation handling
        if (anim != null)
        {
            if (m_moveAmt != Vector2.zero)
            {
                anim.SetBool("IsWalking", true);
            }
            else
            {
                anim.SetBool("IsWalking", false);
            }
        }
    }

    private void HandleContinuousInteractionDetection()
    {
        Ray centerRay = GetCenterRay();
        Debug.DrawRay(centerRay.origin, centerRay.direction * interactRange, Color.green);

        if (Physics.Raycast(centerRay, out RaycastHit hit, interactRange, interactLayerMask, QueryTriggerInteraction.Collide))
        {
            if (hit.transform.TryGetComponent(out IInteractable interactable))
            {
                SetSelectedCounter(interactable);
            }
            else
            {
                SetSelectedCounter(null);
            }
        }
        else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandelInteraction()
    {
        if (GameMangarI.Instance == null || !GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Interaction blocked by game state");
            return;
        }

        Debug.Log("HandelInteraction called");

        Ray centerRay = GetCenterRay();
        Vector3 startPoint = centerRay.origin;
        Vector3 direction = centerRay.direction;

        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, interactRange, interactLayerMask, QueryTriggerInteraction.Collide))
        {
            HandleHit(hit);
            return;
        }

        Debug.Log("No interactables found");
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
        if (GameMangarI.Instance == null || !GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Alternate interaction blocked by game state");
            return;
        }

        Debug.Log("HandelInteractAlternate called");

        Ray centerRay = GetCenterRay();
        Vector3 startPoint = centerRay.origin;
        Vector3 direction = centerRay.direction;

        Debug.DrawRay(startPoint, direction * interactRange, Color.yellow, 2f);

        if (Physics.Raycast(startPoint, direction, out RaycastHit raycastHit, interactRange, interactLayerMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log("Interact Alternate Hit: " + raycastHit.transform.name);

            if (raycastHit.transform.TryGetComponent(out IInteractable interactableCounter))
            {
                Debug.Log("Interact Alternate with: " + raycastHit.transform.name);
                interactableCounter.InteractAlternate(this);
            }
            else
            {
                Debug.Log("Hit object " + raycastHit.transform.name + " but no IInteractable component");
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
        {
            m_jumpPressed = true;
            if (anim != null)
                anim.SetTrigger("Jump");
        }
        else
            m_jumpPressed = false;
    }

    [SerializeField] private float interactCooldown = 0.15f;
    private float nextInteractTime = 0f;

    public void OnInteract(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;

        // Check for null references first
        if (GameMangarI.Instance == null)
        {
            Debug.LogError("GameMangarI.Instance is null!");
            return;
        }

        if (!GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Interact blocked - game not in play state");
            return;
        }

        if (anim == null)
        {
            Debug.LogError("Animator is null! Cannot play interact animation");
            // Continue without animation rather than crashing
        }
        else
        {
            anim.SetTrigger("Interact");
        }

        nextInteractTime = Time.time + interactCooldown;
        HandelInteraction();
    }

    public void OnInteractAlternate(InputAction.CallbackContext ctx)
    {
        if (!ctx.performed) return;
        if (Time.time < nextInteractTime) return;

        if (GameMangarI.Instance == null)
        {
            Debug.LogError("GameMangarI.Instance is null!");
            return;
        }

        if (!GameMangarI.Instance.CanPlayerInteract())
        {
            Debug.Log("Interact Alternate blocked - game not in play state");
            return;
        }

        nextInteractTime = Time.time + interactCooldown;
        HandelInteractAlternate();
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
        if (playerCamera == null)
        {
            Debug.LogError("Player camera is null!");
            return new Ray(transform.position + Vector3.up, transform.forward);
        }
        return playerCamera.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
    }

    // Emergency fix for missing animator
    private void FindAnimator()
    {
        if (anim == null)
        {
            anim = GetComponentInChildren<Animator>();
            if (anim == null)
            {
                Debug.LogError("No Animator found in PlayerMovement! Please add an Animator component.");
            }
            else
            {
                Debug.Log("Animator found and assigned: " + anim.name);
            }
        }
    }

    // Call this if you need to manually fix the animator
    [ContextMenu("Find Missing Animator")]
    public void FindMissingAnimator()
    {
        FindAnimator();
    }
}