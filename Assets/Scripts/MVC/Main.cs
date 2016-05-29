﻿using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;
using AssetBundles;
using System.Runtime.InteropServices;
using UnityEngine.Events;

public class Main : MonoBehaviour
{
	//top left: (gps format is 22.60,113.82)
	public static float TOP = 22.60f;
	public static float LEFT = 113.82f;

	//bottom right: (gps format is 22.12, 114.42)
	public static float BOTTOM = 22.12f;
	public static float RIGHT = 114.42f;
	public int maxNumUser;
	public Hashtable audioSources;
	public GameObject canvas;
	public GameObject user;
	public GameObject sphere;
	public GameObject center;
	public Hashtable users;
	public GameObject aboutCanvas;
	public GameObject getUserObject;
	public GameObject loadAssetObject;

	public int numObjects;
	public GameObject dicoCircle;
	float max = 5.0f;

	GetUsers getUser;
	LoadAssets loadAsset;

	List<string> soundfields;
	List<string> dialogs;

	public Hashtable assetBundleRef;
	AudioSource dialogAudioSourceRef;

	LocationInfo lastData;


	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		soundfields = new List<string> ();	
		dialogs = new List<string> ();	
		users = new Hashtable ();
		assetBundleRef = new Hashtable ();
		audioSources = new Hashtable ();

		getUser = getUserObject.GetComponent<GetUsers> ();
		getUser.fetchedUserDelegate += UserFetched;
//		AssetLoaded ("audio");

		loadAssetObject = GameObject.Find ("Load");
		try {
			loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
			loadAsset.assetLoadedDelegate += AssetLoaded;
			StartCoroutine (loadAsset.reload ());
		} catch (Exception e) {
			Debug.Log (e.ToString ());
		}

