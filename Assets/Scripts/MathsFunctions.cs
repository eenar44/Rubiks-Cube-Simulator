using UnityEngine;

public class MathsFunctions : MonoBehaviour
{
    public float AbsoluteValue(float Val1, float Val2)
    {
        /*Finds the absolute value between the two values that are inputted*/
        float AbsVal = Val1 - Val2;
        if (AbsVal < 0)
        {
            return AbsVal * -1;
        }
        return AbsVal;
    }

    public float ConvertVectorToAngle(Vector2 vector)
    {
        float angleRadians = Mathf.Atan2(vector.y, vector.x);
        float angleDegrees = angleRadians * Mathf.Rad2Deg;

        if (angleDegrees < 0)
        {
            angleDegrees += 360;
        }

        return angleDegrees;
    }
}