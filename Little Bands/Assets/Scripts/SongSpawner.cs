using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class SongSpawner : MonoBehaviour
{
    public GameObject SongListItem;
    public GameObject ListContainer;

    private string songsPath;

    // !IMPORTANT: All files must be structured the same way inorder for this to work
    void Start()
    {
        //Directory for all song child folders
        songsPath = Application.dataPath + "/Songs/";               //This may no longer be the correct path upon building game

        //Select all song child folders
        string[] songFolders = Directory.GetDirectories(songsPath);

        //Create A SongListItem GameObject for every song child folder
        foreach(string path in songFolders) {
            GameObject song = Instantiate(SongListItem, new Vector3(0, 0, 0), Quaternion.identity, ListContainer.transform);

            
            SongItem songInfo = song.GetComponent<SongItem>();      //Script that holds all unique song information

            songInfo.setTitle(Path.GetFileName(path));              // Folder Name is the songs Title
            
            string[] songFiles = Directory.GetFiles(path);          //Grab filepath for all files in the song folder
            
            songInfo.setAlbumArt(LoadPNG(songFiles[0]));            //Set album art

            //Prepare to Read Audio Files
            AudioClipArrayCombiner audioClipArrayCombiner = this.gameObject.GetComponent<AudioClipArrayCombiner>();

            //Full Audio
            songInfo.setFullAudio(audioClipArrayCombiner.ToAudioClip(songFiles[2]));

            //Audio Guide Files
            songInfo.instruction_bass = audioClipArrayCombiner.ToAudioClip(songFiles[4]);
            songInfo.instruction_drums = audioClipArrayCombiner.ToAudioClip(songFiles[6]);
            songInfo.instruction_guitar = audioClipArrayCombiner.ToAudioClip(songFiles[8]);
            songInfo.instruction_piano = audioClipArrayCombiner.ToAudioClip(songFiles[10]);
            songInfo.instruction_voice = audioClipArrayCombiner.ToAudioClip(songFiles[12]);

            //Original Song Layers
            songInfo.original_bass = audioClipArrayCombiner.ToAudioClip(songFiles[14]);
            songInfo.original_drums = audioClipArrayCombiner.ToAudioClip(songFiles[16]);
            songInfo.original_guitar = audioClipArrayCombiner.ToAudioClip(songFiles[18]);
            songInfo.original_piano = audioClipArrayCombiner.ToAudioClip(songFiles[20]);
            songInfo.original_voice = audioClipArrayCombiner.ToAudioClip(songFiles[22]);

            //Sheet Music Images
            songInfo.bassPages = new Texture[] { LoadPNG(songFiles[24]) };
            songInfo.drumsPages = new Texture[] { LoadPNG(songFiles[26]) };
            songInfo.guitarPages = new Texture[] { LoadPNG(songFiles[28]) };
            songInfo.pianoPages = new Texture[] { LoadPNG(songFiles[30]) };
            songInfo.voicePages = new Texture[] { LoadPNG(songFiles[32]) };

            //Video  Variables
            //songInfo.video_bass = new Texture[] { LoadPNG(songFiles[34]) };
            //songInfo.video_drums = new Texture[] { LoadPNG(songFiles[36]) };
            //songInfo.video_guitar = new Texture[] { LoadPNG(songFiles[38]) };
            //songInfo.video_piano = new Texture[] { LoadPNG(songFiles[40]) };
            //songInfo.video_voice = new Texture[] { LoadPNG(songFiles[42]) };

            //Add On Click To Select Song
            song.GetComponent<Button>().onClick.AddListener( delegate { this.gameObject.GetComponent<GameManager>().selectSong(song); });
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
