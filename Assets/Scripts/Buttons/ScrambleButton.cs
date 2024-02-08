using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ScrambleButton : MonoBehaviour, IPointerClickHandler
{
    public RotateLayers rotateLayersScript;
    public string[] moves = { "Up", "Down", "Left", "Right", "Back", "Front" };

    void Start()
    {
        rotateLayersScript = FindObjectOfType<RotateLayers>();
    }

    public void OnPointerClick(PointerEventData reset_rot)
    {
        StartCoroutine(ScrambleCube());
    }

    private IEnumerator ScrambleCube()
    {
        System.Random random = new System.Random(); // to generate either a true or false
        string[] scrambleMoves = GenerateRandomMoves(); // generates a random sequence of 20 moves
        bool clockwise;

        foreach (string move in scrambleMoves) // execute each move in the scramble
        {
            int clockwiseNum = random.Next(2); // generates a 1 or 0
            clockwise = (clockwiseNum == 1); // converts it to a boolean

            rotateLayersScript.EnqueueRotation(move, clockwise, 8.0f);

            yield return new WaitForSeconds(0.35f);
        }
    }

    private string[] GenerateRandomMoves()
    {
        System.Random random = new System.Random();
        List<string> scrambleList = new List<string>();

        for (int i = 0; i < 20; i++)
        {
            int randomIndex = random.Next(moves.Length);
            scrambleList.Add(moves[randomIndex]);
        }

        string[] scrambleArray = scrambleList.ToArray();
        return scrambleArray;
    }
}
