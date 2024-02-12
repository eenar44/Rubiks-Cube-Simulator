//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class CFOPSolve : BaseSolver
//{
//    static public float threshold = 0.001f;
    
    
//    void Start()
//    {
//        rotateLayersScript = FindObjectOfType<RotateLayers>();
//        cubeStatesScript = FindObjectOfType<CubeStates>();
//        Maths = FindObjectOfType<MathsFunctions>();
//    }

//    public override IEnumerator Solve()
//    {
//        yield return StartCoroutine(SolveCross());
//        yield return StartCoroutine(SolveF2L());
//        yield return StartCoroutine(SolveOLL());
//        yield return StartCoroutine(SolvePLL());
//    }

//    /*
//        foreach (var face in cubeStatesScript.FaceIndexTrans) // Iterate through each face (except the top face)
//        {
//            string layerName = face.Key;

//            if (layerName != "Up" && layerName != "Down")
//            {
//                List<int> whiteEdgeIndices = GetEdgeIndices(layerName); // intial state
//                Debug.Log($"'W' appears more than once on face {layerName} at indices: {string.Join(", ", whiteEdgeIndices)}");

//                if (whiteEdgeIndices.Contains(7)) // Check if the face is already solved
//                {
//                    Debug.Log($"Face {layerName} is solved.");
//                }

//                int faceCounter = 0;
//                while (whiteEdgeIndices.Count != 1 && faceCounter != 8)
//                {
//                    faceCounter++;
//                    if (whiteEdgeIndices.Contains(7) || whiteEdgeIndices.Count == 0)
//                    {
//                        ApplyRotation(changedFrontFace[face.Value]["Down"], true); Debug.Log("moving Down");
//                    }
//                    ApplyRotation(changedFrontFace[face.Value]["Front"], true); Debug.Log("moving Front");

//                    whiteEdgeIndices = GetEdgeIndices(layerName);
//                    Debug.Log("there are number: " + whiteEdgeIndices.Count + "and index: " + string.Join(", ", whiteEdgeIndices));
//                }

//                if (whiteEdgeIndices.Count == 1)
//                {
//                    Debug.Log(layerName + " there is one");
//                    for (int i = 0; i <= 4; i++)
//                    {
//                        if (!whiteEdgeIndices.Contains(7))
//                        {
//                            ApplyRotation(changedFrontFace[face.Value]["Front"], true); Debug.Log("moving Front");
//                        }
//                    }
//                }
//            }
//            else
//            {
//                Debug.Log("I'm not supposed to be checking face: " + layerName);
//            }
//        } */

//    IEnumerator SolveF2L()
//    {
//        Debug.Log("CFOP Step 2: Solve F2L");
//        yield return new WaitForSeconds(5.0f);
//        foreach (var face in cubeStatesScript.FaceIndexTrans)
//        {
//            string layerName = face.Key;
//            int layerIndex = face.Value;
//            if (layerName != "Down" && layerName != "Up")
//            {
//                List<bool> solvedFaces = new List<bool>();
//                /* first need to look where the corner is in the correct place
//                 * search each face for a white corner, once found get name of object at that vector
//                 * the name is in the form "YBR", this can be split into a char array, 
//                 * use LayerColor dictionary, to get the char version of the layerName that we are searching
//                 * if char layerColour in the char[] 
//                 */
//                Debug.Log("Checking " + layerName);
//                List<GameObject> cubetGameObjs = rotateLayersScript.GetCubetGameObj(layerName);
//                List<GameObject> whiteCornerGameObjs = new List<GameObject>(); // initialise list that will have all the corner gameobj
//                List<string> cubetNames = new List<string>(); // list of names of gameobj

//                List<Vector3> whiteCornerVec = new List<Vector3>();
//                List<string> whiteCornerPos = new List<string>();

//                 get white corner GameObjects and their names
//                foreach (var cubetObj in cubetGameObjs)
//                {
//                    string cubetName = cubetObj.name;
//                    Debug.Log("WOAGG " + cubetName);
//                    if (cubetName.Length == 3 && cubetName.Contains("W"))
//                    {
//                        whiteCornerGameObjs.Add(cubetObj);
//                        cubetNames.Add(cubetName);
//                        whiteCornerVec.Add(cubetObj.transform.position);
//                        Debug.Log("ADDED to approriate lists");
//                    }
//                }

