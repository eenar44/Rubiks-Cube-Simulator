using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ResetRotationButton : MonoBehaviour, IPointerClickHandler
{
    private static float turnSpeed = 1.0f;
    public Button reset_rot;
    public GameObject Main_Cam;

    public void OnPointerClick(PointerEventData reset_rot)
    {
        StartCoroutine(RotateToZero());
    }

    IEnumerator RotateToZero()
    {
        Quaternion startRot = Main_Cam.transform.rotation;
        Quaternion targetRot = Quaternion.Euler(0, 0, 0);

        float elapsedTime = 0f;

        while (elapsedTime < turnSpeed)
        {
            Main_Cam.transform.rotation = Quaternion.Slerp(startRot, targetRot, elapsedTime / turnSpeed);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        Main_Cam.transform.rotation = targetRot;
    }

}
