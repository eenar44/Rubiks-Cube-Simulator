using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BeginnersSolve : BaseSolver // inheriting from BaseSolver
{
    public override IEnumerator Solve()
    {
        /* method that is being overwritten in the BaseSolver script to execute the beginners coroutine */
        yield return StartCoroutine(Beginners()); // executes the Beginners coroutine
    }

    IEnumerator Beginners()
    {
        /* executes the appropriate coroutines in the correct order */
        yield return StartCoroutine(SolveCross());
        yield return StartCoroutine(SolveFirstLayerCorners());
        yield return StartCoroutine(SolveSecondLayer());
        yield return StartCoroutine(SolveYellowCross());
        yield return StartCoroutine(OrientYellowCorners());
        yield return StartCoroutine(PermuteYellowCorners());
        yield return StartCoroutine(PositionYellowEdges());

        AddToMoveList(moveCounter); // adds to the numberOfMoves list when solve is complete

        yield return null;
    }

    // SOLVING CROSS ==========================================================================================================

    IEnumerator SolveCross()
    {
        /* coroutine that executes the steps in solving the cross in order */
        yield return StartCoroutine(PlaceWhiteEdges());
        yield return new WaitForSeconds(2.0f); // waits for 2 seconds for the execution to complete

        yield return StartCoroutine(FlipCrossEdges());
        yield return new WaitForSeconds(2.0f);

        matchingCentres.Clear(); // clears the matching centres list so it can be used
        yield return StartCoroutine(PositionCrossEdges());
        yield return new WaitForSeconds(3.0f);

        yield return StartCoroutine(OrientCrossEdges());
        yield return new WaitForSeconds(2.0f);

        yield return null;
    }

    IEnumerator PlaceWhiteEdges()
    {
        /* coroutine that places the white edges into the bottom layer */
        while (!IsAllWhiteEdgesPlaced()) // loops while not all the white edges are in plave
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through each face
            {
                string layerName = face.Key; // gets the layer name 
                int layerIndex = face.Value; // gets the layer index
                if (layerName != "Up" && layerName != "Down") // does not consider the Up or Down face
                {
                    while (GetWhiteEdgeCubets(layerName).Count != 1) // loops white there isnt only one white edge cubet
                    {
                        yield return new WaitForSeconds(2.0f); // waits to slow down the execution of thw while loop

                        if (GetWhiteEdgeCubets(layerName).Count == 0) // if there are no white edges on the face
                        {
                            List<GameObject> upWhiteEdges = GetWhiteEdgeCubets("Up"); // gets a list of white edges in the up face
                            if (upWhiteEdges.Count > 0) // if there are white edges in the top layer
                            {
                                while (GetWhiteEdgeCubets(layerName).Count != 1) // loops while the number of edge cubets on the face is not 1
                                {
                                    yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                                    ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], true); // move the down layer
                                    yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                                }
                            }
                        }
                        if (GetWhiteEdgeCubets(layerName).Count > 1) // if there are more than one white edge on the layer
                        {
                            // "dispenses" the cubets into the bottom layer
                            while (!IsEdgePlaced(layerName)) // loops while there is no edge in the correct place
                            {
                                yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                                ApplyRotation(changedWhiteFrontFace[layerIndex]["Up"], true); // moves the up layer
                                yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                            }
                            ApplyRotation(changedWhiteFrontFace[layerIndex]["Front"], true); // moves the front face
                            yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                        }
                        yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                    }

                    while (!IsEdgePlaced(layerName) && GetWhiteEdgeCubets(layerName).Count == 1) // if the edge is not in the correct place and there is only one
                    {
                        ApplyRotation(changedWhiteFrontFace[face.Value]["Front"], true); // moves the front face
                        yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                    }
                }
            }
        }
        yield return null;
    }

    List<GameObject> GetWhiteEdgeCubets(string layerName)
    {
        /* function that returns a list of game objects that containt he colour white and are edge peices */
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName); // gets the list of cubets for that face
        List<GameObject> whiteEdgeCubets = new List<GameObject>(); // initalises a new list that will hold the white edge cubets

        foreach (var cubet in cubets) // loops through each cubet in the face
        {
            string cubetName = cubet.name; // gets the cubet's name
            /* all cubets are named so tht it contains the colours it has, 
			 * eg. a white-orange edge peice will be called 'WO' 
			 *     or a white-red-blue edge peice may be called 'WRB'
			 * all edge peice names have a length of 2 (as it contains 2 colours) so all corner peices have a length of 2 and centre length of 1
			 */
            Vector3 cubetPos = cubet.transform.position; // gets the cubet's transform
            if (cubetName.Length == 2 && cubetName.Contains('W')) // gets the edge peices that contain 'W'
            {
                whiteEdgeCubets.Add(cubet); // adds onto the list
            }
        }
        return whiteEdgeCubets; // returns the list
    }

    bool IsEdgePlaced(string layerName)
    {
        /* method that will check if the edge in the layer is in the correct place */
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName); // gets all the cubets in that layer

        foreach (var cubet in cubets) // loops through each cubet in that layer
        {
            string cubetName = cubet.name; // gets the name of the cubet
            Vector3 cubetPos = cubet.transform.position; // gets the position of the cubet
                                                         // if the peice is an edge peice, contains 'W' and is in the bottom layer
            if (cubetName.Length == 2 && cubetName.Contains('W') && Maths.AbsoluteValue(-1.025f, cubetPos.y) > vecThreshold)
            {
                return true; // return true, for placed
            }
        }
        return false; // if not, its not placed
    }

    bool IsAllWhiteEdgesPlaced()
    {
        /* method that checks if all white edges are placed */
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj("Down"); // gets all the cubets in the bottom layer 
        List<GameObject> edgeCubets = new List<GameObject>(); // initilised a new list that will hold all the edge cubets

        foreach (var cubet in cubets) // loops through each cubet in the layer
        {
            string cubetName = cubet.name; // gets the name of the cubet
                                           // if the cubet is an edge and contains 'W'
            if (cubetName.Length == 2 && cubetName.Contains('W'))
            {
                edgeCubets.Add(cubet); // add onto the list
            }
        } // gets all the white edge cubets

        if (edgeCubets.Count != 4) // if all the edge cubets contain white are not in the bottom layer
        {
            return false;
        }
        return true;
    }

    IEnumerator FlipCrossEdges()
    {
        /* coroutine that flips the edges to make the white cross */
        foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through each adjacent face
        {
            string layerName = face.Key; // gets the name of that layer
                                         // gets a list of indeces at which a white edge is placed
            List<int> currentWhiteEdgeIndices = GetEdgeIndices(layerName, 'W');
            if (currentWhiteEdgeIndices.Contains(7)) // if there is a white edge at the correct index
            {
                if (layerName != "Up" && layerName != "Down") // and if the layer is neither up nor down
                {
                    // apply appropriate algorithm
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
        /* mvoes the down layer until there are two faces that have a matching cnetre and edge piece in the down layer */
        for (int i = 0; i <= 5; i++) // loops through 0 to 5
        {
            if (matchingCentres.Count >= 2) // checks if there are more than 2 matching centres
            {
                break; // exit the loop
            }

            matchingCentres.Clear(); // empties the list

            yield return new WaitForSeconds(2.0f); // delay so that the code can finsih executing

            foreach (var face in cubeStatesScript.FaceIndexTrans) // loops thruogh each adjacent face
            {
                string layerName = face.Key; // gets the name of the layer
                int layerIndex = face.Value; // gets the layer index
                if (layerName != "Up" && layerName != "Down") // if the layer is neither up nor down
                {
                    // gets the colour of the centre peice on the face, by indexing at 4
                    Color centre = cubeStatesScript.ColourTrans[cubeStatesScript.LayerColour[layerName]];
                    // gets the colour of the panel at index 7
                    Color currentPanelColour = cubeStatesScript.cubePanels[layerIndex, 7];
                    // checks if the colour at the centre is the same as the the panels colour
                    if (cubeStatesScript.ColourApproximatelyEqual(centre, currentPanelColour))
                    {
                        matchingCentres.Add(layerIndex); // adds the index onto the list of matching centres
                    }
                }
            }

            if (matchingCentres.Count < 2) // if there are less that two matching centres
            {
                rotateLayersScript.EnqueueRotation("Down", true, 4.0f); // rotates the down layer
            }
        }
    }

    IEnumerator OrientCrossEdges()
    {
        /* coroutine that orients the flipped edge peices */
        if (matchingCentres.Count == 2) // if there are two matching edges
        {
            int layerIndex = -1; // initialises layerIndex to -1

            if ((matchingCentres[0] == 1 && matchingCentres[1] == 4)
                || matchingCentres[0] == 2 && matchingCentres[1] == 3) // if the matching centres are opposite each other
            {
                // finds the correct front face index according to which side had opposite edges matching 
                if (matchingCentres.Contains(1)) { layerIndex = 3; }
                else { layerIndex = 1; }

                yield return StartCoroutine(OppositeCross(layerIndex)); // applies algorithm for opposite cross
            }
            else
            {
                // assume it is an adjacent matching edges, so assigns the appropriate front face layerIndex
                if (matchingCentres.Contains(1) && matchingCentres.Contains(2)) { layerIndex = 2; }
                else if (matchingCentres.Contains(2) && matchingCentres.Contains(4)) { layerIndex = 4; }
                else if (matchingCentres.Contains(3) && matchingCentres.Contains(4)) { layerIndex = 3; }
                else if (matchingCentres.Contains(1) && matchingCentres.Contains(3)) { layerIndex = 1; }
                // apply algorithm for adjacent cross
                yield return StartCoroutine(AdjacentCross(layerIndex));
            }
        }
        yield return null;
    }

    IEnumerator OppositeCross(int layerIndex)
    {
        /* coroutine for opposite cross matching centres algorithm */
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
        /* coroutine for adjacent cross matchinig centres algorithm */
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
        yield return StartCoroutine(MoveWhiteCornerCubets());
        yield return new WaitForSeconds(2.0f);
        yield return StartCoroutine(PlaceWhiteCorners());
        yield return new WaitForSeconds(2.0f);

        yield return null;
    }

    IEnumerator MoveWhiteCornerCubets()
    {
        /* moves the white corners into place on the white face */

        while (!CheckDownLayerWhiteCorners()) // checks the white corners on the down alyer
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through every face 
            {
                string layerName = face.Key; // gets the layer name
                int layerIndex = face.Value; // gets the layer index
                if (layerName != "Up" && layerName != "Down") // fi the layer name is neither up nor down
                {
                    // get the cubets for the current face

                    while (WhiteCornerCubets(layerName) != null)
                    {
                        yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop

                        int frontLayerIndex = -1; // initalises frontLayerIndex

                        if (WhiteCornerCubets(layerName) == null) { break; } // if there are no misplaces white corner cubets
                        else
                        {
                            string frontFace = WhiteCornerCubets(layerName); // get the name of the face with the misplaced corner cubets
                            frontLayerIndex = cubeStatesScript.FaceIndexTrans[frontFace]; // gets the corresponding index
                        }

                        yield return new WaitForSeconds(1.0f); // wait to slow down the execution of while loop
                                                               // apply algorithm
                        ApplyRotation(changedWhiteFrontFace[frontLayerIndex]["Right"], false);
                        ApplyRotation(changedWhiteFrontFace[frontLayerIndex]["Down"], false);
                        ApplyRotation(changedWhiteFrontFace[frontLayerIndex]["Right"], true);
                        yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
                    }
                }
            }
            yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
        }
        yield return new WaitForSeconds(2.0f);
    }

    private string WhiteCornerCubets(string layerName)
    {
        /* function that returns the name fo the front face that has a misplaced white corner pice in the right top corner of the face */
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName); // gets all the cubets for that layer
        string frontFace = null; // initalise front face
        foreach (var cubet in cubets) // loops through every cubet in the layer
        {
            string cubetName = cubet.name; // gets the name of the cubet
            Vector3 cubetPos = cubet.transform.position; // gets the transform of the cubet
                                                         // checks if it is a corner peice, contains 'W' and is in the top layer
            if (cubetName.Length == 3 && cubetName.Contains('W') && Maths.AbsoluteValue(cubetPos.y, -1.025f) > vecThreshold)
            {
                // only returns if its on the right side of the face
                if (layerName == "Front" && Maths.AbsoluteValue(cubetPos.x, 1.025f) > vecThreshold) { frontFace = "Front"; }
                if (layerName == "Left" && Maths.AbsoluteValue(cubetPos.z, 1.025f) > vecThreshold) { frontFace = "Left"; }
                if (layerName == "Right" && Maths.AbsoluteValue(cubetPos.z, -1.025f) > vecThreshold) { frontFace = "Right"; }
                if (layerName == "Back" && Maths.AbsoluteValue(cubetPos.x, -1.025f) > vecThreshold) { frontFace = "Back"; }
            }
        }
        return frontFace; // returns the front face
    }

    private bool CheckDownLayerWhiteCorners()
    {
        /* checks if all the white corner cubets are in the up layer for correct placement */
        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj("Down"); // gets all the cubets in the down layer
        List<GameObject> whiteCornerCubets = new List<GameObject>(); // initalises the list that will hold the white corner cubets
        foreach (var cubet in cubets) // loops through each cubet
        {
            string cubetName = cubet.name; // gets the name of the cubet
                                           // if its a corner and contains 'W'
            if (cubetName.Length == 3 && cubetName.Contains('W'))
            {
                whiteCornerCubets.Add(cubet); // add onto the cubet list
            }
        }

        if (whiteCornerCubets.Count == 0)
        {
            return true; // if there are no cubets in the down layer
        }
        return false;
    }

    IEnumerator PlaceWhiteCorners()
    {
        /* coroutine that places the white corner cubets in the correct place */

        while (!IsFirstLayerSolved()) // loops until the first layer is not solved
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans) // loops for every adjacent face`
            {
                string layerName = face.Key; // gets the layerName
                int layerIndex = face.Value; // gets the layerIndex`

                if (layerName != "Up" && layerName != "Down") // if the layer name is neither up nor down
                {
                    bool cornerFound = false; // set the bool value of the correct corner found to be false
                    List<GameObject> cornerCubets = new List<GameObject>(); // list to hold the corner cubets
                    List<int> incorrectIndex = new List<int>(); // list to hold the indecies of misplaced peices
                    string side = null; // initalising side to null

                    while (!cornerFound) // loops while the correct corner is not found
                    {
                        yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop

                        List<GameObject> cubets = rotateLayersScript.GetCubetGameObj(layerName); // gets the cubets for that layer

                        foreach (var cubet in cubets) // loops through each cubet
                        {
                            string cubetName = cubet.name; // gets the name for each cubet
                            Vector3 cubetPos = cubet.transform.position; // gets the position fo each cubet
                                                                         // if the cubet is a corner peice, contains 'W' and is in the bottom layer
                            if (cubetName.Length == 3 && cubetName.Contains('W') && Maths.AbsoluteValue(cubetPos.y, -1.025f) > vecThreshold)
                            {
                                cornerCubets.Add(cubet); // add onto the corner layer list
                            }
                        }

                        char layerChar = cubeStatesScript.LayerColour[layerName]; // get the corresponding colour for the layer name
                        char[] adjacentFaces = GetAdjacentFaces(layerChar); // get the adjacent colours for that layer
                        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // get the current face state for the layer
                        int cubetCounter = 0; // initialise cubetCounter

                        foreach (var cubet in cornerCubets) // loop through each corner cubet
                        {
                            string cubetName = cubet.name; // get the cubets name
                                                           // checks if the cubetName contains the colour on the left of the current face
                            if (cubetName.Contains(currentFaceState[1]) && cubetName.Contains(adjacentFaces[0])) // left
                            {
                                side = "Left"; // assgin side to left
                                StartCoroutine(MisorientedCorner(layerName, adjacentFaces[0], side)); // apply algorithm
                                yield return new WaitForSeconds(2.0f); // wait for algorithm to finsih applying
                                cornerFound = true; // assign cornerFound to true
                            }
                            // checks if the cubetName contains the colour on the right of the current face
                            if (cubetName.Contains(currentFaceState[1]) && cubetName.Contains(adjacentFaces[1])) // right
                            {
                                side = "Right"; // assign side to right
                                StartCoroutine(MisorientedCorner(layerName, adjacentFaces[1], side)); // apply algorithm
                                yield return new WaitForSeconds(2.0f); // wait for algorithm to finsih applying 
                                cornerFound = true; // assign cornerFound to true
                            }
                            else
                            {
                                ApplyRotation(changedWhiteFrontFace[layerIndex]["Down"], true); // apply down roataion
                                yield return new WaitForSeconds(1.0f); // wait for roation to finsih executing
                                incorrectIndex.Add(cubetCounter); // add index onto incorrect index
                            }
                            cubetCounter++; // increments incorrect index
                        }
                        yield return new WaitForSeconds(2.0f);  // wait to slow down the execution of while loop
                    }

                    foreach (int index in incorrectIndex) // loops through the incorrect indexes
                    {
                        cornerCubets.Remove(cornerCubets[index]); // remove the corner cubets at the incorrect indecies
                    } // only left with the correct cubet(s)

                    // apply appropriate algorithm for respective side
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
                    yield return new WaitForSeconds(2.0f);  // wait to slow down the execution of while loop
                }
            }
        }
        yield return null;
    }

    IEnumerator MisorientedCorner(string layerName, char adjacentSide, string side)
    {
        /* orients a misoriented corner */

        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the current face state
                                                                               // gets the name of the adjacent face
        string adjacentFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == adjacentSide).Key;
        // gets the adjacent face state 
        char[] adjacentFaceState = cubeStatesScript.FaceColourState(adjacentFace);
        string frontFace = null; // initalises the front face to be null
        int frontFaceIndex = -1; // initalises frontFaceIndex to be null value

        // checks if the corner is on the right side of the face and assigns appropriate front face and index for the correct front face
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

        yield return new WaitForSeconds(2.0f); // wait for code to finsih executing
                                               // apply algorithm
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Front"], true);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Down"], false);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Front"], false);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Down"], true);
        ApplyRotation(changedWhiteFrontFace[frontFaceIndex]["Down"], true);
        yield return new WaitForSeconds(2.0f); // wait for algorithm to finish executing

        yield return null;
    }

    private bool IsFirstLayerSolved()
    {
        /* funciton that will return a bool if the first layer is solved */
        foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through each face
        {
            string layerName = face.Key; // gets the name of the layer
            char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the currentFaceState
            if (currentFaceState[0] != currentFaceState[1] || currentFaceState[1] != currentFaceState[2])
            {
                // checks the indeces of the current face state where it shoulf be the same
                // if not the same then return false
                return false;
            }
        }
        return true;
    }

    // SOLVING SECOND LAYER ====================================================================================================================

    IEnumerator SolveSecondLayer()
    {
        while (!IsSecondLayerSolved()) // loops while the second layer is not solved
        {
            foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through every face
            {
                string layerName = face.Key; // gets the name of the face
                int layerIndex = face.Value; // gets the layer index of the face

                if (layerName != "Up" && layerName != "Down") // if the layer name is neither up nor down
                {
                    for (int i = 0; i < 4; i++) // loops between 0 and 4
                    {
                        if (!IsSecondLayerSolved()) // checks again if the second layer is solved 
                        {
                            char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the curent face state

                            if (currentFaceState[1] == currentFaceState[4]) // checks if the panel direclty above the centre is the same colour
                            {
                                yield return StartCoroutine(MovingSecondLayerPeices(layerName, layerIndex)); // mvoe the layer peice 
                            }
                            ApplyRotation(changedFrontFace[1]["Up"], true); // rotate the up layer if not
                            yield return new WaitForSeconds(1.0f); // wait to slow down the execution of while loop
                        }
                        else { break; }
                    }
                    yield return StartCoroutine(ApplyMisplacedEdge(layerName, layerIndex)); // check for misplaces edges in the seond layer
                }
            }
        }
        yield return null;
    }

    IEnumerator MovingSecondLayerPeices(string layerName, int layerIndex)
    {
        /* coroutine that moves edge peices out of the top layer into the correct plave on the second layer */
        // gets the other colour of the panel that is going to be moved
        Color panelColour = cubeStatesScript.cubePanels[cubeStatesScript.FaceIndexTrans["Up"], otherEdgeIndex[layerName][3]];
        char panelColourChar = cubeStatesScript.GetColour(panelColour); // gets the char value of the colour
        char[] adjacentFaces = GetAdjacentFaces(cubeStatesScript.LayerColour[layerName]); // gets the adjacent faces to the current face

        if (panelColourChar == adjacentFaces[0]) // if its on the left
        {
            // apply rotation
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Left"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Left"], false);

            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], false);

        }
        else if (panelColourChar == adjacentFaces[1]) // if its on the right
        {
            // apply rotation
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);

            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
        }
        yield return new WaitForSeconds(2.0f); // wait for execution to finish
    }

    bool IsSecondLayerSolved()
    {
        /* returns bool if the second layer is solved */
        foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through every face
        {
            string layerName = face.Key; // gets the name of the face
            char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the current face state of the layer
            if (layerName != "Up") // if the layer name is not up
            {
                // if the two panels on the side of the centres is not equal to the cnetre pieces
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
        /* coroutine that will check if there is a misplaced edge */
        /* get all the cubets in the top layer
         * get the names length == 2 so edges add onto list
         * get the name for each of them
         * and if they all contain Y then execute Misplaced Edges
         */

        List<GameObject> cubetGameObjs = rotateLayersScript.GetCubetGameObj("Up"); // get all the cubets in the top layer
        List<GameObject> edgeCubets = new List<GameObject>(); // initialise list that will hold edge cubets
        foreach (var cubet in cubetGameObjs) // loop through each cubet
        {
            string cubetName = cubet.name; // gets the name of the cubet
            if (cubetName.Length == 2 && cubetName.Contains('Y')) // if the cubet is an edge cubet and conains 'Y'
            {
                edgeCubets.Add(cubet); // add into the list
            }
        }

        if (edgeCubets.Count == 4) // if all 4 edges in the up layer contains 'Y' and second layer is not solved then there is a misplaced edge
        {
            yield return StartCoroutine(MisplacedEgdeCheck(layerName, layerIndex)); // apply misplaced edge
        }

        yield return null;
    }

    IEnumerator MisplacedEgdeCheck(string layerName, int layerIndex)
    {
        /* applies the algorthim for a misplaced edge */
        List<GameObject> cubetGameObjs = rotateLayersScript.GetCubetGameObj(layerName); // gets all the cubets in the layer
        char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the current face state
        List<GameObject> edgeCubets = new List<GameObject>(); // initialises list to contain edge cubets
        foreach (var cubet in cubetGameObjs) // loops through each cubet in the layer
        {
            string cubetName = cubet.name; // gets the name of each cubet
            Vector3 cubetPos = cubet.transform.position; // gets the position of each cubet
                                                         // if the cubet is in the middle layer and it is an edge peice, then add into edge cubet list
            if (Maths.AbsoluteValue(cubetPos.y, 0.0f) > vecThreshold && cubetName.Length == 2) { edgeCubets.Add(cubet); }
        }

        foreach (var cubet in edgeCubets) // loops through each edge cubet
        {
            string cubetName = cubet.name; // gets the name of the cubet
            if (!(cubetName.Contains('Y'))) // if the cubet does not contain 'Y'
            {
                if (currentFaceState[3] != currentFaceState[4]) // LEFT
                {
                    // apply left algorithm
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
                    // apply right algorithm
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
        List<int> yellowEdgeIndices = new List<int>(); // initialises the list that will contain all yellow edges
        string frontFace = ""; // initialises front face
        while (yellowEdgeIndices.Count != 4) // while there arent 4 edge cubets in the yellow layer
        {
            foreach (var layer in otherEdgeIndex) // loops through every layer
            {
                string layerName = layer.Key; // gets the name of the layer
                List<int> layerEdgeIndices = layer.Value; // gets the list of indicies of the top layre reletive to the layerName
                                                          // finds the front face to apply the rotation depending on where the edge indeces are
                if (yellowEdgeIndices.Count == 0) { frontFace = "Front"; break; }
                else if (yellowEdgeIndices.Contains(1) && yellowEdgeIndices.Contains(7)) { frontFace = "Left"; break; }
                else if (yellowEdgeIndices.Contains(3) && yellowEdgeIndices.Contains(5)) { frontFace = "Right"; break; }
                else if (yellowEdgeIndices.Contains(layerEdgeIndices[0])
                    && yellowEdgeIndices.Contains(layerEdgeIndices[1])) { frontFace = layerName; break; }
            }

            int layerIndex = cubeStatesScript.FaceIndexTrans[frontFace]; // gets the layerIndex

            // applys algorithm
            ApplyRotation(changedFrontFace[layerIndex]["Front"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);

            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Front"], false);
            yield return new WaitForSeconds(2.0f); // wait for algorithm to finsih executing 
            yellowEdgeIndices = GetEdgeIndices("Up", 'Y'); // gets the edge indeces for the yellow face
        }
        yield return null;
    }

    // ORIENTING Y CORNERS ====================================================================================================================

    IEnumerator OrientYellowCorners()
    {
        /* coroutine that finds the pattern in the yellow panels to find the corresponding front face and algorithm */

        while (!IsYellowFaceOriented()) // checks if all the yellow tiles are oriented correctly
        {
            char[] currentFaceState = cubeStatesScript.FaceColourState("Up"); // gets the current face state of the up layer
            int unYellowCounter = 0; // counter for how many panels are not yellow
            int panelI = 0; // initialising panel index
            string frontFace = ""; // initalising front face
            int layerIndex = -1; // initi layer index
            List<int> yellowIndex = new List<int>(); // initi list that will contain indecies that contain 'Y'

            foreach (char panel in currentFaceState) // loops through currentFaceState
            {
                if (panel != 'Y') { unYellowCounter++; } // increments countre for every panel that does not contain 'Y'
                else { yellowIndex.Add(panelI); } // if not add panel index to the list
                panelI++; // increments panel index
            }
            yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop

            if (unYellowCounter == 4) // if all 4 corners do not contain 'Y'
            {
                // find the panel to correspond to the patten to apply the adjacent colour
                // in this case, there need to be a 'Y' corner panel on the top right corner of one of the adjacent faces
                string adjacentFace = FindFaceWithPanel("Right");
                char adjacentColour = cubeStatesScript.LayerColour[adjacentFace]; // gets the char of the adjacent colour
                char rightFace = GetAdjacentFaces(adjacentColour)[1]; // gets the char of the right face
                                                                      // gets the name of the layer corresponding to the right face
                frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == rightFace).Key;
            }
            if (unYellowCounter == 3) // if there are 3 missing yellow tiles
            {
                // pattern of a fish
                // looks for that pattern and assigns appropriate front face
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
                // if there are either 2 or 1 missing yellow corners, then look for panel on the adjacent faces on the top left side
                frontFace = FindFaceWithPanel("Left");
            }

            layerIndex = cubeStatesScript.FaceIndexTrans[frontFace]; // get the corresponding layer index

            yield return new WaitForSeconds(1.0f); // wait to slow down the execution of while loop

            // apply algorithm
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], false);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
            ApplyRotation(changedFrontFace[layerIndex]["Right"], true);
            yield return new WaitForSeconds(3.0f); // wait for rotation to finish
        }

        yield return null;
    }

    string FindFaceWithPanel(string side)
    {
        /* finds the adjacent face that has the panel on the corresponding side to match the pattern */
        foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through every face
        {
            string layerName = face.Key; // gets the layer name
            if (layerName != "Up" && layerName != "Down") // if layer name is neither up nor down
            {
                char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the current face state 

                if (side == "Left" && currentFaceState[0] == 'Y') // if looking on the left side and contains 'Y' at index 0
                {
                    return layerName;
                }
                if (side == "Right" && currentFaceState[2] == 'Y') // if looking on the right side and contains 'Y' at index 2
                {
                    return layerName;
                }
            }
        }
        return null;
    }

    bool IsYellowFaceOriented()
    {
        /* returns bool if the yellow face is oriented */
        char[] currentFaceState = cubeStatesScript.FaceColourState("Up"); // gets the current face state for the up layer
        foreach (char panel in currentFaceState) // loops through each panel in the current face
        {
            if (panel != 'Y') // if there is a panel that is not 'Y' then not solved
            {
                return false;
            }
        }
        return true;
    }

    // PERMUTE Y CORNERS ====================================================================================================================

    IEnumerator PermuteYellowCorners()
    {
        /* coroutine that places the yellow corners in the correct place */

        while (!IsYellowCornersSolved()) // loops while yellow corners are not` solved
        {
            int frontFaceIndex = -1; // initialises the frontFaceIndex to null
            string frontFace = null; // intilaises frontFace to null

            foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through every face 
            {
                string layerName = face.Key; // gets the layer name
                int layerIndex = face.Value; // gets the layer index
                char faceColour = cubeStatesScript.LayerColour[layerName]; // gets the corresponding char for the layer name

                yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop

                if (layerName != "Up" && layerName != "Down") // if layer name is neither up nor down
                {
                    char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // get the current face state

                    if (currentFaceState[0] == faceColour && currentFaceState[2] == faceColour) // finds the correct back face
                    {
                        char layerColour = cubeStatesScript.LayerColour[layerName]; // converts the layerName to char equivilant
                        char oppositeFace = GetOppositeFace(layerColour); // get the char val for the opposite face
                        frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == oppositeFace).Key; // gets the string of the opposite face
                    }
                    else if (currentFaceState[0] == currentFaceState[2]) // if you find the two that are supposed to be the back face
                    {
                        char frontChar; // intilaises the frontChar
                        char[] adjacentFaces = GetAdjacentFaces(faceColour); // gets a char array of adjacent faces
                        if (adjacentFaces[0] == currentFaceState[0]) // if the corners match the left face
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            frontChar = adjacentFaces[0];
                        }
                        else if (adjacentFaces[1] == currentFaceState[0]) // if the corners match the right face
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], false);
                            frontChar = adjacentFaces[1];
                        }
                        else // the corners match the opposite face
                        {
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            ApplyRotation(changedFrontFace[layerIndex]["Up"], true);
                            frontChar = GetOppositeFace(faceColour);
                        }
                        frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == frontChar).Key; // gets the front face
                    }
                    else if (currentFaceState[0] == 'G' && currentFaceState[2] == 'B'
                        || currentFaceState[2] == 'G' && currentFaceState[0] == 'B') // if not find the face that has a blue and green corner to be the front
                    {
                        // apply appropriate rotation to make the back face the front face
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
                    }
                    if (frontFace != null)
                    {
                        break;
                    }
                }
            }

            frontFaceIndex = cubeStatesScript.FaceIndexTrans[frontFace]; // gets the corresponding index
            yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop

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

            yield return new WaitForSeconds(5.0f); // wait to finsh executing algorithm
        }

        // if need be, fix orientaion of the top layer to match centres
        char[] faceState = cubeStatesScript.FaceColourState("Front"); // gets the current face state of the front face
        StartCoroutine(fixTopLayerOrientation(faceState, "Front", 1));

        yield return null;
    }

    IEnumerator fixTopLayerOrientation(char[] currentFaceState, string layerName, int layerIndex)
    {
        /* if orientation of the top layer intil it matches the correct face */
        char faceColour = cubeStatesScript.LayerColour[layerName]; // gets the char of the face colour
        char[] adjacentFaces = GetAdjacentFaces(faceColour); // get the corresponding adjacent faces
                                                             // checks where to move and applies appropriate rotation
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
        yield return new WaitForSeconds(2.0f); // wait for rotation to finish executing
    }

    bool IsYellowCornersSolved()
    {
        /* bool to be returned if the yellow corners are solved */
        foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through every face
        {
            string layerName = face.Key; // gets the name of the layer
            if (layerName != "Up" && layerName != "Down") // if the layer name is not up nor down
            {
                char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the currentFaceState
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
        /* coroutine that will position the yellow edges */
        while (!cubeStatesScript.IsCubeSolved()) // loops while the cube is not solved
        {
            yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop
            StartCoroutine(fixTopLayerOrientation(cubeStatesScript.FaceColourState("Front"), "Front", 1)); // fixes the orientaion of the top layer to match the correct centres

            string frontFace = null; // initialises front face to null
            int layerIndex = -1; // intilaises layer index to null

            string backFace = FindSolvedFace(); // finds the solved face and makes it the back face

            yield return new WaitForSeconds(2.0f); // wait to slow down the execution of while loop

            if (backFace != null) // if back face is found
            {
                // find opposite face
                char charBackFace = cubeStatesScript.LayerColour[backFace]; // finds the colour of the back face
                char charFrontFace = GetOppositeFace(charBackFace); // gets the opposite face of the back face
                                                                    // finds the name of that front face
                frontFace = cubeStatesScript.LayerColour.FirstOrDefault(x => x.Value == charFrontFace).Key;
                layerIndex = cubeStatesScript.FaceIndexTrans[frontFace]; // gets the corresponding layer index
            }
            else
            {
                layerIndex = 1; // if not make default front face, the front face
            }

            // applys algorithm
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
            yield return new WaitForSeconds(5.0f); // wait to finish executing algorithm
        }
        yield return null;
    }

    private string FindSolvedFace()
    {
        /* function that will find and return the name of the solved face */
        foreach (var face in cubeStatesScript.FaceIndexTrans) // loops through each face
        {
            string layerName = face.Key; // gets the name of the layer
            if (layerName != "Up" && layerName != "Down") // if the name is neither up nor down
            {
                char[] currentFaceState = cubeStatesScript.FaceColourState(layerName); // gets the current face state

                // Check if all panels are the same color or the same color as the center panel
                bool solved = true; // initialises solved to true
                for (int i = 0; i < currentFaceState.Length; i++)
                {
                    if (currentFaceState[i] != currentFaceState[4]) // checks if each panel is the same as the centre
                    {
                        solved = false; // if not solves is false and break
                        break;
                    }
                }

                if (solved) // if solved is true, then return layer name 
                {
                    return layerName;
                }
            }
        }
        return null; // No solved face found
    }
}
