using UnityEngine;

public class SoundManger : MonoBehaviour
{
    

    private void PlaySound(AudioSource audioClip,Vector3 position, float voulum = 1f)
    {
        AudioSource.PlayClipAtPoint(audioClip.clip, position, voulum);
    }
}
