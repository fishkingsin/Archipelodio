using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class SubmitLocation : MonoBehaviour
{
	public string url = "http://www.mb09.com/ARCHIPELAUDIO/api/update";
	// Use this for initialization
	public double latitude;
	public double longitude;
	public double altitude;
	public double heading;

	IEnumerator Start ()
	{
//		InvokeRepeating ("IntervalFunction", 0.0f, 10.0F);
		latitude = 0;
		longitude = 0;
		altitude = 0;
		if (!Input.location.isEnabledByUser)
			yield break;

		// Start service before querying location
		Input.location.Start ();

		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0) {
			yield return new WaitForSeconds (1);
			maxWait--;
		}
		Input.compass.enabled = true;
		Input.gyro.enabled = true;

		// Service didn't initialize in 20 seconds
		if (maxWait < 1) {
			Debug.Log ("Timed out");
			yield break;
		}
		if (Input.location.status == LocationServiceStatus.Failed) {
			Debug.Log ("Unable to determine device location");
			yield break;
		} else {
			// Access granted and location value could be retrieved
			Debug.Log ("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);


		}

		// Stop service if there is no need to query location updates continuously
		StartCoroutine (sendLocation ());

	}
	
	// Update is called once per frame
	void Update ()
	{
	
	}

//	void IntervalFunction ()
//	{
//		
//		if (Input.location.status == LocationServiceStatus.Running) {
//			// Access granted and location value could be retrieved
//			Debug.Log ("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
//			latitude = Input.location.lastData.latitude;
//			longitude = Input.location.lastData.longitude;
//			altitude = Input.location.lastData.altitude;
//			if (Input.compass.enabled) {
//				heading = Input.compass.trueHeading;
//			}
//
//
//		}
//		sendLocation ();
////		InvokeRepeating ("IntervalFunction", 10.0f, 1.0F);
//	}

	IEnumerator sendLocation ()
	{
		
		yield return new WaitForSeconds (3.0f);
		WWWForm form = new WWWForm ();
		try {
			if (Input.location.status != LocationServiceStatus.Failed) {
				form.AddField ("loclat", Input.location.lastData.latitude.ToString ());
				form.AddField ("loclong", Input.location.lastData.longitude.ToString ());
				form.AddField ("heading", Input.compass.trueHeading.ToString ());
				form.AddField ("altitude", Input.location.lastData.altitude.ToString ());
				Debug.Log ("Verbose: form :" + form.ToString ());
				#if UNITY_EDITOR
				form.AddField ("uid", "debugger");
				#else
			form.AddField ("uid", SystemInfo.deviceUniqueIdentifier);
				#endif
			}
		} catch (Exception e) {
			Debug.Log ("Error :" + e);  
		}
		WWW www = new WWW (url, form);

		// this is what you need to add
		yield return www;


		if (www.error != null) {
			Debug.Log ("Error uploading: " + www.error);
		} else {
			Debug.Log ("Finished uploading data");  
		}
		
	}

	public override string ToString ()
	{
		if (Input.location.status != LocationServiceStatus.Running) {
			return "Faile to read Location";
		}
		return "latitude: " + Input.location.lastData.latitude +
		"\n| longitude: " + Input.location.lastData.longitude +
		"\n| altitude: " + Input.location.lastData.altitude +
		"\n| trueHeading: " + Input.compass.trueHeading +
		"\n| magneticHeading: " + Input.compass.magneticHeading;
	}
}
