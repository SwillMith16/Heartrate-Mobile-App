using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ConnectionManager : MonoBehaviour
{
    public DeviceListManager deviceListManager;
    public DataManager dataManager;
    public TextMeshProUGUI connectedNameTMP, connectedStrengthTMP;

    public Button disconnectButton;
    

    private bool connected, subscribed, attemptConnection, attemptSubscription, attemptUnsubscription, attemptDisconnection, attemptRequestMtu;
    private string connectedAddress = "";
    private string connectedName = "";
    private string connectedStrength = "";

    private string ServiceUUID = "2A36";
	private string CharacteristicUUID = "2A37";

    private float readTimer, delayTimer;

    void Start()
    {
        connected = false;
        subscribed = false;
        disconnectButton.interactable = false;

        attemptConnection = false;
        attemptRequestMtu = false;
        attemptSubscription = false;
        attemptUnsubscription = false;
        attemptDisconnection = false;

        readTimer = 0f;
        delayTimer = -1f;
    }


    void Update()
    {
        if (delayTimer >= 0) delayTimer -= Time.deltaTime;

        deviceListManager.connected = this.connected;

        // connect to a device - working
        if (attemptConnection)
        {
            attemptConnection = false; // don't repeat connection
            BluetoothLEHardwareInterface.ConnectToPeripheral(connectedAddress, null, null, (returnedAddress, serviceUUID, characteristicUUID) => {
                
                if (IsEqual (serviceUUID, ServiceUUID))
                {
                    if (IsEqual (characteristicUUID, CharacteristicUUID))
                    {
                        connected = true;
                        
                        connectedNameTMP.text = connectedName;
                        connectedStrengthTMP.text = connectedStrength;

                        
                        attemptRequestMtu = true; // move on to requesting MTU next
                    }
                }
            }, (disconnectedAddress) => {
                connected = false;
                disconnectButton.interactable = false;
                connectedAddress = "";
                Debug.Log("Disconnected: " + connectedName);
                connectedName = "";

                connectedNameTMP.text = "NOT CONNECTED";
                connectedStrengthTMP.text = "--";
            });
        }

        // update MTU to set data size of transmissions - working
        if (attemptRequestMtu)
        {
            attemptRequestMtu = false;

            BluetoothLEHardwareInterface.RequestMtu(connectedAddress, 185, (address, newMTU) =>
            {
                attemptSubscription = true; // move on to attempting to subscribe next
            });
        }

        // subscribe to characteristic - working
        if (attemptSubscription)
        {
            attemptSubscription = false;
            Debug.Log("Subscribing to: " + connectedName);
            BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (connectedAddress, FullUUID(ServiceUUID), FullUUID(CharacteristicUUID), (notifyAddress, notifyCharacteristic) =>
            {
                Debug.Log("Characteristic Notifications setup...");
                subscribed = true;
                delayTimer = 2f;
                disconnectButton.interactable = true;

            }, (address, characteristicUUID, bytes) =>
            {
                string byteString = "";
                foreach (byte b in bytes)
                    byteString += string.Format ("{0:X2}", b);

                // Debug.Log("Received data: " + byteString);
                if (bytes != new byte[2]{0,0})
                    dataManager.NewData(bytes);
            });
        }

        // unsubscribe - working
        if (attemptUnsubscription)
        {
            attemptUnsubscription = false;
            Debug.Log("Attempting unsubscription");
            BluetoothLEHardwareInterface.UnSubscribeCharacteristic (connectedAddress, FullUUID(ServiceUUID), FullUUID(CharacteristicUUID), (response) => {
                Debug.Log("Unsubscribed from: " + connectedName);
                attemptDisconnection = true; // move on to attempting disconnection next
            });
        }

        // disconnect - working
        if (attemptDisconnection)
        {
            attemptDisconnection = false;
            Debug.Log("Attempting disconnection");
            BluetoothLEHardwareInterface.DisconnectAll();
        }

        // update RSSI - working
        if (connected)
        {
            BluetoothLEHardwareInterface.ReadRSSI(connectedAddress, (connectedAddress, rssi) => {
                int rssiInverted = rssi * -1;
                connectedStrengthTMP.text = rssiInverted.ToString();
            });
        }
    }

    public void ConnectDevice(string address, string name, string strength)
    {
        connectedAddress = address;
        connectedName = name;
        connectedStrength = strength;
        if (!connected)
        {
            attemptConnection = true;
        }
    }

    public void DisconnectDevice()
    {
        if (subscribed)
            attemptUnsubscription = true;
    }

    string FullUUID (string uuid)
	{
		return "0000" + uuid + "-0000-1000-8000-00805F9B34FB";
	}
	
	bool IsEqual(string uuid1, string uuid2)
	{
		if (uuid1.Length == 4)
			uuid1 = FullUUID (uuid1);
		if (uuid2.Length == 4)
			uuid2 = FullUUID (uuid2);

		return (uuid1.ToUpper().Equals(uuid2.ToUpper()));
	}
}






/*


BluetoothLEHardwareInterface.SubscribeCharacteristicWithDeviceAddress (connectedAddress, FullUUID(ServiceUUID), FullUUID(CharacteristicUUID), (notifyAddress, notifyCharacteristic) =>
            {
                Debug.Log("Notification Received!");
                subscribed = true;
                delayTimer = 2f;
                disconnectButton.interactable = true;

                // notification has been received, read characteristic
                // BluetoothLEHardwareInterface.ReadCharacteristic(connectedAddress, FullUUID(ServiceUUID), FullUUID(CharacteristicUUID), (characteristic, bytes) =>
                // {
                //     string byteString = "";
                //     foreach (byte b in bytes)
                //         byteString += string.Format ("{0:X2}", b);

                //     Debug.Log("Received data: " + byteString);
                // });

            }, (address, characteristicUUID, bytes) =>
            {
                string byteString = "";
                foreach (byte b in bytes)
                    byteString += string.Format ("{0:X2}", b);

                // Debug.Log("Received data: " + byteString);
                if (bytes != new byte[2]{0,0})
                    dataManager.NewData(bytes);
            });

*/