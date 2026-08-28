using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSolver : MonoBehaviour
{
    public abstract IEnumerator Solve(); // initalises the sovle method that will be overwritten in the BeginnersSolve script
    public CubeStates cubeStatesScript; // reference to CubeStates script
    public RotateLayers rotateLayersScript; // reference to RotateLayers script
    public MathsFunctions Maths; // reference to MathsFunctions script
    public DataManager dataMangerScript; // reference to DataManager script

    public int moveCounter; // public int that will be incremented to hold the number of moves that have been made in this solve
    public List<int> matchingCentres = new List<int>(); // public list of face indexes that have matching centres to a panel
    // list of face colours that are directly adjacent to each other cycling around the cube in a clockwise direction, keeping consistant yellow as top and white as bottom
    private char[] adjacentFaces = { 'G', 'O', 'B', 'R' }; 
    static public float vecThreshold = 0.001f; // vector threshold for error

    // a 'p' in front of the roation indicates prime for example: Up => U and pUp => U'
    public Dictionary<string, string>[] changedFrontFace = new Dictionary<string, string>[]
    {
        new Dictionary<string, string>() { { "Up", "pBack" }, { "Front", "Up"     }, { "Left", "Left"   }, { "Right", "Right"  }, { "Back", "Down"   }, { "Down", "Front" } },  // Up
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "pFront" }, { "Left", "Left"   }, { "Right", "pRight" }, { "Back", "Back"   }, { "Down", "Down"  } },  // Front
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "pLeft"  }, { "Left", "pBack"  }, { "Right", "Front"  }, { "Back", "Right"  }, { "Down", "Down"  } },  // Left
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "Right"  }, { "Left", "Front"  }, { "Right", "pBack"  }, { "Back", "Left"   }, { "Down", "Down"  } },  // Right
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "Back"   }, { "Left", "pRight" }, { "Right", "Left"   }, { "Back", "pFront" }, { "Down", "Down"  } },  // Back
        new Dictionary<string, string>() { { "Up", "Front" }, { "Front", "pDown"  }, { "Left", "Left"   }, { "Right", "Right"  }, { "Back", "pUp"    }, { "Down", "Back"  } },  // Down
    }; // dictionary that considers manipulations from a differnet face keeping consistant the top face being yellow and the bottom face being white. indexes for each layer remain the same

    public Dictionary<string, string>[] changedWhiteFrontFace = new Dictionary<string, string>[]
    {
        new Dictionary<string, string>() { { "Up", "pBack" }, { "Front", "pDown"  }, { "Left", "Left"   }, { "Right", "Right"  }, { "Back", "Down"   }, { "Down", "Front" } },  // Up
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "Front"  }, { "Left", "pRight" }, { "Right", "pLeft"  }, { "Back", "Back"   }, { "Down", "pUp"   } },  // Front
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "Left"   }, { "Left", "Front"  }, { "Right", "Back"   }, { "Back", "Right"  }, { "Down", "pUp"   } },  // Left
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "pRight" }, { "Left", "pBack"  }, { "Right", "pFront" }, { "Back", "Left"   }, { "Down", "pUp"   } },  // Right
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "pBack"  }, { "Left", "Left"   }, { "Right", "Right"  }, { "Back", "pFront" }, { "Down", "pUp"   } },  // Back
        new Dictionary<string, string>() { { "Up", "Front" }, { "Front", "Up"     }, { "Left", "pRight" }, { "Right", "pLeft"  }, { "Back", "Down"   }, { "Down", "Back"  } },  // Down
    }; // dictionary that considers manipulations from a differnet face now considering the top face being white and the bottom face being yellow. indexes for each layer remain the same
    public Dictionary<string, List<int> > otherEdgeIndex = new Dictionary<string, List<int>>
    {
        // { "layerName", new List<int> { list of indexes from top left to bottom right } },
        { "Front", new List <int> { 1, 3, 5, 7 } },
        { "Left" , new List <int> { 5, 1, 7, 3 } },
        { "Right", new List <int> { 3, 7, 1, 5 } },
        { "Back" , new List <int> { 7, 5, 3, 1 } }
    }; // dictionary that considers the adjacent layers, paired with a list of edge indeces from top left to bottom right, had that face been the front face
    /* illustration of the top face panels (reprsented by the grid) and the layers positions according to that
              B
          0 | 1 | 2
       L  3 | 4 | 5   R
          6 | 7 | 8
              F
     */

    void Start()
    {
        rotateLayersScript = FindObjectOfType<RotateLayers>(); // finds reference to RotateLayers in scene
        cubeStatesScript = FindObjectOfType<CubeStates>(); // finds reference to CubeStates in scene
        dataMangerScript = FindObjectOfType<DataManager>(); // finds reference to DataManager in scene
        Maths = FindObjectOfType<MathsFunctions>(); // finds reference to MathsFunctions in scene
    }

    public void ApplyRotation(string move, bool clockwise)
    {
        /* function that applys the rotation specified in the solve script */
        string layer; // initalises the variable holding the layer name
        if (move[0] == 'p') // checks if the rotation is prime/ opposite direction
        {
            clockwise = !clockwise; // if it is, then change the direction of the rotation
            layer = move.Substring(1); // remove the 'p' prefix and assigns it to layer
        }
        else
        {
            layer = move; // assigns move to layer
        }
        rotateLayersScript.EnqueueRotation(layer, clockwise, 4.0f); // enqueues the rotation queue to execute
        moveCounter++; // increments move counter, totals the number of moves made in a solve
    }

    public void AddToMoveList(int moveCounter)
    {
        /* checks fi the cube is sovled, if it is, then adds move counter to the list */
        if (cubeStatesScript.IsCubeSolved()) // checks if the cube is actually solved
        {
            Debug.Log("number of moves: " + moveCounter); ///////////////////////////////////////////
            dataMangerScript.AddMove(moveCounter); // appends moveCounter to the list 
        }
    }

    public string RemoveP(string move)
    {
        /* method that removes the 'p' from the move */
        string layer;
        if (move[0] == 'p') // checks if the p prefix is present
        {
            layer = move.Substring(1); // remove the 'p' prefix and assigns it to layer
        }
        else
        {
            layer = move; // assigns move to layer
        }
        return layer; // returns the layer name
    }

    public char[] GetAdjacentFaces(char face)
    {
        /* returns a char array of the faces adjacent to the face that is passed into the function */
        int index = Array.IndexOf(adjacentFaces, face); // gets the index that the face is present at
        if (index == -1) // if the index is -1
        {
            return new char[0]; // face not found, return an empty array or handle error
        }

        // calculate the indices of the adjacent faces
        int leftIndex = (index - 1 + adjacentFaces.Length) % adjacentFaces.Length;
        int rightIndex = (index + 1) % adjacentFaces.Length;

        // return the adjacent faces
        return new char[] { adjacentFaces[leftIndex], adjacentFaces[rightIndex] };
    }

    public char GetOppositeFace(char face)
    {
        int index = Array.IndexOf(adjacentFaces, face);
        if (index == -1)
        {
            // face not found, handle error or return a default value
            return '\0'; // default return value
        }

        // calculate the index of the opposite face
        int oppositeIndex = (index + 2) % adjacentFaces.Length;

        // return the character of the opposite face
        return adjacentFaces[oppositeIndex];
    }

    public List<int> GetEdgeIndices(string layerName, char edgeColour)
    {
        /* method that will return the indeces that edge colour is present at */
        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the current face state
        List<int> edgeIndices = new List<int>(); // list to hold the indicies of the edges that have edgeColour on the current face

        for (int i = 0; i < currentFaceState.Length; i++) // loops through the current face
        {
            // chechs if the colour on the panel is the same as the desires edge colour, and checks if it is truly an edge (edge indecies are 1, 3, 5 and 7)
            if (currentFaceState[i] == edgeColour && (i == 1 || i == 3 || i == 5 || i == 7))
            { 
                edgeIndices.Add(i); // adds the counter to the list
            }
        }
        return edgeIndices; // returns the list of indecies
    }
}
