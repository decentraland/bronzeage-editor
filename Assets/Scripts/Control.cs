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

	private string api_url = "http://138.197.44.64:6748";

	public void OnSubmit() {
		GameObject tile = GameObject.Find("My Tile");
 		STile original = new STile (tile);

		// Serialize
 		string content = original.ToBase64();
 		PublishTile(content);
		// TODO: Display submit feedback
	}

	void PublishTile(string content) {
		publishing = true;
		publishError = false;
		published = false;

		// Basic Auth
		Dictionary<string,string> headers = new Dictionary<string, string>();
		byte[] data = System.Text.Encoding.ASCII.GetBytes(content);

		print(content);
		WWW www = new WWW(api_url, data, headers);

		while (! www.isDone) {} // busy wait

		if (string.IsNullOrEmpty(www.error)) {
			RPCResponse response = JsonUtility.FromJson<RPCResponse>(www.text);
			Debug.Log("Successfully published tile!");
			Debug.Log(www.text);
			published = true;
		} else {
			Debug.Log("Error publishing tile! " + www.error);
			publishError = true;
		}

		publishing = false;
	}
}