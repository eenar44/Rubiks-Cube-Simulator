using UnityEngine;
using UnityEngine.SceneManagement;

public class InformationButton : MonoBehaviour
{
    public void LoadInformationScene()
    {
        BackButton.SetPreviousScene(SceneManager.GetActiveScene().name);
        SceneManager.LoadScene("InformationScene");
    }
}
