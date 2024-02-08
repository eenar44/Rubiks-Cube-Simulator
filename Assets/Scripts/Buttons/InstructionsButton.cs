using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsButton : MonoBehaviour
{
    public void LoadInstructionsScene()
    {
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("InstructionsScene");
    }
}
