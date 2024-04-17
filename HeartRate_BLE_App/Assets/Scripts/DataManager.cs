using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataManager : MonoBehaviour
{

    private int[] dataArray = new int[16]{0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0};
    [SerializeField] private WindowGraph windowGraph;

    public void NewData(byte[] dataBytes)
    {
        Debug.Log("NewData() called");
        // shift data array left so dataArray[9] can be the new data
        dataArray = ShiftIntArrayLeft(dataArray);

        // convert byte data to int
        int newData = ConvertByteToInt(dataBytes[0]);

        // store int at dataArray[9]
        dataArray[dataArray.Length-1] = newData;

        // update graph plot
        UpdateGraph();
    }

    private int[] ShiftIntArrayLeft(int[] array)
    {
        for (int i = 0; i < array.Length-1; i++)
        {
            array[i] = array[i+1];
        }

        return array;
    }

    private int ConvertByteToInt(byte dataByte)
    {
        int data = (int)dataByte;

        return data;
    }

    private void UpdateGraph()
    {
        windowGraph.ClearGraph();
        windowGraph.ShowGraph(dataArray);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
