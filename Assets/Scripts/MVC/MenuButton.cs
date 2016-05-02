using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
public class MenuButton : MonoBehaviour {

	// Use this for initialization
	public void onClick () {
		SceneManager.LoadScene ("Main");
	}

}
