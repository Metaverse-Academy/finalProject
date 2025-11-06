using System;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress
{
    public event EventHandler<IHasProgress.OnProgressChangeEventArgs> OnProgressChange;
    
    public event EventHandler OnCut;

    [SerializeField] private CuttingRecipesSO[] cuttingRecipesSOsArray;
    private int cuttingProgress;
    public override void Interact(PlayerMovement player)
    {
        if (!HasKitchenObject())
        {
            //there is no kitchen object on the counter
            if (player.HasKitchenObject())
            {
                //player is carrying something so put it on the counter
                player.GetKitchenObject().SetKitchenObjectParent(this);
                cuttingProgress = 0;

                CuttingRecipesSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

                OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
                {
                    progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
                });
            }
            else
            {
                //both are empty do nothing
            }
        }
        else
        {
            if (player.HasKitchenObject())
            {
                //there is a kitchen object here
            }
            else
            {
                //player is not carrying anything
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlterante(PlayerMovement player)
    {
        if (HasKitchenObject() && HasRecipeWithInput(GetKitchenObject().GetKitchenObjectSO()))
        {
            // There is KitchenObject here

            cuttingProgress++;
            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingRecipesSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(GetKitchenObject().GetKitchenObjectSO());

            OnProgressChange?.Invoke(this, new IHasProgress.OnProgressChangeEventArgs
            {
                progressNormalized = (float)cuttingProgress / cuttingRecipeSO.cuttingProgressMax
            });

            if (cuttingProgress >= cuttingRecipeSO.cuttingProgressMax)
            {
                // Cut the object

                KitchenObjectSO inputKitchenObjectSO = GetKitchenObject().GetKitchenObjectSO();
                KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(inputKitchenObjectSO);

                if (outputKitchenObjectSO != null)
                {
                    // Destroy the current object
                    GetKitchenObject().DestroySelf();
                    ClearKitchenObject();

                    // Spawn the new output object
                    KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
                }
            }
            else
            {
                Debug.LogWarning("No recipe found for this ingredient!");
            }
        }
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipesSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        return cuttingRecipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO)
    {
        CuttingRecipesSO cuttingRecipeSO = GetCuttingRecipeSOWithInput(inputKitchenObjectSO);
        if (cuttingRecipeSO != null) {
            return cuttingRecipeSO.output;
        }else{
            return null;
        }
        
    }

    private CuttingRecipesSO GetCuttingRecipeSOWithInput(KitchenObjectSO inputKitchenObjectSO)
    {
        foreach (CuttingRecipesSO cuttingRecipesSO in cuttingRecipesSOsArray)
        {
            if (cuttingRecipesSO.input == inputKitchenObjectSO)
            {
                return cuttingRecipesSO;
            }
        }
        return null;
    }



}
