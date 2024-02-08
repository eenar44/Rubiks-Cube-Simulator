using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseSolver : MonoBehaviour
{
    public abstract IEnumerator Solve();
    public CubeStates cubeStatesScript;
    public RotateLayers rotateLayersScript;
    public MathsFunctions Maths;
    public DataManager dataMangerScript;

    public int moveCounter;
    public List<int> matchingCentres = new List<int>();
    private char[] adjacentFaces = { 'G', 'O', 'B', 'R' };
    static public float vecThreshold = 0.001f;

    void Start()
    {
        rotateLayersScript = FindObjectOfType<RotateLayers>();
        cubeStatesScript = FindObjectOfType<CubeStates>();
        dataMangerScript = FindObjectOfType<DataManager>();
        Maths = FindObjectOfType<MathsFunctions>();
    }

    public Dictionary<string, string>[] changedFrontFace = new Dictionary<string, string>[]
    {
        new Dictionary<string, string>() { { "Up", "pBack" }, { "Front", "Up"     }, { "Left", "Left"   }, { "Right", "Right" }, { "Back", "Down"   }, { "Down", "Front" } },  // Up
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "pFront" }, { "Left", "Left"   }, { "Right", "pRight" }, { "Back", "Back"   }, { "Down", "Down"  } },  // Front
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "pLeft"  }, { "Left", "pBack"  }, { "Right", "Front" }, { "Back", "Right"  }, { "Down", "Down"  } },  // Left
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "Right"  }, { "Left", "Front"  }, { "Right", "pBack"  }, { "Back", "Left"   }, { "Down", "Down"  } },  // Right
        new Dictionary<string, string>() { { "Up", "Up"    }, { "Front", "Back"   }, { "Left", "pRight" }, { "Right", "Left"  }, { "Back", "pFront" }, { "Down", "Down"  } },  // Back
        new Dictionary<string, string>() { { "Up", "Front" }, { "Front", "pDown"  }, { "Left", "Left"   }, { "Right", "Right" }, { "Back", "pUp"    }, { "Down", "Back"  } },  // Down
    };

    public Dictionary<string, string>[] changedWhiteFrontFace = new Dictionary<string, string>[]
    {
        new Dictionary<string, string>() { { "Up", "pBack" }, { "Front", "pDown"  }, { "Left", "Left"   }, { "Right", "Right"  }, { "Back", "Down"   }, { "Down", "Front" } },  // Up
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "Front"  }, { "Left", "pRight" }, { "Right", "pLeft"  }, { "Back", "Back"   }, { "Down", "pUp"   } },  // Front
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "Left"   }, { "Left", "Front"  }, { "Right", "Back"   }, { "Back", "Right"  }, { "Down", "pUp"   } },  // Left
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "pRight" }, { "Left", "pBack"  }, { "Right", "pFront" }, { "Back", "Left"   }, { "Down", "pUp"   } },  // Right
        new Dictionary<string, string>() { { "Up", "pDown" }, { "Front", "pBack"  }, { "Left", "Left"   }, { "Right", "Right"  }, { "Back", "pFront" }, { "Down", "pUp"   } },  // Back
        new Dictionary<string, string>() { { "Up", "Front" }, { "Front", "Up"     }, { "Left", "pRight" }, { "Right", "pLeft"  }, { "Back", "Down"   }, { "Down", "Back"  } },  // Down
    };
    public Dictionary<string, List<int> > otherEdgeIndex = new Dictionary<string, List<int>>
    {
        // { "layerName", new List<int> { list of indexes from top left to bottom right } },
        { "Front", new List <int> { 1, 3, 5, 7 } },
        { "Left" , new List <int> { 5, 1, 7, 3 } },
        { "Right", new List <int> { 3, 7, 1, 5 } },
        { "Back" , new List <int> { 7, 5, 3, 1 } }
    };
    /*        B
          0 | 1 | 2
       L  3 | 4 | 5   R
          6 | 7 | 8
              F
     */

    public void ApplyRotation(string move, bool clockwise)
    {
        string layer;
        if (move[0] == 'p')
        {
            clockwise = !clockwise;
            layer = move.Substring(1); // remove the 'p' prefix
        }
        else
        {
            layer = move;
        }
        rotateLayersScript.EnqueueRotation(layer, clockwise, 4.0f);

        moveCounter++;
    }

    public void AddToMoveList(int moveCounter)
    {
        if (cubeStatesScript.IsCubeSolved())
        {
            Debug.Log("number of moves: " + moveCounter);
            dataMangerScript.AddMove(moveCounter);
        }
    }

    public string RemoveP(string move)
    {
        string layer;
        if (move[0] == 'p')
        {
            layer = move.Substring(1); // remove the 'p' prefix
        }
        else
        {
            layer = move;
        }
        return layer;
    }

    public char[] GetAdjacentFaces(char face)
    {
        int index = Array.IndexOf(adjacentFaces, face);
        if (index == -1)
        {
            // Face not found, return an empty array or handle error
            return new char[0];
        }

        // Calculate the indices of the adjacent faces
        int leftIndex = (index - 1 + adjacentFaces.Length) % adjacentFaces.Length;
        int rightIndex = (index + 1) % adjacentFaces.Length;

        // Return the adjacent faces
        return new char[] { adjacentFaces[leftIndex], adjacentFaces[rightIndex] };
    }

    public char GetOppositeFace(char face)
    {
        int index = Array.IndexOf(adjacentFaces, face);
        if (index == -1)
        {
            // Face not found, handle error or return a default value
            return '\0'; // Default return value, you can change it as per your requirement
        }

        // Calculate the index of the opposite face
        int oppositeIndex = (index + 2) % adjacentFaces.Length;

        // Return the character of the opposite face
        return adjacentFaces[oppositeIndex];
    }

    public List<int> GetEdgeIndices(string layerName, char edgeColour)
    {
        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
        List<int> edgeIndices = new List<int>(); // on current face

        for (int i = 0; i < currentFaceState.Length; i++)
        {
            if (currentFaceState[i] == edgeColour && (i == 1 || i == 3 || i == 5 || i == 7))
            {
                edgeIndices.Add(i);
            }
        }
        return edgeIndices;
    }

    // can be accessed as following from other scripts:
    // BaseSolver solver = GetComponent<BaseSolver>();
    // Color redColor = solver.colourTrans["R"];

    // methods that prints the solution on the screen
}
