using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ScrambleButton : MonoBehaviour, IPointerClickHandler
{
    public RotateLayers rotateLayersScript; // reference to RotateLayers script
    [SerializeField] public Button scramble_botton; // reference to the button in the scene
    public static string[] moves = { "Up", "Down", "Left", "Right", "Back", "Front" }; // list of all the posiible moves
    public System.Random random = new System.Random(); // used to generate the random moves and directions

    void Start()
    {
        rotateLayersScript = FindObjectOfType<RotateLayers>(); // finds reference to RotateLayers in the scene
    }

    public void OnPointerClick(PointerEventData scramble_button)
    {
        /* method that executes when the button referenced is clicked and executes the coroutine */
        StartCoroutine(ScrambleCube()); // coroutine that scrambles the cube
    }

    private IEnumerator ScrambleCube()
    { 
        /* method that takes the scramblled list, assigns a random direction to them and executes the move */
        string[] scrambleMoves = GenerateRandomMoves(); // generates a random sequence of 20 moves
        bool clockwise; // initialises the clockwise bool

        foreach (string move in scrambleMoves) // execute each move in the scramble
        {
            int clockwiseNum = random.Next(2); // generates a 1 or 0
            clockwise = (clockwiseNum == 1); // converts it to a boolean

            rotateLayersScript.EnqueueRotation(move, clockwise, 8.0f); // adds the move onto the list

            yield return new WaitForSeconds(0.35f); // wait until the move is finished executing
        }
    }

    private string[] GenerateRandomMoves()
    {
        /* method that generates a random array of moves and returns them */
        List<string> scrambleList = new List<string>(); // intialises a new list that will hold scramblled strings

        for (int i = 0; i < 20; i++) // loops between 0 and 20
        {
            int randomIndex = random.Next(moves.Length); // gets a random index between 0 and the length of the moves list
            scrambleList.Add(moves[randomIndex]); // adds the move onto the list of scramblled moves
        }

        string[] scrambleArray = scrambleList.ToArray(); // converts the list into an array
        return scrambleArray; // returns the array
    }
}
