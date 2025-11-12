using UnityEngine;

public class SoundManger : MonoBehaviour
{
    [SerializeField] private AudioClipRefsSO audioClipRefsSO;

    private void Start()
    {
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeFailed;
        DeliveryManager.Instance.OnRecipeCompleted += DeliveryManager_OnRecipeFailed;
        //CuttingCounter.OnAnyCut += CuttingCounter_OnAnyCut;
        PlayerMovement.OnPickupSomething += PlayerMovement_OnPlayerPickupSomething;
        BaseCounter.OnAnyObjectPlacedHere += BaseCounter_OnAnyObjectPlacedHere;
        TrashCounter.OnAnyObjectTrashed += TrashCounter_OnAnyObjectTrashed;
    }
    private void TrashCounter_OnAnyObjectTrashed(object sender, System.EventArgs e)
    {
        TrashCounter trashCounter = sender as TrashCounter;
        PlaySound(audioClipRefsSO.trash, trashCounter.transform.position);
    }
    private void BaseCounter_OnAnyObjectPlacedHere(object sender, System.EventArgs e)
    {
        BaseCounter baseCounter = sender as BaseCounter;
        PlaySound(audioClipRefsSO.ObjectDrop, baseCounter.transform.position);
    }
    private void PlayerMovement_OnPlayerPickupSomething(object sender, System.EventArgs e)
    {
        //  ÕÊÌ· sender ≈·Ï PlayerMovement ··Ê’Ê· ≈·Ï transform
        PlayerMovement player = sender as PlayerMovement;
        if (player != null)
        {
            PlaySound(audioClipRefsSO.ObjectPickup, player.transform.position);
        }
        else
        {
            // ≈–« ›‘· «· ÕÊÌ·° «” Œœ„ „Êﬁ⁄ «·ﬂ«„Ì—« ﬂ»œÌ·
            PlaySound(audioClipRefsSO.ObjectPickup, Camera.main.transform.position);
        }
    }
    private void CuttingCounter_OnAnyCut(object sender, System.EventArgs e)
    {
        CuttingCounter cuttingCounter = sender as CuttingCounter;
        PlaySound(audioClipRefsSO.chope, cuttingCounter.transform.position);
    }
    private void DeliveryManager_OnRecipeFailed(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.delivartfail, deliveryCounter.transform.position);
    }
    private void DeliveryManager_OnRecipeCompleted(object sender, System.EventArgs e)
    {
        DeliveryCounter deliveryCounter = DeliveryCounter.Instance;
        PlaySound(audioClipRefsSO.delivartsuccess, deliveryCounter.transform.position);
    }

    private void PlaySound(AudioClip[] audioClipArray,Vector3 position, float voulum = 1f)
    {
        PlaySound(audioClipArray[Random.Range(0, audioClipArray.Length)], position, voulum);
    }
    private void PlaySound(AudioClip audioClip,Vector3 position, float voulum = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip, position, voulum);
    }
}
