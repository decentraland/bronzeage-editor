using UnityEngine;

using System.Runtime.Serialization;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;

public class LandGenerator : MonoBehaviour
{

    public GameObject player;
    public GameObject borderBox;
    public GameObject baseTile;
    public GameObject loading;
    public GameObject station;


    private static float TILE_SCALE = 4;
    private static float TILE_SIZE = TILE_SCALE * 10;

    private Dictionary<Vector2, bool> world = new Dictionary<Vector2, bool>();
    private Dictionary<Vector2, string> names = new Dictionary<Vector2, string>();
    private Dictionary<Vector2, bool> visited = new Dictionary<Vector2, bool>();

    private Vector2 currentTile;

    // Use this for initialization
    void Start()
    {
        Cursor.visible = false;
        currentTile = new Vector2(0, 0);
        currentTile.x = PlayerPrefs.GetInt("TileX");
        currentTile.y = PlayerPrefs.GetInt("TileY");
        
        player.transform.position = indexToPosition(currentTile);


        CreatePlaneAt(currentTile);
    }

    // Get initial position from url querystring
    Vector2 GetInitialPosition(string url)
    {
        if (url.Length > 0)
        {
            char[] querySplit = { '=', '&' };
            string[] parts = url.Split(querySplit);
            if (parts.Length >= 4)
            {
                try
                {
                    int x = int.Parse(parts[1]);
                    int y = int.Parse(parts[3]);
                    return new Vector2(x, y);
                }
                catch { }
            }
        }
        return new Vector2(0, 0);
    }

    // This function creates planes for adjacent enviroment
    void CreateEnvironment(Vector3 position)
    {
        Vector2 current = GetCurrentPlane(player.transform.position);

        // Update Border Box

        if (current != currentTile)
        {
            borderBox.transform.position = indexToPosition(current);
            currentTile = current;
        }

        // Exit if we already visited this tile
        if (visited.ContainsKey(current)) return;

        // Expand Area
        visited.Add(current, true);
        CreatePlaneAt(current + new Vector2(0, 1));
        CreatePlaneAt(current + new Vector2(0, -1));
        CreatePlaneAt(current + new Vector2(1, 0));
        CreatePlaneAt(current + new Vector2(-1, 0));
        CreatePlaneAt(current + new Vector2(1, 1));
        CreatePlaneAt(current + new Vector2(-1, -1));
        CreatePlaneAt(current + new Vector2(1, -1));
        CreatePlaneAt(current + new Vector2(-1, 1));

        CreatePlaneAt(current + new Vector2(0, 2));
        CreatePlaneAt(current + new Vector2(0, -2));
        CreatePlaneAt(current + new Vector2(2, 0));
        CreatePlaneAt(current + new Vector2(-2, 0));
        CreatePlaneAt(current + new Vector2(2, 2));
        CreatePlaneAt(current + new Vector2(-2, -2));
        CreatePlaneAt(current + new Vector2(2, -2));
        CreatePlaneAt(current + new Vector2(-2, 2));

        CreatePlaneAt(current + new Vector2(1, 2));
        CreatePlaneAt(current + new Vector2(-1, 2));
        CreatePlaneAt(current + new Vector2(1, -2));
        CreatePlaneAt(current + new Vector2(-1, -2));
        CreatePlaneAt(current + new Vector2(2, 1));
        CreatePlaneAt(current + new Vector2(2, -1));
        CreatePlaneAt(current + new Vector2(-2, 1));
        CreatePlaneAt(current + new Vector2(-2, -1));
        CreatePlaneAt(current + new Vector2(2, 2));
        CreatePlaneAt(current + new Vector2(-2, -2));
        CreatePlaneAt(current + new Vector2(2, -2));
        CreatePlaneAt(current + new Vector2(-2, 2));
    }

    void CreatePlaneAt(Vector2 index)
    {
        if (world.ContainsKey(index)) return;
        StartCoroutine("FetchTile", index);
        world.Add(index, true);
    }

    // Plane dimentions are TILE_SIZE x TILE_SIZE, with center in the middel.
    Vector2 GetCurrentPlane(Vector3 position)
    {
        int x = Mathf.CeilToInt((position[0] - (TILE_SIZE / 2)) / TILE_SIZE);
        int z = Mathf.CeilToInt((position[2] - (TILE_SIZE / 2)) / TILE_SIZE);
        return new Vector2(x, z);
    }

    // Update is called once per frame
    void Update()
    {
        CreateEnvironment(player.transform.position);
    }

    void OnGUI()
    {
        string tileName;
        if (!names.TryGetValue(currentTile, out tileName)) tileName = "Empty Land";
        string message = tileName + " (" + currentTile[0] + ":" + currentTile[1] + ")";
        GUI.Label(new Rect(10, 10, 200, 20), message);
    }

    private Vector3 indexToPosition(Vector2 index)
    {
        float x = (index[0] * TILE_SIZE);
        float z = (index[1] * TILE_SIZE);
        return new Vector3(x, 0, z);
    }

    IEnumerator FetchTile(Vector2 index)
    {
        Vector3 pos = indexToPosition(index);

        // Temporal Placeholder
        GameObject plane = Instantiate(baseTile, pos, Quaternion.identity);
        GameObject loader = Instantiate(loading, pos, Quaternion.identity);
        loader.transform.position = new Vector3(pos.x, pos.y + 2, pos.z);
        string fileName = "" + index[0] + "." + index[1] + ".lnd";
        WWW www = new WWW("https://decentraland.org/content/" + fileName);
        yield return www;

        if (!string.IsNullOrEmpty(www.error))
        {

            Debug.Log("Can't fetch tile content! " + index + " " + www.error);
            names.Add(index, "Unclaimed Land");
            Destroy(loader);
        }
        else
        {
            Debug.Log("Downloaded content for tile (" + index[0] + "," + index[1] + ")");
            try
            {
                STile t = STile.FromBytes(www.bytes);
                if (t.GetName().StartsWith("_sta") || index == Vector2.zero)
                {
                    Debug.Log("a");
                    Instantiate(station, pos + new Vector3(20, 0, -20) , Quaternion.Euler(-90, 90, 0));
                    Destroy(plane);
                }
                else
                {
                    Debug.Log("b");
                    t.ToInstance(pos);
                    names.Add(index, t.GetName());
                }
            }
            catch (EndOfStreamException e)
            {
                Debug.Log("Invalid" + index + e.ToString());
            }
            catch (SerializationException e)
            {
                Debug.Log("Invalid" + index + e.ToString());
            }
            catch (Exception e)
            {
                Debug.Log("Exception found in " + index + e.ToString());
            }
            finally
            {
                Destroy(loader);
            }
        }
    }
}

[System.Serializable]
public class RPCResponse
{
    public string result = null;
    public string error = null;
    public string id = null;

    public bool IsUnmined()
    {
        return this.result == "";
    }

    public bool IsEmpty()
    {
        return this.result == "0000000000000000000000000000000000000000000000000000000000000000";
    }

    public bool HasData()
    {
        return !(this.IsEmpty() || this.IsUnmined());
    }
}