//                 determine their posiiton add to the list
//                foreach (Vector3 cubetVec in  whiteCornerVec)
//                {
//                    string cubetPos = GetCubetPos(layerName, cubetVec);
//                    whiteCornerPos.Add(cubetPos);
//                }

//                 gets adjacent colours
//                char currentFaceColour = cubeStatesScript.LayerColour[layerName];
//                char adjacentColour = ' ';
//                if (layerName == "Front") { adjacentColour = 'O'; }
//                else if (layerName == "Left") { adjacentColour = 'G'; }
//                else if (layerName == "Back") { adjacentColour = 'R'; }
//                else if (layerName == "Right") { adjacentColour = 'B'; }

//                bool found = false; 
//                 check if colours in name, and correct position, if not remove.
//                for (int cubetCounter = cubetNames.Count - 1; cubetCounter >= 0; cubetCounter--)
//                {
//                    try
//                    {
//                        string cubet = cubetNames[cubetCounter];
//                        Debug.Log("cubet: " + cubet + " Current FC: " + currentFaceColour + " ADJ FC: " + adjacentColour);
//                        Debug.Log("Does it contain " + cubet.Contains(currentFaceColour) + " and does it contain " + cubet.Contains(adjacentColour));
//                        if (!(cubet.Contains(currentFaceColour) && cubet.Contains(adjacentColour)))
//                        {
//                            Debug.Log("not right!");
//                             if the string DOES NOT have both
//                            if (whiteCornerPos[cubetCounter].Substring(0) == "U")
//                            {
//                                rotateLayersScript.EnqueueRotation("Up", true, 4.0f);
//                            }
//                            else
//                            {
//                                continue;
//                            }
//                            Debug.Log("AM I HERE?");
//                            whiteCornerGameObjs.RemoveAt(cubetCounter);
//                            cubetNames.RemoveAt(cubetCounter);
//                            whiteCornerVec.RemoveAt(cubetCounter);
//                            whiteCornerPos.RemoveAt(cubetCounter);
//                        }
//                        else
//                        {
//                            Debug.Log("found the right one! at: " + cubet);
//                            if (whiteCornerPos[cubetCounter] == "UL")
//                            {
//                                rotateLayersScript.EnqueueRotation("Up", false, 4.0f);
//                            }
//                            if (whiteCornerPos[cubetCounter] == "DL") // WRONG POSITION
//                            {
//                                whiteCornerGameObjs.RemoveAt(cubetCounter);
//                                cubetNames.RemoveAt(cubetCounter);
//                                whiteCornerVec.RemoveAt(cubetCounter);
//                                whiteCornerPos.RemoveAt(cubetCounter);
//                                continue;
//                            }
//                        }
//                        Debug.Log("WORKINGGGG");
//                        found = true;
//                    }
//                    catch { Debug.Log("didnt work"); break; } // NOT WORKING ENITRELY?
//                }


//                Debug.Log("list length: " + cubetNames.Count);

//                /* locate edge peice currentFaceColour-adjacentColour -> which layer it is in -> which orientation the peice is in
//                 * can be anywhere except:
//                 *  front face, left column
//                 *  back face right column
//                 * if it is, then remove it...
//                 */
//                if (cubetNames.Count == 1 && found)
//                {
//                    Transform cubetEdgePeice = null;
//                    Transform cubeTrans = GameObject.Find("Cube").transform;
//                    for (int cubetIndex = 0; cubetIndex < cubeTrans.childCount; cubetIndex++)
//                    {
//                        Transform cubet = cubeTrans.GetChild(cubetIndex);
//                        string cubetName = cubet.name;
//                        if (cubetName.Contains(currentFaceColour) && cubetName.Contains(adjacentColour))
//                        {
//                            cubetEdgePeice = cubet;
//                            break;
//                        }
//                    }

