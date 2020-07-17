using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

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



        }


    }
}
