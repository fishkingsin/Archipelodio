using UnityEngine;
using System.Collections;
using System;
using AssemblyCSharp;

public class User : MonoBehaviour
{
	public delegate void UserDeadDelegate (string uid);

	public UserDeadDelegate userDeadDelegate;

	public delegate AudioClip GetAudioClipDelegate (AudioSource audioSource);

	public GetAudioClipDelegate getClipDelegate;

	public delegate void AudioCompletedDelegate (string uid);

	public AudioCompletedDelegate audioSourceCompleted;



	public int maxAge;
	public int age;
	public string uid;

	public GameObject centerRef;

	//add the characters you want
	AudioSource audioSource;
	// Use this for initialization
	Vector3 mScale;

	void Start ()
	{
		mScale = transform.localScale;
		age = maxAge;//UnityEngine.Random.value * maxAge + maxAge * 0.5f;
		audioSource = GetComponent <AudioSource> ();
		audioSource.loop = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
		


		if (age <= 0) {

			if (userDeadDelegate != null) {
//				Debug.Log (uid + " : dead");	
				userDeadDelegate (uid);

				userDeadDelegate = null;
			}
		} else {
			age -= 1;
			float s = Mathf.Min (age,100.0f) / 100.0f;

			transform.localScale = mScale * s;

			if (audioSource != null) {

				if (!audioSource.isPlaying && audioSource.clip != null) {
					audioSource.volume = s;
					if (getClipDelegate != null) {
//						Debug.Log ("User : " + uid + " player stop load new ");
						if (audioSourceCompleted != null) {
							audioSourceCompleted (uid);
						}
//						AudioClip audioClip = getClipDelegate (audioSource);
//						if (audioClip != null) {
//												
//							audioSource.clip = audioClip;
//							audioSource.volume = 0;
//							audioSource.Play ();
////							StartCoroutine (AudioFadeOut.FadeIn (audioSource, 0.5f));
//						}
					}
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

		}

		transform.LookAt (Camera.main.transform);

	}
	public void recharge(){
		age = maxAge;
	}
}
