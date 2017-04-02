using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour {

	public GameObject builder;
	public GameObject tile;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			GameObject newObject = Object.Instantiate (builder);
			newObject.transform.SetParent (this.transform);
			newObject.transform.position = builder.transform.position;
		}

		if (Input.GetMouseButtonDown (1)) {
			STile original = new STile (tile);
			string content = original.ToBase64();
			Debug.Log (content);
			Application.ExternalEval("console.log('" + content + "')");
		}

	}
}
