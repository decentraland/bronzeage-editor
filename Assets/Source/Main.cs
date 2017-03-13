using UnityEngine;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


[System.Serializable]
public class STile {
	public static float TILE_SCALE = 4;
	public static float TILE_SIZE = TILE_SCALE * 10;
	public static int MAX_CHILDREN = 1024;

	string name = "";
	Color color = Color.white;
	byte[] texture = null;
	SObject[] children;

	public static STile FromFile(string path){
		FileStream file = File.Open (path, FileMode.Open);
		return FromStream (file);
	}

	public static STile FromBytes(byte[] bytes){
		Stream stream = new MemoryStream(bytes);
		return FromStream (stream);
	}

	private static STile FromStream(Stream stream) {
		using (stream) {
			IFormatter formatter = GetFormater ();
			return (STile) formatter.Deserialize (stream);
		}
	}

	private static IFormatter GetFormater() {
		IFormatter formatter = new BinaryFormatter();
		SurrogateSelector ss = new SurrogateSelector();
		ss.AddSurrogate(
			typeof(Vector3),
			new StreamingContext(StreamingContextStates.All),
			new Vector3SerializationSurrogate()
		);
		ss.AddSurrogate(
			typeof(Color),
			new StreamingContext(StreamingContextStates.All),
			new ColorSerializationSurrogate()
		);
		formatter.SurrogateSelector = ss;
		return formatter;
	}

	public STile(GameObject tile) {
		// Store Local Transform
		this.name = tile.name;

        // Store children
        Bounds bounds = new Bounds(Vector3.zero, new Vector3(100, 100, 100));
		children = new SObject[tile.transform.childCount];
		int index = 0;
		int childCount = 0;
		foreach (Transform t in tile.transform) {
			SObject child = new SObject(t.gameObject, bounds);
			children [index++] = child;
			childCount += child.ObjectCount();
		}

		// Check Max Child Objects
		if (childCount > MAX_CHILDREN) {
			throw new SerializationException ("Tail contains too many game objects (MAX " + MAX_CHILDREN +")");
		}
	}

	public GameObject ToInstance(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
        go.name = this.name;
		go.transform.localScale = new Vector3 (TILE_SCALE, TILE_SCALE, TILE_SCALE);
		go.transform.position = position;

		MeshRenderer renderer = go.GetComponent<MeshRenderer>();
		renderer.material.color = this.color;

		if (this.texture != null) {
			Texture2D tex = new Texture2D (2, 2);
			tex.LoadImage (this.texture);
			renderer.material.mainTexture = tex;
		}

		int objCount = 0;
		Bounds bounds = go.GetComponent<Renderer>().bounds;
		foreach (SObject child in this.children) {
			// Check Object Count
			if (objCount >= MAX_CHILDREN) { return go;}
			objCount = child.ToInstance(go, bounds, objCount);
		}

		return go;
	}

	public string GetName() {
		return this.name;
	}

	public void ToFile(string path) {
		FileStream file = File.Open (path, FileMode.Create);
		using (file) {
			IFormatter formatter = GetFormater ();
			formatter.Serialize(file, this);
		}
	}

	public string ToBase64() {
		using (MemoryStream stream = new MemoryStream()) {
			IFormatter formatter = GetFormater ();
			formatter.Serialize(stream, this);
			return Convert.ToBase64String(stream.ToArray());
		}
	}
}


[System.Serializable]
public class SObject {

	PrimitiveType mesh = PrimitiveType.Cube;
	Vector3 position = new Vector3(0,0,0);
	Vector3 angles = new Vector3(0,0,0);
	Vector3 scale = new Vector3(0,0,0);
	Color color = Color.white;
	byte[] texture = null;
	SObject[] children;

	private static bool IsInBoundaries(GameObject go, Bounds bounds) {
		Bounds goBounds = go.GetComponent<Renderer>().bounds;

		Vector3 parentMinBounds = bounds.center - bounds.extents;
		Vector3 parentMaxBounds = bounds.center + bounds.extents;

		Vector3 objMinBounds = goBounds.center - goBounds.extents;
		Vector3 objMaxBounds = goBounds.center + goBounds.extents;

		return (objMinBounds.x > parentMinBounds.x &&
			objMaxBounds.x < parentMaxBounds.x &&
			objMinBounds.z > parentMinBounds.z &&
			objMaxBounds.z < parentMaxBounds.z);
	}

