using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class AboutButton : MonoBehaviour {

	public void onClick () {
		SceneManager.LoadScene ("About");
	}
}
