using UnityEngine;

using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

public class LandGenerator : MonoBehaviour {

	public GameObject player;
	public GameObject borderBox;

	private static float TILE_SCALE = 4;
	private static float TILE_SIZE = TILE_SCALE * 10;

	private Dictionary<Vector2, bool> world = new Dictionary<Vector2, bool>();
	private Dictionary<Vector2, string> names = new Dictionary<Vector2, string>();
	private Dictionary<Vector2, bool> visited = new Dictionary<Vector2, bool>();

	private Vector2 currentTile = new Vector2(0, 0);

	// Use this for initialization
	void Start () {
		CreatePlaneAt (new Vector2 (0, 0));
	}

	// This function creates planes for adjacent enviroment
	void CreateEnvironment(Vector3 position) {
		Vector2 current = GetCurrentPlane(player.transform.position);

		// Update Border Box
		if (current != currentTile) {
			borderBox.transform.position = indexToPosition (current);
			currentTile = current;
		}

		// Exit if we already visited this tile
		if (visited.ContainsKey (current)) return;

		// Expand Area
		visited.Add (current, true);
		CreatePlaneAt (current + new Vector2 (0, 1));
		CreatePlaneAt (current + new Vector2 (0, -1));
		CreatePlaneAt (current + new Vector2 (1, 0));
		CreatePlaneAt (current + new Vector2 (-1, 0));
		CreatePlaneAt (current + new Vector2 (1, 1));
		CreatePlaneAt (current + new Vector2 (-1, -1));
		CreatePlaneAt (current + new Vector2 (1, -1));
		CreatePlaneAt (current + new Vector2 (-1, 1));

		CreatePlaneAt (current + new Vector2 (0, 2));
		CreatePlaneAt (current + new Vector2 (0, -2));
		CreatePlaneAt (current + new Vector2 (2, 0));
		CreatePlaneAt (current + new Vector2 (-2, 0));
		CreatePlaneAt (current + new Vector2 (2, 2));
		CreatePlaneAt (current + new Vector2 (-2, -2));
		CreatePlaneAt (current + new Vector2 (2, -2));
		CreatePlaneAt (current + new Vector2 (-2, 2));

		CreatePlaneAt (current + new Vector2 (1, 2));
		CreatePlaneAt (current + new Vector2 (-1, 2));
		CreatePlaneAt (current + new Vector2 (1, -2));
		CreatePlaneAt (current + new Vector2 (-1, -2));
		CreatePlaneAt (current + new Vector2 (2, 1));
		CreatePlaneAt (current + new Vector2 (2, -1));
		CreatePlaneAt (current + new Vector2 (-2, 1));
		CreatePlaneAt (current + new Vector2 (-2, -1));
		CreatePlaneAt (current + new Vector2 (2, 2));
		CreatePlaneAt (current + new Vector2 (-2, -2));
		CreatePlaneAt (current + new Vector2 (2, -2));
		CreatePlaneAt (current + new Vector2 (-2, 2));
	}

	void CreatePlaneAt(Vector2 index) {
		if (world.ContainsKey (index)) return;
		StartCoroutine("FetchTile", index);
		world.Add (index, true);
	}

	// Plane dimentions are TILE_SIZE x TILE_SIZE, with center in the middel.
	Vector2 GetCurrentPlane(Vector3 position) {
		int x = Mathf.CeilToInt ((position[0] - (TILE_SIZE/2)) / TILE_SIZE);
		int z = Mathf.CeilToInt ((position[2] - (TILE_SIZE/2)) / TILE_SIZE);
		return new Vector2(x, z);
	}

	// Update is called once per frame
	void Update () {
		CreateEnvironment (player.transform.position);
	}

	void OnGUI () {
		string tileName;
		if (!names.TryGetValue (currentTile, out tileName)) tileName = "Empty Land";
		string message = tileName + " (" + currentTile [0] + ":" + currentTile [1] + ")";
		GUI.Label (new Rect (10, 10, 200, 20), message);
	}

