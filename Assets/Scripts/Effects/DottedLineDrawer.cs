
using UnityEngine;
using System.Collections;
using AssemblyCSharp;

public class DottedLineDrawer : MonoBehaviour {
	public GameObject dot;
	GameObject[] dot1;
	GameObject[] dot2;
	GameObject[] dot3;

	public int numObjects=100;
	public float _scale = 0.01f ;
	void Start () {
		dot1 = new GameObject[numObjects];
		dot2 = new GameObject[numObjects];
		dot3 = new GameObject[numObjects];
		//x axis
		for (int i = 0; i < numObjects; i++) {
			GameObject e = Instantiate (dot);

			e.transform.position = new Vector3 ( (i*0.5f)-25.0f, 0, 0);
			e.transform.localScale = new Vector3 (_scale, _scale, _scale);
			e.transform.parent = transform;
			SpriteRenderer renderer = e.GetComponents<SpriteRenderer>()[0];
			renderer.color = HSBColor.ToColor(
				new HSBColor(
					Utils.Map(150.0f,0.0f,256.0f,0.0f,1.0f),
					1.0f,
					1.0f,
					1f)); 
			dot1[i]=e;

		}
		//z axis
		for (int i = 0; i < numObjects; i++) {
			GameObject e = Instantiate (dot);
			e.transform.position = new Vector3 (0, 0, (i*0.5f)-25.0f);
			e.transform.parent = transform;
			e.transform.localScale = new Vector3 (_scale, _scale, _scale);
			SpriteRenderer renderer = e.GetComponents<SpriteRenderer>()[0];
			renderer.color = HSBColor.ToColor(
				new HSBColor(
					Utils.Map(150.0f,0.0f,256.0f,0.0f,1.0f),
					1.0f,
					1.0f,
					1f)); 
			dot2[i]=e;
		}

		for (int i = 0; i < numObjects; i++) {
			GameObject e = Instantiate (dot);
			e.transform.parent = transform;
			float y = Mathf.Sin(((i*1.0f)/numObjects)*Mathf.PI*2)*5;
			float x = 0 ;
			float z = Mathf.Cos(((i*1.0f)/numObjects)*Mathf.PI*2)*5;

			e.transform.position = new Vector3 (x, y, z);
			e.transform.localScale = new Vector3 (_scale, _scale, _scale);
			SpriteRenderer renderer = e.GetComponents<SpriteRenderer>()[0];
			renderer.color = HSBColor.ToColor(
				new HSBColor(
					Utils.Map(150.0f,0.0f,256.0f,0.0f,1.0f),
					1.0f,
					1.0f,
					1f)); 
			dot3[i]=e;
		}
		//circle
	}

	void Update() {
		for(int i = 0 ; i < dot1.Length ; i++){
			dot1[i].transform.LookAt (Camera.main.transform);
		}
		for(int i = 0 ; i < dot2.Length ; i++){
			dot2[i].transform.LookAt (Camera.main.transform);
		}for(int i = 0 ; i < dot3.Length ; i++){
			dot3[i].transform.LookAt (Camera.main.transform);
		}
	}
}
