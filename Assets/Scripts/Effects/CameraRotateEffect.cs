using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;



public class CameraRotateEffect : MonoBehaviour
{

	public GameObject[] audioSources;

	private float _smoothTime = 0.1F;
	private float yVelocity = 0.1F;

	private float heading = 0;
	// Use this for initialization
	private float target; 
	void Start ()
	{
		for (int i = 0; i < audioSources.Length; i++) {
			audioSources [i].transform.LookAt (Camera.main.transform);
			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer> () [0];
			renderer.color = new Color (0f, 0.0f, 1.0f, 1f); 
		}
	
//		Camera.main.transform.LookAt (transform);
	}
	
	// Update is called once per frame
	void Update ()
	{

		transform.localRotation = Quaternion.Euler(0, 360.0f-heading, 0);

		for (int i = 0; i < audioSources.Length; i++) {
			audioSources [i].transform.LookAt (Camera.main.transform);
			float dist = Math.Abs (Vector3.Distance (audioSources [i].transform.position, transform.position));
			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer> () [0];
			renderer.color = HSBColor.ToColor (
				new HSBColor (
					Utils.Map (150.0f, 0.0f, 256.0f, 0.0f, 1.0f),
					Utils.Map (dist, 10.0f, 0.0f, 0.0F, 1.0f),
					1.0f,
					1f)); 
		}
		if (Input.compass.enabled) {
			target = -Input.compass.trueHeading;
			heading = Mathf.Lerp (heading, target, 0.005f);
		} else {
			target = -360 + Time.deltaTime * 0.1f;
			heading = Mathf.Lerp (heading, target, 0.005f);


		}

	}
}
