using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphPlotter : MonoBehaviour
{
    public MathsFunctions Maths; // reference to MathsFunctions script
    public DataManager dataManagerScript; // reference to DataManager script

    public RectTransform graphPanel; // reference to the RectTransform panel that is in the scene. on this panel, my graph will be plotted
    [SerializeField] public Sprite dotSprite; // reference to the dot sprite that is initialised in the scene
    // public List<int> numberOfMoves = new List<int> { 200, 205, 148, 145, 144, 150, 142, 138, 141, 147, 149 };

    void Start() // runs on start
    {
        Maths = FindObjectOfType<MathsFunctions>(); // finds the reference to MathsFunctions in the scene
        dataManagerScript = FindObjectOfType<DataManager>(); // finds the reference to DataManager in the scene

        graphPanel = transform.Find("graphPanel").GetComponent<RectTransform>(); // finds the graphPanel in the scene that has a RectTransform component attached to it
        DrawGraph(dataManagerScript.GetNumberOfMoves()); // draws the graph by executing DrawGraph, passes in the numberOfMoves list, using the GetNumberOfMoves function in DataManager
        //DrawGraph(new List<int> { 50, 25, 100, 200, 50, 250 });
    }

    private GameObject CreateDot(Vector2 newPos)
    {
        /* function that places a new dot in the scene as specified by the vector passed into the function*/
        GameObject gameObject = new GameObject("dot", typeof(Image)); // creates a new image game object called "dot"
        gameObject.transform.SetParent(graphPanel, false); // sets this new gameobject as a child of the graphPanel object
        gameObject.GetComponent<Image>().sprite = dotSprite; // gets the image component of the game object, then sets the sprite of the image component to dotSprite
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>(); // gets the RectTransform components from the game object
        rectTransform.anchoredPosition = newPos; // sets the anchroed position of the rect transfrom to the one sepcified by newPos
        rectTransform.sizeDelta = new Vector2(11, 11); // sets the size of the rect transform to 11x11 units
        rectTransform.anchorMin = new Vector2(0, 0); // sets the minimum anchored point to (0, 0), so the bottom-left corner of the parent
        rectTransform.anchorMax = new Vector2(0, 0); // sets the maximum anchored point to (0, 0), so also the bottom left corner of the parent
        return gameObject; // returns the dot game object
    }

    private void CreateLine(Vector2 posA, Vector2 posB)
    {
        /* function that draws a line between two dots defined as posA and posB */
        GameObject gameObject = new GameObject("line", typeof(Image)); // creates a new image game object called "line"
        gameObject.transform.SetParent(graphPanel, false); // set the new gmeobject as a chaild of the graphPanel object
        gameObject.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f); // sets the colour to a a green colour (same as the dot) , but slightly transparent

        Vector2 direction = (posB - posA).normalized; // calculates the normlaised direction between point A and B as a vector2
        float disance = Vector2.Distance(posA, posB); // calculates the distance between point A and B

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>(); // gets the RectTransform components from the game object
        rectTransform.anchorMin = new Vector2(0, 0); // sets the minimum anchored point to (0, 0), so the bottom-left corner of the parent
        rectTransform.anchorMax = new Vector2(0, 0); // sets the maximum anchored point to (0, 0), so also the bottom left corner of the parent
        rectTransform.sizeDelta = new Vector2(disance, 3f); // sets the size of the line to be the distance 3f
        rectTransform.anchoredPosition = posA + direction * disance * 0.5f; // place the anchored position directly in the middle of the two points
        // to rotate the line toward the next dot, assigns the z value of the dot to be the angle differenece between the two angles
        rectTransform.localEulerAngles = new Vector3(0, 0, Maths.ConvertVectorToAngle(direction));

    }

    public void DrawGraph(List<int> numberOfMoves)
    {
        /* using the list of numberOfMoves will pass each point into each function in a loop and plot accordingly */
        float graphHeight = graphPanel.sizeDelta.y; // gets the height of the graphPanel, so we can plot according to the size of the panel
        float yMax = 250.0f; // the largest value y can be
        float xMag = graphPanel.rect.width / 10f; ; // size distance between each point on the x axis
        GameObject previousDot = null; // initalises the previous dot variable to null
        Debug.Log("no: " + numberOfMoves.Count); ////////////////////////////////////////////////////
        for (int i = 0; i < numberOfMoves.Count; i++) // loops through the list indexes
        {
            Debug.Log("Drawing"); ////////////////////////
            float xPos = i * xMag; // finds the x coordinate by multiplying the i value with the x magnitude, as the x axis values will have discreate spacing between each point
            float yPos = (numberOfMoves[i]/ yMax) * graphHeight; // calculates the y coordinate relative to the graphPanel's height and the maximum y value 
            GameObject dot = CreateDot(new Vector2(xPos, yPos)); // passes in the new x and y coordinates into CreateDot to place the dot correctly
            if (previousDot != null) // checks if there is a previous dot to connect the line between
            {
                // passes in the anchored posiiton of the previous dot and the anchored posiiton fo the current dot into create line to plot the line
                CreateLine(previousDot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition);
            }
            previousDot = dot; // assigns the cuurent dot to the previousDot variable for the next loop
        }
    }
}
