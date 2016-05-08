using UnityEngine;
using System.Collections;
using AssetBundles;
using System;
using System.Collections.Generic;



public class LoadAssets : MonoBehaviour
{
    public const string AssetBundlesOutputPath = "/AssetBundles/";
    public string assetBundleName;
	public List<string> assetNames;
	public delegate void AssetLoadedDelegate();
	public AssetLoadedDelegate assetLoadedDelegate;


    // Use this for initialization
    IEnumerator Start()
    {
        yield return StartCoroutine(Initialize());

        // Load asset.
		assetNames = new List<string>();
		assetNames.Add("sample");
		for(int i = 0; i < assetNames.Count ; i++) {
			yield return StartCoroutine(InstantiateGameObjectAsync(assetBundleName, assetNames[i], typeof (AudioClip)));
		}
    }

    // Initialize the downloading URL.
    // eg. Development server / iOS ODR / web URL
    void InitializeSourceURL()
    {
        // If ODR is available and enabled, then use it and let Xcode handle download requests.
        #if ENABLE_IOS_ON_DEMAND_RESOURCES
        if (UnityEngine.iOS.OnDemandResources.enabled)
        {
            AssetBundleManager.SetSourceAssetBundleURL("odr://");
            return;
        }
        #endif
		#if DEVELOPMENT_BUILD || UNITY_EDITOR
        // With this code, when in-editor or using a development builds: Always use the AssetBundle Server
        // (This is very dependent on the production workflow of the project.
        //      Another approach would be to make this configurable in the standalone player.)

        AssetBundleManager.SetDevelopmentAssetBundleServer();
        return;
        #else
        // Use the following code if AssetBundles are embedded in the project for example via StreamingAssets folder etc:
        //AssetBundleManager.SetSourceAssetBundleURL(Application.dataPath + "/");
        // Or customize the URL based on your deployment or configuration
		Debug.Log("===============AssetBundleManager.SetSourceAssetBundleURL(\"http://www.moneme.com/Archipelodio/AssetBundles\");=========================");
		AssetBundleManager.SetSourceAssetBundleURL("http://www.moneme.com/Archipelodio/AssetBundles/");
        return;
        #endif
    }

    // Initialize the downloading url and AssetBundleManifest object.
    protected IEnumerator Initialize()
    {
        // Don't destroy this gameObject as we depend on it to run the loading script.
        DontDestroyOnLoad(gameObject);

        InitializeSourceURL();

        // Initialize AssetBundleManifest which loads the AssetBundleManifest object.
        var request = AssetBundleManager.Initialize();
        if (request != null)
            yield return StartCoroutine(request);
    }

	protected IEnumerator InstantiateGameObjectAsync(string assetBundleName, string assetName , Type t)
    {
        // This is simply to get the elapsed time for this phase of AssetLoading.
        float startTime = Time.realtimeSinceStartup;

        // Load asset from assetBundle.

        AssetBundleLoadAssetOperation request = AssetBundleManager.LoadAssetAsync(assetBundleName, assetName, t);
        if (request == null)
            yield break;
        yield return StartCoroutine(request);

        // Get the asset.
        GameObject prefab = request.GetAsset<GameObject>();
//		if (prefab != null)

		if (assetLoadedDelegate != null) {
			assetLoadedDelegate ();
		}
        
        // Calculate and display the elapsed time.
        float elapsedTime = Time.realtimeSinceStartup - startTime;
        Debug.Log(assetName + (prefab == null ? " was not" : " was") + " loaded successfully in " + elapsedTime + " seconds");

    }
}
