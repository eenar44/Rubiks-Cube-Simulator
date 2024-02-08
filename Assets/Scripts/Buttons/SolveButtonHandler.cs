using UnityEngine;
using UnityEngine.UI;

public class SolveButtonHandler : MonoBehaviour
{
    public BeginnersSolve beginnersSolve; // Reference to the BeginnersSolve script

    private void Start()
    {
        // Find the BeginnersSolve script in the scene
        beginnersSolve = FindObjectOfType<BeginnersSolve>();

        // Get a reference to the button component
        Button solveButton = GetComponent<Button>();

        // Add a listener to the button's click event
        solveButton.onClick.AddListener(OnSolveButtonClick);
    }

    private void OnSolveButtonClick()
    {
        // Call the Solve method of the BeginnersSolve script when the button is clicked
        if (beginnersSolve != null)
        {
            StartCoroutine(beginnersSolve.Solve());
        }
        else
        {
            Debug.LogError("BeginnersSolve script not found!");
        }
    }
}
