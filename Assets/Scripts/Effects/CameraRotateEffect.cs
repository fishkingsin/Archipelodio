using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;
using System.Runtime.InteropServices;



public class CameraRotateEffect : MonoBehaviour
{
	
	private float currentDegree;
	private float diff;
	void Start ()
	{
		currentDegree = 0.0f;
	}
	
	// Update is called once per frame
	void Update ()
	{
		float degree = 0;
		if (!Input.compass.enabled) {
			degree = Time.time * 5;
		}else{
			degree = Mathf.Round(Input.compass.trueHeading);
		}
		
		diff = -degree - currentDegree ; 
		transform.Rotate (0,diff,0);
		currentDegree = -degree;

	}

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
