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

	public delegate void AudioCompletedDelegate (AudioSource audioSource, string uid);

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
	IEnumerator UserIsDying(){
		yield return new WaitForSeconds(2);
		userDeadDelegate (uid);
	}
	// Update is called once per frame
	void Update ()
	{
		if (age <= 0) {
			if (userDeadDelegate != null) {
				
				StartCoroutine (AudioFadeOut.FadeOut (audioSource, 1.0f));
				StartCoroutine ( UserIsDying () );


				userDeadDelegate = null;
			}
		} else {
			age -= 1;
			float s = Mathf.Min (age, 100.0f) / 100.0f;

			transform.localScale = mScale * Utils.Map ( s , 0.0f, 1.0f, 0.1f, 1.0f);

			if (audioSource != null) {

				if (!audioSource.isPlaying && audioSource.clip != null) {
					audioSource.volume = s;
					if (getClipDelegate != null) {
						if (audioSourceCompleted != null) {
							audioSourceCompleted (audioSource,uid);
						}
					}
				}


			}
			if (centerRef) {


				float dist = Math.Abs (Vector3.Distance (transform.position, centerRef.transform.position));

				transform.localScale = mScale * Utils.Map (dist, 0.0f, 5.0f, mScale.x * 0.5f, mScale.x);


				SpriteRenderer renderer = GetComponents<SpriteRenderer> () [0];
				renderer.color = HSBColor.ToColor (
					new HSBColor (
						Utils.Map (dist, 0.0f, 5, 0.60833333333333f, 0.51388888888889f),
						Utils.Map (dist, 10.0f, 0.0f, 0.61F, 0.85f),
						1.0f,
						1f)); 
			}

		}

		transform.LookAt (Camera.main.transform);

	}

	public void recharge ()
	{
		age = maxAge;
	}
}
