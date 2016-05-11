using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;
using System.Runtime.InteropServices;



public class CameraRotateEffect : MonoBehaviour
{
	//top left: (gps format is 22.60,113.82)
	public static float TOP = 22.60f;
	public static float LEFT = 113.82f;

	//bottom right: (gps format is 22.12, 114.42)
	public static float BOTTOM = 22.12f;
	public static float RIGHT = 114.42f;

	float max = 5.0f;

	private int currentDegree;
	private int diff;
	void Start ()
	{
		currentDegree = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
//		int degree = 0;
//		if (!Input.compass.enabled) {
//			degree = (int)(-Time.time * 5);
//		}else{
//			degree = (int)Mathf.Round(Input.compass.trueHeading);
//		}
		

//		StartCoroutine (RotateCamera((int)Mathf.Round(Input.compass.trueHeading)));
		#if UNITY_EDITOR
		transform.rotation = Quaternion.Euler (0, (int)Mathf.Round(Time.time*20), 0);
		#else
		transform.rotation = Quaternion.Euler (0,(int)Mathf.Round(Input.compass.trueHeading),0);
		#endif
//		currentDegree = -degree;

//		if(Input.location.status == LocationServiceStatus.Running){
//			float x = (float)Utils.Mapf (Input.location.lastData.longitude, LEFT, RIGHT, -max, max, false);
//			float y = (float)Utils.Mapf (Input.location.lastData.altitude, 0, 100, 0.0f, max, true);
//			float z = (float)Utils.Mapf (Input.location.lastData.latitude, TOP, BOTTOM, -max, max, true);
//			Debug.Log ( " | x: " + x + " | y: " + y + " | z: " + z);
//			transform.position = new Vector3 (x, y, z);
//		}

	}
//	IEnumerator RotateCamera(float toDegree){
//		while (Mathf.Abs (currentDegree-toDegree) > 1 ) {
//			
//			currentDegree = (int)Mathf.Lerp(currentDegree , -toDegree , 0.1f ); 
//			transform.rotation = Quaternion.Euler (0,currentDegree,0);
//
//			yield return null;
//		}
//
//	}
	public float Sum (params float[] customerssalary)
	{
		float result = 0;

		for (int i = 0; i < customerssalary.Length; i++) {
			result += customerssalary [i];
		}

		return result;
	}

	public float Average (params float[] customerssalary)
	{
		float sum = Sum (customerssalary);
		float result = (float)sum / customerssalary.Length;
		return result;
	}
}
