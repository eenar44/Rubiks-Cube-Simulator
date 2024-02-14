using UnityEngine;

public class RotateCube : MonoBehaviour
{
    private static float turnSpeed = 5.0f; // speed at which the camera will rotate around the cube

    void Update()
    {
        /* check for input in every frame to roate the camera */
        RotateWorldCamera();
    }

    void RotateWorldCamera()
    {
        /* checks for user input and when key is pressed, depending on the key pressed, rotates the camera */

        // Check for arrow key input
        if (Input.GetKey(KeyCode.DownArrow)) // if down arrow is presses
        {
            ApplyCameraRotation(Vector3.right); // rotate on corresponding axis
        } // repeat for every arrow key
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            ApplyCameraRotation(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            ApplyCameraRotation(Vector3.down);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            ApplyCameraRotation(Vector3.up);
        }
    }

    void ApplyCameraRotation(Vector3 axis)
    {
        /* changed the rotation of the camera */
        transform.Rotate(axis * turnSpeed); // changes the rotation of the camera, based on the axis to roate on and the speed that it should rotate at 
    }
}
