using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DeviceManager : MonoBehaviour
{
    private ConnectionManager connectionManager;
    public string deviceName, deviceStrength;
    private string deviceAddress;

    void Start()
    {
        connectionManager = GameObject.Find("ConnectionManager").GetComponent<ConnectionManager>();
        deviceName = transform.Find("Name").GetComponent<TextMeshProUGUI>().text;
        deviceAddress = transform.Find("Address").GetComponent<TextMeshProUGUI>().text;
        deviceStrength = transform.Find("Strength").GetComponent<TextMeshProUGUI>().text;
    }

    public void Connect()
    {
        connectionManager.ConnectDevice(deviceAddress, deviceName, deviceStrength);
    }
}
