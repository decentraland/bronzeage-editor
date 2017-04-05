using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Builder : MonoBehaviour {

	public GameObject tool;
	public GameObject floor;
	public GameObject parent;

	public Material m1;
	public Material m2;
	public Material m3;

	public static float CUBE_SIZE = 0.5f;

	// Use this for initialization
	void Start () {
		float cubes = floor.transform.localScale.x / CUBE_SIZE / 2;
		for (float i = -cubes ; i < cubes; i++) {
			for (float j = -cubes; j < cubes; j++) {
				float x = i * CUBE_SIZE + 0.25f;
				float z = j * CUBE_SIZE + 0.25f;
				CreateCube (new Vector3 (x, -0.25f, z));
			}
		}
	}


	private GameObject CreateCube(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = position;
		go.transform.localScale = new Vector3 (CUBE_SIZE, CUBE_SIZE, CUBE_SIZE);
		go.GetComponent<MeshRenderer> ().material = tool.GetComponent<MeshRenderer> ().material;
		return go;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetMouseButtonDown (0)) {
			GameObject cube = CreateCube (tool.transform.position);
			cube.transform.parent = parent.transform;
		}

		RaycastHit hit;
		if (Physics.Raycast (transform.position, transform.forward, out hit, 4)) {
			tool.transform.position = hit.transform.position + (hit.normal * CUBE_SIZE);
			tool.transform.rotation = hit.transform.rotation;
		} else {
			tool.transform.position = new Vector3 (0, -10, 0);
		}

		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			tool.GetComponent<MeshRenderer> ().material = m1;
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			tool.GetComponent<MeshRenderer> ().material = m2;
		}

		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			tool.GetComponent<MeshRenderer> ().material = m3;
		}

	}
}
