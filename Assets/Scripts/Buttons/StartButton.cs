using UnityEngine;
using UnityEngine.SceneManagement;

public class StartButton : MonoBehaviour
{
    public void StartSimulation()
    {
        /* method that will load the main simulation scene */
        SceneManager.LoadScene("RubiksMainScene"); // loads the simulator's scene
    }
}
