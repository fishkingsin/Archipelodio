using UnityEngine;
using System.Collections;
using System;

public class GetUsers : MonoBehaviour
{
	public string url = "http://www.mb09.com/ARCHIPELAUDIO/api/users";
	// Use this for initialization
	void Start ()
	{
		InvokeRepeating ("IntervalFunction", 1.0f, 2.0F);
		fetchUser ();

	}

	IEnumerator fetchUser ()
	{

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
	}

	void IntervalFunction ()
	{
		
		StartCoroutine ("fetchUser");
	}

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
			"| loclat: " + obj ["loclat"].n +
			"| loclong: " + obj ["loclong"].n +
			"| heading: " + obj ["heading"].n +
			"| altitude: " + obj ["altitude"].n +
			"| timestamp: " + obj ["timestamp"].str +
			"| tester: " + obj ["tester"].b);

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
