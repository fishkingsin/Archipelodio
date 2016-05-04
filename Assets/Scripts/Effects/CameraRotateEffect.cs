using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;



public class CameraRotateEffect : MonoBehaviour {
	public GameObject sphere;
	public GameObject[] audioSources;

//	private float _smoothTime = 0.3F;
//	private float yVelocity = 0.0F;
	private Time time;
	private float lastAngle;
	private float heading = 0;
	// Use this for initialization
	void Start () {
		lastAngle = 0;
		
		for(int i = 0 ; i < audioSources.Length ; i++){
			audioSources[i].transform.LookAt (Camera.main.transform);
			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer>()[0];
			renderer.color = new Color(0f, 0.0f, 1.0f, 1f); 
		}
	
//		Camera.main.transform.LookAt (transform);
	}
	
	// Update is called once per frame
	void Update() {
		float diff = heading - lastAngle;
		Camera.main.transform.RotateAround(transform.position, Vector3.up, diff);
		for(int i = 0 ; i < audioSources.Length ; i++){
			audioSources[i].transform.LookAt (Camera.main.transform);
			float dist = Math.Abs(Vector3.Distance(audioSources[i].transform.position, transform.position));
			SpriteRenderer renderer = audioSources[i].GetComponents<SpriteRenderer>()[0];
			renderer.color = HSBColor.ToColor(
				new HSBColor(
					Utils.Map(150.0f,0.0f,256.0f,0.0f,1.0f),
					Utils.Map(dist,10.0f, 0.0f, 0.0F, 1.0f),
					1.0f,
					1f)); 
		}
		lastAngle = Mathf.Lerp (lastAngle, heading, Time.deltaTime); 
		heading = Mathf.Lerp(heading, -Input.compass.trueHeading, Time.deltaTime*0.5f);
		Camera.main.transform.LookAt (sphere.transform);
	}
}
