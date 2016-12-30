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

	[MenuItem("Window/Decentraland Editor")]
	public static void ShowWindow()
	{
		EditorWindow window = EditorWindow.GetWindow(typeof(DecentralandEditor));
		window.titleContent = new GUIContent ("Decentraland");
	}

	void OnGUI()
	{
		GameObject tile = Selection.activeGameObject;

		if (GUILayout.Button ("Get Position")) {
			// Get Parent
			GameObject parent = tile;
			while (parent.transform.parent) parent = tile.transform.parent.gameObject;

			// Check Bounds
			Bounds parentBounds = parent.GetComponent<Renderer>().bounds;
			Bounds objBounds = tile.GetComponent<Renderer>().bounds;

			Vector3 parentMinBounds = parentBounds.center - parentBounds.extents;
			Vector3 parentMaxBounds = parentBounds.center + parentBounds.extents;

			Vector3 objMinBounds = objBounds.center - objBounds.extents;
			Vector3 objMaxBounds = objBounds.center + objBounds.extents;

			if (objMinBounds.x < parentMinBounds.x ||
				objMaxBounds.x > parentMaxBounds.x ||
				objMinBounds.z < parentMinBounds.z ||
				objMaxBounds.z > parentMaxBounds.z) {
				Debug.LogWarning(tile.name + " is outside tile limits, it won't be serialized");
			}
		}

		if (GUILayout.Button ("New tile")) {
			CreateEmtpy (GetEmptyPosition ());
		}

		if (GUILayout.Button("Save to file")) {
			STile original = new STile (tile);
			float x = tile.transform.position.x / TILE_SIZE;
			float z = tile.transform.position.z / TILE_SIZE;
			Vector2 index = new Vector2(x, z);
			string path = Application.persistentDataPath + "/" + index [0] + ":" + index [1] + ".tld";
			original.ToFile (path);
			Debug.Log ("File Successfully exported to " + path);
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
}