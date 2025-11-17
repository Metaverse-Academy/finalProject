using UnityEngine;

// Attach to your player/camera
public class PickupController : MonoBehaviour
{
    [Header("Pickup Settings")]
    public float pickupRange = 3f;
    public Transform holdPosition;
    public KeyCode pickupKey = KeyCode.E;
    public KeyCode dropKey = KeyCode.Q;
    
    [Header("References")]
    public Camera playerCamera;
    
    private Pickupable heldObject;
    private float holdDistance = 2f;
    
    void Start()
    {
        if (playerCamera == null)
        {
            playerCamera = Camera.main;
        }
        
        // Create hold position if not assigned
        if (holdPosition == null)
        {
            GameObject holdPoint = new GameObject("HoldPosition");
            holdPoint.transform.parent = playerCamera.transform;
            holdPoint.transform.localPosition = new Vector3(0, -0.3f, holdDistance);
            holdPosition = holdPoint.transform;
        }
    }
    
    void Update()
    {
        if (heldObject == null)
        {
            CheckForPickup();
        }
        else
        {
            HoldObject();
            
            if (Input.GetKeyDown(dropKey))
            {
                DropObject();
            }
        }
    }
    
    void CheckForPickup()
    {
        if (Input.GetKeyDown(pickupKey))
        {
            Ray ray = playerCamera.ScreenPointToRay(new Vector3(Screen.width / 2, Screen.height / 2));
            RaycastHit hit;
            
            if (Physics.Raycast(ray, out hit, pickupRange))
            {
                Pickupable pickupable = hit.collider.GetComponent<Pickupable>();
                if (pickupable != null)
                {
                    PickupObject(pickupable);
                }
            }
        }
    }
    
    void PickupObject(Pickupable obj)
    {
        heldObject = obj;
        heldObject.OnPickup();
        heldObject.transform.parent = holdPosition;
        heldObject.transform.localPosition = Vector3.zero;
        heldObject.transform.localRotation = Quaternion.identity;
    }
    
    void HoldObject()
    {
        heldObject.transform.position = holdPosition.position;
        heldObject.transform.rotation = holdPosition.rotation;
    }
    
    void DropObject()
    {
        heldObject.transform.parent = null;
        heldObject.OnDrop();
        heldObject = null;
    }
    
    void OnDrawGizmos()
    {
        if (playerCamera != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(playerCamera.transform.position, 
                          playerCamera.transform.forward * pickupRange);
        }
    }
}