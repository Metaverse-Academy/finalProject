using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadScene : MonoBehaviour
{

    [SerializeField] private string sceneToLoad;

    public void LoadNextScene()
    {
        SceneManager.LoadScene(sceneToLoad);
    }


}
