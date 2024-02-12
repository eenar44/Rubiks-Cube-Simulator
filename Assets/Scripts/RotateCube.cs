using UnityEngine;


public class RotateCube : MonoBehaviour
{
    private static float turnSpeed = 5.0f;

    void Update()
    {
        RotateWorldCamera();
    }

    void RotateWorldCamera()
    {
        // Check for arrow key input
        if (Input.GetKey(KeyCode.DownArrow))
        {
            ApplyCameraRotation(Vector3.right);
        }
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
        transform.Rotate(axis * turnSpeed);
    }
}
