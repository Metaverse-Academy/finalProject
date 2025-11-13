// ========== 1. كلاس المنتج (ScriptableObject) ==========
using UnityEngine;

[CreateAssetMenu(fileName = "Product", menuName = "Shopping/Product")]
public class Product : MonoBehaviour
{
    public string productName;
    public float price;
    // public Sprite icon;
}
