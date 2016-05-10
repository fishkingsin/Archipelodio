using UnityEngine;
using System.Collections;
using AssetBundles;
using UnityEngine.SceneManagement;
using ProgressBar;


public class Loading : MonoBehaviour {

	// Use this for initialization
	public GameObject ProgressBar;
	public GameObject loadAssetObject;

	ProgressBarBehaviour BarBehaviour;
	LoadAssets loadAsset;
	bool canLoadScene = false;
	void Start () {
		loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
		loadAsset.assetLoadedDelegate += AssetLoaded;
		loadAsset.assetDownloadProgressDelegate += AssetDownloadProgress;
		BarBehaviour = ProgressBar.GetComponent<ProgressBarBehaviour> ();

	}
	
	// Update is called once per frame
	void Update () {
		if (canLoadScene) {
			canLoadScene = false;
			Debug.Log ("canLoadScene");
			SceneManager.LoadScene ("Menu");
		}
	}
	void AssetLoaded (string assetBundleName)
	{
		Debug.Log ("AssetLoaded " + assetBundleName);
		canLoadScene = true;

	}
	void AssetDownloadProgress(float progress){
		BarBehaviour.Value = progress * 100;
	}

}
