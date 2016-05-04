using UnityEngine;
using System.Collections;
using System;

public class User : MonoBehaviour {
	public float maxAge;
	public float age;
	public string uid;
	// Use this for initialization
	void Start () {
		Debug.Log ("User Created");
		age = UnityEngine.Random.value * maxAge + maxAge * 0.5f;
	}
	
	// Update is called once per frame
	void Update () {
		age = age * 0.01f;
		if (age < 1) {
			Destroy (this);
		}
		transform.localScale *= age;		
	}
}
