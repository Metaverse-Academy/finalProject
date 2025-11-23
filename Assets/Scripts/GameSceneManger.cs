using UnityEngine;

public class GameSceneManger : MonoBehaviour
{
    public int MainMuneSceneIndex = 0;
    public int PlayLevel = 1;
    public void LoadMainMenuScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(MainMuneSceneIndex);
    }

    public void LoadPlayLevelScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene(PlayLevel);
    }
}
