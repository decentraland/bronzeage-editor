using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Serializer : MonoBehaviour {

	public GameObject tile;

	public void Serialize(string title) {
		tile.name = title;
		STile original = new STile (tile);
		string content = original.ToBase64();
		Application.ExternalEval("console.log('" + content + "')");
		Application.ExternalEval("processTile('" + content + "')");
	}

}
