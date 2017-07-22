using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;
using System.IO;

using CEUtilities.Helpers;

public class DecentralandEditor : EditorWindow {
	private static float TILE_SCALE = 1;
	private static float TILE_SIZE = TILE_SCALE * 40;

	private bool publishing = false;
	private bool publishError = false;
	private bool published = false;
	private bool deleteTempFiles = true;

	[Header("Decentraland Editor")]
	string tempFolder = "Assets/_Temp/";
	string nodeAddress = "http://localhost:8301";
	string nodeAuth = "";
	int xOffset = 0;
	int zOffset = 0;


	[MenuItem("Window/Decentraland Editor")]
	public static void ShowWindow() {
		EditorWindow window = EditorWindow.GetWindow(typeof(DecentralandEditor));
		window.titleContent = new GUIContent("Decentraland");
	}

	public void OnInspectorUpdate() {
		Repaint();
	}

	void OnGUI() {
		GameObject tile = Selection.activeGameObject;

		if (tile == null || ! tile.CompareTag("Tile")) {
			EditorGUILayout.HelpBox("Select a tile in the hierarchy view to enable Decentraland tile uploader.", MessageType.Warning);
			return;
		}

		GUILayout.Label("\n");
		GUILayout.Label("Configuration\n");

		nodeAddress = EditorGUILayout.TextField("Your Node RPC URL", nodeAddress);
		nodeAuth = EditorGUILayout.TextField("Node RPC Auth Token", nodeAuth);

		GUILayout.Label("\n");
		GUILayout.Label("\n");
		GUILayout.Label("Editor\n");
		EditorGUILayout.BeginHorizontal();
		GUILayout.Label("Tile Coordinates");
		GUILayout.Label("X");
		xOffset = EditorGUILayout.IntField(xOffset);
		GUILayout.Label("Y");
		zOffset = EditorGUILayout.IntField(zOffset);

		EditorGUILayout.EndHorizontal();

		// Delete temporal assets
		if (deleteTempFiles && DirUtil.Exists("Assets/_Temp")) {
			DirUtil.DeleteFolder("Assets/_Temp");
		}

		if (GUILayout.Button("Publish Selected Tile ")) {
			var prefab = PrefabUtil.CreatePrefabFromActiveGO(tempFolder + "Prefab/");
			var manifest = AssetBundleEditorUtil.CreateAssetBundle(prefab, "TileBundle", tempFolder + "Bundle/", BuildTarget.WebGL);
			var bytes = AssetBundleEditorUtil.GetBytesFromManifest(manifest, tempFolder + "Bundle/");
			string content = IOHelper.BytesToBase64String(bytes);

			Vector2 index = new Vector2(
				(tile.transform.position.x / TILE_SIZE) + xOffset,
				(tile.transform.position.z / TILE_SIZE) + zOffset
			);

			PublishTile(index, content);
		}

		if (publishError) {
			EditorGUILayout.HelpBox("Error publishing tile!", MessageType.Error);
		} else {
			EditorGUILayout.HelpBox(publishing ? ("Publishing tile at (" + xOffset + "," + zOffset + ")") : (published ? "Published!" : ""), MessageType.Info);
		}
	}

	void PublishTile(Vector2 index, string content) {
		publishing = true;
		publishError = false;
		published = false;

		// Basic Auth
		Dictionary<string, string> headers = new Dictionary<string, string>();
		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
			System.Text.Encoding.ASCII.GetBytes("bitcoinrpc:" + nodeAuth)
		);

		// TODO: fix compression efforts
		// byte[] binaryContent = System.Text.Encoding.ASCII.GetBytes(content);
		// byte[] compressedBinaryContent = CLZF2.Compress(binaryContent);

		// Debug.Log("Content length:");
		// Debug.Log(binaryContent.Length);
		// Debug.Log("Content length compressed:");
		// Debug.Log(compressedBinaryContent.Length);

		// string json = "{\"method\":\"settile\",\"params\":[" + index [0] + "," + index [1] + ",\"" + compressedBinaryContent + "\"],\"id\":0}";

		string json = "{\"method\":\"settile\",\"params\":[" + index[0] + "," + index[1] + ",\"" + content + "\"],\"id\":0}";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

		WWW www = new WWW(nodeAddress, data, headers);

		while (!www.isDone) { } // busy wait

		// Web transaction error
		if (!string.IsNullOrEmpty(www.error)) {
			Debug.Log("Error publishing tile! Web error: " + www.error);
			publishError = true;
		} else {
			// Process RPC response
			string responseJson = www.text;
			Debug.Log(responseJson);
			RPCResponse response = JsonUtility.FromJson<RPCResponse>(responseJson);

			// RPC error response
			/*
			 * NOTE: Not great as it assumes code 0 is success, nowhere guaranteed in JSONRPC or the bitcoin API.
			 * Artifact of the JsonUtility deserialization which generates an instance for a `null` value in the JSON.
			 * Should not be done this way in production.
			 */
			if (response.error != null && response.error.code != 0) {
				Debug.Log("Error publishing tile! RPC error: " + response.error.message);
				publishError = true;
			} else {
				Debug.Log("Successfully published tile!");
				published = true;
			}
		}

		publishing = false;
	}

}