		dialogAudioSourceRef = null;
//		InvokeRepeating ("fireDialogInterval", 1.0f, 1.0f);
		aboutCanvas.SetActive (false);
	}

	void UserDead (string uid)
	{
		try {

			GameObject gameObject = (GameObject)users [uid];
			if (gameObject) {
				Debug.Log ("UserDead " + gameObject.ToString () + " of uid " + uid);
				if (gameObject.GetComponent<AudioSource> ()) {
					AudioSource audioSource = gameObject.GetComponent <AudioSource> ();
					if (audioSource.Equals (dialogAudioSourceRef)) {
						dialogAudioSourceRef = null;
					}
					audioSource.clip = null;
				
				}

				if (gameObject.GetComponent<User> ()) {
					Destroy (gameObject.GetComponent<User> (), 0.0f);
				}

				if (gameObject.GetComponent<AudioSource> ()) {
					Destroy (gameObject.GetComponent<AudioSource> (), 0.0f);
				}
				if (gameObject.GetComponent<SpriteRenderer> ()) {
					Destroy (gameObject.GetComponent<SpriteRenderer> (), 0.0f);
				}
				audioSources.Remove (uid);
				Destroy (gameObject, 0.0f);
				users.Remove (uid);
			}
		} catch (Exception e) {
			Debug.Log ("UserDead Exception " + e);
		}
	}

	void fireDialogInterval ()
	{
		StartCoroutine (fireDialog ());
	}

	IEnumerator fireDialog ()
	{
		yield return new WaitForSeconds (1.0f);
		Debug.Log ("fireDialog");

	}


	void Update ()
	{
		
		if ((Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) && !aboutCanvas.activeSelf) {
			
			canvas.SetActive (!canvas.activeSelf);
		}
		if (Input.location.status == LocationServiceStatus.Running) {
			lastData = Input.location.lastData;
			float x = (float)Utils.Mapf (lastData.longitude, LEFT, RIGHT, -max, max, false);
			float y = (float)Utils.Mapf (lastData.altitude, 0, 900, 0.0f, max, true);
			float z = (float)Utils.Mapf (lastData.latitude, TOP, BOTTOM, -max, max, true);
			dicoCircle.transform.position = Vector3.Lerp (dicoCircle.transform.position, new Vector3 (x, y, z), 0.5f);
		}
	}

	void UserFetched (string uid, double longitude, double latitude, double altutide)
	{
		try {
//			Debug.Log ("System.GC.Collect ()");
			System.GC.Collect ();
			#if UNITY_EDITOR
			string myuid = "debugger";
			#else
		string myuid = SystemInfo.deviceUniqueIdentifier;
			#endif

			if (uid.CompareTo (myuid) != 0) {
				
				if (!users.Contains (uid) && users.Count < maxNumUser) {
					try {
						GameObject e = (GameObject)Instantiate (user);
						//map lat lonig alt to 3d 
						float x = (float)Utils.Map (longitude, LEFT, RIGHT, -max, max, false);
						float y = (float)Utils.Map (altutide, 0, 100, 0.0f, max, true);
						float z = (float)Utils.Map (latitude, TOP, BOTTOM, -max, max, true);
//						Log (LogType.Log, "uid :" + uid + " | x: " + x + " | y: " + y + " | z: " + z);
						e.transform.position = new Vector3 (x, y, z);
						e.transform.localScale = new Vector3 (-0.5f, -0.5f, -0.5f);
						User u = e.GetComponent<User> ();
						u.uid = uid;
						u.centerRef = sphere;
						u.userDeadDelegate += UserDead;
						u.audioSourceCompleted += AudioSourceCompleted;
						AudioSource audioSource = e.GetComponent <AudioSource> ();

						users.Add (uid, e);
						AssignAudioClip (ref audioSource, uid);

					} catch (Exception exception) {
						Debug.Log ("UserFetched New User: " + exception.ToString ());
					}

				} else if (users.Contains (uid)) {
					try {
//					
						GameObject o = (GameObject)users [uid];

						User u = o.GetComponent<User> ();
						u.recharge ();
						AudioSource audioSource = o.GetComponent <AudioSource> ();

						AssignAudioClip (ref audioSource, uid);


					} catch (Exception exception) {
						Debug.Log ("UserFetched Exist: " + exception.ToString ());
					}
				}
				
			}
		} catch (Exception exception) {
			Debug.Log ("UserFetched : " + exception.ToString ());
		}
	}

	void AssignAudioClip (ref AudioSource audioSource, string uid)
	{
		if (audioSource.clip != null && audioSource.isPlaying && audioSources.Contains (uid)) {
//			Debug.Log ("AssignAudioClip audioSournce is playing return " + uid);
			return;
		}

		try {
			if (dialogAudioSourceRef == null && dialogs.Count > 0) {
				Debug.Log ("AssignAudioClip : " + audioSource + " to " + uid);

				StartCoroutine (GetAudioClip (uid));
//				Debug.Log ("dialogAudioSource Clip : " + audioSource.clip.ToString ());
//				audioSource.Play ();
//				audioSource.volume = 0.0f;
//				dialogAudioSourceRef = audioSource;
//				Debug.Log ("audioSources.Add  " + uid);
//				audioSources.Add (uid, audioSource);
//				StartCoroutine (AudioFadeOut.FadeIn (audioSource, 1.0f));
			} else if (soundfields.Count > 0) {
				Debug.Log ("AssignAudioClip : " + audioSource);

				if (audioSources.Count < numObjects) {
					StartCoroutine (GetAudioClip (uid));
				}

			} else {
				Debug.Log ("AssignAudioClip nothing assign : audioSources.Count " + audioSources.Count);
			}
		} catch (Exception exception) {
			Debug.Log ("AssignAudioClip : " + exception.Message);
		}
	}

	IEnumerator GetAudioClip (string uid)
	{
		GameObject gameObject = (GameObject)users [uid];
		AudioSource audioSource = gameObject.GetComponent <AudioSource> ();
		if (audioSource != null) {
			if (audioSource.clip != null) {
				
				AudioClip clip = audioSource.clip;

				audioSource.clip = null;
				Destroy (clip);
			}
			if (dialogAudioSourceRef == null && !audioSource.Equals (dialogAudioSourceRef)) {
			
				dialogAudioSourceRef = audioSource;

				if (dialogs.Count > 0) {
				
					int index = (int)(UnityEngine.Random.value * dialogs.Count);
					string[] assetNames = (string[])assetBundleRef [dialogs [index]];
					int index2 = (int)(UnityEngine.Random.value * assetNames.Length);
					Debug.Log ("Got dialogs bundles index" + index);
					Debug.Log ("Got dialogs bundles asset index2" + index2);
					string assetBundelName = dialogs [index];
					string path = assetNames [index2];
					Debug.Log ("Got dialogs path " + path);
					AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync (assetBundelName, path, typeof(AudioClip));
					yield return StartCoroutine (request);

					AudioClip prefab = request.GetAsset<AudioClip> ();
					if (prefab != null) {
						audioSource.clip = (AudioClip)prefab;
						audioSource.Play ();
						audioSource.volume = 0.0f;
						StartCoroutine (AudioFadeOut.FadeIn (audioSource, 1.0f));
						Debug.Log ("audioSources.Add  " + uid);
						audioSources.Add (uid, audioSource);
						string err;
						LoadedAssetBundle loadedBundle = AssetBundleManager.GetLoadedAssetBundle (assetBundelName, out err);
						if (err == null) {
							if (loadedBundle.m_AssetBundle != null) {
								loadedBundle.m_AssetBundle.Unload (false);
							}
						}
					} else {
						Debug.Log ("Error : failed to load " + assetBundelName + " " + path);
					}
				}
			} else {

				if (soundfields.Count > 0) {
					
					int index = (int)(UnityEngine.Random.value * soundfields.Count);

					string[] assetNames = (string[])assetBundleRef [soundfields [index]];
					int index2 = (int)(UnityEngine.Random.value * assetNames.Length);
					Debug.Log ("Got soundfields bundles index" + index);
					Debug.Log ("Got soundfields bundles asset index2" + index2);
					string assetBundelName = soundfields [index];
					string path = assetNames [index2];
					Debug.Log ("Got soundfields path " + path);
					AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync (assetBundelName, path, typeof(AudioClip));
					yield return StartCoroutine (request);

					AudioClip prefab = request.GetAsset<AudioClip> ();
					if (prefab != null) {
						audioSource.clip = (AudioClip)prefab;
						audioSource.Play ();
						audioSource.volume = 0.0f;
						StartCoroutine (AudioFadeOut.FadeIn (audioSource, 1.0f));
						Debug.Log ("audioSources.Add  " + uid);
						audioSources.Add (uid, audioSource);
						string err;
						LoadedAssetBundle loadedBundle = AssetBundleManager.GetLoadedAssetBundle (assetBundelName, out err);
						if (err == null) {
							if (loadedBundle.m_AssetBundle != null) {
								loadedBundle.m_AssetBundle.Unload (false);
							}
						}
					} else {
						Debug.Log ("Error : failed to load " + assetBundelName + " " + path);
					}
				}

			}
		}
	}

	void AudioSourceCompleted (AudioSource audioSource, string uid)
	{
		try {
			


			//TODO more implementation here
			try {
				AudioSource audioSource_ = (AudioSource)audioSources [uid];
				if (audioSource != null) {
					audioSource.Stop ();

					if (audioSource.Equals (dialogAudioSourceRef)) {
					
						dialogAudioSourceRef = null;

					}
					audioSource.clip = null;
					audioSources.Remove (uid);
				}
				if (audioSource_ != null) {
					audioSource_.Stop ();
					audioSource_.clip = null;
				}
			} catch (Exception exception) {
				Debug.Log ("AudioSourceCompleted " + exception.ToString ());
			}


			StartCoroutine (pickNextToPlay ());


		} catch (Exception exception) {
			Debug.Log ("AudioSourceCompleted exception : " + exception.ToString ());
		}
	}

	IEnumerator pickNextToPlay ()
	{
		float watSec = (UnityEngine.Random.value * 60) + 30;
		Debug.Log ("pickNextToPlay watSec : " + watSec);
		yield return new WaitForSeconds (watSec);

		//find oldest 
		//find free user
		GameObject targetUser = null;

		foreach (DictionaryEntry entry in users) {
			//Debug.Log (entry.Key + " " + entry.Value);
			GameObject o = (GameObject)entry.Value;
			User u = o.GetComponent<User> ();
			AudioSource a__ = (AudioSource)audioSources [u.uid];
			if (targetUser == null) {
				targetUser = o;
			} else {
				if ((targetUser.GetComponent <User> ()).weight < u.weight) {
					User tu = (targetUser.GetComponent <User> ());
					Debug.Log ("tu id : " + tu.uid + " | tu.weight " + tu.weight);
					Debug.Log ("u id : " + u.uid + " | u.weight " + u.weight);
					targetUser = o;

				}
			}
		}
		if (targetUser != null) {
			User user = targetUser.GetComponent <User> ();
			AudioSource _as_ = targetUser.GetComponent <AudioSource> ();
			AssignAudioClip (ref _as_, user.uid);
		}
	}


	void AssetLoaded (string assetBundleName)
	{
		if (AssetBundleManager.IsAssetBundleDownloaded (assetBundleName)) {
			Log (LogType.Log, "AssetLoaded : " + assetBundleName);
			string err;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle (assetBundleName, out err);
			if (err == null) {
				
				AssetBundle assetBundle = loadedAssetBundle.m_AssetBundle;
				if (assetBundle != null) {
					assetBundleRef.Add (assetBundleName, assetBundle.GetAllAssetNames ());
					if (assetBundleName.Contains ("_f")) {
						dialogs.Add (assetBundleName);
					} else if (assetBundleName.Contains ("_r")) {
						soundfields.Add (assetBundleName);
					}
	

				}
			}
		}


	}

	//	private void ShuffleDialog ()
	//	{
	//		List<AudioClip> tempList = dialogs;
	//		Utils.Shuffle (ref tempList);
	//
	//		foreach (AudioClip ac in tempList) {
	//			dialogsQueue.Enqueue (ac);
	//		}
	//		tempList = null;
	//
	//	}
	//
	//	private void ShuffleSoundfields ()
	//	{
	//		List<AudioClip> tempList = soundfields;
	//		Utils.Shuffle (ref tempList);
	//
	//		foreach (AudioClip ac in tempList) {
	//			soundfieldsQueue.Enqueue (ac);
	//		}
	//		tempList = null;
	//
	//	}

	static public AssetBundles.AssetBundleManager.LogMode m_LogMode;

	private static void Log (LogType logType, string text)
	{

		if (logType == LogType.Error)
			Debug.LogError ("[Main] " + text);
		else if (m_LogMode == AssetBundles.AssetBundleManager.LogMode.All)
			Debug.Log ("[Main] " + text);
	}

	float Limit (float i, float minVal, float maxVal)
	{
		return Math.Max (minVal, Math.Min (maxVal, i));
	}

	private static User SortByWeight (User o1, User o2)
	{
		return (o1.weight > o2.weight) ? o1 : o2;
	}

	public void onAboutClick ()
	{
		aboutCanvas.SetActive (!aboutCanvas.activeSelf);
	}


}
