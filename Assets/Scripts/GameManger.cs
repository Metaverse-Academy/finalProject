using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManger : MonoBehaviour
{

    public AudioSource audioSource;
    public AudioClip ButtonClick;
    // public void GoFirstScene()
    // {
    //     SceneManager.LoadScene(SceneData.Scene1);
    //     audioSource.PlayOneShot(ButtonClick);

    // }
    // public void GotoGameVideo()
    // {
    //     SceneManager.LoadScene(SceneData.Scene2);
    //     audioSource.PlayOneShot(ButtonClick);

    // }

    public void GotoGamePlay()
    {
        SceneManager.LoadScene(SceneData.Scene1);
        audioSource.PlayOneShot(ButtonClick);

    }

    public void GotoMainMune() 
    {
        SceneManager.LoadScene(SceneData.MainMenu);
        audioSource.PlayOneShot(ButtonClick);

    }
} 