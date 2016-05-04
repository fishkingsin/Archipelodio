using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;

public class Main : MonoBehaviour {
	public GameObject canvas;
	public GameObject enemy;
	public GameObject sphere;
	public GameObject[] enemis;
	private bool isShowing;
	public int numObjects=100;

	void Start() {
		InvokeRepeating("IntervalFunction", 0, 2.0F);
		enemis = new GameObject[numObjects];
		for (int i = 0; i < numObjects; i++) {
			GameObject e = Instantiate (enemy);
			e.transform.position = new Vector3 (UnityEngine.Random.value*10-5, UnityEngine.Random.value*5, UnityEngine.Random.value*10-5);
			e.transform.localScale = new Vector3 (-0.5f, -0.5f, -0.5f);
			enemis[i]=e;
		}
	}
	void IntervalFunction(){
		Debug.Log ("Main IntervalFunction");
	}
	void Update () {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			isShowing = !isShowing;
			canvas.SetActive(isShowing);

		}

		for(int i = 0 ; i < enemis.Length ; i++){
			float dist = Math.Abs(Vector3.Distance(enemis[i].transform.position, sphere.transform.position));

			SpriteRenderer renderer = enemis[i].GetComponents<SpriteRenderer>()[0];
			renderer.color = HSBColor.ToColor(
				new HSBColor(
					Utils.Map(150.0f,0.0f,256.0f,0.0f,1.0f),
					Utils.Map(dist,10.0f, 0.0f, 0.0F, 1.0f),
					1.0f,
					1f)); 
			enemis[i].transform.LookAt (Camera.main.transform);

		}

	}




}
