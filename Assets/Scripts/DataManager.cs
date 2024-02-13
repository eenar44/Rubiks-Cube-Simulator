using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public static List<int> numberOfMoves = new List<int>(); // creates public list that will store the number of moves that will be used to plot the graph

    public void AddMove(int move)
    {
        // public function that will add the move that is passed in
        Debug.Log("Adddingggggggg");
        numberOfMoves.Add(move);
    }

    public List<int> GetNumberOfMoves()
    {
        // public function that will return the list
        return numberOfMoves;
    }
}
