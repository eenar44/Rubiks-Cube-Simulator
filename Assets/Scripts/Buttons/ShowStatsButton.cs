using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowStatsButton : MonoBehaviour
{
    public void LoadGraphScene()
    {
        /* method that will load the "show stats" scene */
        // calls the SetPreviousScene from the BackButton script and passes in the current scenes name
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("ShowStatsScene"); // loads the "Show Stats" scene
    }
}
