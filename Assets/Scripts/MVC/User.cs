using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public class User : MonoBehaviour
{
	public delegate void UserDeadDelegate (GameObject obj, string uid);

	public UserDeadDelegate userDeadDelegate;

	public delegate AudioClip GetAudioClipDelegate (AudioSource audioSource);

	public GetAudioClipDelegate getClipDelegate;

	public float maxAge;
	public float age;
	public string uid;
	public GameObject parentRef;
	public GameObject centerRef;
	const string glyphs = "abcdefghijklmnopqrstuvwxyz0123456789";
	//add the characters you want
	AudioSource audioSource;
	// Use this for initialization
	void Start ()
	{
		
		int charAmount = UnityEngine.Random.Range (0, glyphs.Length); //set those to the minimum and maximum length of your string
		for (int i = 0; i < charAmount; i++) {
			uid += glyphs [UnityEngine.Random.Range (0, glyphs.Length)];
		}
		age = UnityEngine.Random.value * maxAge + maxAge * 0.5f;
		audioSource = GetComponent <AudioSource> ();
		audioSource.loop = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		if (audioSource != null) {
			if (!audioSource.isPlaying) {
				if (getClipDelegate != null) {
					Debug.Log ("User : "+uid+" player stop load new ");
					AudioClip audioClip = getClipDelegate (audioSource);
					if (audioClip != null) {
						
						audioSource.clip = audioClip;
						audioSource.volume = 0;
						audioSource.Play ();
						StartCoroutine (AudioFadeOut.FadeIn (audioSource, 0.5f));
					}
				}
			}
				

		}
		age = age * 0.9999f;
		if (age < 0.1f) {
			Debug.Log (uid + " : dead");
			if (userDeadDelegate != null) {
				userDeadDelegate (parentRef, uid);
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
