using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetRotationButton : MonoBehaviour/*, IPointerClickHandler*/
{
    private static float turnSpeed = 1.0f; // static turn speed for the rotation to move at
    [SerializeField] public Button reset_rot; // reference to the button in the scene
    [SerializeField] public GameObject Main_Cam_Holder; // reference to the main camera object holder in the scene

    public void OnPointerClick(PointerEventData reset_rot)
    {
        /* method that executes when the button referenced is clicked and executes the coroutine */
        StartCoroutine(RotateToZero()); // coroutine that rotates the camera back to default position
    }

    IEnumerator RotateToZero()
    {
        /* coroutine that resets the rotation of the camera back to its default position */
        Quaternion startRot = Main_Cam_Holder.transform.rotation; // gets the current transform of the camera holder
        Quaternion targetRot = Quaternion.Euler(0, 0, 0); // static original rotation of the camera holder

        float elapsedTime = 0f; // initlaising the float that will increment the time elapsed

        while (elapsedTime < turnSpeed) // loops while the time elapsed is less than the turn speed
        {
            // interpolates between the current transform and the target rotation, in the elaped time
            Main_Cam_Holder.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / turnSpeed); 
            elapsedTime += Time.deltaTime; //  increments time elapsed
            yield return null;
        }
        Main_Cam_Holder.transform.rotation = targetRot; // assigns the rotation to the target rotation, to aviod minor error margin
    }
}
