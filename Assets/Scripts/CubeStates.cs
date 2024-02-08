using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class CubeStates : MonoBehaviour
{
    /*
     * this script contains and manages the data for the states of each panel on each face of the rubiks cube in a 3d list:
     */

    public MathsFunctions Maths;

    public (Color color, Vector3 position)[,] cubePanels = new (Color, Vector3)[6, 9]; // uses a tuple
    public Color[,] solvedColourState = new Color[6, 9];
    public Camera[] orthoCameras = new Camera[6]; // ReadPanel

    // in order of faces in CubeStates: yellow(top), red(left), orange(right), blue(back), white(down)
    public char[] caseBase = "xxxxYxxxxxxxxRxxxxxxxxGxxxxxxxxOxxxxxxxxBxxxxxxxxWxxxx".ToCharArray(); // string that can be manipulaed to present the case
    public char[] solvedState = "YYYYYYYYYRRRRRRRRRGGGGGGGGGOOOOOOOOOBBBBBBBBBWWWWWWWWW".ToCharArray(); 
    public Dictionary<char, Color> ColourTrans { get; private set; } // has the colour translation, so if "R" -> red RGBA value etc
    public Dictionary<string, int> FaceIndexTrans = new Dictionary<string, int> // has the index translation so if layerName = Up -> index:1 
    {
        { "Up"    , 0 },
        { "Front" , 1 },
        { "Left"  , 2 },
        { "Right" , 3 },
        { "Back"  , 4 },
        { "Down"  , 5 }
    }; // these indexes are standard for every list that considers all 6 faces.
    public Dictionary<string, char> LayerColour = new Dictionary<string, char> // has the colour translation e.g. layerName = Up -> 'Y' 
    {
        { "Up"    , 'Y' },
        { "Front" , 'G' },
        { "Left"  , 'R' },
        { "Right" , 'O' },
        { "Back"  , 'B' },
        { "Down"  , 'W' }
    }; // all colours are represented as their char value

    public List<List<Vector3>> RayVectors = new List<List<Vector3>>() // ReadPanel
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

    public void InitialiseColourDict()
    {
        ColourTrans = new Dictionary<char, Color>();

        ColourTrans['R'] = new Color(1.0f, 0.0f,       0.0f); // red
        ColourTrans['G'] = new Color(0.0f, 1.0f,       0.0f); // green
        ColourTrans['B'] = new Color(0.0f, 0.0f,       1.0f); // blue
        ColourTrans['O'] = new Color(1.0f, 0.4431373f, 0.0f); // orange
        ColourTrans['Y'] = new Color(1.0f, 1.0f,       0.0f); // yellow
        ColourTrans['W'] = new Color(1.0f, 1.0f,       1.0f); // white
    }

    void Start()
    {
        Maths = FindObjectOfType<MathsFunctions>();

        InitialiseColourDict();
        ReadPanel();

        // makes a dupliate to compare with (un)solved state
        for (int faceI = 0; faceI < 6; faceI++)
        {
            for (int panelI = 0; panelI < 9; panelI++)
            {
                solvedColourState[faceI, panelI] = cubePanels[faceI, panelI].color;
            }
        }
    }

    void Update()
    {
        ReadPanel();
    }

    public void ReadPanel()
    {
        /* Reads the colour of each panel on the 3D Rubiks cube and write its to an array in CubeStates which stores the colours of each panel.*/
        for (int camIndex = 0; camIndex < orthoCameras.Length; camIndex++) //looping through all 6 cameras (per face)
        {
            Camera currentCamera = orthoCameras[camIndex];
            List<Vector3> currentFaceVectors = RayVectors[camIndex]; // gets the vectors corresponding to that face and camera

            for (int vectorIndex = 0; vectorIndex < currentFaceVectors.Count; vectorIndex++) // for every vector in currentFaceVector
            {
                Vector3 localPos = currentFaceVectors[vectorIndex]; //assigns the first vector to localPos
                Vector3 camRayPos = currentCamera.transform.TransformPoint(localPos);
                Vector3 rayDirection = camRayPos - currentCamera.transform.position;

                if (Physics.Raycast(currentCamera.transform.position, rayDirection, out var hit))
                {
                    if (hit.transform.CompareTag("RubiksCubePanel")) // checks if the what the rays hits, is a panel on the face and not another object
                    {
                        Color panelColour = hit.transform.GetComponent<Renderer>().material.color; // gets the colour and updates the list in CubeStates
                        Vector3 panelVec = hit.transform.position;
                        // calculates the index in the cubePanels array (in CubeStates) and updates the color

                        int faceIndex = camIndex;
                        cubePanels[faceIndex, vectorIndex] = (panelColour, panelVec);
                        //Debug.Log($"Updated cubePanels[{faceIndex}, {vectorIndex}] with color {panelColour} and vector {panelVec}.");//
                    }
                }
                Debug.DrawRay(currentCamera.transform.position, rayDirection, Color.red); // shows the ray
            }
        }
    }

    public char GetColour(Color targetColor)
    {
        /*loops through the ColourTranslation dict and returns the char value for the inputted RGB value*/
        foreach (var kvp in ColourTrans)
        {
            if (ColourApproximatelyEqual(kvp.Value, targetColor))
            {
                return kvp.Key;
            }
        }
        return 'X';
    }

    public char[] CubeColourState() 
    {
        /*returns a char array of the letter colours for the entire cube*/
        char[] currentState = caseBase;
        int panelCounter = 0;
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            for (int panelIndex = 0; panelIndex < 9; panelIndex++)
            {
                Color currentPanelColour = cubePanels[faceIndex, panelIndex].color;
                currentState[panelCounter] = GetColour(currentPanelColour);
                panelCounter++;
            }
        }
        return currentState;
    }

    public char[] FaceColourState(string layerName)
    {
        /*returns a char array of the letter colours on a specific face*/
        char[] currentState = "xxxxxxxxx".ToCharArray();
        switch (layerName)
        {
            case "Up": // Yellow
                for (int panelIndex = 0; panelIndex <= 8; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Up"], panelIndex].color;
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Front": // Green
                for (int panelIndex = 0; panelIndex < 9; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Front"], panelIndex].color;
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Left": // Red
                for (int panelIndex = 0; panelIndex < 9; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Left"], panelIndex].color;
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Right": // Orange
                for (int panelIndex = 0; panelIndex < 9; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Right"], panelIndex].color;
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Back": // Blue
                for (int panelIndex = 0; panelIndex < 9; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Back"], panelIndex].color;
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            case "Down": // White
                for (int panelIndex = 0; panelIndex < 9; panelIndex++)
                {
                    Color currentPanelColour = cubePanels[FaceIndexTrans["Down"], panelIndex].color;
                    currentState[panelIndex] = GetColour(currentPanelColour);
                }
                break;
            default:
                Debug.LogError("Face not found");
                break;
        }
        return currentState;
    }

    public List<bool> IsColourOnFaces(char colourChar)
    {
        /*returns a list of bools, an index for each face, and a true/ false if the specified colour is on the face
         loop though each panel on each face, for each face check if the colour (eg) 'W' is there, dont check centres [4] and add it onto a list*/
        List<bool> colourList = new List<bool>();

        foreach (var facePair in FaceIndexTrans)
        {
            string layerName = facePair.Key;
            char[] currentFaceState = FaceColourState(layerName);

            bool inFace = currentFaceState.Any(c => c == colourChar) && Array.IndexOf(currentFaceState, colourChar) != 4;
            colourList.Add(inFace);
        }
        return colourList;
    }

    public bool ColourApproximatelyEqual(Color color1, Color color2)
    {
        /*comapres the RGB value to make sure that it is approximately equal
         (mostly here for the orange, which is a pretty odd RGBA value)*/
        float threshold = 0.01f;

        return Maths.AbsoluteValue(color1.r, color2.r) < threshold &&
               Maths.AbsoluteValue(color1.g, color2.g) < threshold &&
               Maths.AbsoluteValue(color1.b, color2.b) < threshold;
    }

    public bool IsCubeSolved()
    {
        /*checks if the cube is solved by using the duplicate colour list initialised in the Start() function and comparing it to CubeStates*/
        for (int faceIndex = 0; faceIndex < 6; faceIndex++)
        {
            for (int panelIndex = 0; panelIndex < 9; panelIndex++)
            {
                Color currentPanelColor = cubePanels[faceIndex, panelIndex].color;
                Color solvedPanelColor = solvedColourState[faceIndex, panelIndex];

                if (!ColourApproximatelyEqual(currentPanelColor, solvedPanelColor))
                {
                    //Debug.Log("False, not solved");
                    return false; // Cube is not solved
                }
            }
        }
        Debug.Log("true, solveDD");
        return true; // Cube is solved
    }
}
