using UnityEngine;


public class PlayerPickUpDrop : MonoBehaviour
{
    [SerializeField] private Transform playerCameraTransform;
    [SerializeField] private Transform ObjectPointTransform;
    [SerializeField] private LayerMask pickableLayerMask;

    private ObjectGrabbable ObjectGrabbable;
    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (ObjectGrabbable == null)
            {
            float pickupRange = 2f;
            if (Physics.Raycast(playerCameraTransform.position, playerCameraTransform.forward, out RaycastHit raycastHit, pickupRange))
            {
                if(raycastHit.transform.TryGetComponent(out ObjectGrabbable)){
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
}
