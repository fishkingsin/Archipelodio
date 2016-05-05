using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class CloseButtonMenu : MonoBehaviour {

	public void onClick () {
		SceneManager.LoadScene ("Scenes/Menu");
	}
}
