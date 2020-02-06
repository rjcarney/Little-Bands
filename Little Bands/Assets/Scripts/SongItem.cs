using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongItem : MonoBehaviour
{
    [Header("Song Information")]
    // public Texture albumArt;
    public string title;
    public string artist;
    public string length;

    [Header ("Original Audio")]
    public AudioClip original_guitar;
    public AudioClip original_bass;
    public AudioClip original_piano;
    public AudioClip original_drums;
    public AudioClip original_voice;

    // [Header ("Instruction Audio")]
    // public AudioClip instruction_guitar;
    // public AudioClip instruction_bass;
    // public AudioClip instruction_piano;
    // public AudioClip instruction_drums;
    // public AudioClip instruction_voice;

    [Header("Recorded Audio")]
    public AudioClip recorded_guitar;
    public AudioClip recorded_bass;
    public AudioClip recorded_piano;
    public AudioClip recorded_drums;
    public AudioClip recorded_voice;

    [Header("List Item UI Objects")]
    public GameObject albumArt;
    public GameObject title_text;
    public GameObject artist_text;
    public GameObject length_text;
    public GameObject badge_guitar;
    public GameObject badge_bass;
    public GameObject badge_piano;
    public GameObject badge_drums;
    public GameObject badge_voice;


    // Start is called before the first frame update
    void Start()
    {
        title_text.GetComponent<UnityEngine.UI.Text>().text = title;
        artist_text.GetComponent<UnityEngine.UI.Text>().text = artist;
        length_text.GetComponent<UnityEngine.UI.Text>().text = length;

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
