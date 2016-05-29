using UnityEngine;
using System.Collections;
using AssetBundles;
using UnityEngine.SceneManagement;
using ProgressBar;


public class Loading : MonoBehaviour
{

	// Use this for initialization
	public GameObject ProgressBar;
	public GameObject loadAssetObject;
	public GameObject retryCanvas;
	ProgressBarBehaviour BarBehaviour;
	LoadAssets loadAsset;

	void Start ()
	{
		Screen.sleepTimeout = SleepTimeout.NeverSleep;
		loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
		loadAsset.assetLoadedDelegate += AssetLoaded;
		loadAsset.assetDownloadProgressDelegate += AssetDownloadProgress;
		loadAsset.assetLoadedErrorDelegate += AssetLoadError;
		BarBehaviour = ProgressBar.GetComponent<ProgressBarBehaviour> ();
		retryCanvas.SetActive (false);
	}

	void AssetLoaded (string assetBundleName)
	{
		loadAsset.assetLoadedDelegate = null;
		Debug.Log ("AssetLoaded " + assetBundleName);
		SceneManager.LoadScene ("Menu");

	}

	void AssetLoadError (string error)
	{
		Debug.Log ("AssetLoadError " + error);

		retryCanvas.SetActive (true);

	}

	void AssetDownloadProgress (float progress)
	{
		
		BarBehaviour.Value = progress * 100;
	}


	public void onRetryClick ()
	{
		retryCanvas.SetActive (false);
		Scene scene = SceneManager.GetActiveScene ();
		SceneManager.LoadScene (scene.name);


	}
}
