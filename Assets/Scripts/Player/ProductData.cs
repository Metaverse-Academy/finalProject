using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "ProductData1", menuName = "Scriptable Objects/ProductData1")]
public class ProductData : ScriptableObject
{
    [Header("معلومات المنتج")]
    public string itemName = "منتج";
    public float price = 10f;
    public Sprite icon;
    
    [Header("معلومات إضافية (اختياري)")]
    [TextArea(2, 4)]
    public string description = "وصف المنتج";
    public string barcode = "";
    
    [Header("الإعدادات")]
    public Vector3 holdOffset = Vector3.zero; // تعديل موقع الحمل
    public Vector3 holdRotation = Vector3.zero; // تعديل دوران الحمل
}