//                     find position of this edge peice
//                    Vector3 cubetEdgeVec = cubetEdgePeice.transform.position; //HERE 
//                    string cubetEdgePos = "";
//                    if (Maths.AbsoluteValue(cubetEdgeVec.z, 1.025f) < threshold) { cubetEdgePos += (RemoveP(changedFrontFace[layerIndex]["Front"]) + " "); }
//                    if (Maths.AbsoluteValue(cubetEdgeVec.z, -1.025f) < threshold) { cubetEdgePos += (RemoveP(changedFrontFace[layerIndex]["Back"]) + " "); }
//                    if (Maths.AbsoluteValue(cubetEdgeVec.x, 1.025f) < threshold) { cubetEdgePos += (RemoveP(changedFrontFace[layerIndex]["Left"]) + " "); }
//                    if (Maths.AbsoluteValue(cubetEdgeVec.x, -1.025f) < threshold) { cubetEdgePos += (RemoveP(changedFrontFace[layerIndex]["Right"]) + " "); }
//                    if (Maths.AbsoluteValue(cubetEdgeVec.y, 1.025f) < threshold) { cubetEdgePos += (RemoveP(changedFrontFace[layerIndex]["Up"]) + " "); }
//                    if (Maths.AbsoluteValue(cubetEdgeVec.y, -1.025f) < threshold) { cubetEdgePos += (RemoveP(changedFrontFace[layerIndex]["Down"]) + " "); }
//                    Debug.Log("edge pos: " + cubetEdgePos);

//                     use what i have to apply algorithm
//                }




//            }



//             Identify the adjacent face index (to the right)
            



//            /* basic cases
//             * the edge peice is in the top layer
//             * the edge is either on the correct layer or, opposite the layer*/
//        }
//        yield return null;
//    }

//    IEnumerator SolveOLL()
//    {
//         CFOP Step 3: Solve the orientation of the last layer (OLL)
//        Debug.Log("CFOP Step 3: Solve OLL");
//         Your implementation here
//        yield return null;
//    }

//    IEnumerator SolvePLL()
//    {
//         CFOP Step 4: Solve the permutation of the last layer (PLL)
//        Debug.Log("CFOP Step 4: Solve PLL");
//         Your implementation here
//        yield return null;
//    }

    

//    private string GetCubetPos(string layerName, Vector3 cubetVec)
//    {
        
//        string cubetPos = "";

//        if (Maths.AbsoluteValue(cubetVec.y, -1.025f) < threshold)
//        {
//            cubetPos += "D";
//        }
//        else if (Maths.AbsoluteValue(cubetVec.y, 1.025f) < threshold)
//        {
//            cubetPos += "U";
//        }

//        if (layerName == "Front" && Maths.AbsoluteValue(cubetVec.x, -1.025f) < threshold) { cubetPos += "L"; }
//        else if (layerName == "Front" && Maths.AbsoluteValue(cubetVec.x, 1.025f) < threshold) { cubetPos += "R"; }

//        if (layerName == "Back" && Maths.AbsoluteValue(cubetVec.x, 1.025f) < threshold) { cubetPos += "R"; }
//        else if (layerName == "Back" && Maths.AbsoluteValue(cubetVec.x, -1.025f) < threshold) { cubetPos += "L"; }

//        if (layerName == "Left" && Maths.AbsoluteValue(cubetVec.z, 1.025f) < threshold) { cubetPos += "L"; }
//        else if (layerName == "Left" && Maths.AbsoluteValue(cubetVec.z, -1.025f) < threshold) { cubetPos += "R"; }

//        if (layerName == "Right" && Maths.AbsoluteValue(cubetVec.z, 1.025f) < threshold) { cubetPos += "L"; }
//        else if (layerName == "Right" && Maths.AbsoluteValue(cubetVec.z, -1.025f) < threshold) { cubetPos += "R"; }

//        Debug.Log(cubetPos);
//        return cubetPos;
//    }

//    private List<int> GetwhiteCornerIndices(string layerName)
//    {
//        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
//        List<int> whiteCornerIndices = new List<int>(); // on current face

//        for (int i = 0; i < currentFaceState.Length; i++)
//        {
//            if (currentFaceState[i] == 'W' && (i == 0 || i == 2 || i == 6 || i == 8))
//            {
//                whiteCornerIndices.Add(i);
//            }
//        }
//        return whiteCornerIndices;
//    }

//    private bool NeedsRotation(string layerMoving)
//    {
//        int panelI = otherEdgeIndex[layerMoving][2];
//        int faceI = cubeStatesScript.FaceIndexTrans[layerMoving];
//        Color panelColour = cubeStatesScript.cubePanels[faceI, panelI].color;

//        return !cubeStatesScript.ColourApproximatelyEqual(panelColour, cubeStatesScript.ColourTrans['W']);
//    }
//}
