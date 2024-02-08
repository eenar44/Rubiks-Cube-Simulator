using UnityEngine;


public class RotateCube : MonoBehaviour
{
    private static float turnSpeed = 5.0f;

    void Update()
    {
        using_worldcam();
    }

    void using_worldcam()
    {
        // Check for arrow key input
        if (Input.GetKey(KeyCode.DownArrow))
        {
            RotateCamera(Vector3.right);
        }
        else if (Input.GetKey(KeyCode.UpArrow))
        {
            RotateCamera(Vector3.left);
        }
        else if (Input.GetKey(KeyCode.LeftArrow))
        {
            RotateCamera(Vector3.down);
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            RotateCamera(Vector3.up);
        }
    }

    void RotateCamera(Vector3 axis)
    {
        transform.Rotate(axis * turnSpeed);
    }
}
