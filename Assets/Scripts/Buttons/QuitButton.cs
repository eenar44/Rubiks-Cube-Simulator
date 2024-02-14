using UnityEngine;

public class QuitButton : MonoBehaviour
{
   public void QuitGame()
   {
        /* method that will quit the game */
        Debug.Log("Quit");
        Application.Quit(); // quits the application
   }
}
