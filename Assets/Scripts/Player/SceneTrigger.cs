using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneTrigger : MonoBehaviour
{
    [Header("إعدادات Scene")]
    public string sceneName = "NewScene"; // اسم الـ Scene الجديد
    public float sceneDuration = 15f; // المدة بالثواني
    
    private string previousScene;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            OpenNewScene();
        }
    }
    
    void OpenNewScene()
    {
        previousScene = SceneManager.GetActiveScene().name;
        
        Debug.Log("🚪 فتح Scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
        
        // ارجع بعد 15 ثانية
        Invoke("ReturnToPreviousScene", sceneDuration);
    }
    
    void ReturnToPreviousScene()
    {
        Debug.Log("🔙 العودة إلى: " + previousScene);
        SceneManager.LoadScene(previousScene);
    }
}