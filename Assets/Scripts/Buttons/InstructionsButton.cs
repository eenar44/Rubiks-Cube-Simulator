using UnityEngine;
using UnityEngine.SceneManagement;

public class InstructionsButton : MonoBehaviour
{
    public void LoadInstructionsScene()
    {
        /*method that will load the instructions scene */
        // calls the SetPreviousScene from the BackButton script and passes in the current scenes name
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("InstructionsScene"); // loads the instruction scene
    }
}
