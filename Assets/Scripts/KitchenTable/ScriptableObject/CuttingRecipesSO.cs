using UnityEngine;

[CreateAssetMenu()]
public class CuttingRecipesSO : ScriptableObject {

    public KitchenObjectSO input;
    public KitchenObjectSO output;
    public int cuttingProgressMax;

}
