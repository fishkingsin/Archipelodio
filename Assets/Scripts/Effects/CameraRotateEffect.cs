using UnityEngine;
using System.Collections;
using System;

public class CameraRotateEffect : MonoBehaviour {
	Time time;

	GameObject sphere;
	GameObject audioSource;
	GameObject audioSource1;
	GameObject audioSource2;

	GameObject[] audioSources;
//	GameObject[] spheres;

	int numSphere;
	// Use this for initialization
	void Start () {
		numSphere = 3;
		// Fixed update will be performed 20 time per second:

		// Getting MainCamera gameobject:

		sphere =  GameObject.Find("Sphere");
		audioSources = new GameObject[numSphere];
//		spheres = new GameObject[numSphere];
		audioSources[0] = GameObject.Find("Env Audio Source");
		audioSources[1] = GameObject.Find("Env Audio Source 1");
		audioSources[2] = GameObject.Find("Env Audio Source 2");

//		spheres = new GameObject[numSphere];


//		for(int i = 0 ; i < numSphere ; i++){
//
//			spheres[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);;
//			try{
//				Material newMat = Resources.Load("LightBlue", typeof(Material)) as Material;
//				spheres[i].GetComponent<Renderer>().material = newMat;
//
//			}catch(Exception e){
//				Debug.Log("Error: "+e);
//			}
//		}

	}
	
	// Update is called once per frame
	void FixedUpdate() {
		for(int i = 0 ; i < numSphere ; i++){
			audioSources[i].transform.RotateAround(sphere.transform.position, Vector3.up,  20 * Time.deltaTime);
//			audioSources[i].transform.RotateAround(sphere.transform.position, Vector3.up, Input.compass.trueHeading);
//			spheres[i].transform.position = audioSources[i].transform.position;
			audioSources[i].transform.LookAt (Camera.main.transform);
			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer>()[0];
			renderer.color = new Color(0f, 0.0f, 1.0f, 1f); 
		}
		sphere.transform.LookAt (Camera.main.transform);

	}
}
