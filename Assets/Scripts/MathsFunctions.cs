using UnityEngine;

public class MathsFunctions : MonoBehaviour
{
    public float AbsoluteValue(float Val1, float Val2)
    {
        /* Finds the absolute value between the two values that are inputted */
        float AbsVal = Val1 - Val2; // calculates the absolute value
        // returns the modulas of the value
        if (AbsVal < 0) // checks if the value is less than 0
        {
            return AbsVal * -1; // if the value is negative multiply by -1 to make it positive and retun it 
        }
        return AbsVal; // return the value if it is already positive
    }

    public float ConvertVectorToAngle(Vector2 vector)
    {
        /* takes in a vector and converts it into an angle between 0 and 360 degrees */
        float angleRadians = Mathf.Atan2(vector.y, vector.x); // calculates the angle between the two vectors
        float angleDegrees = angleRadians * RadiansToDegrees(); // converts this angle into degrees

        if (angleDegrees < 0) // checks if the angle is between 0 and 360
        {
            angleDegrees += 360; // if its less than 0 then add 360
        }

        return angleDegrees; // returns the angle
    }

    public float RadiansToDegrees()
    {
        /* constant that is multiplied by a radian angle to convert to degrees */
        return 180f / Mathf.PI; // deg = (180/pi) * rad
    }
}