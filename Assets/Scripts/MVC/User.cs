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
	public int weight;
	public float  birth;

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
		birth = Time.realtimeSinceStartup;
	}

	IEnumerator UserIsDying ()
	{
		yield return new WaitForSeconds (2);
		if (userDeadDelegate != null) {
			userDeadDelegate (uid);
			userDeadDelegate = null;
		}
	}
	// Update is called once per frame
	void Update ()
	{
		if (age <= 0) {
			if (userDeadDelegate != null) {
				
				StartCoroutine (AudioFadeOut.FadeOut (audioSource, 1.0f));
				StartCoroutine (UserIsDying ());



			}
		} else {
			age -= 1;
			float s = Mathf.Min (age, 500.0f) / 500.0f;
			float m = Utils.Mapf (s, 0.0f, 1.0f, 0.1f, 1.0f, true);
			transform.localScale = mScale * m;

			if (audioSource != null) {
				if (audioSource.isPlaying) {
					weight = 0;
				} else if (!audioSource.isPlaying && audioSource.clip != null) {
					audioSource.volume = s;

					if (audioSourceCompleted != null) {
						
						audioSourceCompleted (audioSource, uid);

					}

				} else {
					weight++;
				}


			}
			if (centerRef) {


				float dist = Math.Max (0.1f, Math.Min (5.0f, Math.Abs (Vector3.Distance (transform.position, centerRef.transform.position))));

				transform.localScale = mScale * Utils.Mapf (dist, 0.0f, 5.0f, mScale.x * 0.5f, mScale.x , true);


				SpriteRenderer renderer = GetComponents<SpriteRenderer> () [0];
				renderer.color = HSBColor.ToColor (
					new HSBColor (
						Utils.Mapf (Mathf.Min (dist, 5), 0.0f, 5, 0.60833333333333f, 0.51388888888889f , true),
						Utils.Mapf (dist, 10.0f, 0.0f, 0.61F, 0.85f , true),
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
