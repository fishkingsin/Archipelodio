using UnityEngine;
using System.Collections;

public static class AudioFadeOut {

	public static IEnumerator FadeOut (AudioSource audioSource, float FadeTime) {
		float startVolume = audioSource.volume;

		while (audioSource.volume > 0) {
			audioSource.volume -= startVolume * Time.deltaTime / FadeTime;

			yield return null;
		}

		audioSource.Stop ();
		audioSource.volume = startVolume;
	}

	public static IEnumerator FadeIn (AudioSource audioSource, float FadeTime) {
		
		while (audioSource.volume < 1.0) {
//			Debug.Log ("Debug: "+audioSource.clip + ": volume :"+audioSource.volume);
			audioSource.volume +=  Time.deltaTime / FadeTime;

			yield return null;
		}



	}

}
