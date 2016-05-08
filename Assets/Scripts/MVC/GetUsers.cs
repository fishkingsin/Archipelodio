using UnityEngine;
using System.Collections;
using System;

public class GetUsers : MonoBehaviour
{
	public GameObject user;
	#if UNITY_EDITOR || DEVELOPMENT_BUILD
	public string url = "http://www.mb09.com/ARCHIPELAUDIO/api/users";
	#elif
	public string url = "http://www.moneme.com/Archipelodio/api/api/users";
	#endif
	// Use this for initialization
	public delegate void FetchedUserDelegate(string a, float b, float c, float d);
	public FetchedUserDelegate fetchedUserDelegate;
	IEnumerator Start ()
	{
		Debug.Log ("Verbose: GetUsers Start");
		InvokeRepeating("repeat",0.0f, 10.0f);
//		StartCoroutine (fetchUser ());
//		yield return fetchUser ();
		yield return null;
	}
	void repeat(){
//		Debug.Log("GetUsers repeat");
		StartCoroutine(fetchUser ());
	}
	IEnumerator fetchUser ()
	{
//		Debug.Log ("Verbose: fetchUser");

		WWW www = new WWW (url);
		yield return www;
		try {
			if (www.error == null) {
			
				Processjson (www.text);
			} else {
				Debug.Log ("ERROR: " + www.error);
			}
		} catch (Exception e) {
			Debug.Log ("Error: " + e);
		}
	}
		

	private void Processjson (string jsonString)
	{
//		Debug.Log ("Processjson: " + jsonString);
		JSONObject jsonvale = new JSONObject (jsonString);

		accessData (jsonvale);

	}

	void accessData (JSONObject obj)
	{
		switch (obj.type) {
		case JSONObject.Type.OBJECT:
			Debug.Log ("lid:" + obj ["lid"].str +
			"| uid: " + obj ["uid"].str +
			"| loclat: " + obj ["loclat"].str +
			"| loclong: " + obj ["loclong"].str +
			"| heading: " + obj ["heading"].str +
			"| altitude: " + obj ["altitude"].str +
			"| timestamp: " + obj ["timestamp"].str +
			"| tester: " + obj ["tester"].str);
			float loclong,loclat,altitude;
			float.TryParse (obj ["loclong"].str, out loclong); 
			float.TryParse(obj ["loclat"].str, out loclat);
			float.TryParse(obj ["altitude"].str, out altitude);
			try{
			fetchedUserDelegate (obj ["uid"].str, 
				loclong,
				loclat,
				altitude
				);
			}catch(Exception exception ){
				Debug.Log(exception.Message);
			}
			break;
		case JSONObject.Type.ARRAY:
			foreach (JSONObject j in obj.list) {
				accessData (j);

			}
			break;

		}
	}
	// Update is called once per frame
	void Update ()
	{
		
	}
}
