using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject SongListPage;
    public GameObject InstrumentSelect;
    public GameObject RecordPage;

    public GameObject fullAudio_playBtn;
    public Texture playTexture;
    public Texture stopTexture;
    public GameObject fullAudio_playBtnTxt;

    private AudioSource audioSource;
    private SongItem SelectedSong;
    private string SelectedInstrument;

    private bool playing_fullAudio;

    // Start is called before the first frame update
    void Awake()
    {
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        SongListPage.SetActive(true);
        InstrumentSelect.SetActive(false);
        RecordPage.SetActive(false);
        SelectedSong = null;
        playing_fullAudio = false;
    }

    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();
        SongListPage.SetActive(false);
        InstrumentSelect.SetActive(true);
    }

   public void backToSongs() {
        SelectedSong = null;
        InstrumentSelect.SetActive(false);
        SongListPage.SetActive(true);
    }

    public void selectInstrument(string instrument) {
        SelectedInstrument = instrument;
        InstrumentSelect.SetActive(false);
        RecordPage.SetActive(true);
    }

    public void backToInstrument() {
        SelectedInstrument = null;
        RecordPage.SetActive(false);
        InstrumentSelect.SetActive(true);
    }

    public void playFullAudio() {
        if (playing_fullAudio == false) {
            audioSource.clip = SelectedSong.original_full_audio;
            audioSource.Play();
            fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
            fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nStop";
            playing_fullAudio = true;
        } else {
            audioSource.Stop();
            audioSource.clip = null;
            fullAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            fullAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\n\n\nPlay";
            playing_fullAudio = false;
        }
    }
    
    // Update is called once per frame
    void Update()
    {
        
    }
}
