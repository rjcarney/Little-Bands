using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public GameObject SongListPage;
    public GameObject InstrumentSelect;
    public GameObject RecordPage;

    public AudioRead audioReader;

    public GameObject fullAudio_playBtn;
    public Texture playTexture;
    public Texture stopTexture;
    public GameObject fullAudio_playBtnTxt;
    public GameObject audioSlider;

    public GameObject layeredAudio_playBtn;
    public GameObject layeredAudio_playBtnTxt;

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

        audioSlider.GetComponent<UnityEngine.UI.Slider>().direction = Slider.Direction.LeftToRight;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().minValue = 0;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = 1;
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

    public void playLayeredAudio() {
        if (playing_fullAudio == false) {
            audioSource.clip = SelectedSong.original_full_audio;
            audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;
            audioSource.Play();
            layeredAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
            layeredAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\nStop";
            playing_fullAudio = true;
        } else {
            audioSource.Stop();
            audioSource.clip = null;
            layeredAudio_playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            layeredAudio_playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\n\n\n\nPlay";
            playing_fullAudio = false;
        }
    }

    public void Record() {
        playLayeredAudio();
        /*
        switch (SelectedInstrument) {
            case "guitar":
                SelectedSong.recorded_guitar = ;
                break;
            case "bass":
                SelectedSong.recorded_bass = ;
                break;
            case "piano":
                SelectedSong.recorded_piano = ;
                break;
            case "drums":
                SelectedSong.recorded_drums = ;
                break;
            case "voice":
                SelectedSong.recorded_voice = ;
                break;
        }
        */
    }
    
    // Update is called once per frame
    void Update()
    {
        audioSlider.GetComponent<UnityEngine.UI.Slider>().value = GetComponent<AudioSource>().time;
    }
}