	private Vector3 indexToPosition(Vector2 index) {
		float x = (index [0] * TILE_SIZE);
		float z = (index [1] * TILE_SIZE);
		return new Vector3 (x, 0, z);
	}

//	IEnumerator PublishTile(Vector2 index) {
//		Vector3 pos = indexToPosition (index);
//
//		// Temporal Placeholder
//		GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
//		plane.transform.position = pos;
//		plane.transform.localScale = new Vector3 (TILE_SCALE, TILE_SCALE, TILE_SCALE);
//
//		// Basic Auth
//		Dictionary<string,string> headers = new Dictionary<string, string>();
//		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
//			System.Text.Encoding.ASCII.GetBytes("bitcoinrpc:38Dpwnjsj2zn3QETJ6GKv8YkHomA"));
//
//		string json = "{\"method\":\"gettile\",\"params\":[" + index [0] + "," + index [1] + "],\"id\":0}";
//		byte[] data = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());
//
//		WWW www = new WWW("http://localhost:8001/", data, headers);
//		yield return www;
//
//		if (string.IsNullOrEmpty(www.error)) {
//			RPCResponse response = JsonUtility.FromJson<RPCResponse>(www.text);
//			Debug.Log("Tail " + index + " -> " + response.IsEmpty() + " " + response.IsUnmined());
//
//			MeshRenderer renderer = plane.GetComponent<MeshRenderer> ();
//			if (response.IsEmpty ()) {
//				renderer.material.color = Color.green;
//			} else if (response.IsUnmined ()) {
//				renderer.material.color = Color.gray;
//				names.Add (index, "Unclaimed Land");
//			}
//
//			//			STile t = STile.FromBytes (www.bytes);
//			//			t.ToInstance (pos);
//			//			names.Add (index, t.GetName ());
//			//			Destroy(plane);
//		} else {
//			Debug.Log("Error! " + www.error);
//		}
//	}

	IEnumerator FetchTile(Vector2 index) {
		Vector3 pos = indexToPosition (index);

		// Temporal Placeholder
		GameObject plane = GameObject.CreatePrimitive (PrimitiveType.Plane);
		plane.transform.position = pos;
		plane.transform.localScale = new Vector3 (TILE_SCALE, TILE_SCALE, TILE_SCALE);

		// Basic Auth
		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
			System.Text.Encoding.ASCII.GetBytes("bitcoinrpc:38Dpwnjsj2zn3QETJ6GKv8YkHomA"));

		// JSON Data
		string json = "{\"method\":\"gettile\",\"params\":[" + index [0] + "," + index [1] + "],\"id\":0}";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

		WWW www = new WWW("http://localhost:8001/", data, headers);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			RPCResponse response = JsonUtility.FromJson<RPCResponse>(www.text);
			MeshRenderer renderer = plane.GetComponent<MeshRenderer> ();

			if (response.IsEmpty ()) {
				renderer.material.color = Color.green;
			
			} else if (response.IsUnmined ()) {
				renderer.material.color = Color.gray;
				names.Add (index, "Unclaimed Land");
			
			} else if (response.HasData()) {

				// Download tile content
				string fileName = "" + index [0] + "." + index [1] + ".lnd";
				www = new WWW("http://localhost:9000/tile/" + fileName);
				yield return www;

				if (string.IsNullOrEmpty (www.error)) {
					STile t = STile.FromBytes (www.bytes);
					t.ToInstance (pos);
					names.Add (index, t.GetName ());
					Destroy (plane);
				} else {
					Debug.Log("Can't fetch tile content! " + index + " " + www.error);
				}
			}

		} else {
			Debug.Log("Error on RPC call 'gettile': " + www.error);
		}
	}
}

[System.Serializable]
public class RPCResponse {
	public string result = null;
	public string error = null;
	public string id = null;

	public bool IsUnmined() {
		return this.result == "";
	}

	public bool IsEmpty() {
		return this.result == "0000000000000000000000000000000000000000000000000000000000000000";
	}

	public bool HasData() {
		return !(this.IsEmpty () || this.IsUnmined ());
	}
}