	public SObject(GameObject go, Bounds bounds) {
		if (!IsInBoundaries (go, bounds)) {
			throw new SerializationException (go.name + " is outside of it's tile limits");
		}

		// Store Local Transform
		this.position = go.transform.localPosition;
		this.angles = go.transform.eulerAngles;
		this.scale = go.transform.localScale;

		// Store Mesh Primitive
		MeshFilter meshFilter = go.GetComponent<MeshFilter>();
		if (meshFilter.sharedMesh.name.StartsWith ("Cube")) {
			this.mesh = PrimitiveType.Cube;
		} else if (meshFilter.sharedMesh.name.StartsWith ("Sphere")) {
			this.mesh = PrimitiveType.Sphere;
		} else if (meshFilter.sharedMesh.name.StartsWith ("Capsule")) {
			this.mesh = PrimitiveType.Capsule;
		} else if (meshFilter.sharedMesh.name.StartsWith ("Plane")) {
			this.mesh = PrimitiveType.Plane;
		} else if (meshFilter.sharedMesh.name.StartsWith ("Cylinder")) {
			this.mesh = PrimitiveType.Cylinder;
		} else {
			this.mesh = PrimitiveType.Cube;
		}

		// Store Renderer Color
		MeshRenderer meshRenderer = go.GetComponent<MeshRenderer>();
		if (meshRenderer.sharedMaterial.HasProperty ("_Color")) {
			this.color = meshRenderer.sharedMaterial.color;
		}

		// Store Renderer Material
		if (meshRenderer.sharedMaterial.mainTexture) {
			Texture2D tex = (Texture2D)meshRenderer.sharedMaterial.mainTexture;
			this.texture = tex.EncodeToJPG ();
		}

		// Serialize children
		children = new SObject[go.transform.childCount];
		int index = 0;
		foreach (Transform t in go.transform) {
			children [index++] = new SObject(t.gameObject, bounds);
		}
	}

	public int ObjectCount() {
		int count = 1;
		foreach (SObject child in this.children) {
			count += child.ObjectCount ();
		}
		return count;
	}

	public int ToInstance(GameObject parent, Bounds bounds, int objCount) {
		GameObject go = GameObject.CreatePrimitive(this.mesh);
		if (parent) go.transform.SetParent (parent.transform);
		go.transform.localPosition = this.position;
		go.transform.localEulerAngles = this.angles;
		go.transform.localScale = this.scale;

		// Check Boundaries
		if (!IsInBoundaries (go, bounds)) {
			UnityEngine.Object.Destroy(go);
			return objCount;
		}

		MeshRenderer renderer = go.GetComponent<MeshRenderer>();
		renderer.material.color = this.color;

		if (this.texture != null) {
			Texture2D tex = new Texture2D (2, 2);
			tex.LoadImage (this.texture);
			renderer.material.mainTexture = tex;
		}

		objCount++;
		foreach (SObject child in children) {
			// Check Object Count
			if (objCount >= STile.MAX_CHILDREN) { return objCount; }
			objCount = child.ToInstance(go, bounds, objCount);
		}

		return objCount;
	}
}

public class Vector3SerializationSurrogate : ISerializationSurrogate 
{

	public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context) {
		Vector3 vector = (Vector3) obj;
		info.AddValue("x", vector.x);
		info.AddValue("y", vector.y);
		info.AddValue("z", vector.z);
	}

	public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
		Vector3 vector = (Vector3) obj;
		vector.x = (float) info.GetValue("x", typeof(float));
		vector.y = (float) info.GetValue("y", typeof(float));
		vector.z = (float) info.GetValue("z", typeof(float));
		obj = vector;
		return obj;
	}
}


public class ColorSerializationSurrogate : ISerializationSurrogate 
{

	public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context) {
		Color color = (Color) obj;
		info.AddValue("r", color.r);
		info.AddValue("g", color.g);
		info.AddValue("b", color.b);
		info.AddValue("a", color.a);
	}

	public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector) {
		Color color = (Color) obj;
		color.r = (float) info.GetValue("r", typeof(float));
		color.g = (float) info.GetValue("g", typeof(float));
		color.b = (float) info.GetValue("b", typeof(float));
		color.a = (float) info.GetValue("a", typeof(float));
		obj = color;
		return obj;
	}
}