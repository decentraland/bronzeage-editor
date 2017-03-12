using UnityEditor;
using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;

public class DecentralandEditor : EditorWindow
{
	private static float TILE_SCALE = 4;
	private static float TILE_SIZE = TILE_SCALE * 10;

    private bool publishing = false;
    private bool publishError = false;
    private bool published = false;

    [Header("Decentraland Editor")]
    string nodeAddress = "http://localhost:8301";
    string nodeAuth = "";
	int xOffset = 0;
	int zOffset = 0;


    [MenuItem("Window/Decentraland Editor")]
	public static void ShowWindow()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(DecentralandEditor));
		window.titleContent = new GUIContent ("Decentraland");
	}

	public void OnInspectorUpdate() {
		Repaint();
	}

	void OnGUI()
	{
		GameObject tile = Selection.activeGameObject;

		if (!tile) {
            EditorGUILayout.HelpBox("Select a tile in the hierarchy view to enable Decentraland tile uploader.", MessageType.Warning);
			return;
		}

        GUILayout.Label("\n");
        GUILayout.Label("Configuration\n");

        nodeAddress = EditorGUILayout.TextField ("Your Node RPC URL", nodeAddress);
		nodeAuth = EditorGUILayout.TextField ("Node RPC Auth Token", nodeAuth);
		

        GUILayout.Label("\n");
        GUILayout.Label("\n");
        GUILayout.Label("Editor\n");
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label("Tile Coordinates");
		GUILayout.Label("X");
		xOffset = EditorGUILayout.IntField (xOffset);
		GUILayout.Label("Y");
		zOffset = EditorGUILayout.IntField (zOffset);

		EditorGUILayout.EndHorizontal ();

		if (GUILayout.Button("Publish Selected Tile ")) {
			// Serialize to file
			STile original = new STile (tile);
			Vector2 index = new Vector2(
				(tile.transform.position.x / TILE_SIZE) + xOffset,
				(tile.transform.position.z / TILE_SIZE) + zOffset
			);
			string content = original.ToBase64();

            PublishTile(index, content);
		}

        if (publishError)
        {
            EditorGUILayout.HelpBox("Error publishing tile!", MessageType.Error);
        }
        else
        {
            EditorGUILayout.HelpBox(publishing ? ("Publishing tile at (" + xOffset + "," + zOffset + ")") : (published?"Published!":""), MessageType.Info);
        }
        
    }

	private void CreateEmtpy(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		go.name = "Tile " + (position[0] / TILE_SIZE) + ":" + (position[2] / TILE_SIZE);
		go.transform.position = position;
		go.transform.localScale = new Vector3 (TILE_SCALE, TILE_SCALE, TILE_SCALE);
	}

	private Vector3 GetEmptyPosition() {
		Vector3 position = new Vector3 (0, 0, 0);
		float radius = 1;

		// Try the center
		if (!Physics.CheckSphere (position, radius)) return position;

		// Spiral check
		position = new Vector3 (1, 0, -1) * TILE_SIZE;
		int landSize = 3;
		int directionIndex = 0;
		int directionCount = 1;

		Vector3[] directions = {
			new Vector3 (0, 0, 1) * TILE_SIZE,
			new Vector3 (-1, 0, 0) * TILE_SIZE,
			new Vector3 (0, 0, -1) * TILE_SIZE,
			new Vector3 (1, 0, 0) * TILE_SIZE,
		};

		while (Physics.CheckSphere (position, radius)) {
			position += directions [directionIndex];
			directionCount++;

			// Make a turn
			if (directionCount == landSize) {
				directionCount = 1;
				directionIndex++;

				// Jump to next ring
				if (directionIndex == directions.Length) {
					landSize += 2;
					directionIndex = 0;
					directionCount = 1;
					position += new Vector3 (1, 0, -1) * TILE_SIZE;
				}
			}
		}

		return position;
	}

    void PublishTile(Vector2 index, string content) {
        publishing = true;
        publishError = false;
        published = false;

        // Basic Auth
        Dictionary<string,string> headers = new Dictionary<string, string>();
		headers["Authorization"] = "Basic " + System.Convert.ToBase64String(
			System.Text.Encoding.ASCII.GetBytes("bitcoinrpc:"+nodeAuth));

		string json = "{\"method\":\"settile\",\"params\":[" + index [0] + "," + index [1] + ",\"" + content + "\"],\"id\":0}";
		byte[] data = System.Text.Encoding.ASCII.GetBytes(json.ToCharArray());

		WWW www = new WWW(nodeAddress, data, headers);

		while (!www.isDone) {} // busy wait

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