using UnityEngine;
using System.Collections;
using AssetBundles;
using System;
using System.Collections.Generic;



public class LoadAssets : MonoBehaviour
{
	public const string AssetBundlesOutputPath = "/AssetBundles/";
	public List<string> assetBundleNames;

	public delegate void AssetLoadedDelegate (string assetBundleName);

	public AssetLoadedDelegate assetLoadedDelegate;

	public delegate void AssetDownloadProgressDelegate (float progress);

	public AssetDownloadProgressDelegate assetDownloadProgressDelegate;

	public delegate void AssetLoadedErrorDelegate (string error);

	public AssetLoadedErrorDelegate assetLoadedErrorDelegate;



	// Use this for initialization
	IEnumerator Start ()
	{
		
		
		Application.targetFrameRate = 30;
		yield return StartCoroutine (Initialize ());
#if UNITY_EDITOR || DEVELOPMENT_BUILD
		string url = "http://www.mb09.com/ARCHIPELAUDIO/api/assetBundles";
#else
		string url = "http://www.moneme.com/Archipelodio/api/assetBundles";
#endif
//		WWW www = new WWW (url);
//		yield return www;
//		try {
//			if (www.error == null) {
//
//				Processjson (www.text);
//			} else {
//				Debug.Log ("ERROR: WWW" + www.error);
//			}
//		} catch (Exception e) {
//			Debug.Log ("Error: " + e);
//		}
//		assetBundleNames = new List<string> ();
//		assetBundleNames.Add ("audio");
		// Load asset.

//		foreach (string assetBundleName in assetBundleNames) {
//			yield return StartCoroutine (InstantiateGameObjectAsync (assetBundleName, "sample", typeof(AudioClip)));
//		}
//		yield return StartCoroutine (InstantiateGameObjectAsync (assetBundleName, assetName, typeof(AudioClip)));

	}

	public IEnumerator reload ()
	{
		
		foreach (string assetBundleName in assetBundleNames) {
			AssetBundleManager.UnloadAssetBundle (assetBundleName);
			yield return StartCoroutine (InstantiateGameObjectAsync (assetBundleName, "sample", typeof(AudioClip)));
		}
	}

	private void Processjson (string jsonString)
	{
		Debug.Log ("Processjson: " + jsonString);
		JSONObject jsonvale = new JSONObject (jsonString);

		accessData (jsonvale);

	}

	void accessData (JSONObject obj)
	{
		switch (obj.type) {
		case JSONObject.Type.OBJECT:
			Debug.Log ("accessData: OBJECT :" + obj ["files"]);
			accessData (obj ["files"]);


			//			Debug.Log ("lid:" + obj ["lid"].str +
			//			"| uid: " + obj ["uid"].str +
			//			"| loclat: " + obj ["loclat"].str +
			//			"| loclong: " + obj ["loclong"].str +
			//			"| heading: " + obj ["heading"].str +
			//			"| altitude: " + obj ["altitude"].str +
			//			"| timestamp: " + obj ["timestamp"].str +
			//			"| tester: " + obj ["tester"].str);
			//			float loclong, loclat, altitude;
			//			float.TryParse (obj ["loclong"].str, out loclong); 
			//			float.TryParse (obj ["loclat"].str, out loclat);
			//			float.TryParse (obj ["altitude"].str, out altitude);

			break;
		case JSONObject.Type.BOOL:

			break;
		case JSONObject.Type.NUMBER:

			break;
		case JSONObject.Type.STRING:

			break;
		case JSONObject.Type.ARRAY:
			string variant = (Application.platform == RuntimePlatform.Android) ? "Android" : "iOS";
			foreach (JSONObject j in obj.list) {
				
				if (j.str.Contains (variant)) {
					Debug.Log ("JSONObject  :" + j.str);
					string objectString = j.str;

					string path = "..\\/AssetBundles\\/" + variant + "\\/";
					string bundleName = objectString.Substring (path.Length);
					assetBundleNames.Add (bundleName);
				}


			}
			StartCoroutine (reload ());

			break;

		}
	}

