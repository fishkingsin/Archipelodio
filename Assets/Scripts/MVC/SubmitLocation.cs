using UnityEngine;
using System.Collections;

public class SubmitLocation : MonoBehaviour {
	public string url = "http://www.mb09.com/ARCHIPELAUDIO/api/update";
	// Use this for initialization
	public double latitude;
	public double longitute;
	public double altitude;
	IEnumerator Start () {
		latitude = 0;
		longitute = 0;
		altitude = 0;
		if (!Input.location.isEnabledByUser)
			yield break;

		// Start service before querying location
		Input.location.Start();

		// Wait until service initializes
		int maxWait = 20;
		while (Input.location.status == LocationServiceStatus.Initializing && maxWait > 0)
		{
			yield return new WaitForSeconds(1);
			maxWait--;
		}

		// Service didn't initialize in 20 seconds
		if (maxWait < 1)
		{
			Debug.Log("Timed out");
			yield break;
		}
		if (Input.location.status == LocationServiceStatus.Failed)
		{
			Debug.Log("Unable to determine device location");
			yield break;
		}
		else
		{
			// Access granted and location value could be retrieved
			Debug.Log("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);

			InvokeRepeating("IntervalFunction", 2, 0.3F);
		}

		// Stop service if there is no need to query location updates continuously


	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void IntervalFunction(){
		if (Input.location.status == LocationServiceStatus.Running) {
			// Access granted and location value could be retrieved
			Debug.Log ("Location: " + Input.location.lastData.latitude + " " + Input.location.lastData.longitude + " " + Input.location.lastData.altitude + " " + Input.location.lastData.horizontalAccuracy + " " + Input.location.lastData.timestamp);
			latitude = Input.location.lastData.latitude;
			longitute = Input.location.lastData.longitude;
			altitude = Input.location.lastData.altitude;

			sendLocation (Input.location.lastData.latitude,
				Input.location.lastData.longitude,
				Input.location.lastData.altitude,
				0);

		} else {
			sendLocation (0, 0, 0, 0);
		}		
	}

	IEnumerable sendLocation(double latitude, double longitude, double altitude, double heading){
		WWWForm form = new WWWForm ();

		form.AddField("loclat", latitude.ToString());
		form.AddField("loclong", longitude.ToString());
		form.AddField("heading", heading.ToString());
		form.AddField("altitude", altitude.ToString());
		form.AddField("uid", SystemInfo.deviceUniqueIdentifier);
		WWW www = new WWW(url, form);

		// this is what you need to add
		yield return www;


		if (www.error!=null) {
			Debug.Log( "Error uploading: " + www.error );
		}else {
			Debug.Log("Finished uploading data");  
		}
	}

	public override string ToString(){
		return "latitude: " + latitude +
			"| longitude: " + longitute +
			"| altitude: " + altitude;
	}
}
