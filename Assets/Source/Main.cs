using UnityEngine;

using System;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;


[System.Serializable]
public class STile {
	public static float TILE_SCALE = 1;
	public static float TILE_SIZE = 40;
	public static float CHECK_BOUND = 8;
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
		Bounds bounds = new Bounds(Vector3.zero, new Vector3(40, 40, 40));
		children = new SObject[tile.transform.childCount];
		int index = 0;
		int childCount = 0;

		foreach (Transform t in tile.transform) {
			SObject child = new SObject(t.gameObject, bounds);
			children [index++] = child;
			childCount += child.ObjectCount();
		}

		Debug.Log(this.name + ": " + childCount);

		// Check Max Child Objects
		if (childCount > MAX_CHILDREN) {
			throw new SerializationException ("Tail contains too many game objects (MAX " + MAX_CHILDREN +")");
		}
	}

	public GameObject ToInstance(Vector3 position) {
		GameObject go = GameObject.CreatePrimitive(PrimitiveType.Plane);
		go.name = this.name;
		go.transform.position = position;

		MeshRenderer renderer = go.GetComponent<MeshRenderer>();
		renderer.material.color = this.color;

		if (this.texture != null) {
			Texture2D tex = new Texture2D (2, 2);
			tex.LoadImage (this.texture);
			renderer.material.mainTexture = tex;
		}

		int objCount = 0;
		Bounds bounds = new Bounds(position, new Vector3(TILE_SIZE, TILE_SIZE, TILE_SIZE));

		foreach (SObject child in this.children) {
			// Check Object Count
			if (objCount >= MAX_CHILDREN) {
				return go;
			}

			objCount = child.ToInstance(go, bounds, objCount);
		}

		Debug.Log(this.name + ": " + objCount);

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

	public static float CHECK_BOUND = 8;
	PrimitiveType mesh = PrimitiveType.Cube;
	Vector3 position = new Vector3(0,0,0);
	Vector3 angles = new Vector3(0,0,0);
	Vector3 scale = new Vector3(0,0,0);
	Color color = Color.white;
	byte[] texture = null;
	SObject[] children;

	// Leave known-working bounds check alone for publishing
	private static bool IsInBoundaries(GameObject go, Bounds bounds) {
		Bounds goBounds = go.GetComponent<Renderer>().bounds;

		Vector3 parentMinBounds = bounds.center - bounds.extents;
		Vector3 parentMaxBounds = bounds.center + bounds.extents;

		Vector3 objMinBounds = goBounds.center / CHECK_BOUND - goBounds.extents / CHECK_BOUND;
		Vector3 objMaxBounds = goBounds.center / CHECK_BOUND + goBounds.extents / CHECK_BOUND;

		return (objMinBounds.x > parentMinBounds.x &&
			objMaxBounds.x < parentMaxBounds.x &&
			objMinBounds.z > parentMinBounds.z &&
			objMaxBounds.z < parentMaxBounds.z
		);
	}
	
	private static bool InArea(Bounds objBounds, Bounds areaBounds) {
		// Calculate area limits
		Vector3 areaMin = areaBounds.center - areaBounds.extents;
		Vector3 areaMax = areaBounds.center + areaBounds.extents;

		// Calculate object limits
		Vector3 objMin = objBounds.center - objBounds.extents;
		Vector3 objMax = objBounds.center + objBounds.extents;

		// Check for in top down area
		/// No vertical bounds
		bool inArea = (
			objMin.x > areaMin.x &&
			objMax.x < areaMax.x &&
			objMin.z > areaMin.z &&
			objMax.z < areaMax.z
		);

		// Return result
		return inArea;
	}
	
	private static bool InArea(GameObject obj, Bounds areaBounds) {
		// Acquire object bounds
		Bounds objBounds = obj.GetComponent<Renderer>().bounds;
		// Check for in area
		return SObject.InArea(objBounds, areaBounds);
	}

	public SObject(GameObject go, Bounds bounds) {
		if (!IsInBoundaries (go, bounds)) {
			throw new SerializationException (go.name + " is outside of its tile limits");
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
		if (!InArea (go, bounds)) {
			Debug.Log("Object not in bounds!" + go.GetComponent<Renderer>().bounds.ToString() + bounds.ToString());
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
			if (objCount >= STile.MAX_CHILDREN) {
				return objCount;
			}
			objCount = child.ToInstance(go, bounds, objCount);
		}

		return objCount;
	}
}

public class Vector3SerializationSurrogate : ISerializationSurrogate {

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


public class ColorSerializationSurrogate : ISerializationSurrogate {

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