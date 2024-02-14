using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void StartSimulation()
    {
        /* method that will load the main simulation scene */
        // calls the SetPreviousScene from the BackButton script and passes in the current scenes name
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("RubiksMainScene"); // loads the simulator's scene
    }
}
