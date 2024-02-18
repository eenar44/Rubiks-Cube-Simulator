using UnityEngine;
using UnityEngine.UI;

public class CubeMap : MonoBehaviour
{
    public CubeStates cubeStatesScript; // Reference to CubeSates script

    public Color[] colours; // array of colours that will be used on my cube map
    public string[] faceTags = { "Top", "Front", "Left", "Right", "Back", "Bottom" }; // List of the name face game objects in the scene
    public Transform[] faces = new Transform[6]; // list of the game objects in the scene
    public GameObject[,] panels2DNet; // list of panels that make the 2D net 

    void Start()
    {
        cubeStatesScript = FindObjectOfType<CubeStates>(); // finds the reference to CubeStates in the scene

        // initialises faces array
        for (int i = 0; i < 6; i++) // loops through every face
        {
            faces[i] = transform.Find(faceTags[i]); // finds the object and adds the parent object for the face onto the list
        }

        InitialiseNet(); // initialises the 2D net
    }

    void Update()
    {
        UpdateColors(); // updates the 2D map in every frame
    }

    void InitialiseNet()
    {
        /* function that gets all the panel game objects in the scene */
        panels2DNet = new GameObject[6, 9]; // initialises the size of the 2D net

        for (int faceI = 0; faceI < 6; faceI++) // loops through each face
        {
            int panelCounter = 0; // makes a penl counter
            foreach (Transform panel in faces[faceI]) // loops through each panel in the face
            {
                if (panelCounter <= 8) // the 8th panel is the last so checks
                {
                    panels2DNet[faceI, panelCounter] = panel.gameObject; // adds the panel game object onto the list
                    panelCounter++; // increments the panel counter
                }
                else
                {
                    break; // breaks if more that 8
                }
            }
        }
    }

    void UpdateColors()
    {
        /* applies the colour to the panels in the scene */
        for (int faceI = 0; faceI < 6; faceI++) // loop through the 6 faces
        {
            for (int panelI = 0; panelI < 9; panelI++) // loop through the 9 panels on each face
            {
                Color panelColor = cubeStatesScript.cubePanels[faceI, panelI]; // gets the color from cubePanels

                GameObject panelObject = panels2DNet[faceI, panelI]; // gets the panel
                Image panelImageComponent = panelObject.GetComponent<Image>(); // gets the image component of the game object

                if (panelImageComponent != null) // checks if the image component is present
                {
                    panelImageComponent.color = panelColor; // if it is, then set the panel colour
                }
                else
                {
                    Debug.LogError($"Renderer not found on panel {panelI} on face {faceI}."); // if not, then log error
                }
            }
        }
    }
}
