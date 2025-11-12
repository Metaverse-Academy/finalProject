using UnityEngine;
using UnityEngine.UI;

public class plateIconSingleUI : MonoBehaviour
{
    [SerializeField] private Image image; 

    public void SetKitchenObjectSO(KitchenObjectSO kitchenObjectSO)
    {
        image.sprite = kitchenObjectSO.sprite;
    }


}
