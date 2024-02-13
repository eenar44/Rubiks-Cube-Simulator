using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeStates : MonoBehaviour
{
    /*
     * this script contains and manages the data for the states of each panel on each face of the rubiks cube in a 3d list:
     */

    public MathsFunctions Maths; // reference to Maths functiosn script

    public Color[,] cubePanels = new Color[6, 9]; // array of colors
    public Color[,] solvedColourState = new Color[6, 9]; // array that will hold the solved colour state
    [SerializeField] public Camera[] orthoCameras = new Camera[6]; // list of orthographic cameras in the scene, that are used for raycasts

    public Dictionary<char, Color> ColourTrans = new Dictionary<char, Color> // has the colour translation, so if 'R' -> red RGB value: (1, 0, 0)
    {
        { 'R', new Color(1.0f, 0.0f,       0.0f)},
        { 'G', new Color(0.0f, 1.0f,       0.0f)},
        { 'B', new Color(0.0f, 0.0f,       1.0f)},
        { '0', new Color(1.0f, 0.4431373f, 0.0f)},
        { 'Y', new Color(1.0f, 1.0f,       0.0f)},
        { 'W', new Color(1.0f, 1.0f,       1.0f)},
    };

    /* all lists that considers all the faces are in the order: top (yellow), front (green), left (red), right (orange), back (blue), down (white) 
     * panels are indexed as follows:
     *    0 | 1 | 2
     *   ---+---+---
     *    3 | 4 | 5
     *   ---+---+---
     *    6 | 7 | 8
     * every list follows these indexing
     */
    public Dictionary<string, int> FaceIndexTrans = new Dictionary<string, int> // has the index translation so if layerName = Up -> index = 1 
    {
        { "Up"    , 0 },
        { "Front" , 1 },
        { "Left"  , 2 },
        { "Right" , 3 },
        { "Back"  , 4 },
        { "Down"  , 5 }
    }; // these indexes are standardised for every data structure
    public Dictionary<string, char> LayerColour = new Dictionary<string, char> // stores the relationship between layerName and colour e.g. layerName = Up -> 'Y' 
    {
        { "Up"    , 'Y' },
        { "Front" , 'G' },
        { "Left"  , 'R' },
        { "Right" , 'O' },
        { "Back"  , 'B' },
        { "Down"  , 'W' }
    }; // all colours are represented as their char value

    public List<List<Vector3>> RayVectors = new List<List<Vector3>>() // 2d list that contains the raycast vectors to read each panel on each face
    {
        new List<Vector3>() // TOP
        {
            new Vector3(-1,  1,  5),
            new Vector3( 0,  1,  5),
            new Vector3( 1,  1,  5),
            new Vector3(-1,  0,  5),
            new Vector3( 0,  0,  5),
            new Vector3( 1,  0,  5),
            new Vector3(-1, -1,  5),
            new Vector3( 0, -1,  5),
            new Vector3( 1, -1,  5),
        },
        new List<Vector3>() // FRONT
        {
            new Vector3(-1,  1, 5),
            new Vector3( 0,  1, 5),
            new Vector3( 1,  1, 5),
            new Vector3(-1,  0, 5),
            new Vector3( 0,  0, 5),
            new Vector3( 1,  0, 5),
            new Vector3(-1, -1, 5),
            new Vector3( 0, -1, 5),
            new Vector3( 1, -1, 5),
        },
        new List<Vector3>() // LEFT
        {
            new Vector3(-1,  1, 5),
            new Vector3( 0,  1, 5),
            new Vector3( 1,  1, 5),
            new Vector3(-1,  0, 5),
            new Vector3( 0,  0, 5),
            new Vector3( 1,  0, 5),
            new Vector3(-1, -1, 5),
            new Vector3( 0, -1, 5),
            new Vector3( 1, -1, 5),
        },

        new List<Vector3>() // RIGHT
        {
            new Vector3(-1,  1, 5),
            new Vector3( 0,  1, 5),
            new Vector3( 1,  1, 5),
            new Vector3(-1,  0, 5),
            new Vector3( 0,  0, 5),
            new Vector3( 1,  0, 5),
            new Vector3(-1, -1, 5),
            new Vector3( 0, -1, 5),
            new Vector3( 1, -1, 5),
        },
        new List<Vector3>() // BACK
        {
            new Vector3( -1,  1, 5),
            new Vector3( 0,  1, 5),
            new Vector3( 1,  1, 5),
            new Vector3(-1,  0, 5),
            new Vector3( 0,  0, 5),
            new Vector3( 1,  0, 5),
            new Vector3(-1, -1, 5),
            new Vector3( 0, -1, 5),
            new Vector3( 1, -1, 5),
        },

        new List<Vector3>() // DOWN
        {
            new Vector3( 1, -1, 5),
            new Vector3( 0, -1, 5),
            new Vector3(-1, -1, 5),
            new Vector3( 1,  0, 5),
            new Vector3( 0,  0, 5),
            new Vector3(-1,  0, 5),
            new Vector3( 1,  1, 5),
            new Vector3( 0,  1, 5),
            new Vector3(-1,  1, 5),
        }
    };

    void Start()
    {
        Maths = FindObjectOfType<MathsFunctions>(); // finds the reference to MathsFunctions in the scene

        ReadPanel(); // executes ReadPanel on start to get the initial solved state of the cube
        solvedColourState = cubePanels; // assigns the initial solved state to the solvedColourState list 
    }

    void Update()
    {
        ReadPanel(); // executes ReadPanel so that the cubePanels list updates in every frame.
    }

    public void ReadPanel()
    {
        for (int camIndex = 0; camIndex < orthoCameras.Length; camIndex++) // loops through every orthographic camera in the list
        {
            Camera currentCamera = orthoCameras[camIndex]; // gets the current camera using the index
            List<Vector3> currentFaceVectors = RayVectors[camIndex]; // gets the list of vectors for the same index

            for (int vectorIndex = 0; vectorIndex < currentFaceVectors.Count; vectorIndex++) // loops through each vector in the list of vectors for that face
            {
                Vector3 localPos = currentFaceVectors[vectorIndex]; // gets the local raycast vector for the panel relative to the camera
                Vector3 camRayPos = currentCamera.transform.TransformPoint(localPos); // translates the local position of the raycast into a world vector so it can be plotted correctly
                Vector3 rayDirection = camRayPos - currentCamera.transform.position; // calculates the direction of the ray in world space

                if (Physics.Raycast(currentCamera.transform.position, rayDirection, out var hit)) // checks if the raycast hits an object in the scene
                {
                    if (hit.transform.CompareTag("RubiksCubePanel")) // checks if it is the correct objectby comparing the tag. each panel has the tag: "RubiksCubePanel"
                    {
                        Color panelColour = hit.transform.GetComponent<Renderer>().material.color; // gets the colour of the panel 
                        int faceIndex = camIndex; // assigns the camera index to a faceIndex holder for carity
                        cubePanels[faceIndex, vectorIndex] = panelColour; // adds the colour of the panel to the corresponding face index and panel index
                    }
                }
                Debug.DrawRay(currentCamera.transform.position, rayDirection, Color.red); // **********
            }
        }
    }

    public char GetColour(Color targetColor)
    {
        /* loops through the ColourTrans dictionary and returns the char value for the inputted RGB value */
        foreach (var kvp in ColourTrans) // loops through each value in the ColourTans dictionary
        {
            if (ColourApproximatelyEqual(kvp.Value, targetColor)) // lighting may effect the actual colour so check if its approximately equal to the target colour
            {
                return kvp.Key; // if it is approximately equal then return the corresponding character
            }
        }
        return 'X'; // returns 'X', a null value if colour is not found
    }

    public char[] FaceColourState(string layerName)
    {
        /* returns a char array of the letter colours on a specific face */
        char[] currentState = "xxxxxxxxx".ToCharArray(); // base array for the current face state. 
        switch (layerName) // switch case that will index and overwrite the base string based on the colour of each panel based on standard indexing as mentioned above
        {
            case "Up": // Yellow
                for (int panelIndex = 0; panelIndex < currentState.Length; panelIndex++) // loops between 0 and and the length of the currentState
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Up"], panelIndex]; // gets the colour of the "Up" face at that panel index
                    currentState[panelIndex] = GetColour(currentPanelColour); // gets the character of the corresponding colour on that panel.
                } //  repeats for every face
                break;
            case "Front": // Green
                for (int panelIndex = 0; panelIndex < currentState.Length; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Front"], panelIndex];
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Left": // Red
                for (int panelIndex = 0; panelIndex < currentState.Length; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Left"], panelIndex];
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Right": // Orange
                for (int panelIndex = 0; panelIndex < currentState.Length; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Right"], panelIndex];
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Back": // Blue
                for (int panelIndex = 0; panelIndex < currentState.Length; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Back"], panelIndex];
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Down": // White
                for (int panelIndex = 0; panelIndex < currentState.Length; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Down"], panelIndex];
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            default:
                Debug.LogError("Face not found"); // debug error if the layerName is null
                break;
        }
        return currentState; // return the char array of the current face state
    }

    public bool ColourApproximatelyEqual(Color color1, Color color2)
    {
        /*comapres the RGB value to make sure that it is approximately equal
         (mostly here for the orange, which is a pretty odd RGBA value)*/
        float threshold = 0.01f; // colour threshold, if the colour approximation is less than the threshold then it is not the correct colour

        // returns the boolean true or false if the colour is approximately equal
        return Maths.AbsoluteValue(color1.r, color2.r) < threshold && // compares the absolute value of both red colours to the threshold
               Maths.AbsoluteValue(color1.g, color2.g) < threshold && // compares the absolute value of both green colours to the threshold
               Maths.AbsoluteValue(color1.b, color2.b) < threshold;   // compares the absolute value of both blue colours to the threshold
    }

    public bool IsCubeSolved()
    {
        /* checks if the cube is solved by using the duplicate colour list initialised in the Start() function and 
         * comparing it to CubeStates
         */

        if (cubePanels == solvedColourState) // check if the current colour list is the same as the solved colour state list
        {
            Debug.Log("SOLVEEEEEEEEEEEEEEEDDDDDDDDDDD");
            return true; // returns true if its the same
        }
        return false; //  if not return false
    }
}
