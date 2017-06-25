using UnityEngine;

using System.Collections;
using System.Collections.Generic;

using System.IO;
using System.IO.Compression;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.UI;

public class PublishTile : MonoBehaviour {
  private string api_url = "http://138.197.44.64:6748";
  private string res = "";
  public GameObject response;
  public GameObject tileName;

  public void OnSubmit() {
    GameObject tile = GameObject.Find("My Tile");
    tile.name = tileName.GetComponent<InputField>().text;

    // Serialize
    STile original = new STile (tile);
    string content = original.ToBase64();

    Publish(content);
  }

  void Publish(string content) {
    byte[] data = System.Text.Encoding.UTF8.GetBytes(content);

    Dictionary<string,string> headers = new Dictionary<string, string>();
    headers.Add("Content-Type", "text/plain; charset=utf-8");

    WWW www = new WWW(api_url, data, headers);

    while (! www.isDone) {} // busy wait

    if (string.IsNullOrEmpty(www.error)) {
      APIResponse response = JsonUtility.FromJson<APIResponse>(www.text);
      string url = "https://decentraland.org/app/?x=" + response.x + "&y=" + response.y;
      res = "Success! Tile available in " + url;
    } else {
      res = "Error publishing tile! " + www.error;
    }

    response.GetComponent<InputField>().text = res;
  }

  public static byte[] Compress(byte[] data) {
    MemoryStream output = new MemoryStream();
    using (DeflateStream dstream = new DeflateStream(output, CompressionMode.Compress)) {
      dstream.Write(data, 0, data.Length);
    }
    return output.ToArray();
  }
}