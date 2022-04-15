using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
//using CodeMonkey.Utils;

public class Graph : MonoBehaviour {
    [SerializeField] private Sprite circleSprite;
    private RectTransform graphContainer;

    private void Awake() {
        graphContainer = transform.Find("graphContainer").GetComponent<RectTransform>();

        CreateCircle(new Vector2(200,200));
        List<int> valueList = new List<int>() {1, 3 , 2, 6, 5};
        showGraph(valueList);
    }

    private GameObject CreateCircle(Vector2 anchoredPosition) {
        GameObject gameObject = new GameObject("circle", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPosition;
        rectTransform.sizeDelta = new Vector2(11, 11);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        return gameObject;

    }
    private void showGraph(List<int> valueList) {
        float graphHeight = graphContainer.sizeDelta.y;
        float yMaximum = 100f;
        float xSize = 50f;

        GameObject lastCircleGameObject = null;
        for (int i = 0; i < valueList.Count; i++) {
            float xPosition = xSize + i * xSize;
            float yPosition = (valueList[i] / yMaximum) * graphHeight;
            GameObject circleGameObject = CreateCircle(new Vector2(xPosition, yPosition));
            if (lastCircleGameObject != null) {
                CreateDotConnection(lastCircleGameObject.GetComponent<RectTransform>().anchoredPosition, circleGameObject.GetComponent<RectTransform>().anchoredPosition);
            }
            lastCircleGameObject = circleGameObject;
        }
    }
    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB) {
        GameObject gameObject = new GameObject("dorConnection", typeof(Image));
        gameObject.transform.SetParent(graphContainer, false);
        gameObject.GetComponent<Image>().color = new Color(1,1,1, .5f);
        RectTransform rectTransform = gameObject.GetComponent<RectTransform>();
        Vector2 dir = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.sizeDelta = new Vector2(100,3f);
        rectTransform.anchoredPosition=dotPositionA;
        rectTransform.localEulerAngles = new Vector3(0,0);

    }
} 