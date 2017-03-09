using UnityEngine;
using System.Collections;

public class MusicPlayer : MonoBehaviour
{
    public AudioClip[] songs;
    public float musicVolume = 0.7f;

    private int index;

    void Awake()
    {
        if (GetComponent<AudioSource>() == null)
        {
            gameObject.AddComponent<AudioSource>();
            GetComponent<AudioSource>().volume = musicVolume;
        }
        index = 0;
    }

    void Update()
    {
        if (!GetComponent<AudioSource>().isPlaying)
        {
            float r = Random.value;
            index = (int)(r * songs.Length);
            GetComponent<AudioSource>().clip = songs[index];
            GetComponent<AudioSource>().Play();
        }
    }

    public void PauseMusic()
    {
        GetComponent<AudioSource>().Pause();
        enabled = false;
    }
}