using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class Control : MonoBehaviour {
	private static float TILE_SCALE = 4;
	private static float TILE_SIZE = TILE_SCALE * 10;

	private bool publishing = false;
	private bool publishError = false;
	private bool published = false;

	public InputField input_url;
	public InputField input_token;
	public InputField input_x;
	public InputField input_y;

	public void OnSubmit() {
		GameObject tile = GameObject.Find("My Tile");
 		STile original = new STile (tile);

		// Compute index
 		int xOffset = int.Parse (input_x.text);
 		int yOffset = int.Parse (input_y.text);

 		Vector2 index = new Vector2(
 			(tile.transform.position.x / TILE_SIZE) + xOffset,
 			(tile.transform.position.z / TILE_SIZE) + yOffset
 		);

		// Serialize
 		string content = original.ToBase64();

 		PublishTile(index, content);

		// TODO: Display submit feedback
	}

	void PublishTile(Vector2 index, string content) {
		publishing = true;
		publishError = false;
		published = false;

		// Basic Auth
		Dictionary<string,string> headers = new Dictionary<string, string>();
		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
			System.Text.Encoding.ASCII.GetBytes("bitcoinrpc:" + input_token.text)
		);

		string json = "{\"method\":\"settile\",\"params\":[" + index [0] + "," + index [1] + ",\"" + content + "\"],\"id\":0}";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

		WWW www = new WWW(input_url.text, data, headers);

		while (! www.isDone) {} // busy wait

		if (string.IsNullOrEmpty(www.error)) {
			RPCResponse response = JsonUtility.FromJson<RPCResponse>(www.text);
			Debug.Log("Successfully published tile!");
			published = true;
		} else {
			Debug.Log("Error publishing tile! " + www.error);
			publishError = true;
		}

		publishing = false;
	}
}