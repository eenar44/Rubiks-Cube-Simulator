using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GraphPlotter : MonoBehaviour
{
    public MathsFunctions Maths;
    public DataManager dataManagerScript;

    public RectTransform graphPanel;
    [SerializeField] public Sprite dotSprite;
    //public List<int> numberOfMoves = new List<int> { 200, 205, 148, 145, 144, 150, 142, 138, 141, 147, 149 };

    private void Start()
    {
        Maths = FindObjectOfType<MathsFunctions>();
        dataManagerScript = FindObjectOfType<DataManager>();

        graphPanel = transform.Find("graphPanel").GetComponent<RectTransform>();
        DrawGraph(dataManagerScript.GetNumberOfMoves());
        //DrawGraph(new List<int> { 50, 25, 100, 200, 50, 250 });
    }

    private GameObject CreateCircle(Vector2 newPos)
    {
        GameObject gameObject = new GameObject("dot", typeof(Image));
        gameObject.transform.SetParent(graphPanel, false);
        gameObject.GetComponent<Image>().sprite = dotSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = newPos;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return gameObject;
    }

    private void CreateLine(Vector2 posA, Vector2 posB)
    {
        GameObject gameObject = new GameObject("line", typeof(Image));
        gameObject.transform.SetParent(graphPanel, false);
        gameObject.GetComponent<Image>().color = new Color(0, 1, 0, 0.5f);

        Vector2 direction = (posB - posA).normalized;
        float disance = Vector2.Distance(posA, posB);

        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.sizeDelta = new Vector2(disance, 3f);
        rectTransform.anchoredPosition = posA + direction * disance *0.5f;
        rectTransform.localEulerAngles = new Vector3(0, 0, Maths.ConvertVectorToAngle(direction));

    }

    public void DrawGraph(List<int> numberOfMoves)
    {
        float graphHeight = graphPanel.sizeDelta.y;
        float yMax = 250.0f;
        float xMag = graphPanel.rect.width / 10f; ; // size distance between each point on the x axis
        GameObject previousDot = null;
        Debug.Log("no: " + numberOfMoves.Count);
        for (int i = 0; i < numberOfMoves.Count; i++) 
        {
            Debug.Log("Drawing");
            float xPos = i * xMag;
            float yPos = (numberOfMoves[i]/ yMax) * graphHeight;
            GameObject dot = CreateCircle(new Vector2(xPos, yPos));
            if (previousDot != null)
            {
                CreateLine(previousDot.GetComponent<RectTransform>().anchoredPosition, dot.GetComponent<RectTransform>().anchoredPosition);
            }
                previousDot = dot;
        }
    }

}
