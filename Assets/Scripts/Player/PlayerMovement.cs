using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour, IKitchenObjectParant
{
    [Header("References")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private float interactRange = 2f;
    private Vector3 lastIntaractinDir;
    [SerializeField] private LayerMask counterLayerMask;
    private ClearCounterInteraction SelectedCounter;
    public event EventHandler<SelectedCounterChangedEventArgs> OnSelectedCounterChanged;
    public class SelectedCounterChangedEventArgs : EventArgs
    {
        public ClearCounterInteraction selectedCounter;
    }
    private KitchenObject kitchenObject;
    [SerializeField] private Transform holdPoint;         // ‰ﬁÿ… «·Ìœ



    [Header("Move")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float acceleration = 12f;
    [SerializeField] private float sprintSpeed = 9f;

    [Header("Jump")]
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float groundDistanceCheck = 0.3f;
    [SerializeField] private float rayStartOffset = 0.06f;
    

    [Header("Crouch")]
    [SerializeField] private bool useToggleCrouch = true;
    [SerializeField] private float crouchSpeed = 2.5f;
    [SerializeField] private float standingHeight = 2.0f;
    [SerializeField] private float crouchHeight = 1.0f;

    [SerializeField] private PlayerInput playerInput; 

    private Vector3 planarMoveDir;
    private Rigidbody rb;
    private CapsuleCollider capsule;
    private Vector2 moveInput;
    private bool isGrounded;
    private bool isSprinting;
    private bool isCrouching;
    public ClearCounterInteraction clearCounterInteraction;


    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        capsule = GetComponent<CapsuleCollider>();


        rb.interpolation = RigidbodyInterpolation.Interpolate;
        rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;

    }

    private void FixedUpdate()
    {
        HandleMovement();
    }

    private void Update()
    {
        Vector3 rayOrigin = transform.position + Vector3.up * rayStartOffset;
        isGrounded = Physics.Raycast(rayOrigin, Vector3.down, groundDistanceCheck, groundLayer, QueryTriggerInteraction.Ignore);

    }

    private void HandelInteraction()
    {
        // ⁄œÌ· ·Ê »‰” Œœ„ «·ﬂ«„Ì—«
        //Physics.Raycast(cameraTransform.position, cameraTransform.forward, out var hit, interactRange,
        //                    ~0, QueryTriggerInteraction.Collide);
        if (transform.forward != Vector3.zero)
        {
            lastIntaractinDir = transform.forward;
        }
        if (Physics.Raycast(transform.position, lastIntaractinDir, out RaycastHit raycastHit, interactRange, counterLayerMask))
        {
            if (raycastHit.transform.TryGetComponent(out ClearCounterInteraction clearCounter))
            {
                // has clear counter
                clearCounter.Interact(this); // «·¬‰ this Ìÿ»¯ﬁ IKitchenObjectParant° ›Ì„—¯ »œÊ‰ Œÿ√
                if (clearCounter != SelectedCounter) 
                {
                    SetSelectedCounter(clearCounter);
                }
            }else
            {
                 SetSelectedCounter(null);
            }
        }else
        {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement()
    {
        Vector3 f = cameraTransform.forward; f.y = 0f; f.Normalize();
        Vector3 r = cameraTransform.right; r.y = 0f; r.Normalize();

        Vector3 desiredPlanar = f * moveInput.y + r * moveInput.x;
        planarMoveDir = desiredPlanar.sqrMagnitude > 1e-4f ? desiredPlanar.normalized : Vector3.zero;

        float targetSpeed = isCrouching ? crouchSpeed : (isSprinting ? sprintSpeed : walkSpeed);
        Vector3 targetVelH = planarMoveDir * targetSpeed;

        float RotationSpeed = 10f;
        Vector3 moveDir = new Vector3(targetVelH.x, 0f, targetVelH.z);
        transform.forward = Vector3.Slerp(transform.forward, moveDir, Time.deltaTime * RotationSpeed);

        Vector3 v = rb.linearVelocity;
        Vector3 vH = Vector3.Lerp(new Vector3(v.x, 0f, v.z), targetVelH, acceleration * Time.fixedDeltaTime);
        rb.linearVelocity = new Vector3(vH.x, v.y, vH.z);
    }

    private void ApplyCrouchState()
    {
        if (capsule)
            capsule.height = isCrouching ? crouchHeight : standingHeight;

        if (isCrouching)
            isSprinting = false;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + Vector3.down * groundDistanceCheck);
    }

   
    #region Input System Callbacks

    public void OnMove(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

    }

    public void OnJump(InputAction.CallbackContext ctx)
    {
        if (ctx.performed && isGrounded)
        {
            Vector3 cur = rb.linearVelocity;
            if (cur.y < 0f) cur.y = 0f;
            rb.linearVelocity = cur;

            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
    }

    public void OnSprint(InputAction.CallbackContext ctx)
    {
        if (ctx.performed) isSprinting = true;
        else if (ctx.canceled) isSprinting = false;
    }

    public void OnCrouch(InputAction.CallbackContext ctx)
    {
        if (useToggleCrouch)
        {
            if (ctx.performed)
            {
                isCrouching = !isCrouching;
                ApplyCrouchState();
            }
        }
        else
        {
            // hold-to-crouch
            if (ctx.performed)
            {
                isCrouching = true;
                ApplyCrouchState();
            }
            else if (ctx.canceled)
            {
                isCrouching = false;
                ApplyCrouchState();
            }
        }
    }


    public void OnInteract(InputAction.CallbackContext ctx)
    {
        Debug.Log("Interact pressed");
        //if (SelectedCounter != null)
        //{
        //    SelectedCounter.Interact();
        //}
        HandelInteraction();
    }

    private void SetSelectedCounter(ClearCounterInteraction selectedCounter)
    {
        this.SelectedCounter = selectedCounter;
        OnSelectedCounterChanged?.Invoke(this, new SelectedCounterChangedEventArgs
        {
            selectedCounter = SelectedCounter
        });
    }
    private void OnDrawGizmosSelected()
    {
        if (cameraTransform == null) return;
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(cameraTransform.position, cameraTransform.position + cameraTransform.forward * interactRange);
    }

    public Transform GetKitchenObjectFollowTransform() => holdPoint;

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
    #endregion
}
