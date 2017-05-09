using UnityEngine;

using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;

public class LandGenerator : MonoBehaviour {

	public GameObject player;
	public GameObject borderBox;
    public GameObject baseTile;
    public GameObject loading;


    private static float TILE_SCALE = 4;
	private static float TILE_SIZE = TILE_SCALE * 10;

	private Dictionary<Vector2, bool> world = new Dictionary<Vector2, bool>();
	private Dictionary<Vector2, string> names = new Dictionary<Vector2, string>();
	private Dictionary<Vector2, bool> visited = new Dictionary<Vector2, bool>();

	private Vector2 currentTile;

	private string posX = "0";
	private string posZ = "0";

	// Use this for initialization
	void Start () {
		currentTile = GetInitialPosition (Application.absoluteURL);
		player.transform.position = indexToPosition (currentTile);
		posZ = indexToPosition (currentTile).z.ToString ();
		posX = indexToPosition(currentTile).x.ToString ();
		CreatePlaneAt (currentTile);
	}

	// Get initial position from url querystring
	Vector2 GetInitialPosition(string url) {
		if (url.Length > 0) {
			char[] querySplit = { '=', '&' };
			string[] parts = url.Split (querySplit);
			if (parts.Length >= 4) {
				try {
					int x = int.Parse (parts[1]);
					int y = int.Parse (parts[3]);
					return new Vector2 (x, y);
				} catch {}
			}
		}
		return new Vector2(0, 0);
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

		GUI.Box (new Rect (Screen.width - 105,5,100,120), "Teleportation");


		posX = GUI.TextField (new Rect (Screen.width - 85,35,60,20), posX.ToString());
		posZ = GUI.TextField (new Rect (Screen.width - 85,65,60, 20), posZ.ToString());
		if (GUI.Button (new Rect (Screen.width - 85, 95, 60, 20), "Teleport!")) {
			int valueX;
			bool successX = int.TryParse(posX, out valueX);

			int valueZ;
			bool successZ = int.TryParse(posZ, out valueZ);
			if (successX && successZ) {


				player.transform.position = indexToPosition (new Vector2 ((float)valueX, (float)valueZ));
				posZ = valueZ.ToString ();
				posX = valueX.ToString ();
				CreatePlaneAt (new Vector2 ((float)valueX, (float)valueZ));
			} else {
				Debug.Log ("Error parsing int");
			}
		}
	}

	private Vector3 indexToPosition(Vector2 index) {
		float x = (index [0] * TILE_SIZE);
		float z = (index [1] * TILE_SIZE);
		return new Vector3 (x, 0, z);
	}
    
	IEnumerator FetchTile(Vector2 index) {
		Vector3 pos = indexToPosition (index);

		// Temporal Placeholder
        GameObject plane = Instantiate(baseTile, pos, Quaternion.identity);
        GameObject loader = Instantiate(loading, pos, Quaternion.identity);
        loader.transform.position = new Vector3(pos.x, pos.y + 2, pos.z);

        // Basic Auth
        Dictionary<string,string> headers = new Dictionary<string, string>();
		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
			System.Text.Encoding.ASCII.GetBytes("bitcoinrpc:38Dpwnjsj1zn3QETJ6GKv8YkHomA"));

		// JSON Data
		string json = "{\"method\":\"gettile\",\"params\":[" + index [0] + "," + index [1] + "],\"id\":0}";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

		WWW www = new WWW("http://s1.decentraland.org:8301", data, headers);
		yield return www;

		if (string.IsNullOrEmpty(www.error)) {
			RPCResponse response = JsonUtility.FromJson<RPCResponse>(www.text);
            Destroy(loader);
			if (response.IsEmpty ()) {
                // TODO: do empty behavior
			} else if (response.IsUnmined ()) {
				names.Add(index, "Unclaimed Land");
			} else if (response.HasData()) {
				// Download tile content
				string fileName = "" + index [0] + "." + index [1] + ".lnd";
				www = new WWW("http://s1.decentraland.org:9301/tile/" + fileName);
				yield return www;

				if (string.IsNullOrEmpty (www.error)) {
                    Debug.Log("Downloaded content for tile (" + index[0]+","+index[1]+")");
					STile t = STile.FromBytes(www.bytes);
					t.ToInstance(pos);
					names.Add(index, t.GetName());
					
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
