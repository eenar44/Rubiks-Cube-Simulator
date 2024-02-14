using UnityEngine;
using UnityEngine.UI;

public class SolveButtonHandler : MonoBehaviour
{
    public BeginnersSolve beginnersSolve; // reference to the BeginnersSolve script
    public CubeStates cubeStatesScript; // reference to the CubeStats script

    private void Start()
    {
        beginnersSolve = FindObjectOfType<BeginnersSolve>(); // find the BeginnersSolve script in the scene
        cubeStatesScript = FindObjectOfType<CubeStates>(); // find the CubeStates script in the scene

        Button solveButton = GetComponent<Button>(); // gets a reference to the button component

        solveButton.onClick.AddListener(OnSolveButtonClick); // adds a listener to the button's click event
    }

    private void OnSolveButtonClick()
    {
        /* calls the solve method of the BeginnersSolve script when the button is clicked */
        if (beginnersSolve != null) // checks if the script is being properly referecned
        {
            if (!cubeStatesScript.IsCubeSolved()) // checks if the cube is solved first
            {
                StartCoroutine(beginnersSolve.Solve()); // if its not solved, then execute Solve method in BeginnersSolve
            }
            else
            {
                Debug.Log("Scramble cube first");
            }
        }
        else
        {
            Debug.LogError("BeginnersSolve script not found!"); // warning if script not found
        }
    }
}
