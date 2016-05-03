using UnityEngine;
using System.Collections;
using System;

public class CameraRotateEffect : MonoBehaviour {
	Time time;
	float lastAngle;
	public GameObject sphere;

	public GameObject[] audioSources;

	// Use this for initialization
	void Start () {
		lastAngle = 0;
		
	
		sphere =  GameObject.Find("Sphere");

	}
	
	// Update is called once per frame
	void Update() {
		float diff = Quaternion.Euler (0, -Input.compass.trueHeading, 0).eulerAngles.y - lastAngle;
		Camera.main.transform.RotateAround(sphere.transform.position, Vector3.up, diff);
		for(int i = 0 ; i < audioSources.Length ; i++){
			audioSources[i].transform.LookAt (Camera.main.transform);
			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer>()[0];
			renderer.color = new Color(0f, 0.0f, 1.0f, 1f); 
		}
		sphere.transform.LookAt (Camera.main.transform);

		lastAngle = Mathf.Lerp(lastAngle, Quaternion.Euler (0, -Input.compass.trueHeading, 0).eulerAngles.y, Time.time);

	}
}
