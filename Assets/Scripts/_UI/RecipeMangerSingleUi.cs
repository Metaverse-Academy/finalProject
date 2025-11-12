using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class RecipeMangerSingleUi : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI recipeName;
    [SerializeField] private Transform iconeContainer;
    [SerializeField] private Transform iconTeplate;


    private void Awake()
    {
        iconTeplate.gameObject.SetActive(false);
    }
    public void SetRecipeSO(RecipeSO recipeSO)
    {
        recipeName.text = recipeSO.recipeName;

        foreach (Transform child in iconeContainer)
        {
            if (child == iconTeplate) continue;
            Destroy(child.gameObject);
        }

        foreach (KitchenObjectSO kitchenObjectSO in recipeSO.kitchenObjectSOList)
        {
            Transform iconTransform = Instantiate(iconTeplate, iconeContainer);
            iconTransform.gameObject.SetActive(true);
            iconTransform.GetComponent<Image>().sprite = kitchenObjectSO.sprite;
        }
    }


}
