using UnityEngine;
using System.Collections;
using System;

public class Main : MonoBehaviour {
	public Color c1 = Color.blue;
	public Color c2 = Color.blue;
	public int lengthOfLineRenderer = 20;
	void Start()
	{
		LineRenderer lineRenderer = gameObject.AddComponent<LineRenderer>();
		lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
		lineRenderer.SetColors(c1, c2);
		lineRenderer.SetWidth(0.2F, 0.2F);
		lineRenderer.SetVertexCount(lengthOfLineRenderer);
	}
	// Update is called once per frame
	void Update () {
		LineRenderer lineRenderer = GetComponent<LineRenderer>();
		Vector3[] points = new Vector3[lengthOfLineRenderer];
			float t = Time.time;
		int i = 0;
		while (i < lengthOfLineRenderer) {
			points[i] = new Vector3(i * 0.5F, 0, 0);
			i++;
		}
		lineRenderer.SetPositions(points);
	}

}
