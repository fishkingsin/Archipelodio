using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;
using System.IO;
using System.Runtime.InteropServices;



public class CameraRotateEffect : MonoBehaviour
{
	private CircularBuffer<float> circularBuffer;
	private float heading = 0;
	private float target; 
	void Start ()
	{
		circularBuffer = new CircularBuffer<float> (20);

			
//		for (int i = 0; i < audioSources.Length; i++) {
//			audioSources [i].transform.LookAt (Camera.main.transform);
//			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer> () [0];
//			renderer.color = new Color (0f, 0.0f, 1.0f, 1f); 
////			audioSources [i] = Resources.Load("Audios/Dialog",typeof(AudioClip));i
//
//		}
	}
	
	// Update is called once per frame
	void Update ()
	{

		transform.localRotation = Quaternion.Euler(0, -heading, 0);
//		for (int i = 0; i < audioSources.Length; i++) {
//			audioSources [i].transform.LookAt (Camera.main.transform);
//			float dist = Math.Abs (Vector3.Distance (audioSources [i].transform.position, transform.position));
//			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer> () [0];
//			renderer.color = HSBColor.ToColor (
//				new HSBColor (
//					Utils.Map (150.0f, 0.0f, 256.0f, 0.0f, 1.0f),
//					Utils.Map (dist, 10.0f, 0.0f, 0.0F, 1.0f),
//					1.0f,
//					1f)); 
//		}
		if (Input.compass.enabled) {
			circularBuffer.Add (Input.compass.trueHeading);

			target = Average(circularBuffer.ToArray ());

			heading = Mathf.Lerp (heading, target ,0.9f);
		} else {
			circularBuffer.Add (Time.deltaTime * 0.1f);
			target = Average(circularBuffer.ToArray ());
			heading = Mathf.Lerp (heading, target, 0.005f);


		}

	}

	public float Sum(params float[] customerssalary)
	{
		float result = 0;

		for(int i = 0; i < customerssalary.Length; i++)
		{
			result += customerssalary[i];
		}

		return result;
	}
	public float Average(params float[] customerssalary)
	{
		float sum = Sum(customerssalary);
		float result = (float)sum / customerssalary.Length;
		return result;
	}
}
