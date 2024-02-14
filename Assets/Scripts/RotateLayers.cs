using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLayers : MonoBehaviour
{
    public MathsFunctions Maths; // reference to MathsFunctions script
    private bool isRotating = false; // blooean that indicates if a layer is rotating
    public Queue<RotationCommand> rotationQueue = new Queue<RotationCommand>(); // queue that contains the rotations that are being execeuted

    public class RotationCommand
    {
        /* nested class that initialises the queue */
        public string layerName; // name of the layer that will be rotated
        public bool clockwise; // direction of the rotation
        public float turnSpeed; // speed of the rotation

        public RotationCommand(string layerName, bool clockwise, float turnSpeed)
        {
            /* a constructor that initalises the variables */
            this.layerName = layerName; // initalises the layer name
            this.clockwise = clockwise; // initialises the direction of rotation
            this.turnSpeed = turnSpeed; // initialises the turn speed
        }
    }

    private void Start()
    {
        Maths = FindObjectOfType<MathsFunctions>(); // finds the reference to MathsFunctions in the scene
    }

    void Update()
    {
        if (!isRotating) // checks in every frame if it is not rotating, if not then execute
        {
            // this is to ensure that another layer isnt trying to rotate while another one is in mid-rotation
            UserInputRotation(); // gets the user input for rotation
            DequeueRotation(); // executes the rotation, and removes it from the queue
        }
    }

    public IEnumerator RotateLayer(string layerName, bool clockwise, float turnSpeed)
    {
        /* Coroutine that rotates the layer */
        isRotating = true; // the layer is now rotating
        List<GameObject> cubetsToRot = GetCubetGameObj(layerName); // get the list of cubet GameObjects that will be rotated
        GameObject cubeObj = GameObject.Find("Cube"); // finds the parent game object that holds the cube
        GameObject parentLayer = CreateParent(cubetsToRot, cubeObj); // creates tha parent game object, and places all the cubets in it

        float angle = clockwise ? 90.0f : -90.0f; // rotation value
        Quaternion targetRotation = Quaternion.identity; // initialises the target rotation
        // depending on the layer, decided what the target rotation must look like using angles
        if (layerName == "Up" || layerName == "Down") // if the layer name is either up or down 
        {
            targetRotation = Quaternion.Euler(0, angle, 0); // rotate in the y axis
        }
        else if (layerName == "Right" || layerName == "Left") // if the layer is either right or left
        {
            targetRotation = Quaternion.Euler(angle, 0, 0); // rotate in the x axis
        }
        else if (layerName == "Front" || layerName == "Back") // if the layer id either front or back
        {
            targetRotation = Quaternion.Euler(0, 0, angle); // rotate in the z axis
        }

        // transforms the parents game object, around the centre point
        float elapsedTime = 0.0f; // initlaising the float that will increment the time elapsed
        while (elapsedTime < 1.0f) // loops while the time elapsed is less than one
        {
            elapsedTime += Time.deltaTime * turnSpeed; //  increments time elapsed
            // interpolates between the current transform and the target rotation, in the elaped time
            parentLayer.transform.rotation = Quaternion.Slerp(parentLayer.transform.rotation, targetRotation, elapsedTime);
            yield return null;
        }
        parentLayer.transform.rotation = targetRotation; // assigns the roation to the target rotation, to aviod minor error margin

        DestroyParent(parentLayer, cubeObj); // destroys the parent
        isRotating = false; // layer is set to not rotating
    }

    private void UserInputRotation()
    {
        /* handles the user input, when they want to rotate the cube */

        // Rotating the UP layer
        if (Input.GetKeyDown(KeyCode.E))
        {
            EnqueueRotation("Up", true); // Clockwise
        }
        else if (Input.GetKeyDown(KeyCode.O))
        {
            EnqueueRotation("Up", false); // anti clockwise
        }
        // Rotating the BOTTOM layer
        else if (Input.GetKeyDown(KeyCode.X))
        {
            EnqueueRotation("Down", true); // Clockwise
        }
        else if (Input.GetKeyDown(KeyCode.Comma))
        {
            EnqueueRotation("Down", false); // anti clockwise
        }
        // Rotating the LEFT column
        else if (Input.GetKeyDown(KeyCode.W))
        {
            EnqueueRotation("Left", false); //upwards
        }
        else if (Input.GetKeyDown(KeyCode.S))
        {
            EnqueueRotation("Left", true); // downwards
        }
        // Rotating the RIGHT column
        else if (Input.GetKeyDown(KeyCode.P))
        {
            EnqueueRotation("Right", true); //upwards
        }
        else if (Input.GetKeyDown(KeyCode.Semicolon))
        {
            EnqueueRotation("Right", false); // downwards
        }
        // Rotating the BACK side
        else if (Input.GetKeyDown(KeyCode.C))
        {
            EnqueueRotation("Back", false); //anti clockwise
        }
        else if (Input.GetKeyDown(KeyCode.M))
        {
            EnqueueRotation("Back", true); // clockwise
        }
        // Rotating the FRONT side
        else if (Input.GetKeyDown(KeyCode.R))
        {
            EnqueueRotation("Front", true); // clockwise
        }
        else if (Input.GetKeyDown(KeyCode.I))
        {
            EnqueueRotation("Front", false); // anti clockwise
        }
    }

    public void EnqueueRotation(string layerName, bool clockwise, float turnSpeed = 2.0f)
    {
        /* enqueues the rotation queue, the default turnspeed set to 2.0f */
        //Debug.Log("rotating " + layerName + " d: " + clockwise); ////////////////////////////////////
        RotationCommand command = new RotationCommand(layerName, clockwise, turnSpeed); // creates a new command
        rotationQueue.Enqueue(command); // adds the new command to the queue
    }

    public void DequeueRotation()
    {
        /* executes the first item in the rotation queue, as long as the queue isnt empty */
        if (rotationQueue.Count > 0) // if the queu is not empty
        {
            RotationCommand command = rotationQueue.Dequeue(); // gets the first rotation in the queue 
            StartCoroutine(RotateLayer(command.layerName, command.clockwise, command.turnSpeed)); // applys the rotation
        }
    }

    public List<GameObject> GetCubetGameObj(string layerName)
    {
        /* based on the layer name and the approximate position of the cubet in that layer, adds the cubets to a list of 
         * cubet GameObjects that shoul be rotated, returns that list
         */
        List<GameObject> cubetsToRot = new List<GameObject>(); // initlaises a list of gameobjects that will be rotated
        Transform cubeTrans = GameObject.Find("Cube").transform; // finds the "Cube" gameobject in the scene. this game object is the parent of all the cubets

        float threshold = 0.001f; // vector inaccuracy threshold

        for (int cubetIndex = 0; cubetIndex < cubeTrans.childCount; cubetIndex++) // loops through every cubet int he cube transform
        {
            Transform cubet = cubeTrans.GetChild(cubetIndex); // gets the transform at that index
            
            if (layerName == "Up" && Maths.AbsoluteValue(cubet.position.y, 1.025f) < threshold)
            {
                cubetsToRot.Add(cubet.gameObject); // if the cubet has the correct y vector to be in the up, then ad to cubet list
            }
            if (layerName == "Front" && Maths.AbsoluteValue(cubet.position.z, -1.025f) < threshold) //-1.025, opposite sign flip (*)
            {
                cubetsToRot.Add(cubet.gameObject); // if the cubet has the correct z vector to be in the front, then ad to cubet list
            }
            if (layerName == "Left" && Maths.AbsoluteValue(cubet.position.x, -1.025f) < threshold) // *
            {
                cubetsToRot.Add(cubet.gameObject); // if the cubet has the correct x vector to be in the left, then ad to cubet list
            }
            if (layerName == "Right" && Maths.AbsoluteValue(cubet.position.x, 1.025f) < threshold)
            {
                cubetsToRot.Add(cubet.gameObject); // if the cubet has the correct x vector to be in the right, then ad to cubet list
            }
            if (layerName == "Back" && Maths.AbsoluteValue(cubet.position.z, 1.025f) < threshold)
            {
                cubetsToRot.Add(cubet.gameObject); // if the cubet has the correct z vector to be in the back, then ad to cubet list
            }
            if (layerName == "Down" && Maths.AbsoluteValue(cubet.position.y, -1.025f) < threshold) // *
            {
                cubetsToRot.Add(cubet.gameObject); // if the cubet has the correct y vector to be in the down, then ad to cubet list
            }
        }
        return cubetsToRot; // returns the list of cubets in that layer
    }

    public GameObject CreateParent(List<GameObject> cubetsToRot, GameObject cubeObj)
    {
        /* a new game object is created called LayerParent, this is made into a parent game object in the Cube Holder parent, 
         * all the cubets in the list are added to this parent gameobject
         */
        GameObject parentLayer = new GameObject("LayerParent"); // creates a new gameobject in the scene called "LayerParent"
        parentLayer.transform.parent = cubeObj.transform; // sets the new game object as a child of the cube

        foreach (var cubet in cubetsToRot) // loops through every cubet in the layer
        {
            cubet.transform.SetParent(parentLayer.transform); // sets each cubet to be a child of the new layer parent
        }
        return parentLayer; // returns the gamobject that contains all the children gameobject to be rotated
    }

    void DestroyParent(GameObject parentLayer, GameObject cubeObj)
    {
        /* Adds all the cubets in to a List of Transforms, then sets the parent of all the cubets to the Cube game objest 
         * in the scene then destroys the parent gameobject
         */
        List<Transform> cubetsToRot = new List<Transform>(); // creates a new list that will contain all the gameobjects in the parent layer
        foreach (Transform cubet in parentLayer.transform) // loops through each cubet in the parent layer
        {
            cubetsToRot.Add(cubet); // adds each cubet to the list
        }

        foreach (Transform cubet in cubetsToRot) // loops through the list
        {
            cubet.SetParent(cubeObj.transform); // changes the parent of the cubet to the original "Cube" game object
        }
        Destroy(parentLayer); // destrys the now, empty game object
    }
}