	// Initialize the downloading URL.
	// eg. Development server / iOS ODR / web URL
	void InitializeSourceURL ()
	{
		// If ODR is available and enabled, then use it and let Xcode handle download requests.
		#if ENABLE_IOS_ON_DEMAND_RESOURCES
		if (UnityEngine.iOS.OnDemandResources.enabled) {
			AssetBundleManager.SetSourceAssetBundleURL ("odr://");
			return;
		}
		#endif
		#if DEVELOPMENT_BUILD || 	UNITY_EDITOR
		// With this code, when in-editor or using a development builds: Always use the AssetBundle Server
		// (This is very dependent on the production workflow of the project.
		//      Another approach would be to make this configurable in the standalone player.)

//        AssetBundleManager.SetDevelopmentAssetBundleServer();
		AssetBundleManager.SetSourceAssetBundleURL ("http://www.mb09.com/ARCHIPELAUDIO/AssetBundles/");
//		AssetBundleManager.SetSourceAssetBundleURL ("http://www.moneme.com/Archipelodio/AssetBundles/");
		return;
		#else
		// Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
		//AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
		// Or customize the URL based on your deployment or configuration
		AssetBundleManager.SetSourceAssetBundleURL("http://www.moneme.com/Archipelodio/AssetBundles/");
		return;
		#endif
	}

	// Initialize the downloading url and AssetBundleManifest object.
	protected IEnumerator Initialize ()
	{
		// Don't destroy this gameObject as we depend on it to run the loading script.
		DontDestroyOnLoad (gameObject);

		InitializeSourceURL ();

		// Initialize AssetBundleManifest which loads the AssetBundleManifest object.
		var request = AssetBundleManager.Initialize ();
		if (request != null)
			yield return StartCoroutine (request);
		
		AssetBundleManifest manifest = AssetBundleManager.m_AssetBundleManifest;
		assetBundleNames = new List<string> (manifest.GetAllAssetBundles ());
		foreach (string assetBundleName in assetBundleNames) {
			yield return StartCoroutine (InstantiateGameObjectAsync (assetBundleName, "sample", typeof(AudioClip)));
		}
	}

	void Update ()
	{
		float progress = 0; 

		List<AssetBundleLoadOperation> operations = AssetBundleManager.GetInProgressOperations ();


		float part = operations.Count;
		foreach (AssetBundleLoadOperation operation in operations) {
			
			if (operation.GetType ().Equals (typeof(AssetBundleDownloadFromWebOperation))) {
				
				float downloadProgress = ((AssetBundleDownloadFromWebOperation)operation).GetProgress ();
				Debug.Log ("operation.GetProgress () : " + downloadProgress);
				progress += downloadProgress / part;
			}
		}
		if (assetDownloadProgressDelegate != null) {
			assetDownloadProgressDelegate (progress);
		}
	}

	protected IEnumerator InstantiateGameObjectAsync (string assetBundleName, string assetName, Type t)
	{
		// This is simply to get the elapsed time for this phase of AssetLoading.
		float startTime = Time.realtimeSinceStartup;

		// Load asset from assetBundle.
		Debug.Log ("InstantiateGameObjectAsync : assetBundleName : " + assetBundleName);
		AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync (assetBundleName, assetName, t);
		List<AssetBundleLoadOperation> operations = AssetBundleManager.GetInProgressOperations ();
		if (operations.Count > 0) {
			
			if (operations [0].GetType ().Equals (typeof(AssetBundleDownloadFromWebOperation))) {
				AssetBundleDownloadFromWebOperation operation = (AssetBundleDownloadFromWebOperation)operations [0];
				operation.downloadFailedDelegate += DownloadFail;

			}
		}

		if (request == null)
			yield break;
		yield return StartCoroutine (request);



		// Get the asset.
		GameObject prefab = request.GetAsset<GameObject> ();
		//		if (prefab != null)
		if (AssetBundleManager.IsAssetBundleDownloaded (assetBundleName)) {
			if (assetLoadedDelegate != null) {
				assetLoadedDelegate (assetBundleName);
			}
		}

		// Calculate and display the elapsed time.
		float elapsedTime = Time.realtimeSinceStartup - startTime;
		Debug.Log (assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");

	}

	void DownloadFail (string err)
	{
		if (assetLoadedErrorDelegate != null) {
			assetLoadedErrorDelegate (err);
		}
	}
}