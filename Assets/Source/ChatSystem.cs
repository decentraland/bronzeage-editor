/*
 * This is the front facing script to control how MumbleUnity works.
 * It's expected that, to fit in properly with your application,
 * You'll want to change this class (and possible SendMumbleAudio)
 * in order to work the way you want it to
 */
using UnityEngine;
using System;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections.Generic;
using System.Collections;
using Mumble;

public class ChatSystem : MonoBehaviour {

    public GameObject MyMumbleAudioPlayerPrefab;
    public MumbleMicrophone MyMumbleMic;
    public DebugValues DebuggingVariables;
    public GameObject player;

    private float lastTime = 0;
    private MumbleClient _mumbleClient;
    public string HostName = "1.2.3.4";
    public int Port = 64738;
    public string Username = "ExampleUser";
    public string Password = "1passwordHere!";
    public string ChannelToJoin = "";

    private readonly Dictionary<string, PositionUpdater> Users = new Dictionary<string, PositionUpdater>();

    void Start () {

        if (HostName == "1.2.3.4")
        {
            Debug.LogError("Please set the mumble host name to your mumble server");
            return;
        }
        Application.runInBackground = true;
        _mumbleClient = new MumbleClient(HostName, Port, CreateMumbleAudioPlayerFromPrefab, DestroyMumbleAudioPlayer, OnMessage, DebuggingVariables);
        _mumbleClient.JoinChannel(ChannelToJoin);
        if (DebuggingVariables.UseRandomUsername)
            Username += UnityEngine.Random.Range(0, 100f);
        _mumbleClient.Connect(Username, Password);

        if(MyMumbleMic != null)
            _mumbleClient.AddMumbleMic(MyMumbleMic);

#if UNITY_EDITOR
        if (DebuggingVariables.EnableEditorIOGraph)
        {
            EditorGraph editorGraph = EditorWindow.GetWindow<EditorGraph>();
            editorGraph.Show();
            StartCoroutine(UpdateEditorGraph());
        }
#endif
    }
    private MumbleAudioPlayer CreateMumbleAudioPlayerFromPrefab(string name)
    {
        // Depending on your use case, you might want to add the prefab to an existing object (like someone's head)
        // If you have users entering and leaving frequently, you might want to implement an object pool
        GameObject newObj = GameObject.Instantiate(MyMumbleAudioPlayerPrefab);
        newObj.name = name;
        MumbleAudioPlayer newPlayer = newObj.GetComponent<MumbleAudioPlayer>();
        Users.Add(name, newObj.GetComponent<PositionUpdater>());
        return newPlayer;
    }
    private void OnMessage(MumbleProto.TextMessage message)
    {
        Debug.Log("Received " + _mumbleClient.GetUserName(message.actor) + " - " + message.message);
        if (IsPositionMessage(message)) {
            Users[_mumbleClient.GetUserName(message.actor)].newPosition = GetPosition(message);
        }
    }
    private bool IsPositionMessage(MumbleProto.TextMessage message)
    {
        return message.message.StartsWith("!POS:");
    }
    private char[] querySplit = { ':', ',' };

    private Vector3 GetPosition(MumbleProto.TextMessage message)
    {
        string[] parts = message.message.Split(querySplit);
        
        float x = float.Parse(parts[1]);
        float y = float.Parse(parts[2]);
        float z = float.Parse(parts[3]);
        return new Vector3(x, y, z);
    }
    private void DestroyMumbleAudioPlayer(MumbleAudioPlayer playerToDestroy)
    {
        Users.Remove(playerToDestroy.name);
        UnityEngine.GameObject.Destroy(playerToDestroy.gameObject);
    }
    void OnApplicationQuit()
    {
        Debug.LogWarning("Shutting down connections");
        if(_mumbleClient != null)
            _mumbleClient.Close();
    }
    IEnumerator UpdateEditorGraph()
    {
        long numPacketsReceived = 0;
        long numPacketsSent = 0;
        long numPacketsLost = 0;

        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            long numSentThisSample = _mumbleClient.NumUDPPacketsSent - numPacketsSent;
            long numRecvThisSample = _mumbleClient.NumUDPPacketsReceieved - numPacketsReceived;
            long numLostThisSample = _mumbleClient.NumUDPPacketsLost - numPacketsLost;

            Graph.channel[0].Feed(-numSentThisSample);//gray
            Graph.channel[1].Feed(-numRecvThisSample);//blue
            Graph.channel[2].Feed(-numLostThisSample);//red

            numPacketsSent += numSentThisSample;
            numPacketsReceived += numRecvThisSample;
            numPacketsLost += numLostThisSample;
        }
    }
	void Update () {
        if (Time.time - lastTime > 0.1)
        {
            lastTime = Time.time;
            if (player != null && player.transform != null && player.transform.position != null && _mumbleClient != null)
            {
                _mumbleClient.SendUnreliableTextMessage("!POS:" + this.player.transform.position.x + "," + this.player.transform.position.y + "," + this.player.transform.position.z, "Root");
            }
        }
	}
}
