using UnityEngine;
using UnityEngine.SceneManagement;

public class BackButton : MonoBehaviour
{
    private static string previousSceneName; // variable to store the name of the previous scene

    public void LoadPreviousScene()
    {
        /* method to load the previous scene */
        if (!string.IsNullOrEmpty(previousSceneName)) // checks if the previous scene name is not null or empty
        {
            SceneManager.LoadScene(previousSceneName); // if not then load previous scene
        }
        else
        {
            Debug.LogWarning("Previous scene name is null or empty."); // log error if there is not a previous scene
            SceneManager.LoadScene("MainMenuScene"); // load default main scene if there is no previous scene recorded
        }
    }

    public static void SetPreviousScene(string sceneName)
    {
        /* Method to set the previous scene name */
        previousSceneName = sceneName;
    }
}
