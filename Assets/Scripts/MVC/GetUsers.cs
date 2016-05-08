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
	void Start ()
	{
		Debug.Log ("Verbose: GetUsers Start");
//		InvokeRepeating ("IntervalFunction", 0.0f, 10.0F);
		StartCoroutine ( fetchUser ());

	}

	IEnumerator fetchUser ()
	{
		yield return new WaitForSeconds (2.0f);
		Debug.Log ("Verbose: fetchUser");
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
		Debug.Log ("Verbose: fetchUser finished");
	}

//	void IntervalFunction ()
//	{
////		InvokeRepeating ("IntervalFunction", 10.0f, 1.0F);
////		StartCoroutine ("fetchUser");
//		fetchUser ();
//	}

	private void Processjson (string jsonString)
	{
		Debug.Log ("Processjson: " + jsonString);
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
			fetchedUserDelegate (obj ["uid"].str, 
				loclong,
				loclat,
				altitude
				);
			break;
		case JSONObject.Type.ARRAY:
			foreach (JSONObject j in obj.list) {
				accessData (j);

			}
			break;
//		case JSONObject.Type.STRING:
//			Debug.Log(obj.str);
//			break;
//		case JSONObject.Type.NUMBER:
//			Debug.Log(obj.n);
//			break;
//		case JSONObject.Type.BOOL:
//			Debug.Log(obj.b);
//			break;
//		case JSONObject.Type.NULL:
//			Debug.Log("NULL");
//			break;

		}
	}
	// Update is called once per frame
	void Update ()
	{
		
	}
}
