using UnityEngine;
using System.Collections;
using System;

public class Main : MonoBehaviour {
	public GameObject canvas;
	private bool isShowing;
	void Start()
	{

	}
	void Update () {
		if (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Ended) {
			// Get movement of the finger since last frame
//			Vector2 touchDeltaPosition = Input.GetTouch(0).deltaPosition;
//
//			// Move object across XY plane
//			transform.Translate(-touchDeltaPosition.x * speed, -touchDeltaPosition.y * speed, 0);

			isShowing = !isShowing;
			canvas.SetActive(isShowing);

		}
		if (Input.GetMouseButtonDown (0)) {
			isShowing = !isShowing;
			canvas.SetActive(isShowing);
		}
	}

}
