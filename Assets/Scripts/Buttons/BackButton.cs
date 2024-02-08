using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    // Static variable to store the name of the previous scene
    private static string previousSceneName;

    // Method to load the previous scene
    public void LoadPreviousScene()
    {
        // Check if the previous scene name is not null or empty
        if (!string.IsNullOrEmpty(previousSceneName))
        {
            SceneManager.LoadScene(previousSceneName);
        }
        else
        {
            Debug.LogWarning("Previous scene name is null or empty.");
            // Load a default scene if there is no previous scene recorded
            SceneManager.LoadScene("MainMenuScene");
        }
    }

    // Method to set the previous scene name
    public static void SetPreviousScene(string sceneName)
    {
        previousSceneName = sceneName;
    }
}
