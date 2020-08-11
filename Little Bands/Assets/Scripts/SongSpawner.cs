using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;

public class SongSpawner : MonoBehaviour
{
    public GameObject SongListItem;
    public GameObject ListContainer;

    public Object[] files;

    private string songsPath;

    // !IMPORTANT: All files must be structured the same way inorder for this to work
    void Start()
    {
        files = Resources.LoadAll("Songs");

        for(int i = 0; i < files.Length/24; i++) {
            int sectionStart = i * 24;
            GameObject song = Instantiate(SongListItem, new Vector3(0, 0, 0), Quaternion.identity, ListContainer.transform);
            SongItem songInfo = song.GetComponent<SongItem>();      //Script that holds all unique song information

            songInfo.setAlbumArt((Texture2D) files[0 + sectionStart]);                //Set album art

            //BPM File
            //System.IO.File textFile = files[2 + sectionStart];
            //string[] lines = ReadAllLines(textFile);
            //songInfo.title = lines[0];
            //songInfo.bpm = int.Parse(lines[1]);
            //songInfo.video_url_bass = lines[2];
            //songInfo.video_url_drums = lines[3];
            //songInfo.video_url_guitar = lines[4];
            //songInfo.video_url_piano = lines[5];
            //songInfo.video_url_voice = lines[6];

            //Prepare to Read Audio Files
            AudioClipArrayCombiner audioClipArrayCombiner = this.gameObject.GetComponent<AudioClipArrayCombiner>();
            songInfo.setFullAudio((AudioClip) files[3 + sectionStart]);

            //Audio Guide Files
            songInfo.instruction_bass = (AudioClip) files[4 + sectionStart];
            songInfo.instruction_drums = (AudioClip) files[5 + sectionStart];
            songInfo.instruction_guitar = (AudioClip) files[6 + sectionStart];
            songInfo.instruction_piano = (AudioClip) files[7 + sectionStart];
            songInfo.instruction_voice = (AudioClip) files[8 + sectionStart];

            //Original Song Layers
            songInfo.original_bass = (AudioClip) files[9 + sectionStart];
            songInfo.original_drums = (AudioClip) files[10 + sectionStart];
            songInfo.original_guitar = (AudioClip) files[11 + sectionStart];
            songInfo.original_piano = (AudioClip) files[12 + sectionStart];
            songInfo.original_voice = (AudioClip) files[13 + sectionStart];

            //Sheet Music Images
            songInfo.bassPages = new Texture[] { (Texture2D) files[14 + sectionStart] };
            songInfo.drumsPages = new Texture[] { (Texture2D) files[16 + sectionStart] };
            songInfo.guitarPages = new Texture[] { (Texture2D) files[18 + sectionStart] };
            songInfo.pianoPages = new Texture[] { (Texture2D) files[20 + sectionStart] };
            songInfo.voicePages = new Texture[] { (Texture2D) files[22 + sectionStart] };

            //Add On Click To Select Song
            song.GetComponent<Button>().onClick.AddListener(delegate { this.gameObject.GetComponent<GameManager>().selectSong(song); });
        }
        
    }


    //From StackOverflow
    public static Texture2D LoadPNG(string filePath) {

        Texture2D tex = null;
        byte[] fileData;

        if (File.Exists(filePath)) {
            fileData = File.ReadAllBytes(filePath);
            tex = new Texture2D(2, 2);
            tex.LoadImage(fileData); //..this will auto-resize the texture dimensions.
        }
        return tex;
    }
}
