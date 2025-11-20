using UnityEngine;

public class PlayerPickUpDrop : MonoBehaviour
{
    public static PlayerPickUpDrop Instance { get; private set; }

    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform ObjectPointTransform;
    [SerializeField] private LayerMask pickableLayerMask;

    private ObjectGrabbable ObjectGrabbable;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // »«ﬁÌ «·ﬂÊœ ﬂ„« ÂÊ...

    public void HandelInteract1()
    {
        if (ObjectGrabbable == null)
        {
            float pickupRange = 2f;
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupRange))
            {
                if (raycastHit.transform.TryGetComponent(out ObjectGrabbable))
                {
                    ObjectGrabbable.Grab(ObjectPointTransform);
                }
            }
        }
        else
        {
            ObjectGrabbable.Drop();
            ObjectGrabbable = null;
        }
    }
}