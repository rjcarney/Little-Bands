using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SongItem : MonoBehaviour
{
    [Header("Song Information")]
    public Texture albumArt;
    public string title;
    public string artist;
    public string length;

    [Header("Original Audio")]
    public AudioClip original_full_audio;
    public AudioClip original_guitar;
    public AudioClip original_bass;
    public AudioClip original_piano;
    public AudioClip original_drums;
    public AudioClip original_voice;

    [Header("Sheet Music Page Arrays")]
    public Texture[] guitarPages;
    public Texture[] bassPages;
    public Texture[] pianoPages;
    public Texture[] drumsPages;
    public Texture[] voicePages;

    [Header ("Instruction Audio")]
    public AudioClip instruction_guitar;
    public AudioClip instruction_bass;
    public AudioClip instruction_piano;
    public AudioClip instruction_drums;
    public AudioClip instruction_voice;
    
    [Header("Recorded Audio")]
    public AudioClip recorded_guitarClip;
    public AudioClip recorded_bassClip;
    public AudioClip recorded_pianoClip;
    public AudioClip recorded_drumsClip;
    public AudioClip recorded_voiceClip;

    [Header("Videos")]
    public AudioClip video_guitar;
    public AudioClip video_bass;
    public AudioClip video_piano;
    public AudioClip video_drums;
    public AudioClip video_voice;

    [Header("List Item UI Objects")]
    public GameObject albumArt_image;
    public GameObject title_text;
    public GameObject artist_text;
    public GameObject length_text;
    public GameObject badge_guitar;
    public GameObject badge_bass;
    public GameObject badge_piano;
    public GameObject badge_drums;
    public GameObject badge_voice;

    public int guitarToggleCount;
    public int bassToggleCount;
    public int pianoToggleCount;
    public int drumsToggleCount;
    public int voiceToggleCount;
    


    // Start is called before the first frame update
    void Start()
    {
        // Set all song list item UI elements
        title_text.GetComponent<UnityEngine.UI.Text>().text = title;
        artist_text.GetComponent<UnityEngine.UI.Text>().text = artist;
        length_text.GetComponent<UnityEngine.UI.Text>().text = length;
        albumArt_image.GetComponent<UnityEngine.UI.RawImage>().texture = albumArt;

        // preset all audio to teacher tracks
        guitarToggleCount = 0;
        bassToggleCount = 0;
        pianoToggleCount = 0;
        drumsToggleCount = 0;
        voiceToggleCount = 0;
    }

    public void setTitle(string title) {
        this.title = title;
        title_text.GetComponent<UnityEngine.UI.Text>().text = title;
    }

    public void setAlbumArt(Texture2D tex) {
        albumArt = tex;
        albumArt_image.GetComponent<UnityEngine.UI.RawImage>().texture = albumArt;
    }

    public void setFullAudio(AudioClip fullAudio) {
        original_full_audio = fullAudio;
        length = Mathf.Floor(original_full_audio.length / 60) + ":" + (original_full_audio.length % 60);
        length_text.GetComponent<UnityEngine.UI.Text>().text = length;
    }
}
