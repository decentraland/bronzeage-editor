using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;

public class Builder : MonoBehaviour {

	public enum Mode {
		control, edit
	};

	public GameObject tool;
	public GameObject floor;
	public GameObject parent;
	public GameObject character;

	public FirstPersonController fpc;
	public GameObject controller;

	private Mode mode;

	public Material[] materials;
	public float CUBE_SIZE = 0.5f;
	bool deleting = false;

	private GameObject pivot;
	private Material pivot_material;

	// Lifecycle

	void Awake() {
		SetMode(Mode.edit);
	}

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

	void Update () {
		if (Input.GetKeyDown (KeyCode.Escape)) {
			ToggleMode ();
		}

		if (mode == Mode.edit) {
			EditMode();
		}
	}

	// Modes

	private void EditMode() {
		SetToolBindings();
		Ray ray = new Ray(character.transform.position, character.transform.forward);
		RaycastHit hit;

		if (deleting) {
			HideTool();

			if (Physics.Raycast (character.transform.position, character.transform.forward, out hit, 8)) {
				GameObject target = hit.transform.gameObject;

				if (pivot == null) {
					pivot = target;
				}

				if (IsSameObject(pivot, target)) {
					pivot.GetComponent<MeshRenderer> ().material = null;
				} else {
					pivot.GetComponent<MeshRenderer> ().material = pivot_material;
					pivot_material = target.GetComponent<MeshRenderer> ().material;
					pivot = target;
				}

				if (Input.GetMouseButtonDown(0)) {
					Destroy(target);
				}
			}
		} else {
			if (Physics.Raycast (character.transform.position, character.transform.forward, out hit, 8)) {
				tool.transform.localScale = new Vector3(CUBE_SIZE, CUBE_SIZE, CUBE_SIZE);
				tool.transform.position = hit.transform.position + (hit.normal * CUBE_SIZE);
				tool.transform.rotation = hit.transform.rotation;
			} else {
				HideTool();
			}

			if (Input.GetMouseButtonDown(0)) {
				GameObject cube = CreateCube (tool.transform.position);
				cube.transform.parent = parent.transform;
			}
		}
	}

	private void ToggleMode(){
		if (mode == Mode.control) {
			SetMode (Mode.edit);
		} else {
			SetMode (Mode.control);
		}
	}

	private void SetMode(Mode mode) {
		bool isEdit = mode == Mode.edit;
		this.mode = mode;
		Cursor.visible = ! isEdit;
		fpc.enabled = isEdit;
		controller.SetActive (! isEdit);
	}

	// Tool

	private void HideTool() {
		tool.transform.position = new Vector3 (0, -10, 0);
	}

	private void ToggleToolAction() {
		deleting = ! deleting;
	}

	private void SetToolBindings() {
		if (Input.GetKeyDown(KeyCode.Alpha1)) {
			tool.GetComponent<MeshRenderer> ().material = materials[0];
		}

		if (Input.GetKeyDown(KeyCode.Alpha2)) {
			tool.GetComponent<MeshRenderer> ().material = materials[1];
		}

		if (Input.GetKeyDown(KeyCode.Alpha3)) {
			tool.GetComponent<MeshRenderer> ().material = materials[2];
		}

		if (Input.GetKeyDown(KeyCode.Alpha4)) {
			tool.GetComponent<MeshRenderer> ().material = materials[3];
		}

		if (Input.GetKeyDown(KeyCode.Alpha0)) {
			ToggleToolAction();
		}
	}

	// GameObjects

	private GameObject CreateCube(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Cube);
		go.transform.position = position;
		go.transform.localScale = new Vector3 (CUBE_SIZE, CUBE_SIZE, CUBE_SIZE);
		go.GetComponent<MeshRenderer> ().material = tool.GetComponent<MeshRenderer> ().material;
		return go;
	}

	private bool IsSameObject(GameObject a, GameObject b) {
		return a.GetInstanceID() == b.GetInstanceID();
	}
}
