using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InvoiceItem : MonoBehaviour
{
    public TMP_Text itemName;
    public TMP_Text itemPrice;
    public TMP_Text itemQuantity;
    public Image itemIcon;

    public void Initialize(string name, float price, int quantity, Sprite icon)
    {
        itemName.text = name;
        itemPrice.text= price.ToString();
        itemQuantity .text= quantity.ToString();
        itemIcon.sprite = icon;
    }
}
