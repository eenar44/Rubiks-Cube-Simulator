using UnityEngine;

public class CubeMap : MonoBehaviour
{
    /*
     Issue: the colour of the material is changing in the inspector, when the code is run, but the panel isnt changing colour
    so the logic is correct, however, there is an issue with implication
     */


    public CubeStates cubeStatesScript; // Reference to CubeSates script

    public Color[] colours;
    public string[] faceTags = { "Top", "Front", "Left", "Right", "Back", "Bottom" };
    public Transform[] faces = new Transform[6];
    public GameObject[,] panels2DNet;

    private Material targetMaterial;

    void Start()
    {
        cubeStatesScript = FindObjectOfType<CubeStates>(); // reference to CubeStates

        // Initialize faces array
        for (int i = 0; i < 6; i++)
        {
            faces[i] = transform.Find(faceTags[i]);
        }
    }

    void Update()
    {
        Initialize2DNet();
        Update2DNetColors();
    }

    void Initialize2DNet()
    {
        panels2DNet = new GameObject[6, 9];

        for (int faceI = 0; faceI < 6; faceI++)
        {
            int i = 0;
            foreach (Transform panel in faces[faceI])
            {
                if (i <= 8)
                {
                    panels2DNet[faceI, i] = panel.gameObject;
                    i++;
                }
                else
                {
                    break;
                }
            }
        }
    }

    void Update2DNetColors()
    {
        // Loop through the 6 faces
        for (int faceI = 0; faceI < 6; faceI++)
        {
            // Loop through the 9 panels on each face
            for (int panelI = 0; panelI < 9; panelI++)
            {
                // Get the color and position from the CubeStates array
                //var (panelColor, _) = cubeStatesScript.cubePanels[faceI, panelI];
                Color panelColor = cubeStatesScript.cubePanels[faceI, panelI];

                // Set the color of the corresponding panel on the 2D net
                GameObject panelObject = panels2DNet[faceI, panelI];
                Renderer panelRenderer = panelObject.GetComponent<Renderer>();

                if (panelRenderer != null)
                {
                    panelRenderer.material.color = panelColor;
                    //Debug.Log($"Updated color of panel {panelI} on face {faceI} to {panelColor}.");
                }
                else
                {
                    Debug.LogError($"Renderer not found on panel {panelI} on face {faceI}.");
                }
            }
        }
    }

}
