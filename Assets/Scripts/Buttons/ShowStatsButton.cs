using UnityEngine;
using UnityEngine.SceneManagement;

public class ShowStatsButton : MonoBehaviour
{
    public void LoadGraphScene()
    {
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("ShowStatsScene");
    }
}
