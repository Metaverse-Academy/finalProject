
using UnityEngine;
public class Door : MonoBehaviour
{
    public Transform insideTarget;
    public Transform outsideTarget;
    public bool playerIsInside = false;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (playerIsInside)
            {
                // Move outside
                other.transform.position = outsideTarget.position;
                playerIsInside = false;
            }
            else
            {
                // Move inside
                other.transform.position = insideTarget.position;
                playerIsInside = true;
            }
        }
    }
}
