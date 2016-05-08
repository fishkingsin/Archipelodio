using UnityEngine;
using System.Collections;
using AssetBundles;
using UnityEngine.SceneManagement;
public class Loading : MonoBehaviour {

	// Use this for initialization
	public GameObject loadAssetObject;
	LoadAssets loadAsset;
	bool canLoadScene = false;
	void Start () {
		loadAsset = loadAssetObject.GetComponent <LoadAssets> ();
		loadAsset.assetLoadedDelegate += AssetLoaded;

	}
	
	// Update is called once per frame
	void Update () {
		if (canLoadScene) {
			canLoadScene = false;
			Debug.Log ("canLoadScene");
			SceneManager.LoadScene ("Menu");
		}
	}
	void AssetLoaded ()
	{
		Debug.Log ("AssetLoaded");
		canLoadScene = true;

	}
}
