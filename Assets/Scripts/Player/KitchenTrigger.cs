using UnityEngine;

public class KitchenTrigger : MonoBehaviour
{
    [Header("UI Settings")]
    public GameObject kitchenUI;
    
    [Header("Objects to Hide")]
    public GameObject[] objectsToHide;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (kitchenUI != null)
            {
                kitchenUI.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (kitchenUI != null)
            {
                kitchenUI.SetActive(false);
            }

            // إخفاء الكائنات
            foreach (GameObject obj in objectsToHide)
            {
                if (obj != null)
                {
                    obj.SetActive(false);
                }
            }
        }
    }
}