//using UnityEngine;
//using UnityEngine.Events;
//using UnityEngine.UI;

//public class SolveCubeDropdown : MonoBehaviour
//{
//    public Dropdown solveDropdown;
//    public CubeStates cubeStatesScript; // Reference to CubeSates script

//    void Start()
//    {
//        cubeStatesScript = FindObjectOfType<CubeStates>(); // reference to CubeStates
//        solveDropdown = GameObject.Find("Dropdown_Solve").GetComponent<Dropdown>();
//        // Ensure the reference to the dropdown is set
//        if (solveDropdown != null)
//        {
//            // Add a listener to the dropdown's onValueChanged event
//            solveDropdown.onValueChanged.AddListener(new UnityAction<int>(OnValueChanged));
//        }
//    }

//    public void OnValueChanged(int value)
//    {
//        GameObject cubeObj = GameObject.Find("Cube"); // finds Cube gameobj, it has the solver scripts attached to it
//        if (cubeObj == null)
//        {
//            Debug.LogError("RubiksCube game object not found!");
//            return;
//        }

//        BaseSolver baseSolverScript = null;

//        if (!cubeStatesScript.IsCubeSolved())
//        {
//            switch (value)
//            {
//                case 0:
//                    Debug.Log("Pick a solution!");
//                    break;
//                case 1: // Beginners
//                    baseSolverScript = cubeObj.GetComponent<BeginnersSolve>();
//                    break;
//                case 2: // CFOP
//                    baseSolverScript = cubeObj.GetComponent<CFOPSolve>();
//                    break;
//                default:
//                    Debug.LogWarning("Unhandled solving option selected.");
//                    break;
//            }
//            if (baseSolverScript != null)
//                StartCoroutine(baseSolverScript.Solve());
//            else
//                Debug.LogError("Solver script not found on Cube!");
//        }
//        else
//        {
//            Debug.Log("Cube is Solved! Scramble the Cube");
//        }
//    }
//}
