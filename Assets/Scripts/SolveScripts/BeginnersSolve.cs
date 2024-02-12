using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeginnersSolve : BaseSolver
{
    public List<bool> solved = new List<bool>(new bool[4]);


    public override IEnumerator Solve()
    {
        yield return StartCoroutine(Beginners());
    }

    IEnumerator Beginners()
    {
        Debug.Log("Beginner's Method");

        yield return StartCoroutine(SolveCross());
        yield return StartCoroutine(SolveFirstLayerCorners());
        //yield return StartCoroutine(SolveSecondLayer());
        //yield return StartCoroutine(SolveYellowCross());
        //yield return StartCoroutine(OrientYellowCorners());
        //yield return StartCoroutine(PermuteYellowCorners());
        //yield return StartCoroutine(PositionYellowEdges());

        Debug.Log("Solving completed using Beginner's Method.");

        AddToMoveList(moveCounter);

        yield return null;
    }

    // SOLVING CROSS ==========================================================================================================
    
    IEnumerator SolveCross()
    {
        Debug.Log("Step 1: Solve Cross");

        // need to add a cross checker

        //yield return StartCoroutine(PlaceWhiteEdges());
        //Debug.Log("WhiteEdged have been placed");
        //yield return new WaitForSeconds(2.0f);

        yield return StartCoroutine(FlipCrossEdges());
        Debug.Log("Edges have been flipped");
        yield return new WaitForSeconds(2.0f);

        matchingCentres.Clear();
        yield return StartCoroutine(PositionCrossEdges());
        Debug.Log("Cross has been oriented");
        yield return new WaitForSeconds(3.0f);

        yield return StartCoroutine(OrientCrossEdges());
        Debug.Log("Cross has been SOLVED");
        yield return new WaitForSeconds(2.0f);

        yield return null;
    }
    /*
    IEnumerator PlaceWhiteEdges()
    {
        while (!IsAllWhiteEdgesPlaced())
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans)
            {
                string layerName = face.Key;
                int layerIndex = face.Value;
                if (layerName != "Up" && layerName != "Down")
                {
                    Debug.Log("=========== " + layerName + " ============");
                    while (GetWhiteEdgeCubets(layerName).Count != 1)
                    {
                        Debug.Log("THERES NOT ONE");
                        yield return new WaitForSeconds(2.0f);

                        if (GetWhiteEdgeCubets(layerName).Count == 0)
                        {
                            Debug.Log("  There are none!!!");
                            List<GameObject> upWhiteEdges = GetWhiteEdgeCubets("Up");
                            if (upWhiteEdges.Count > 0)
                            {
                                while (GetWhiteEdgeCubets(layerName).Count != 1)
                                {
                                    Debug.Log("    keep goign down uptil theres one");
                                    yield return new WaitForSeconds(2.0f);
                                    ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], true);
                                    yield return new WaitForSeconds(2.0f);
                                }
                            }
                            Debug.Log("DONE");
                        }
                        if (GetWhiteEdgeCubets(layerName).Count > 1)
                        {
                            Debug.Log("  There are nmore than one!!!");
                            while (!IsEdgePlaced(layerName))
                            {
                                Debug.Log("    the edge is placed so UP");
                                yield return new WaitForSeconds(2.0f);
                                ApplyRotation(changedWhiteFrontFace[layerIndex]["Up"], true);
                                yield return new WaitForSeconds(2.0f);
                            }
                            ApplyRotation(changedWhiteFrontFace[layerIndex]["Front"], true);
                            yield return new WaitForSeconds(2.0f);
                        }
                        yield return new WaitForSeconds(2.0f);
                    }

                    while (!IsEdgePlaced(layerName) && GetWhiteEdgeCubets(layerName).Count == 1)
                    {
                        ApplyRotation(changedWhiteFrontFace[face.Value]["Front"], true);
                        yield return new WaitForSeconds(2.0f);
                    } // i gotta move this, so that it executes before it moves onto the next face
                }
            }
        }
        yield return null;
    }

    List<GameObject> GetWhiteEdgeCubets(string layerName)
    {
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName);
        List<GameObject> whiteEdgeCubets = new List<GameObject>();

        foreach (var cubet in cubets)
        {
            string cubetName = cubet.name;
            Vector3 cubetPos = cubet.transform.position;
            if (cubetName.Length == 2 && cubetName.Contains('W'))
            {
                whiteEdgeCubets.Add(cubet);
            }
        }

        return whiteEdgeCubets;
    }

    bool IsEdgePlaced(string layerName)
    {
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName);

        foreach (var cubet in cubets)
        {
            string cubetName = cubet.name;
            Vector3 cubetPos = cubet.transform.position;
            Debug.Log("length: " + cubetName.Length + " does contain W: " + cubetName.Contains('W') + "and its in the y: " + (Maths.AbsoluteValue(-1.025f, cubetPos.y) > vecThreshold));
            if (cubetName.Length == 2 && cubetName.Contains('W') && Maths.AbsoluteValue(-1.025f, cubetPos.y) > vecThreshold)
            {
                Debug.Log("PLACED");
                return true;
            }
        }
        Debug.Log("NOT PACED :(");
        return false;
    }

    bool IsAllWhiteEdgesPlaced()
    {
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj("Down");
        List<GameObject> edgeCubets = new List<GameObject>();

        foreach (var cubet in cubets)
        {
            string cubetName = cubet.name;
            if (cubetName.Length == 2 && cubetName.Contains('W'))
            {
                edgeCubets.Add(cubet);
            }
        } // gets all the white edge cubets

        if (edgeCubets.Count != 4)
        {
            return false;
        }
        return true;
    }
    */

    IEnumerator FlipCrossEdges()
    {
        //moving one now just going to make the code for flipping the edges
        foreach (var face in cubeStatesScript.FaceIndexTrans)
        {
            string layerName = face.Key;
            List<int> currentWhiteEdgeIndices = GetEdgeIndices(layerName, 'W');
            if (currentWhiteEdgeIndices.Contains(7))
            {
                if (layerName != "Up" && layerName != "Down")
                {
                    ApplyRotation(changedWhiteFrontFace[face.Value]["Front"], false);
                    ApplyRotation(changedWhiteFrontFace[face.Value]["Right"], false);
                    ApplyRotation(changedWhiteFrontFace[face.Value]["Down"], true);
                    ApplyRotation(changedWhiteFrontFace[face.Value]["Right"], true);
                    ApplyRotation(changedWhiteFrontFace[face.Value]["Front"], true);
                    ApplyRotation(changedWhiteFrontFace[face.Value]["Front"], true);
                }
            }
        }
        yield return null;
    }

    IEnumerator PositionCrossEdges()
    {
        for (int i = 0; i <= 5; i++)
        {
            if (matchingCentres.Count >= 2)
            {
                break; // Exit the loop
            }

            matchingCentres.Clear();

            yield return new WaitForSeconds(2.0f); // Adjust the delay time as needed

            foreach (var face in cubeStatesScript.FaceIndexTrans)
            {
                string layerName = face.Key;
                int layerIndex = face.Value;
                if (layerName != "Up" && layerName != "Down")
                {
                    Color centre = cubeStatesScript.ColourTrans[cubeStatesScript.LayerColour[layerName]];
                    Color currentPanelColour = cubeStatesScript.cubePanels[layerIndex, 7];
                    if (cubeStatesScript.ColourApproximatelyEqual(centre, currentPanelColour))
                    {
                        matchingCentres.Add(layerIndex);
                    }
                }
            }

            if (matchingCentres.Count < 2)
            {
                rotateLayersScript.EnqueueRotation("Down", true, 4.0f);
            }
        }
    }

    IEnumerator OrientCrossEdges()
    {
        if (matchingCentres.Count == 2)
        {
            int layerIndex = -1;

            if ((matchingCentres[0] == 1 && matchingCentres[1] == 4)
                || matchingCentres[0] == 2 && matchingCentres[1] == 3)
            {
                if (matchingCentres.Contains(1)) { layerIndex = 3; }
                else { layerIndex = 1; }

                yield return StartCoroutine(OppositeCross(layerIndex));
            }
            else
            {
                if (matchingCentres.Contains(1) && matchingCentres.Contains(2)) { layerIndex = 2; }
                else if (matchingCentres.Contains(2) && matchingCentres.Contains(4)) { layerIndex = 4; }
                else if (matchingCentres.Contains(3) && matchingCentres.Contains(4)) { layerIndex = 3; }
                else if (matchingCentres.Contains(1) && matchingCentres.Contains(3)) { layerIndex = 1; }
                yield return StartCoroutine(AdjacentCross(layerIndex));
            }
        }
        yield return null;
    }

    IEnumerator OppositeCross(int layerIndex)
    {
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Up"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Up"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], true);
        yield return null;
    }

    IEnumerator AdjacentCross(int layerIndex)
    {
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], false);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Up"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], true);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Up"], false);
        ApplyRotation(changedWhiteFrontFace[layerIndex]["Right"], false);
        yield return null;
    }

    // SOLVING FIRST LAYER ====================================================================================================================
    IEnumerator SolveFirstLayerCorners()
    {
        Debug.Log("Step 2: Solve the Corners of the First Layer");

        yield return StartCoroutine(MoveWhiteCornerCubets());
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(PlaceWhiteCorners());
        yield return new WaitForSeconds(2.0f);

        yield return null;
    }

    IEnumerator MoveWhiteCornerCubets()
    {
        /* remove white tiles from top layer:
         * if its in the top layer then R' D' R
         * loop through each face, get the cubets.
         * filter through, get only corners length=3 and the ones that are in the top layer (y=1.025) and 'W'
         * find out if its on the right, need to use pos:
         * if face == "left" then x=1.025 then move it idk smth like that
         * keep repeating until the list of white corner cubets in the top layer is 0
         */

        while (!CheckDownLayerWhiteCorners())
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans)
            {
                string layerName = face.Key;
                int layerIndex = face.Value;
                if (layerName != "Up" && layerName != "Down")
                {
                    // get the cubets for the current face

                    while (WhiteCornerCubets(layerName) != null)
                    {
                        Debug.Log("in loop");
                        yield return new WaitForSeconds(2.0f);

                        int frontLayerIndex = -1;

                        if (WhiteCornerCubets(layerName) == null) { break; }
                        else
                        {
                            string frontFace = WhiteCornerCubets(layerName);
                            frontLayerIndex = cubeStatesScript.FaceIndexTrans[frontFace];
                        }
                        Debug.Log("APPLYINGGG: ");

                        yield return new WaitForSeconds(1.0f);
                        ApplyRotation(changedWhiteFrontFace[frontLayerIndex]["Right"], false);
                        ApplyRotation(changedWhiteFrontFace[frontLayerIndex]["Down"], false);
                        ApplyRotation(changedWhiteFrontFace[frontLayerIndex]["Right"], true);
                        yield return new WaitForSeconds(2.0f);

                        

                    }
                }
            }
            yield return new WaitForSeconds(2.0f);
        }
        yield return new WaitForSeconds(2.0f);
    }

    private string WhiteCornerCubets(string layerName)
    {
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName);
        string frontFace = null;
        foreach (var cubet in cubets)
        {
            string cubetName = cubet.name;
            Vector3 cubetPos = cubet.transform.position;
            if (cubetName.Length == 3 && cubetName.Contains('W') && Maths.AbsoluteValue(cubetPos.y, -1.025f) > vecThreshold)
            {
                // only if its on the right side of the face
                if (layerName == "Front" && Maths.AbsoluteValue(cubetPos.x, 1.025f)  > vecThreshold) { frontFace = "Front"; Debug.Log("FRONTING"); }
                if (layerName == "Left"  && Maths.AbsoluteValue(cubetPos.z, 1.025f) > vecThreshold) { frontFace = "Left";  Debug.Log("LEFTING"); }
                if (layerName == "Right" && Maths.AbsoluteValue(cubetPos.z, -1.025f)  > vecThreshold) { frontFace = "Right"; Debug.Log("RIGHTING"); }
                if (layerName == "Back"  && Maths.AbsoluteValue(cubetPos.x, -1.025f) > vecThreshold) { frontFace = "Back";  Debug.Log("BACKING"); }

                Debug.Log(cubetName + " vec: " + cubetPos.y + " frontFace: " + frontFace);
            }
        }
        return frontFace;
    }

    private bool CheckDownLayerWhiteCorners()
    {
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj("Down");
        List<GameObject> whiteCornerCubets = new List<GameObject>();
        foreach (var cubet in cubets)
        {
            string cubetName = cubet.name;

            if (cubetName.Length == 3 && cubetName.Contains('W'))
            {
                whiteCornerCubets.Add(cubet);
            }
        }

        if (whiteCornerCubets.Count == 0)
        {
            return true;
        }
        return false;
    }

    IEnumerator PlaceWhiteCorners()
    {
        /* moving them in the correct place:
         * check each face get names of bottom corner peices so check y = -1.025 /
         * get adjacent faces, see if the peice conatains currentFaceState[4] and adjacent face[1] 
         *                    or peice conatains currentFaceState[4] and adjacent face[0]
         
         * need to D until the centres align
         */

        while (!IsFirstLayerSolved())
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans)
            {
                string layerName = face.Key;
                int layerIndex = face.Value;

                if (layerName != "Up" && layerName != "Down")
                {
                    bool cornerFound = false;
                    List<GameObject> cornerCubets = new List<GameObject>(); // list to hold the corner cubets
                    List<int> incorrectIndex = new List<int>();
                    string side = null;

                    while (!cornerFound)
                    {
                        yield return new WaitForSeconds(2.0f);

                        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName);

                        foreach (var cubet in cubets)
                        {
                            string cubetName = cubet.name;
                            Vector3 cubetPos = cubet.transform.position;
                            if (cubetName.Length == 3 && cubetName.Contains('W') && Maths.AbsoluteValue(cubetPos.y, -1.025f) > vecThreshold)
                            {
                                cornerCubets.Add(cubet);
                            }
                        }

                        char layerChar = cubeStatesScript.LayerColour[layerName];
                        char[] adjacentFaces = GetAdjacentFaces(layerChar);
                        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
                        int cubetCounter = 0;

                        foreach (var cubet in cornerCubets)
                        {
                            string cubetName = cubet.name;
                            if (cubetName.Contains(currentFaceState[1]) && cubetName.Contains(adjacentFaces[0])) // left
                            { // if the peice is correct and on the left
                                side = "Left";
                                StartCoroutine(MisorientedCorner(layerName, adjacentFaces[0], side));
                                yield return new WaitForSeconds(2.0f);
                                cornerFound = true;
                            }
                            if (cubetName.Contains(currentFaceState[1]) && cubetName.Contains(adjacentFaces[1])) // right
                            { // if the piece is correct on the right
                                side = "Right";
                                StartCoroutine(MisorientedCorner(layerName, adjacentFaces[1], side));
                                yield return new WaitForSeconds(2.0f);
                                cornerFound = true;
                            }
                            else
                            {
                                ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], true);
                                yield return new WaitForSeconds(1.0f);
                                incorrectIndex.Add(cubetCounter);
                            }
                            cubetCounter++;
                        }
                        yield return new WaitForSeconds(2.0f);
                    }

                    foreach (int index in incorrectIndex)
                    {
                        cornerCubets.Remove(cornerCubets[index]);
                    } // only left with the correct cubet(s)

                    /* if not misoriented, then if left (adj[0]) then D L D' L'
                     * if right (adj[1]) then D' R' D R
                     */

                    if (side == "Left")
                    {
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], true);
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], false);
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], false);
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], true);
                    }
                    else if (side == "Right")
                    {
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], false);
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], true);
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], true);
                        ApplyRotation(changedWhiteFrontFace[layerIndex]["Left"], false);
                    }
                    yield return new WaitForSeconds(2.0f);

                }
            }
        }
        yield return null;
    }

    IEnumerator MisorientedCorner(string layerName, char adjacentSide, string side)
    {
        /* if yes then check if its mis oriented:
         *   get currentFaceState, check (if left at 6) (if right at 8) 
         *   get current faace state of adjacent face, if (6 for current then 8 for adj) etc.
         *   if neither contains 'W' then:
         *       if misoriented piece on left then make adjacent the tempFF
         *       if misoriented piece on right then make FF normal:
         *          F D' F' D2
         */

        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
        string adjacentFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == adjacentSide).Key;
        char[] adjacentFaceState = cubeStatesScript.FaceColourState(adjacentFace);
        string frontFace = null;
        int frontFaceIndex = -1;

        if (side == "Left" && currentFaceState[6] != 'W' && adjacentFaceState[8] != 'W')
        {
            frontFace = adjacentFace;
            frontFaceIndex = cubeStatesScript.FaceIndexTrans[frontFace];
            
        }
        if (side == "Right" && currentFaceState[8] != 'W' && adjacentFaceState[6] != 'W')
        {
            frontFace = layerName;
            frontFaceIndex = cubeStatesScript.FaceIndexTrans[frontFace];
        }
        yield return new WaitForSeconds(2.0f);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Front"], true);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Down"], false);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Front"], false);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Down"], true);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Down"], true);
        yield return new WaitForSeconds(2.0f);

        yield return null;
    }

    private bool IsFirstLayerSolved()
    {
        foreach (var face in cubeStatesScript.FaceIndexTrans)
        {
            string layerName = face.Key;
            char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
            if (currentFaceState[0] != currentFaceState[1] || currentFaceState[1] != currentFaceState[2])
            {
                return false;
            }
        }
        return true;
    }

    // SOLVING SECOND LAYER ====================================================================================================================

    IEnumerator SolveSecondLayer()
    {
        Debug.Log("Step 3: Solve the Second Layer");

        while (!IsSecondLayerSolved()) 
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans)
            {
                string layerName = face.Key;
                int layerIndex = face.Value;

                if (layerName != "Up" && layerName != "Down")
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (!IsSecondLayerSolved())
                        {
                            char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);

                            if (currentFaceState[1] == currentFaceState[4])
                            {
                                yield return StartCoroutine(MovingSecondLayerPeices(layerName, layerIndex));
                            }
                            ApplyRotation(changedFrontFace[1]["Up"], true);
                            yield return new WaitForSeconds(1.0f);
                        }
                        else { break; }
                    }
                    yield return StartCoroutine(ApplyMisplacedEdge(layerName, layerIndex));
                }
            }
        }

        yield return null;
    }

    IEnumerator MovingSecondLayerPeices(string layerName, int layerIndex)
    {
        Color panelColour = cubeStatesScript.cubePanels[cubeStatesScript.FaceIndexTrans["Up"], otherEdgeIndex[layerName][3]];
        char panelColourChar = cubeStatesScript.GetColour(panelColour);
        char[] adjacentFaces = GetAdjacentFaces(cubeStatesScript.LayerColour[layerName]);
        if (panelColourChar == adjacentFaces[0]) //left
        {
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Left"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Left"], false);

            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], false);

        }
        else if (panelColourChar == adjacentFaces[1]) //right
        {
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);

            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
        }
        yield return new WaitForSeconds(2.0f);
    }

    bool IsSecondLayerSolved()
    {
        foreach (var face in cubeStatesScript.FaceIndexTrans)
        {
            string layerName = face.Key;
            char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
            if (layerName != "Up")
            {
                if ((currentFaceState[3] != currentFaceState[4]) || (currentFaceState[4] != currentFaceState[5]))
                {
                    return false;
                }
            }
        }
        return true;
    }
    
    IEnumerator ApplyMisplacedEdge(string layerName, int layerIndex)
    {
        /* get all the cubets in the top layer
         * get the names length == 2 so edges add onto list
         * get the name for each of them
         * and if they all contain Y then execute Misplaced Edges
         */

        List<GameObject> cubetGameObjs = rotateLayersScript.GetCubetGameObj("Up");
        List<GameObject> edgeCubets = new List<GameObject>();
        foreach (var cubet in cubetGameObjs)
        {
            string cubetName = cubet.name;
            if (cubetName.Length == 2 && cubetName.Contains('Y'))
            {
                edgeCubets.Add(cubet);
            }
        }

        if (edgeCubets.Count == 4)
        {
            yield return StartCoroutine(MisplacedEgdeCheck(layerName, layerIndex));
        }

        yield return null;
    }

    IEnumerator MisplacedEgdeCheck(string layerName, int layerIndex) 
    {
        List<GameObject> cubetGameObjs = rotateLayersScript.GetCubetGameObj(layerName);
        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
        List<GameObject> edgeCubets = new List<GameObject>();
        foreach (var cubet in cubetGameObjs)
        {
            string cubetName = cubet.name;
            Vector3 cubetPos = cubet.transform.position;
            if (Maths.AbsoluteValue(cubetPos.y, 0.0f) >  vecThreshold && cubetName.Length == 2) { edgeCubets.Add(cubet); }
        }

        foreach (var cubet in edgeCubets)
        {
            string cubetName = cubet.name;
            if (!(cubetName.Contains('Y')))
            {
                if (currentFaceState[3] != currentFaceState[4]) // LEFT
                {
                    ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                    ApplyRotation(changedFrontFace[layerIndex]["Left"], true);
                    ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                    ApplyRotation(changedFrontFace[layerIndex]["Left"], false);

                    ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                    ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
                    ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                    ApplyRotation(changedFrontFace[layerIndex]["Front"], false);
                }
                if (currentFaceState[5] != currentFaceState[4]) //RIGHT 
                {
                    ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                    ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
                    ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                    ApplyRotation(changedFrontFace[layerIndex]["Right"], true);

                    ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                    ApplyRotation(changedFrontFace[layerIndex]["Front"], false);
                    ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                    ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
                }
            }
        }
        yield return null;
    }

    // SOLVING Y CROSS ====================================================================================================================

    IEnumerator SolveYellowCross()
    {
        Debug.Log("Step 4: Solve the Yellow Cross");
        List<int> yellowEdgeIndices = new List<int>();
        string frontFace = "";
        while (yellowEdgeIndices.Count != 4)
        {
            foreach (var layer in otherEdgeIndex)
            {
                string layerName = layer.Key;
                List<int> layerEdgeIndices = layer.Value;
                if (yellowEdgeIndices.Count == 0) { frontFace = "Front"; break; }
                else if (yellowEdgeIndices.Contains(1) && yellowEdgeIndices.Contains(7)) { frontFace = "Left"; break; }
                else if (yellowEdgeIndices.Contains(3) && yellowEdgeIndices.Contains(5)) { frontFace = "Right"; break; }
                else if (yellowEdgeIndices.Contains(layerEdgeIndices[0])
                    && yellowEdgeIndices.Contains(layerEdgeIndices[1])) { frontFace = layerName; break; }
            }

            int layerIndex = cubeStatesScript.FaceIndexTrans[frontFace];
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);

            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], false);
            yield return new WaitForSeconds(2.0f);
            yellowEdgeIndices = GetEdgeIndices("Up", 'Y');
        }
        yield return null;
    }

    // ORIENTING Y CORNERS ====================================================================================================================

    IEnumerator OrientYellowCorners()
    {
        Debug.Log("Step 5: Orient the Yellow Corners");
        /*
         * find fish
         * get currentFaceState
         * how many not yellow places are there?
         * case (4) => find the face with a yellow tile in index [2]
         * case (3) => fish 
         *  Y Y *     * Y Y     * Y *     * Y *
         *  Y Y Y     Y Y Y     Y Y Y     Y Y Y
         *  * Y *     * Y *     * Y Y     Y Y *
         *  0134      1245       4578     3467
         *    3         1         5         7
         * case (2) => find the face that has a yellow tile index [0] 
         */

        while (!IsYellowFaceOriented())
        {
            Debug.Log("HELOOOOOO");
            char[] currentFaceState = cubeStatesScript.FaceColourState("Up");
            int unYellowCounter = 0;
            int panelI = 0;
            string frontFace = "";
            int layerIndex = -1;
            List<int> yellowIndex = new List<int>();

            foreach (char panel in currentFaceState)
            {
                if (panel != 'Y') { unYellowCounter++; }
                else { yellowIndex.Add(panelI); }
                panelI++;
            }
            yield return new WaitForSeconds(2.0f);

            if (unYellowCounter == 4)
            {
                string adjacentFace = FindFaceWithPanel("Right"); 
                char adjacentColour = cubeStatesScript.LayerColour[adjacentFace];
                char rightFace = GetAdjacentFaces(adjacentColour)[1];
                frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == rightFace).Key;
            }
            if (unYellowCounter == 3)
            {
                //fish
                if (yellowIndex.Contains(0) && yellowIndex.Contains(1) && yellowIndex.Contains(3) && yellowIndex.Contains(4))
                {
                    frontFace = "Left";
                }
                if (yellowIndex.Contains(1) && yellowIndex.Contains(2) && yellowIndex.Contains(4) && yellowIndex.Contains(5))
                {
                    frontFace = "Back";
                }
                if (yellowIndex.Contains(4) && yellowIndex.Contains(5) && yellowIndex.Contains(7) && yellowIndex.Contains(8))
                {
                    frontFace = "Right";
                }
                if (yellowIndex.Contains(3) && yellowIndex.Contains(4) && yellowIndex.Contains(6) && yellowIndex.Contains(7))
                {
                    frontFace = "Front";
                }
            }
            if (unYellowCounter < 3)
            {
                frontFace = FindFaceWithPanel("Left");
            }

            layerIndex = cubeStatesScript.FaceIndexTrans[frontFace];
            
            yield return new WaitForSeconds(1.0f);

            // apply rotations

            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);
            yield return new WaitForSeconds(3.0f);
        }

        yield return null;
    }

    string FindFaceWithPanel(string side)
    {
        foreach(var face in cubeStatesScript.FaceIndexTrans)
        {
            //Debug.Log("finding...");
            string layerName = face.Key;
            if (layerName != "Up" && layerName != "Down")
            {
                char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
                
                if (side == "Left" && currentFaceState[0] == 'Y')
                {
                    return layerName;
                }
                if (side == "Right" && currentFaceState[2] == 'Y')
                {
                    return layerName;
                }
            }
        }
        return null;
    }

    bool IsYellowFaceOriented()
    {
        char[] currentFaceState = cubeStatesScript.FaceColourState("Up");
        foreach (char panel in currentFaceState)
        {
            if (panel != 'Y')
            {
                return false;
            }
        }
        return true;
    }

    // PERMUTE Y CORNERS ====================================================================================================================

    IEnumerator PermuteYellowCorners()
    {
        Debug.Log("Step 6: Permute the Yellow Corners");
        /* read every face, find the face that has the same colour index [0] and [2]
         * the opposite face is the front face, maybe i could use the otherEdgeIndex dict
         * if not, then make back face the front face.
         */

        while (!IsYellowCornersSolved())
        {
            int frontFaceIndex = -1;
            string frontFace = null;

            foreach (var face in cubeStatesScript.FaceIndexTrans)
            {
                string layerName = face.Key;
                int layerIndex = face.Value;
                char faceColour = cubeStatesScript.LayerColour[layerName];

                yield return new WaitForSeconds(2.0f);

                if (layerName != "Up" && layerName != "Down")
                {
                    char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
                    Debug.Log("checking: " + layerName + " current face state: " + string.Join(',', currentFaceState));

                    if (currentFaceState[0] == faceColour && currentFaceState[2] == faceColour) // the correct back face
                    {
                        Debug.Log("in the right place");
                        char layerColour = cubeStatesScript.LayerColour[layerName]; // converts the layerName to char equivilant
                        Debug.Log("Layercolour: " + layerColour);
                        char oppositeFace = GetOppositeFace(layerColour); // get the char val for the opposite face
                        Debug.Log("opposite one: " + oppositeFace);
                        frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == oppositeFace).Key; // gets the string of the opposite face
                    }
                    else if (currentFaceState[0] == currentFaceState[2]) // you find the two that are supposed to be for your back face
                    {
                        /* get the adjacent faces for faceColour
                         * if [0] up true
                         * if [1] up false
                         * else 2U
                         */
                        Debug.Log("trying to make it right");
                        char frontChar;
                        char[] adjacentFaces = GetAdjacentFaces(faceColour);
                        if (adjacentFaces[0] == currentFaceState[0])
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            frontChar = adjacentFaces[0];
                        }
                        else if (adjacentFaces[1] == currentFaceState[0])
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                            frontChar = adjacentFaces[1];
                        }
                        else
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            frontChar = GetOppositeFace(faceColour);
                        }
                        frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == frontChar).Key;
                    }
                    else if (currentFaceState[0] == 'G' && currentFaceState[2] == 'B'
                        || currentFaceState[2] == 'G' && currentFaceState[0] == 'B')
                    {
                        if (layerName == "Front")
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                        }
                        else if (layerName == "Left")
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                        }
                        else if (layerName == "Right")
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                        }

                        frontFace = "Back"; 
                        Debug.Log("AHHH just make it blue**"); 
                    }
                    if (frontFace != null)
                    {
                        Debug.Log("FrontFace: " + frontFace);
                        break;
                    }
                }
            }

            frontFaceIndex = cubeStatesScript.FaceIndexTrans[frontFace];
            yield return new WaitForSeconds(2.0f);
            // apply algorithm

            ApplyRotation(changedFrontFace[frontFaceIndex]["Right"], true);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Front"], true);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Right"], true);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Back"], true);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Back"], true);

            ApplyRotation(changedFrontFace[frontFaceIndex]["Right"], false);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Front"], false);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Right"], true);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Back"], true);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Back"], true);


            ApplyRotation(changedFrontFace[frontFaceIndex]["Right"], false);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Right"], false);
            ApplyRotation(changedFrontFace[frontFaceIndex]["Up"], false);

            yield return new WaitForSeconds(5.0f);
        }

        // if need be, fix orientaion of the top layer to match centres
        char[] faceState = cubeStatesScript.FaceColourState("Front");
        StartCoroutine(fixTopLayerOrientation(faceState, "Front", 1));

        yield return null;
    }

    IEnumerator fixTopLayerOrientation(char[] currentFaceState, string layerName, int layerIndex)
    {
        char faceColour = cubeStatesScript.LayerColour[layerName];
        char[] adjacentFaces = GetAdjacentFaces(faceColour);
        if (adjacentFaces[0] == currentFaceState[0])
        {
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
        }
        else if (adjacentFaces[1] == currentFaceState[0])
        {
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
        }
        else if (GetOppositeFace(faceColour) == currentFaceState[0])
        {
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
        }
        else if (faceColour == currentFaceState[0])
        {
            Debug.Log("Correct Face");
        }
        yield return new WaitForSeconds(2.0f);
    }

    bool IsYellowCornersSolved()
    {
        foreach (var face in cubeStatesScript.FaceIndexTrans)
        {
            string layerName = face.Key;
            if (layerName != "Up" && layerName != "Down")
            {
                char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);
                if (currentFaceState[0] != currentFaceState[2])
                {
                    return false;
                } // if the two top corners are not equal
            }
        }
        return true;
    }

    // POS Y EDGES ====================================================================================================================

    IEnumerator PositionYellowEdges()
    {
        Debug.Log("Step 7: Position Yellow Edges");

        /* find the solved face
         * make the opposite the front face
         * repeat until solved
         */

        
        while (!cubeStatesScript.IsCubeSolved())
        {
            yield return new WaitForSeconds(2.0f);
            StartCoroutine(fixTopLayerOrientation(cubeStatesScript.FaceColourState("Front"), "Front", 1));

            string frontFace = null;
            int layerIndex = -1;
        
            string backFace = FindSolvedFace();
            Debug.Log("back face: " + backFace);

            yield return new WaitForSeconds(2.0f);

            if (backFace != null)
            {
                // find opposite face
                char charBackFace = cubeStatesScript.LayerColour[backFace];
                Debug.Log("back colour: " + charBackFace);
                char charFrontFace = GetOppositeFace(charBackFace);
                Debug.Log("front colour: " + charFrontFace);
                frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == charFrontFace).Key;
                Debug.Log("front face: " + frontFace);
                layerIndex = cubeStatesScript.FaceIndexTrans[frontFace];
            }
            else
            {
                layerIndex = 1; 
            }

            Debug.Log(layerIndex);

            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Left"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);

            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Left"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            yield return new WaitForSeconds(5.0f);

        }
        yield return null;
    }

    private string FindSolvedFace()
    {
        foreach (var face in cubeStatesScript.FaceIndexTrans)
        {
            string layerName = face.Key;
            if (layerName != "Up" && layerName != "Down")
            {
                char[] currentFaceState = cubeStatesScript.FaceColourState(layerName);

                // Check if all panels are the same color or the same color as the center panel
                bool solved = true;
                for (int i = 0; i < currentFaceState.Length; i++)
                {
                    if (currentFaceState[i] != currentFaceState[4])
                    {
                        solved = false;
                        break;
                    }
                }

                if (solved)
                {
                    return layerName;
                }
            }
        }
        return null; // No solved face found
    }
}
