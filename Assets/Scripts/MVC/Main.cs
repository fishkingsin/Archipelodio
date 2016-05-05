using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;

public class Main : MonoBehaviour {
	//top left: (gps format is 22.60,113.82)
	public static float TOP = 22.60f;
	public static float LEFT = 113.82f;

	//bottom right: (gps format is 22.12, 114.42)
	public static float BOTTOM = 22.12f;
	public static float RIGHT = 114.42f;

	public AudioSource audioSourceRef;
	public Hashtable audioSources;
	public GameObject canvas;
	public GameObject user;
	public GameObject sphere;
	public Hashtable users;
	public GameObject getUserObject ;
	private bool isShowing;
	public int numObjects=10;
//	float range = 5.0f;
//	float range_h;
	float max = 5.0f;

	GetUsers getUser;

	AudioClip[] soundfields;
	AudioClip[] dialogs;
	void Start() {
		
//		range_h = range*0.5f;
		InvokeRepeating("IntervalFunction", 1.0f, 2.0F);
		users = new Hashtable();
		audioSources = new Hashtable ();
//		for (int i = 0; i < numObjects; i++) {
//			GameObject e = (GameObject)Instantiate (user);
//			e.transform.position = new Vector3 (UnityEngine.Random.value*range-range_h, UnityEngine.Random.value*range_h, UnityEngine.Random.value*range-range_h);
//			e.transform.localScale = new Vector3 (-0.5f, -0.5f, -0.5f);
//			users.Add (e);
//			(e.GetComponent<User> ()).parent = e;
//			(e.GetComponent<User>()).userDeadDelegate += UserDead;
//			if(i<audioSources.Length){
//				audioSources[i] = users[i].AddComponent <AudioSource>();
//			}
//		}

		string path = "Audios/Dialog";
		dialogs = Utils.GetAtPath<AudioClip> (path);
//		for (int i = 0; i < dialogs.Length; i++) {
//			Debug.Log (path + ": -> "+dialogs[i]);
//		}

		path = "Audios/Soundfields";
		soundfields = Utils.GetAtPath<AudioClip> (path);
//		for (int i = 0; i < soundfields.Length; i++) {
//			Debug.Log (path + ": -> "+soundfields[i]);
//		}
		getUser = getUserObject.GetComponent<GetUsers> ();
		getUser.fetchedUserDelegate += UserFetched;
	}
	void UserDead(GameObject obj){
		AudioSource audioSource = obj.GetComponent <AudioSource>();
		StartCoroutine (AudioFadeOut.FadeOut (audioSource, 0.5f));
		Destroy (obj,0.5f);

	}
	void IntervalFunction(){
		Debug.Log ("Main IntervalFunction");
	}
	void Update () {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			isShowing = !isShowing;
			canvas.SetActive(isShowing);

		}

	}

	void UserFetched(string uid , float longitude, float latitude, float altutide){
		foreach (string key in users.Keys) {
			Debug.Log ("key : " + key);
		}
		if (!users.ContainsKey (uid)) {
			GameObject e = (GameObject)Instantiate (user);
			//map lat lonig alt to 3d 
			float x = (float)Utils.Mapf(longitude, LEFT, RIGHT, -max, max,false);
			float y = (float)Utils.Mapf (latitude, TOP, BOTTOM, -max, max,true);
			float z = (float)Utils.Mapf (altutide, TOP, BOTTOM, 0.0f, max,true);
			Debug.Log ("uid :"+uid +"| x: " + x + "| y: " + y + "| z: " + z);
			e.transform.position = new Vector3 (x,y,z);
//			e.transform.position = new Vector3 (
//				UnityEngine.Random.value * range - range_h,
//				UnityEngine.Random.value * range_h,
//				UnityEngine.Random.value * range - range_h);
			e.transform.localScale = new Vector3 (-0.5f, -0.5f, -0.5f);
			User u = e.GetComponent<User> ();
			u.uid = uid;
			u.parentRef = e;
			u.centerRef = sphere;
			u.userDeadDelegate += UserDead;
			//attach adio source if needed
			if (audioSources.Count < numObjects) {
				AudioSource ac = e.AddComponent <AudioSource> ();
				audioSources [uid] = ac;
				ac.clip = soundfields [(int)(UnityEngine.Random.value * soundfields.Length)];
				ac.Play ();
				ac.volume = 0.0f;
				StartCoroutine (AudioFadeOut.FadeIn (ac, 0.5f));
			}

//			audioSources[i] = e.AddComponent <AudioSource>();
			

			users [uid] = e;

		} else {
			GameObject o = (GameObject)users [uid];
			User u = o.GetComponent<User> ();
			u.age = 100.0f;
		}
	}




}
