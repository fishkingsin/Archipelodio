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
	public GameObject center;
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
	Queue<AudioClip> dialogsQueue;
	AudioSource dialogAudioSourceRef;


	void Start ()
	{
		soundfields = new List<AudioClip> ();	
		dialogs = new List<AudioClip> ();	
		users = new Hashtable ();
		audioSources = new Hashtable ();
		dialogsQueue = new Queue<AudioClip> ();
		getUser = getUserObject.GetComponent<GetUsers> ();
		getUser.fetchedUserDelegate += UserFetched;

		loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
		loadAsset.assetLoadedDelegate += AssetLoaded;

	}

	void UserDead (string uid)
	{
//		Component[] componenets = obj.GetComponents<Component> ();
		GameObject gameObject = (GameObject)users [uid];
		if (gameObject) {
			Debug.Log ("gameObject " + gameObject.ToString () + " of uid " + uid);
			if (gameObject.GetComponent<AudioSource> ()) {
				AudioSource audioSource = gameObject.GetComponent <AudioSource> ();
				if (audioSource.Equals (dialogAudioSourceRef)) {
					dialogAudioSourceRef = null;
				}
//				StartCoroutine (AudioFadeOut.FadeOut (audioSource, 0.5f));
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
	}

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
		string myuid = SystemInfo.deviceUniqueIdentifier;
		#endif

		if (uid.CompareTo (myuid) != 0) {
			if (!users.ContainsKey (uid)) {
//				float range = 5f;
//				float range_h = range * 0.5f;
//				Log (LogType.Log, "New User init...");
				GameObject e = (GameObject)Instantiate (user);
				//map lat lonig alt to 3d 
				float x = (float)Utils.Mapf (longitude, LEFT, RIGHT, -max, max, false);
				float y = (float)Utils.Mapf (altutide, 0, 100, 0.0f, max, true);
				float z = (float)Utils.Mapf (latitude, TOP, BOTTOM, -max, max, true);
				Log (LogType.Log, "uid :" + uid + " | x: " + x + " | y: " + y + " | z: " + z);
				e.transform.position = new Vector3 (x, y, z);
				e.transform.localScale = new Vector3 (-0.5f, -0.5f, -0.5f);
				User u = e.GetComponent<User> ();
				u.uid = uid;
				u.centerRef = sphere;
				u.userDeadDelegate += UserDead;
				u.getClipDelegate += GetAudioClip;
				u.audioSourceCompleted += AudioSourceCompleted;
				if (audioSources.Count < numObjects && soundfields.Count > 0) {
					int index = (int)(UnityEngine.Random.value * soundfields.Count);
					AudioClip audioClip = soundfields [index];
					Log (LogType.Log, "soundfields " + index + " " + audioClip.ToString ());

					AudioSource audioSource = e.GetComponent <AudioSource> ();
					audioSources [uid] = audioSource;
					audioSource.clip = audioClip;
					audioSource.Play ();
					audioSource.volume = 0.0f;
					StartCoroutine (AudioFadeOut.FadeIn (audioSource, 0.5f));
				}

				users [uid] = e;

			} else {
				Log (LogType.Log, "User Exist skip init");
				GameObject o = (GameObject)users [uid];
				User u = o.GetComponent<User> ();
				u.age = 100;
			}
		} 

	}

	AudioClip GetAudioClip (AudioSource audioSource)
	{
		if (dialogAudioSourceRef == null && !audioSource.Equals (dialogAudioSourceRef)) {
			dialogAudioSourceRef = audioSource;
			if (dialogsQueue.Count == 0) {
				ShuffleDialog ();
			}
			AudioClip dialogAudioClip = null;
			if (dialogsQueue.Count != 0) {
				dialogAudioClip = dialogsQueue.Peek ();

				Debug.Log ("Got dailog clip " + dialogAudioClip.ToString ());
			}
			return dialogAudioClip;
		} else {
			AudioClip audioClip = null;
			if (soundfields.Count > 0) {
				int index = (int)(UnityEngine.Random.value * soundfields.Count);
				audioClip = soundfields [index];
				Log (LogType.Log, "GetAudioClip");
				Debug.Log ("Got soudnfield clip " + audioClip.ToString ());
			}
			return audioClip;
		}
	}
	void AudioSourceCompleted(string uid){
		//TODO more implementation here
		AudioSource audioSource = (AudioSource)audioSources[uid];
		if (audioSource != null) {
			audioSource.clip = null;
			if (audioSource.Equals (dialogAudioSourceRef)) {
				dialogAudioSourceRef = null;
			}
			audioSources.Remove (uid);
		}
	}
	void AssetLoaded (string assetBundleName)
	{
		if (AssetBundleManager.IsAssetBundleDownloaded (assetBundleName)) {
			string err;
			LoadedAssetBundle loadedAssetBundle = AssetBundleManager.GetLoadedAssetBundle (assetBundleName, out err);
			if (err == null) {
				AssetBundle assetBundle = loadedAssetBundle.m_AssetBundle;
				if (assetBundle != null) {
					AudioClip[] clips = assetBundle.LoadAllAssets<AudioClip> ();
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

	private void ShuffleDialog ()
	{
		List<AudioClip> tempList = dialogs;
		Utils.Shuffle (ref tempList);	

		foreach (AudioClip ac in tempList) {
			dialogsQueue.Enqueue (ac);
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
