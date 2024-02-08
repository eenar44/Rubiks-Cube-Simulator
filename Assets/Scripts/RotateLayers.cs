using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateLayers : MonoBehaviour
{
    public MathsFunctions Maths;
    private bool isRotating = false; // blooean that indicates if a layer is rotating
    public Queue<RotationCommand> rotationQueue = new Queue<RotationCommand>(); // queue that contains the rotations that are being execeuted

    public class RotationCommand
    {
        /*initialising the queue*/
        public string layerName;
        public bool clockwise;
        public float turnSpeed;

        public RotationCommand(string layerName, bool clockwise, float turnSpeed)
        {
            this.layerName = layerName;
            this.clockwise = clockwise;
            this.turnSpeed = turnSpeed;
        }
    }

    private void Start()
    {
        Maths = FindObjectOfType<MathsFunctions>();
    }

    void Update()
    {
        if (!isRotating)
        {
            UserInputRotation();
            DequeueRotation();
        }
    }

    public IEnumerator RotateLayer(string layerName, bool clockwise, float turnSpeed)
    {
        /*Coroutine that rotates the layer*/
        isRotating = true; // the layer is now rotating
        List<GameObject> cubetsToRot = GetCubetGameObj(layerName); // get the list of cubet GameObjects that will be rotated
        GameObject cubeObj = GameObject.Find("Cube"); // finds the parent game object that holds the cube
        GameObject parentLayer = CreateParent(cubetsToRot, cubeObj); // creates tha parent game object, and places all the cubets in it

        float angle = clockwise ? 90.0f : -90.0f; // rotation value
        Quaternion targetRotation = Quaternion.identity; // initialises the target rotation
        // depending on the layer, decided what the target rotation must look like using angles
        if (layerName == "Up" || layerName == "Down")
        {
            targetRotation = Quaternion.Euler(0, angle, 0);
        }
        else if (layerName == "Right" || layerName == "Left")
        {
            targetRotation = Quaternion.Euler(angle, 0, 0);
        }
        else if (layerName == "Front" || layerName == "Back")
        {
            targetRotation = Quaternion.Euler(0, 0, angle);
        }

        // transforms the parents game object, around the centre point
        float t = 0.0f;
        while (t < 1.0f)
        {
            t += Time.deltaTime * turnSpeed;
            parentLayer.transform.rotation = Quaternion.Slerp(parentLayer.transform.rotation, targetRotation, t);
            yield return null;
        }
        parentLayer.transform.rotation = targetRotation; // sets the final rotation

        DestroyParent(parentLayer, cubeObj); // destroys the parent
        isRotating = false; // layer is set to not rotating
    }

    private void UserInputRotation()
    {
        /*handles the user input, when they want to rotate the cube*/

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
        /*enqueues the rotation queue, the default turnspeed set to 2.0f*/
        //Debug.Log("rotating " + layerName + " d: " + clockwise);
        RotationCommand command = new RotationCommand(layerName, clockwise, turnSpeed);
        rotationQueue.Enqueue(command);
    }

    public void DequeueRotation()
    {
        /*executes the first item in the rotation queue, as long as the queue isnt emty*/
        if (rotationQueue.Count > 0)
        {
            RotationCommand command = rotationQueue.Dequeue();
            StartCoroutine(RotateLayer(command.layerName, command.clockwise, command.turnSpeed));
        }
    }

    public List<GameObject> GetCubetGameObj(string layerName)
    {
        /*based on the layer name and the approximate position of the cubet in that layer, adds the cubets to a list of 
         cubet GameObjects that shoul be rotated, returns that list*/
        List<GameObject> cubetsToRot = new List<GameObject>();
        Transform cubeTrans = GameObject.Find("Cube").transform;

        float threshold = 0.001f;

        for (int cubetIndex = 0; cubetIndex < cubeTrans.childCount; cubetIndex++)
        {
            Transform cubet = cubeTrans.GetChild(cubetIndex);
            
            if (layerName == "Up" && Maths.AbsoluteValue(cubet.position.y, 1.025f) < threshold)
            {
                cubetsToRot.Add(cubet.gameObject);
            }
            if (layerName == "Front" && Maths.AbsoluteValue(cubet.position.z, -1.025f) < threshold) //-1.025, opposite sign flip (*)
            {
                cubetsToRot.Add(cubet.gameObject);
            }
            if (layerName == "Left" && Maths.AbsoluteValue(cubet.position.x, -1.025f) < threshold) // *
            {
                cubetsToRot.Add(cubet.gameObject);
            }
            if (layerName == "Right" && Maths.AbsoluteValue(cubet.position.x, 1.025f) < threshold)
            {
                cubetsToRot.Add(cubet.gameObject);
            }
            if (layerName == "Back" && Maths.AbsoluteValue(cubet.position.z, 1.025f) < threshold)
            {
                cubetsToRot.Add(cubet.gameObject);
            }
            if (layerName == "Down" && Maths.AbsoluteValue(cubet.position.y, -1.025f) < threshold) // *
            {
                cubetsToRot.Add(cubet.gameObject);
            }
        }
        return cubetsToRot;
    }

    public GameObject CreateParent(List<GameObject> cubetsToRot, GameObject cubeObj)
    {
        /*a new game object is created called LayerParent, this is made into a parent game object in the Cube Holder parent, 
         all the cubets in the list are added to this parent gameobject*/
        GameObject parentLayer = new GameObject("LayerParent");
        parentLayer.transform.parent = cubeObj.transform;

        foreach (var cubet in cubetsToRot)
        {
            cubet.transform.SetParent(parentLayer.transform);
        }
        return parentLayer;
    }

    void DestroyParent(GameObject parentLayer, GameObject cubeObj)
    {
        /*Adds all the cubets in to a List of Transforms, then sets the parent of all the cubets to the Cube game objest in the scene
         then destroys the parent gameobject*/
        List<Transform> cubetsToRot = new List<Transform>();
        foreach (Transform cubet in parentLayer.transform)
        {
            cubetsToRot.Add(cubet);
        }

        foreach (Transform cubet in cubetsToRot)
        {
            cubet.SetParent(cubeObj.transform);
        }
        Destroy(parentLayer);
    }

    void RotateMSlice(bool clockwise)
    {
        //
    }
}
