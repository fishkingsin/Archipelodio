using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;
using AssemblyCSharp;


public class CircularBuffer<T>
{
	Queue<T> _queue;
	int _size;

	public CircularBuffer(int size)
	{
		_queue = new Queue<T>(size);
		_size = size;
	}

	public void Add(T obj)
	{
		if (_queue.Count == _size)
		{
			_queue.Dequeue();
			_queue.Enqueue(obj);
		}
		else
			_queue.Enqueue(obj);
	}
	public T Read()
	{
		return _queue.Dequeue();
	}

	public T Peek()
	{
		return _queue.Peek();
	}
}


public class CameraRotateEffect : MonoBehaviour {
	
	Time time;
	float lastAngle;

	public GameObject[] audioSources;

	// Use this for initialization
	void Start () {
		lastAngle = 0;
		
		for(int i = 0 ; i < audioSources.Length ; i++){
			audioSources[i].transform.LookAt (Camera.main.transform);
			SpriteRenderer renderer = audioSources [i].GetComponents<SpriteRenderer>()[0];
			renderer.color = new Color(0f, 0.0f, 1.0f, 1f); 
		}
	

	}
	
	// Update is called once per frame
	void Update() {
		float diff = -Input.compass.trueHeading - lastAngle;
		Camera.main.transform.RotateAround(transform.position, Vector3.up, diff);
		for(int i = 0 ; i < audioSources.Length ; i++){
			audioSources[i].transform.LookAt (Camera.main.transform);
			float dist = Math.Abs(Vector3.Distance(audioSources[i].transform.position, transform.position));
			SpriteRenderer renderer = audioSources[i].GetComponents<SpriteRenderer>()[0];
			renderer.color = HSBColor.ToColor(
				new HSBColor(
					Utils.Map(150.0f,0.0f,256.0f,0.0f,1.0f),
					Utils.Map(dist,10.0f, 0.0f, 0.0F, 1.0f),
					1.0f,
					1f)); 
		}
		transform.LookAt (Camera.main.transform);

		lastAngle = Mathf.Lerp(lastAngle, -Input.compass.trueHeading , 20*Time.time);

	}
}
