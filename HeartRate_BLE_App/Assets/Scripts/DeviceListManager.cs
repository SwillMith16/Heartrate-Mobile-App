using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class DeviceListManager : MonoBehaviour
{
    [SerializeField] private GameObject ScannedDevicePrefab;

	public bool connected;

    private float _timeout;
	private float _startScanTimeout = 10f;
	private float _startScanDelay = 0.5f;
	private bool _startScan = true;
	private Dictionary<string, ScannedItem> _scannedItems;
	private const int numOfServices = 1;
	private string[] ServiceUUID = {"180D"};

	public void OnStopScanning()
	{
		BluetoothLEHardwareInterface.Log ("**************** stopping");
		BluetoothLEHardwareInterface.StopScan ();
	}



	void Start ()
	{
		BluetoothLEHardwareInterface.Log ("START SCANNING...");
		_scannedItems = new Dictionary<string, ScannedItem> ();

		BluetoothLEHardwareInterface.Initialize (true, false, () => {

			_timeout = _startScanDelay;
		}, 
		(error) => {
			Debug.Log("Error: " + error);
			BluetoothLEHardwareInterface.Log ("Error: " + error);

			if (error.Contains ("Bluetooth LE Not Enabled"))
				BluetoothLEHardwareInterface.BluetoothEnable (true);
		});
	}


    void Update ()
	{
		if (connected)
		{
			BluetoothLEHardwareInterface.StopScan ();
			_timeout = 0.1f;
		}
		else if (_timeout > 0f)
		{
			_timeout -= Time.deltaTime;
			if (_timeout <= 0f)
			{
				if (_startScan)
				{
					_startScan = false;
					_timeout = _startScanTimeout;

					BluetoothLEHardwareInterface.ScanForPeripheralsWithServices (ServiceUUID, null, (address, name, rssi, bytes) => {
                        int rssiInverted = rssi * -1;
						BluetoothLEHardwareInterface.Log ("item scanned: " + address);
						if (_scannedItems.ContainsKey (address))
						{
							var scannedItem = _scannedItems[address];
							scannedItem.RSSIObj.text = rssiInverted.ToString();
							BluetoothLEHardwareInterface.Log ("already in list " + address);
						}
						else
						{
							var newItem = Instantiate (ScannedDevicePrefab);
							if (newItem != null)
							{
								BluetoothLEHardwareInterface.Log ("item listed: " + address);
								newItem.transform.parent = transform;
								newItem.transform.localScale = Vector3.one;

								newItem.GetComponent<DeviceManager>().deviceName = name;
								newItem.GetComponent<DeviceManager>().deviceStrength = rssiInverted.ToString();

								var scannedItem = newItem.GetComponent<ScannedItem> ();
								if (scannedItem != null)
								{
									BluetoothLEHardwareInterface.Log ("item values set: " + address);
									scannedItem.AddressObj.text = address;
									scannedItem.NameObj.text = name;
									scannedItem.RSSIObj.text = rssiInverted.ToString();

									_scannedItems[address] = scannedItem;
								}
							}
						}
						
					}, true);
				}
				else
				{
					BluetoothLEHardwareInterface.StopScan ();
					_startScan = true;
					_timeout = _startScanDelay;
				}
			}
		}
	}
}
