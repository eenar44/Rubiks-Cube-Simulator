using UnityEngine;
using UnityEngine.SceneManagement;

public class InformationButton : MonoBehaviour
{
    public void LoadInformationScene()
    {
        /* method that will load the information scene */
        // calls the SetPreviousScene from the BackButton script and passes in the current scenes name
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name); 
        SceneManager.LoadScene("InformationScene"); // loads the information scene
    }
}
