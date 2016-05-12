using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class RetryButton : MonoBehaviour {

	public void onClick () {
		SceneManager.LoadScene ("Loading");
	}
}
