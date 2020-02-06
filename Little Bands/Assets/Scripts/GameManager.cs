using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;

    public SongItem SelectedSong;
    private GameObject canvas;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        SelectedSong = null;
    }

    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
