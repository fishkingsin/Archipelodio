using UnityEngine;
using System.Collections;
using System;

public class FPSDisplay : MonoBehaviour {
	float deltaTime = 0.0f;
	// Use this for initialization
	SubmitLocation submitLocationScript;
	void Start(){
		
		GameObject submitLocation = GameObject.Find("SubmitLocation");
		submitLocationScript  = submitLocation.GetComponent<SubmitLocation>();
	}
	void Update()
	{
		deltaTime += (Time.deltaTime - deltaTime) * 0.1f;
	}

	void OnGUI()
	{
		int w = Screen.width, h = Screen.height;

		GUIStyle style = new GUIStyle();

		Rect rect = new Rect(0, 0, w, h * 2 / 100);
		style.alignment = TextAnchor.UpperLeft;
		style.fontSize = h * 2 / 100;
		style.normal.textColor = new Color (0.0f, 0.0f, 0.5f, 1.0f);
		float msec = deltaTime * 1000.0f;
		float fps = 1.0f / deltaTime;
		try{
			string text = string.Format("{0:0.0} ms ({1:0.} fps)", msec, fps);
			GUI.Label(rect, text+"\n"+submitLocationScript.ToString(), style);
		}catch(Exception e){
			Debug.Log ("Eror: " + e);
		}

	}
}
