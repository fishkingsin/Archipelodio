using UnityEngine;
using System.Collections;
using System;

public class GetUsers : MonoBehaviour {
	public string url = "http://www.mb09.com/ARCHIPELAUDIO/api/users";
	// Use this for initialization
	IEnumerator Start() {
		InvokeRepeating("IntervalFunction", 2, 0.3F);
		return fetchUser ();

	}
	IEnumerator fetchUser(){
		WWW www = new WWW(url);
		yield return www;
		if (www.error == null)
		{
			Processjson(www.text);
		}
		else
		{
			Debug.Log("ERROR: " + www.error);
		}
	}
	IEnumerator IntervalFunction(){
		InvokeRepeating("IntervalFunction", 2, 0.3F);
		return fetchUser();
	}
	private void Processjson(string jsonString)
	{
//		Debug.Log ("Processjson: " + jsonString);
		JSONObject jsonvale = new JSONObject(jsonString);

		accessData(jsonvale);

	}

	void accessData(JSONObject obj){
		switch(obj.type){
		case JSONObject.Type.OBJECT:
			Debug.Log("lid:"+obj["lid"].str+
				"| uid: "+obj["uid"].str+
				"| loclat: "+obj["loclat"].n+
				"| loclong: "+obj["loclong"].n+
				"| heading: "+obj["heading"].n+
				"| altitude: "+obj["altitude"].n+
				"| timestamp: "+obj["timestamp"].str+
				"| tester: "+obj["tester"].b);

//			for(int i = 0; i < obj.list.Count; i++){
//				string key = (string)obj.keys[i];
//				JSONObject j = (JSONObject)obj.list[i];
//
//				Debug.Log(key);
//
//				Debug.Log(j);
//
//				accessData(j);
//
//			}
			break;
		case JSONObject.Type.ARRAY:
			foreach(JSONObject j in obj.list){
				accessData(j);

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
	void Update () {
	
	}
}
