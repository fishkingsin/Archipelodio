using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CloseButton : MonoBehaviour {

	public void onClick () {
		SceneManager.LoadScene ("Main");
	}
}
