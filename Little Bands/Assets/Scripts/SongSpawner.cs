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

    // Start is called before the first frame update
    void Start()
    {
        songsPath = Application.dataPath + "/Songs/";

        Debug.Log(songsPath);

        string[] songFolders = Directory.GetDirectories(songsPath);

        Debug.Log("There are " + songFolders.Length + " song folders");

        foreach(string path in songFolders) {
            GameObject song = Instantiate(SongListItem, new Vector3(0, 0, 0), Quaternion.identity, ListContainer.transform);
            SongItem songInfo = song.GetComponent<SongItem>();

            songInfo.setTitle(Path.GetFileName(path));
            string[] songFiles = Directory.GetFiles(path);

            songInfo.setAlbumArt(LoadPNG(songFiles[0]));

            AudioClipArrayCombiner audioClipArrayCombiner = this.gameObject.GetComponent<AudioClipArrayCombiner>();
            songInfo.setFullAudio(audioClipArrayCombiner.ToAudioClip(songFiles[2]));

            songInfo.instruction_bass = audioClipArrayCombiner.ToAudioClip(songFiles[4]);
            songInfo.instruction_drums = audioClipArrayCombiner.ToAudioClip(songFiles[6]);
            songInfo.instruction_guitar = audioClipArrayCombiner.ToAudioClip(songFiles[8]);
            songInfo.instruction_piano = audioClipArrayCombiner.ToAudioClip(songFiles[10]);
            songInfo.instruction_voice = audioClipArrayCombiner.ToAudioClip(songFiles[12]);

            songInfo.original_bass = audioClipArrayCombiner.ToAudioClip(songFiles[14]);
            songInfo.original_drums = audioClipArrayCombiner.ToAudioClip(songFiles[16]);
            songInfo.original_guitar = audioClipArrayCombiner.ToAudioClip(songFiles[18]);
            songInfo.original_piano = audioClipArrayCombiner.ToAudioClip(songFiles[20]);
            songInfo.original_voice = audioClipArrayCombiner.ToAudioClip(songFiles[22]);


            song.GetComponent<Button>().onClick.AddListener( delegate { this.gameObject.GetComponent<GameManager>().selectSong(song); });
        }


    }

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
