using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public class User : MonoBehaviour {
	public delegate void UserDeadDelegate(GameObject obj);
	public UserDeadDelegate userDeadDelegate;
	public float maxAge;
	public float age;
	public string uid;
	public GameObject parentRef;
	public GameObject centerRef;
	const string glyphs= "abcdefghijklmnopqrstuvwxyz0123456789"; //add the characters you want
	// Use this for initialization
	void Start () {
		
		int charAmount = UnityEngine.Random.Range(0, glyphs.Length); //set those to the minimum and maximum length of your string
		for(int i=0; i<charAmount; i++)
		{
			uid += glyphs[UnityEngine.Random.Range(0, glyphs.Length)];
		}
		age = UnityEngine.Random.value * maxAge + maxAge * 0.5f;
	}
	
	// Update is called once per frame
	void Update () {

		age = age *0.99f;
		if (age < 0) {
			Debug.Log (uid +" : dead");
			if (userDeadDelegate!=null) {
				userDeadDelegate (parentRef);
			}
		}
		if (centerRef) {
			float dist = Math.Abs (Vector3.Distance (transform.position, centerRef.transform.position));

			SpriteRenderer renderer = GetComponents<SpriteRenderer> () [0];
			renderer.color = HSBColor.ToColor (
				new HSBColor (
					Utils.Map (150.0f, 0.0f, 256.0f, 0.0f, 1.0f),
					Utils.Map (dist, 10.0f, 0.0f, 0.0F, 1.0f),
					1.0f,
					1f)); 
		}
		transform.LookAt (Camera.main.transform);

	}
}
