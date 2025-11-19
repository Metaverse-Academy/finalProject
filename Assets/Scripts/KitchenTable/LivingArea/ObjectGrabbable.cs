using UnityEngine;

public class ObjectGrabbable : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private Transform objectGrabPointTransform;
    private Collider objectCollider;

    private void Awake()
    {
        objectRigidbody = GetComponent<Rigidbody>();
        objectCollider = GetComponent<Collider>();
    }

    public void Grab(Transform objectGrabPointTransform)
    {
        this.objectGrabPointTransform = objectGrabPointTransform;

        // «·≈⁄œ«œ«  «·’ÕÌÕ… ··„”ﬂ
        objectRigidbody.useGravity = false;
      //  objectRigidbody.isKinematic = false; //  √ﬂœ √‰Â false

        //  ⁄ÿÌ· «· ’«œ„ „ƒﬁ «
        //if (objectCollider != null)
        //    objectCollider.enabled = false;
    }

    public void Drop()
    {
        this.objectGrabPointTransform = null;
        objectRigidbody.useGravity = true;
      //  objectRigidbody.isKinematic = true; 

    }

    private void FixedUpdate()
    {
        if (objectGrabPointTransform != null)
        {
            float lerpSpeed = 10f;
            //  Õ—Ìﬂ «·Object »”·«”…
            Vector3 targetPosition = Vector3.Lerp(transform.position, objectGrabPointTransform.position, Time.deltaTime * lerpSpeed);
            objectRigidbody.MovePosition(targetPosition);

            // ≈–« √—œ   œÊÌ— «·Object √Ì÷«
            objectRigidbody.MoveRotation(objectGrabPointTransform.rotation);
        }
    }
}