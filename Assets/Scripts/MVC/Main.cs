using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;
using AssetBundles;
using System.Runtime.InteropServices;

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
	public GameObject getUserObject;
	public GameObject loadAssetObject;
	private bool isShowing;
	public int numObjects = 10;
	public GameObject dicoCircle;
	float max = 5.0f;

	GetUsers getUser;
	LoadAssets loadAsset;

	List<AudioClip> soundfields;
	List<AudioClip> dialogs;
	Queue<AudioClip> dialogsQueue;
	AudioSource dialogAudioSourceRef;

	LocationInfo lastData;

	void Start ()
	{
		
		soundfields = new List<AudioClip> ();	
		dialogs = new List<AudioClip> ();	
		users = new Hashtable ();
		audioSources = new Hashtable ();
		dialogsQueue = new Queue<AudioClip> ();
		getUser = getUserObject.GetComponent<GetUsers> ();
		getUser.fetchedUserDelegate += UserFetched;
		loadAssetObject = GameObject.Find("Load");
		loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
		loadAsset.assetLoadedDelegate += AssetLoaded;
		StartCoroutine (loadAsset.reload ());
		dialogAudioSourceRef = null;
//		InvokeRepeating ("fireDialogInterval", 1.0f, 1.0f);
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
	public class myMonsterSorter : IComparer  {

		// Calls CaseInsensitiveComparer.Compare on the monster name string.
		int IComparer.Compare( System.Object x, System.Object y )  {
			return( (new CaseInsensitiveComparer()).Compare( ((GameObject)x).name, ((GameObject)y).name) );
		}

	}
	void Update ()
	{
		if (Input.touchCount > 0 && Input.GetTouch (0).phase == TouchPhase.Ended) {
			isShowing = !isShowing;
			canvas.SetActive (isShowing);
		}
		if (Input.location.status == LocationServiceStatus.Running) {
			lastData = Input.location.lastData;
			float x = (float)Utils.Mapf (Limit (lastData.longitude, LEFT, RIGHT), LEFT, RIGHT, -max, max, false);
			float y = (float)Utils.Mapf (Limit (lastData.altitude, 0, 900), 0, 900, 0.0f, max, true);
			float z = (float)Utils.Mapf (Limit (lastData.latitude, TOP, BOTTOM), TOP, BOTTOM, -max, max, true);
			dicoCircle.transform.position = Vector3.Lerp (dicoCircle.transform.position, new Vector3 (x, y, z), 0.5f);
		}
	}

	void UserFetched (string uid, double longitude, double latitude, double altutide)
	{
		try {
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
						u.getClipDelegate += GetAudioClip;
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
			Debug.Log ("AssignAudioClip audioSournce is playing return " + uid);
			return;
		}

		try {
			if (dialogAudioSourceRef == null && dialogs.Count > 0) {
				Debug.Log ("AssignAudioClip : " + audioSource);

				audioSource.clip = GetAudioClip (audioSource);
				Debug.Log ("dialogAudioSource Clip : " + audioSource.clip.ToString ());
				audioSource.Play ();
				audioSource.volume = 0.0f;
				dialogAudioSourceRef = audioSource;
				Debug.Log ("audioSources.Add  " + uid);
				audioSources.Add (uid, audioSource);
				StartCoroutine (AudioFadeOut.FadeIn (audioSource, 1.0f));
				Debug.Log ("AssignAudioClip : " + audioSource.clip);
				

			} else if (soundfields.Count > 0) {
				Debug.Log ("AssignAudioClip : " + audioSource);
				try {
					if (audioSources.Count < 10) {
						int index = (int)(UnityEngine.Random.value * soundfields.Count);
						AudioClip audioClip = soundfields [index];
						audioSource.clip = audioClip;
						audioSource.Play ();
						audioSource.volume = 0.0f;
						StartCoroutine (AudioFadeOut.FadeIn (audioSource, 1.0f));
						Debug.Log ("audioSources.Add  " + uid);
						audioSources.Add (uid, audioSource);
						Debug.Log ("AssignAudioClip : " + audioSource.clip);
					}
				} catch (Exception exception) {
					Debug.Log ("AssignAudioClip : " + exception.Message);
				}
			} else {
				Debug.Log ("AssignAudioClip nothing assign : audioSources.Count " + audioSources.Count);
			}
		} catch (Exception exception) {
			Debug.Log ("AssignAudioClip : " + exception.Message);
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
				
				dialogAudioClip = dialogsQueue.Dequeue ();

				Debug.Log ("Got dailog clip " + dialogAudioClip.ToString ());

			}
			return dialogAudioClip;
		} else {
			AudioClip audioClip = null;
			if (soundfields.Count > 0) {
				int index = (int)(UnityEngine.Random.value * soundfields.Count);
				audioClip = soundfields [index];
//				Log (LogType.Log,"Got soudnfield clip " + audioClip.ToString ());
			}
			return audioClip;
		}
	}

	void AudioSourceCompleted (AudioSource audioSource, string uid)
	{
		try {
			AudioSource tempDialogAudioSourceRef = null;

			//TODO more implementation here
			AudioSource audioSource_ = (AudioSource)audioSources [uid];
			if (audioSource != null) {
				audioSource.Stop ();

				if (audioSource.Equals (dialogAudioSourceRef)) {
					tempDialogAudioSourceRef = dialogAudioSourceRef;
					dialogAudioSourceRef = null;
				}
				audioSource.clip = null;
				audioSources.Remove (uid);
			}
			if (audioSource_ != null) {
				audioSource_.Stop ();
				audioSource_.clip = null;
			}


//			User []tempArray = new User[users.Count];
//			ICollection keys = users.Keys;

//			users.CopyTo (tempArray,0);
//			Array.Sort(tempArray, CompareCondition);

//			audioSource = ((GameObject)users [tKey]).GetComponent<AudioSource> ();
//
//			while (audioSource.clip != null && count < users.Count) {
//				index = (int)(UnityEngine.Random.value * keys.Length);
//				tKey = keys [index];
//				audioSource = ((GameObject)users [tKey]).GetComponent<AudioSource> ();
//				tKey = keys [index];
//				count++;
//				Debug.Log ("AudioSourceCompleted count : " + count);
//			}
//			AssignAudioClip (ref audioSource, uid);

		} catch (Exception exception) {
			Debug.Log ("AudioSourceCompleted exception : " + exception.ToString ());
		}
	}

//	int CompareCondition(GameObject itemA, GameObject itemB){
//		User scriptA = itemA.GetComponent(User);
//		User scriptB = itemB.GetComponent(User);
//		if (scriptA.weight > scriptB.weight) return 1;
//		if (scriptA.weight < scriptB.weight) return -1;
//		return 0;
//	}


	void AssetLoaded (string assetBundleName)
	{
		if (AssetBundleManager.IsAssetBundleDownloaded (assetBundleName)) {
			Log (LogType.Log, "AssetLoaded : " + assetBundleName);
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
						
//					for (int i = 0; i < dialogs.Count; i++) {
//						Log (LogType.Log, "dialogs : -> " + dialogs [i].ToString () + " | Class : " + dialogs [i].GetType ());
//					}
//					for (int i = 0; i < soundfields.Count; i++) {
//						Log (LogType.Log, "soundfields : -> " + soundfields [i].ToString () + " | Class : " + soundfields [i].GetType ());
//					}

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

	float Limit (float i, float minVal, float maxVal)
	{
		return Math.Max (minVal, Math.Min (maxVal, i));
	}

	private static User SortByWeight(User o1, User o2) {
		return (o1.weight > o2.weight)?o1:o2;
	} 

}
