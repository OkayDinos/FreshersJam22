using System.Collections;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public static MusicManager instance;
    public AudioClip[] music;
    public int track;

    private void Awake()
    {

        if (MusicManager.instance)
            Destroy(this);
        else
            MusicManager.instance = this;

        DontDestroyOnLoad(this.gameObject);
        RandomSong();
    }

    void RandomSong()
    {
        track = (int)Random.Range(0, music.Length);
        GetComponent<AudioSource>().clip = music[track];
        GetComponent<AudioSource>().Play();
        StartCoroutine(WaitForEnd(GetComponent<AudioSource>().clip.length));
    }


    IEnumerator WaitForEnd(float time)
    {
        yield return new WaitForSeconds(time);
        RandomSong();
    }
}