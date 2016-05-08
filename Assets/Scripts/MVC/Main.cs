using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;
using AssetBundles;

public class Main : MonoBehaviour
{
	//top left: (gps format is 22.60,113.82)
	public static float TOP = 22.60f;
	public static float LEFT = 113.82f;

	//bottom right: (gps format is 22.12, 114.42)
	public static float BOTTOM = 22.12f;
	public static float RIGHT = 114.42f;

	public Hashtable audioSources;
	public GameObject canvas;
	public GameObject user;
	public GameObject sphere;
	public Hashtable users;
	public GameObject getUserObject;
	public GameObject loadAssetObject;
	private bool isShowing;
	public int numObjects = 10;

	float max = 5.0f;

	GetUsers getUser;
	LoadAssets loadAsset;

	List<AudioClip> soundfields;
	List<AudioClip> dialogs;

	void Start ()
	{
		soundfields = new List<AudioClip> ();	
		dialogs = new List<AudioClip> ();	
		users = new Hashtable ();
		audioSources = new Hashtable ();
		getUser = getUserObject.GetComponent<GetUsers> ();
		getUser.fetchedUserDelegate += UserFetched;

		loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
		loadAsset.assetLoadedDelegate += AssetLoaded;


	}

	void UserDead (GameObject obj)
	{
		AudioSource audioSource = obj.GetComponent <AudioSource> ();
		StartCoroutine (AudioFadeOut.FadeOut (audioSource, 0.5f));
		Destroy (obj, 0.5f);

	}

	//	void IntervalFunction ()
	//	{
	//		Debug.Log ("Main IntervalFunction");
	//	}

	void Update ()
	{
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			isShowing = !isShowing;
			canvas.SetActive (isShowing);

		}

	}

	void UserFetched (string uid, float longitude, float latitude, float altutide)
	{
		#if UNITY_EDITOR
		string myuid = "debugger";
		#else
		string myuid = SystemInfo.deviceUniqueIdentifier);
		#endif
		if (myuid.CompareTo (myuid) != 0) {
			if (!users.ContainsKey (uid)) {
				float range = 5f;
				float range_h = range * 0.5f;
				Log (LogType.Log, "New User init...");
				GameObject e = (GameObject)Instantiate (user);
				//map lat lonig alt to 3d 
				float x = (float)Utils.Mapf (longitude, LEFT, RIGHT, -max, max, false);
				float y = (float)Utils.Mapf (altutide, 0, 100, 0.0f, max, true);
				float z = (float)Utils.Mapf (latitude, TOP, BOTTOM, -max, max, true);
				Log (LogType.Log, "uid :" + uid + "| x: " + x + "| y: " + y + "| z: " + z);
				e.transform.position = new Vector3 (x, y, z);
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
				u.getClipDelegate += GetAudioClip;
				//attach adio source if needed
				if (audioSources.Count < numObjects && soundfields.Count > 0) {
					int index = (int)(UnityEngine.Random.value * soundfields.Count);
					AudioClip audioClip = soundfields [index];
					Log (LogType.Log, "soundfields " + index + " " + audioClip.ToString ());

					AudioSource ac = e.GetComponent <AudioSource> ();
					audioSources [uid] = ac;
					ac.clip = audioClip;
					ac.Play ();
					ac.volume = 0.0f;
					StartCoroutine (AudioFadeOut.FadeIn (ac, 0.5f));
				}

//			audioSources[i] = e.AddComponent <AudioSource>();
			

				users [uid] = e;

			} else {
				Log (LogType.Log, "User Exist skip init");
				GameObject o = (GameObject)users [uid];
				User u = o.GetComponent<User> ();
				u.age = 100.0f;
			}
		}
	}
	AudioClip GetAudioClip(){
		int index = (int)(UnityEngine.Random.value * soundfields.Count);
		AudioClip audioClip = soundfields [index];
		Log (LogType.Log, "GetAudioClip");
		return audioClip;
	}
	void AssetLoaded ()
	{
		if (AssetBundleManager.IsAssetBundleDownloaded ("audio")) {
			string err;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle ("audio", out err);
			if (err == null) {
				AssetBundle assetBundle = loadedAssetBundle.m_AssetBundle;
				if (assetBundle != null) {
					AudioClip[] clips  = assetBundle.LoadAllAssets<AudioClip> ();
					foreach (AudioClip clip in clips) {
						if (clip.ToString ().Contains ("_OL")) {
							dialogs.Add (clip);
						} else {
							soundfields.Add (clip);
						}
					}
						
					for (int i = 0; i < dialogs.Count; i++) {
						Log (LogType.Log,"dialogs : -> " + dialogs [i].ToString () + " | Class : " + dialogs [i].GetType ());
					}
					for (int i = 0; i < soundfields.Count; i++) {
						Log (LogType.Log,"soundfields : -> " + soundfields [i].ToString () + " | Class : " + soundfields [i].GetType ());
					}
				}
			}
		}


	}

	static public AssetBundles.AssetBundleManager.LogMode m_LogMode;

	private static void Log (LogType logType, string text)
	{

		if (logType == LogType.Error)
			Debug.LogError ("[Main] " + text);
		else if (m_LogMode == AssetBundles.AssetBundleManager.LogMode.All)
			Debug.Log ("[Main] " + text);
	}



}
