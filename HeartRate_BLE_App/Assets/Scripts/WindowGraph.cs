using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CodeMonkey.Utils;
using TMPro;

public class WindowGraph : MonoBehaviour
{
    [SerializeField] private GameObject graphContainerPrefab;
    [SerializeField] private GameObject graphYAxisLabelPrefab;
    [SerializeField] private GameObject gridDashPrefab;

    private GameObject graphContainer;

    private RectTransform graphContainerRect;
    private RectTransform graphYAxisLabelRect;
    private RectTransform gridDashRect;

    private float yMax;
    private bool isStartup;

    [SerializeField] private Sprite circleSprite;

    private void Start()
    {
        isStartup = true;
        graphContainer = Instantiate(graphContainerPrefab, transform.position, transform.rotation, transform);
        graphContainerRect = graphContainer.GetComponent<RectTransform>();
        graphContainerRect.anchoredPosition = new Vector2(5f, 5f);
        yMax = 80f;
        ShowGraph(new int[]{0});
        isStartup = false;
    }

    public void ClearGraph()
    { // destroy graph container and instantiate a new one
        Destroy(graphContainer);
        graphContainer = Instantiate(graphContainerPrefab, transform.position, transform.rotation, transform);
        graphContainerRect = graphContainer.GetComponent<RectTransform>();
        graphContainerRect.anchoredPosition = new Vector2(5f, 5f);
    }

    public void ShowGraph(int[] dataArray)
    {
        float graphHeight = graphContainerRect.sizeDelta.y;
        // This if statement allows yMax to be set in Start function and not overwritten.
        // Can display a graph and axis on start, instead of waiting for data to arrive.
        if (!isStartup)
            yMax = 0f;
        float xInterval = 50f;
        int largestValue = 0;

        // cycle through and get yMax
        for (int i = 0; i < dataArray.Length; i++)
        {
            if (dataArray[i] > largestValue)
            {
                largestValue = dataArray[i];
                yMax = largestValue + 20;
            
                Debug.Log("yMax updated");
            }
                
        }

        // cycle through and plot graph
        GameObject previousCircleObj = null;
        for (int i = 0; i < dataArray.Length; i++)
        {
            float xPos = 200 + ((i+1) * xInterval);
            float yPos = (dataArray[i] / yMax) * graphHeight;
            if (dataArray[i] != 0)
            {
                GameObject circleObj = CreateCircle(new Vector2(xPos, yPos));
            if (previousCircleObj != null)
            {
                CreateDotConnection(previousCircleObj.GetComponent<RectTransform>().anchoredPosition, circleObj.GetComponent<RectTransform>().anchoredPosition);
            }
            previousCircleObj = circleObj;
            }
        }

        // display axis values and grid
        // as yMax changes, axis and grid will adjust
        int axisLabelSeparation = 10;

        // adjust spacing so labels aren't cramped when plotting high values
        if (yMax > 120)
            axisLabelSeparation = 20;
        if (yMax > 150)
            axisLabelSeparation = 30;
        
        // as yMax changes, number of labels changes
        int axisLabelCount = (int)Math.Ceiling(yMax / axisLabelSeparation);

        
        for (int i = 0; i < axisLabelCount; i++)
        {
            GameObject yAxisLabel = Instantiate(graphYAxisLabelPrefab);
            GameObject gridDash = Instantiate(gridDashPrefab);

            graphYAxisLabelRect = yAxisLabel.GetComponent<RectTransform>();
            gridDashRect = gridDash.GetComponent<RectTransform>();

            graphYAxisLabelRect.SetParent(graphContainerRect, false);
            gridDashRect.SetParent(graphYAxisLabelRect, false);

            float normalizedValue = (float)i * (1f / axisLabelCount);
            graphYAxisLabelRect.anchoredPosition = new Vector2(10f, 30f + (normalizedValue * graphHeight));
            gridDashRect.anchoredPosition = new Vector2(170f, 0);

            yAxisLabel.GetComponent<TextMeshProUGUI>().text = (i * axisLabelSeparation).ToString();
        }
        
    }

    private GameObject CreateCircle(Vector2 anchoredPos)
    {
        GameObject circle = new GameObject("circle", typeof(Image));
        circle.transform.SetParent(graphContainerRect, false);
        circle.GetComponent<Image>().sprite = circleSprite;
        RectTransform rectTransform = circle.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = anchoredPos;
        rectTransform.sizeDelta = new Vector2(25, 25);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        return circle;
    }

    private void CreateDotConnection(Vector2 dotPositionA, Vector2 dotPositionB)
    {
        GameObject dotConnection = new GameObject("dotConnection", typeof(Image));
        dotConnection.transform.SetParent(graphContainerRect, false);
        dotConnection.GetComponent<Image>().color = new Color(1, 1, 1, 0.5f); // semi-transparent
        RectTransform rectTransform = dotConnection.GetComponent<RectTransform>();
        Vector2 direction = (dotPositionB - dotPositionA).normalized;
        float distance = Vector2.Distance(dotPositionA, dotPositionB);
        rectTransform.anchorMin = new Vector2(0,0);
        rectTransform.anchorMax = new Vector2(0,0);
        rectTransform.sizeDelta = new Vector2(distance, 10f);
        rectTransform.anchoredPosition = dotPositionA + (direction * distance * 0.5f);

        rectTransform.localEulerAngles = new Vector3(0, 0, UtilsClass.GetAngleFromVectorFloat(direction));
    }
    
}
