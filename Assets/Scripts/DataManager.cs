using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static List<int> numberOfMoves = new List<int>();

    public void AddMove(int move)
    {
        Debug.Log("Adddingggggggg");
        numberOfMoves.Add(move);
    }

    public List<int> GetNumberOfMoves()
    {
        return numberOfMoves;
    }
}
