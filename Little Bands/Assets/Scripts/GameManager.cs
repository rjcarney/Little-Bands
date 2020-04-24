using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class GameManager : MonoBehaviour
{
    public static GameManager instance = null;
    public AudioRead audioReader;
    public AudioWrite audioWriter;

    // Saves individual files to persistentDataPath. Also combines files and saves to persistentDataPath
    public AudioClipArrayCombiner audioClipArrayCombiner;

    // Avatar Select Page Variables
    public GameObject AvatarSelecetPage;
    private Avatar userAvatar;
    public GameObject LoadingPage;
    
    // Song Select Page Variables
    public GameObject SongListPage;
    private SongItem SelectedSong;
    // Record Page Variables
    public GameObject RecordPage;

    private string SelectedInstrument;
    public GameObject avatarDisplay;
    public GameObject avatarDisplayText;
    

    public Texture playTexture;
    public Texture stopTexture;
    public Texture micTexture;

    // PlayOptions Content
    public GameObject PlayOptions;
    private bool playing_recording;
    public GameObject playBtn;
    public GameObject audioSlider_playOptions;
    private bool playing_layeredAudio;
    public GameObject videoButton;
    public Texture videoButtonActive;
    public Texture videoButtonInactive;
    public GameObject sheetMusicButton;
    public Texture sheetMusicButtonActive;
    public Texture sheetMusicButtonInactive;
    public GameObject removeInstrumentButton;
    public GameObject combineAudioFilesButton;
    public GameObject deleteButton;

    public GameObject guitarButton;
    public GameObject bassButton;
    public GameObject pianoButton;
    public GameObject drumsButton;
    public GameObject voiceButton;
    public GameObject guideButton;

    public Texture guitarTexture;
    public Texture bassTexture;
    public Texture pianoTexture;
    public Texture drumsTexture;
    public Texture voiceTexture;
    public Texture guideTexture;

    public Texture guitarTexture_mute;
    public Texture bassTexture_mute;
    public Texture pianoTexture_mute;
    public Texture drumsTexture_mute;
    public Texture voiceTexture_mute;
    public Texture guideTexture_mute;

    public GameObject guitarText;
    public GameObject bassText;
    public GameObject pianoText;
    public GameObject drumsText;
    public GameObject voiceText;
    public GameObject guideText;

    // RecordView Content
    private bool recording;
    public GameObject recordView;
    public GameObject audioSlider_recordView;
    public GameObject confirmPopUp;
    public GameObject savingPopUp;
    public GameObject promptScrollBar;
    public GameObject promptPageContainer;

    // SheetMusic Content
    public GameObject sheetMusicPopUp;
    public GameObject sheetMusicTitle;
    public GameObject sheetMusicScrollBar;
    public GameObject sheetMusicPageContainer;
    private List<GameObject> currentPages;
    public GameObject sheetMusicPage;

    // Video Content
    public GameObject videoPopUp;
    public GameObject videoTitle;
	
	// Metronome
	public GameObject metronomeButton;
	public Texture metronomeTexture;
	public Texture metronomeTexture_mute;
	public bool metronomeActive;

    // Audio Sources
    public AudioSource fullAudioSource;
    public AudioSource guitarAudioSource;
    public AudioSource bassAudioSource;
    public AudioSource pianoAudioSource;
    public AudioSource drumsAudioSource;
    public AudioSource voiceAudioSource;
    public AudioSource audioGuideSource;


    // Awake is called once after all game objects are initialized
    void Awake()
    {
        // Make Sure Only One Game Manager Exists
        if(instance == null) {
            instance = this;
        } else if (instance != this) {
            Destroy(gameObject);
        }
        DontDestroyOnLoad(gameObject);

        //Set Up Scene On Initial Play
        AvatarSelecetPage.SetActive(true);
        SongListPage.SetActive(false);
        RecordPage.SetActive(false);

		metronomeActive = true;
        SelectedSong = null;
        playing_recording = false;
        playing_layeredAudio = false;
        recording = false;
        currentPages = new List<GameObject>();

        audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().direction = Slider.Direction.LeftToRight;
        audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().minValue = 0;
        audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().maxValue = 1;
        audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().direction = Slider.Direction.LeftToRight;
        audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().minValue = 0;
        audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().maxValue = 1;
    }

   /*  AVATAR SELECTION PAGE
    *  The game loads and the user is prompted to either choose an avatar or load last played avatar
    */
    public void SelectAvatar(GameObject avatarButton) {
        userAvatar = avatarButton.GetComponent<Avatar>();
        AvatarSelecetPage.SetActive(false);
        SongListPage.SetActive(true);
    }

    public void Load() {
        // ! UNCOMMENT ONCE LOAD FUNTIONALITY IS ADDED !
        //AvatarSelecetPage.SetActive(false);
        //LoadingPage.SetActive(true);

        //Here is where we will call harris's load method

        // ! UNCOMMENT ONCE LOAD FUNTIONALITY IS ADDED !
        //LoadingPage.SetActive(false);
        //SongListPage.SetActive(true);
    }

    /* SONG SELECTION PAGE
     * Once  the desired avatar has been selected the user will select a song
     * 
     * selecting a song will:
     * Set SelectedSong
     *     Audio Source for instrument files
     *     These will be based on instrument toggels variables associated with the selected song
     */
    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();
        videoTitle.GetComponent<UnityEngine.UI.Text>().text = SelectedSong.title;

        LoadSavedClips();

        //Set Audio Tracks
        fullAudioSource.clip = SelectedSong.original_full_audio;
        // Set guitar track
        switch (SelectedSong.guitarToggleCount) {
            case 0: // Teacher
                guitarAudioSource.clip = SelectedSong.original_guitar;
                guitarAudioSource.volume = 1;
                break;
            case 1: // Student
                guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                guitarAudioSource.volume = 1;
                break;
            case 2: // Muted
                guitarAudioSource.volume = 0;
                break;
        }
        // Set bass track
        switch (SelectedSong.bassToggleCount) {
            case 0:
                bassAudioSource.clip = SelectedSong.original_bass;
                bassAudioSource.volume = 1;
                break;
            case 1:
                bassAudioSource.clip = SelectedSong.recorded_bassClip;
                bassAudioSource.volume = 1;
                break;
            case 2:
                bassAudioSource.volume = 0;
                break;
        }
        // Set piano track
        switch (SelectedSong.pianoToggleCount) {
            case 0:
                pianoAudioSource.clip = SelectedSong.original_piano;
                pianoAudioSource.volume = 1;
                break;
            case 1:
                pianoAudioSource.clip = SelectedSong.recorded_pianoClip;
                pianoAudioSource.volume = 1;
                break;
            case 2:
                pianoAudioSource.volume = 0;
                break;
        }
        // Set drums track
        switch (SelectedSong.drumsToggleCount) {
            case 0:
                drumsAudioSource.clip = SelectedSong.original_drums;
                drumsAudioSource.volume = 1;
                break;
            case 1:
                drumsAudioSource.clip = SelectedSong.recorded_drumsClip;
                drumsAudioSource.volume = 1;
                break;
            case 2:
                drumsAudioSource.volume = 0;
                break;
        }
        // Set voice track
        switch (SelectedSong.voiceToggleCount) {
            case 0:
                voiceAudioSource.clip = SelectedSong.original_voice;
                voiceAudioSource.volume = 1;
                break;
            case 1:
                voiceAudioSource.clip = SelectedSong.recorded_voiceClip;
                voiceAudioSource.volume = 1;
                break;
            case 2:
                voiceAudioSource.volume = 0;
                break;
        }

        // Audio slider value
        audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().maxValue = fullAudioSource.clip.length;
        audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().maxValue = fullAudioSource.clip.length;

        // Set AudioRead record size
        audioReader.seconds = Mathf.CeilToInt(fullAudioSource.clip.length);

        // Change View
        SongListPage.SetActive(false);
        RecordPage.SetActive(true);
    }

    public void LoadSavedClips() {
        string path = Application.persistentDataPath + "/" + userAvatar.avatarName + "/" + SelectedSong.title + "/";

        if(System.IO.File.Exists(path + "guitar/" + userAvatar.avatarName + "_" + SelectedSong.title + "_guitar.wav")) {
            SelectedSong.recorded_guitarClip = audioClipArrayCombiner.ToAudioClip(path + "guitar/" + userAvatar.avatarName + "_" + SelectedSong.title + "_guitar.wav");
        } else {
            Debug.Log("No guitar recording saved");
        }

        if (System.IO.File.Exists(path + "bass/" + userAvatar.avatarName + "_" + SelectedSong.title + "_bass.wav")) {
            SelectedSong.recorded_bassClip = audioClipArrayCombiner.ToAudioClip(path + "bass/" + userAvatar.avatarName + "_" + SelectedSong.title + "_bass.wav");
        } else {
            Debug.Log("No bass recording saved");
        }

        if (System.IO.File.Exists(path + "piano/" + userAvatar.avatarName + "_" + SelectedSong.title + "_piano.wav")) {
            SelectedSong.recorded_pianoClip = audioClipArrayCombiner.ToAudioClip(path + "piano/" + userAvatar.avatarName + "_" + SelectedSong.title + "_piano.wav");
        } else {
            Debug.Log("No piano recording saved");
        }

        if (System.IO.File.Exists(path + "drums/" + userAvatar.avatarName + "_" + SelectedSong.title + "_drums.wav")) {
            SelectedSong.recorded_drumsClip = audioClipArrayCombiner.ToAudioClip(path + "drums/" + userAvatar.avatarName + "_" + SelectedSong.title + "_drums.wav");
        } else {
            Debug.Log("No drums recording saved");
        }

        if (System.IO.File.Exists(path + "voice/" + userAvatar.avatarName + "_" + SelectedSong.title + "_voice.wav")) {
            SelectedSong.recorded_voiceClip = audioClipArrayCombiner.ToAudioClip(path + "voice/" + userAvatar.avatarName + "_" + SelectedSong.title + "_voice.wav");
        } else {
            Debug.Log("No voice recording saved");
        }
    }

    public void exitToAvatars() {
        userAvatar = null;
        SongListPage.SetActive(false);
        AvatarSelecetPage.SetActive(true);
    }


    /* RECORDING PAGE
     * After selecting a song the user will be need to select in instrument before they are able to record
     * Below are all the methods that will be called from the recording page
     */

     /* Back Button on Click Function
      * Resets all variables associated with SelectedSong, Audio Sources and change 
      */
    public void backToSongs() {
        //Deselect song and associated recorded audio
        SelectedSong.recorded_guitarClip = null;
        SelectedSong.recorded_bassClip = null;
        SelectedSong.recorded_pianoClip = null;
        SelectedSong.recorded_drumsClip = null;
        SelectedSong.recorded_voiceClip = null;
        SelectedSong = null;

        // Clear all audio sources
        fullAudioSource.clip = null;
        guitarAudioSource.clip = null;
        bassAudioSource.clip = null;
        pianoAudioSource.clip = null;
        drumsAudioSource.clip = null;
        voiceAudioSource.clip = null;
        audioGuideSource.clip = null;
        SelectedInstrument = null;

        // Change view
        RecordPage.SetActive(false);
        SongListPage.SetActive(true);
        videoPopUp.SetActive(false);
        sheetMusicPopUp.SetActive(false);
        PlayOptions.SetActive(true);
        if (recording) {
            CancelRecording();
        }
    }

    /* Instrument Button On Click Function
     * If SelectedInstrument is null set the instrument as the selected instrument
     * If SelectedInstrument is already set toggle the audio played for instrument associated with the button 
     */
    public void InstrumentButtonOnClick(string instrument) {
        if (!playing_recording && !recording) {
            // First Click Select instrument
            if (SelectedInstrument == null) {
                SelectedInstrument = instrument;
                sheetMusicTitle.GetComponent<UnityEngine.UI.Text>().text = SelectedSong.title + ": " + SelectedInstrument;
                // Set the audio guide track for associated instrument
                switch (instrument) {
                    case "guitar":
                        audioGuideSource.clip = SelectedSong.instruction_guitar;
                        break;
                    case "bass":
                        audioGuideSource.clip = SelectedSong.instruction_bass;
                        break;
                    case "piano":
                        audioGuideSource.clip = SelectedSong.instruction_piano;
                        break;
                    case "drums":
                        audioGuideSource.clip = SelectedSong.instruction_drums;
                        break;
                    case "voice":
                        audioGuideSource.clip = SelectedSong.instruction_voice;
                        break;
                }

            }
            // Every Subsequent Click Toggle Audio
            else {
                if (instrument == "guitar") {
                    switch (SelectedSong.guitarToggleCount) {
                        case 0:  // Teacher
                            if (SelectedSong.recorded_guitarClip != null) {
                                // Change to student
                                guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                                SelectedSong.guitarToggleCount = 1;
                                guitarAudioSource.volume = 1;
                            } else {
                                // No student track, mute
                                guitarAudioSource.clip = SelectedSong.original_guitar;
                                SelectedSong.guitarToggleCount = 2;
                                guitarAudioSource.volume = 0;
                            }
                            break;
                        case 1:  // Student
                            // Mute
                            guitarAudioSource.clip = SelectedSong.original_guitar;
                            SelectedSong.guitarToggleCount = 2;
                            guitarAudioSource.volume = 0;
                            break;
                        case 2:  // Muted
                            // Change to teacher
                            guitarAudioSource.volume = 1;
                            SelectedSong.guitarToggleCount = 0;
                            break;
                    }
                }
                if (instrument == "bass") {
                    switch (SelectedSong.bassToggleCount) {
                        case 0:
                            if (SelectedSong.recorded_bassClip != null) {
                                bassAudioSource.clip = SelectedSong.recorded_bassClip;
                                SelectedSong.bassToggleCount = 1;
                                bassAudioSource.volume = 1;
                            } else {
                                bassAudioSource.clip = SelectedSong.original_bass;
                                SelectedSong.bassToggleCount = 2;
                                bassAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            bassAudioSource.clip = SelectedSong.original_bass;
                            SelectedSong.bassToggleCount = 2;
                            bassAudioSource.volume = 0;
                            break;
                        case 2:
                            bassAudioSource.volume = 1;
                            SelectedSong.bassToggleCount = 0;
                            break;
                    }
                }
                if (instrument == "piano") {
                    switch (SelectedSong.pianoToggleCount) {
                        case 0:
                            if (SelectedSong.recorded_pianoClip != null) {
                                pianoAudioSource.clip = SelectedSong.recorded_pianoClip;
                                SelectedSong.pianoToggleCount = 1;
                                pianoAudioSource.volume = 1;
                            } else {
                                pianoAudioSource.clip = SelectedSong.original_piano;
                                SelectedSong.pianoToggleCount = 2;
                                pianoAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            pianoAudioSource.clip = SelectedSong.original_piano;
                            SelectedSong.pianoToggleCount = 2;
                            pianoAudioSource.volume = 0;
                            break;
                        case 2:
                            pianoAudioSource.volume = 1;
                            SelectedSong.pianoToggleCount = 0;
                            break;
                    }
                }
                if (instrument == "drums") {
                    switch (SelectedSong.drumsToggleCount) {
                        case 0:
                            if (SelectedSong.recorded_drumsClip != null) {
                                drumsAudioSource.clip = SelectedSong.recorded_drumsClip;
                                SelectedSong.drumsToggleCount = 1;
                                drumsAudioSource.volume = 1;
                            } else {
                                drumsAudioSource.clip = SelectedSong.original_drums;
                                SelectedSong.drumsToggleCount = 2;
                                drumsAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            drumsAudioSource.clip = SelectedSong.original_drums;
                            SelectedSong.drumsToggleCount = 2;
                            drumsAudioSource.volume = 0;
                            break;
                        case 2:
                            drumsAudioSource.volume = 1;
                            SelectedSong.drumsToggleCount = 0;
                            break;
                    }
                }
                if (instrument == "voice") {
                    switch (SelectedSong.voiceToggleCount) {
                        case 0:
                            if (SelectedSong.recorded_voiceClip != null) {
                                voiceAudioSource.clip = SelectedSong.recorded_voiceClip;
                                SelectedSong.voiceToggleCount = 1;
                                voiceAudioSource.volume = 1;
                            } else {
                                voiceAudioSource.clip = SelectedSong.original_voice;
                                SelectedSong.voiceToggleCount = 2;
                                voiceAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            voiceAudioSource.clip = SelectedSong.original_voice;
                            SelectedSong.voiceToggleCount = 2;
                            voiceAudioSource.volume = 0;
                            break;
                        case 2:
                            voiceAudioSource.volume = 1;
                            SelectedSong.voiceToggleCount = 0;
                            break;
                    }
                }
            }
        }
    }


    // Deselect the current SelectedInstrument
    public void RemoveInstrument() {
        SelectedInstrument = null;
        audioGuideSource.clip = null;
    }

    // Set audio tracks to users current settings
    public void setAudioTracks() {
        switch (SelectedSong.guitarToggleCount) {
            case 0:  // Teacher
                guitarAudioSource.volume = 1;
                guitarAudioSource.clip = SelectedSong.original_guitar;
                break;
            case 1:  // Student
                guitarAudioSource.volume = 1;
                guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                break;
            case 2:  // Muted
                guitarAudioSource.volume = 0;
                guitarAudioSource.clip = SelectedSong.original_guitar;
                break;
        }
        switch (SelectedSong.bassToggleCount) {
            case 0:  // Teacher
                bassAudioSource.volume = 1;
                bassAudioSource.clip = SelectedSong.original_bass;
                break;
            case 1:  // Student
                bassAudioSource.volume = 1;
                bassAudioSource.clip = SelectedSong.recorded_bassClip;
                break;
            case 2:  // Muted
                bassAudioSource.volume = 0;
                bassAudioSource.clip = SelectedSong.original_bass;
                break;
        }
        switch (SelectedSong.pianoToggleCount) {
            case 0:  // Teacher
                pianoAudioSource.volume = 1;
                pianoAudioSource.clip = SelectedSong.original_piano;
                break;
            case 1:  // Student
                pianoAudioSource.volume = 1;
                pianoAudioSource.clip = SelectedSong.recorded_pianoClip;
                break;
            case 2:  // Muted
                pianoAudioSource.volume = 0;
                pianoAudioSource.clip = SelectedSong.original_piano;
                break;
        }
        switch (SelectedSong.drumsToggleCount) {
            case 0:  // Teacher
                drumsAudioSource.volume = 1;
                drumsAudioSource.clip = SelectedSong.original_drums;
                break;
            case 1:  // Student
                drumsAudioSource.volume = 1;
                drumsAudioSource.clip = SelectedSong.recorded_drumsClip;
                break;
            case 2:  // Muted
                drumsAudioSource.volume = 0;
                drumsAudioSource.clip = SelectedSong.original_drums;
                break;
        }
        switch (SelectedSong.voiceToggleCount) {
            case 0:  // Teacher
                voiceAudioSource.volume = 1;
                voiceAudioSource.clip = SelectedSong.original_voice;
                break;
            case 1:  // Student
                voiceAudioSource.volume = 1;
                voiceAudioSource.clip = SelectedSong.recorded_voiceClip;
                break;
            case 2:  // Muted
                voiceAudioSource.volume = 0;
                voiceAudioSource.clip = SelectedSong.original_voice;
                break;
        }
    }
    
    //  The Play Button will now only play the users recorded audio
     public void PlayButtonOnClick() {
        if (!playing_recording) {
                // Stop all curently playing audio
                stopLayeredAudio();

            if(SelectedSong.recorded_guitarClip == null) {
                guitarAudioSource.clip = SelectedSong.original_guitar;
                guitarAudioSource.volume = 0;
                guitarText.GetComponent<Text>().text = "None";
            } else {
                guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                guitarAudioSource.volume = 1;
                guitarText.GetComponent<Text>().text = "Student";
            }

            if (SelectedSong.recorded_bassClip == null) {
                bassAudioSource.clip = SelectedSong.original_bass;
                bassAudioSource.volume = 0;
                bassText.GetComponent<Text>().text = "None";
            } else {
                bassAudioSource.clip = SelectedSong.recorded_bassClip;
                bassAudioSource.volume = 1;
                bassText.GetComponent<Text>().text = "Student";
            }

            if (SelectedSong.recorded_pianoClip == null) {
                pianoAudioSource.clip = SelectedSong.original_piano;
                pianoAudioSource.volume = 0;
                pianoText.GetComponent<Text>().text = "None";
            } else {
                pianoAudioSource.clip = SelectedSong.recorded_pianoClip;
                pianoAudioSource.volume = 1;
                pianoText.GetComponent<Text>().text = "Student";
            }

            if (SelectedSong.recorded_drumsClip == null) {
                drumsAudioSource.clip = SelectedSong.original_drums;
                drumsAudioSource.volume = 0;
                drumsText.GetComponent<Text>().text = "None";
            } else {
                drumsAudioSource.clip = SelectedSong.recorded_drumsClip;
                drumsAudioSource.volume = 1;
                drumsText.GetComponent<Text>().text = "Student";
            }

            if (SelectedSong.recorded_voiceClip == null) {
                voiceAudioSource.clip = SelectedSong.original_voice;
                voiceAudioSource.volume = 0;
                voiceText.GetComponent<Text>().text = "None";
            } else {
                voiceAudioSource.clip = SelectedSong.recorded_voiceClip;
                voiceAudioSource.volume = 1;
                voiceText.GetComponent<Text>().text = "Student";
            }
            playing_recording = true;
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
            playLayeredAudio();
        } else {
            // Stop All Audio
            stopLayeredAudio();
            setAudioTracks();
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            playing_recording = false;
        }
    }


    // Start playing all audio sources as they are currently set
    public void playLayeredAudio() {
        audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;
        audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;

        guitarAudioSource.Play();
        bassAudioSource.Play();
        pianoAudioSource.Play();
        drumsAudioSource.Play();
        voiceAudioSource.Play();
        playing_layeredAudio = true;
    }

    // Stop playing all audio sources
    public void stopLayeredAudio() {
        audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = 0;
        audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value = 0;
        guitarAudioSource.Stop();
        bassAudioSource.Stop();
        pianoAudioSource.Stop();
        drumsAudioSource.Stop();
        voiceAudioSource.Stop();
        playing_layeredAudio = false;
    }

    // Change view from Play Options to Sheet Music
    public void OpenSheetMusic() {
        if (SelectedInstrument != null) {
            switch(SelectedInstrument) {
                case "guitar":
                    foreach(Texture pageTexture in SelectedSong.guitarPages) {
                        GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                        page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                        page.transform.parent = sheetMusicPageContainer.transform;
                        currentPages.Add(page);
                    }
                    break;
                case "bass":
                    foreach (Texture pageTexture in SelectedSong.bassPages) {
                        GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                        page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                        page.transform.parent = sheetMusicPageContainer.transform;
                        currentPages.Add(page);
                    }
                    break;
                case "piano":
                    foreach (Texture pageTexture in SelectedSong.pianoPages) {
                        GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                        page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                        page.transform.parent = sheetMusicPageContainer.transform;
                        currentPages.Add(page);
                    }
                    break;
                case "drums":
                    foreach (Texture pageTexture in SelectedSong.drumsPages) {
                        GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                        page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                        page.transform.parent = sheetMusicPageContainer.transform;
                        currentPages.Add(page);
                    }
                    break;
                case "voice":
                    foreach (Texture pageTexture in SelectedSong.voicePages) {
                        GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                        page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                        page.transform.parent = sheetMusicPageContainer.transform;
                        currentPages.Add(page);
                    }
                    break;
            }
            sheetMusicPopUp.SetActive(true);
            PlayOptions.SetActive(false);
            sheetMusicScrollBar.GetComponent<Scrollbar>().value = 1;
        }
    }

    // Change view from Sheet Music to Play Options
    public void CloseSheetMusic() {
        foreach(GameObject page in currentPages) {
            Destroy(page);
        }
        currentPages = new List<GameObject>();
        sheetMusicPopUp.SetActive(false);
        PlayOptions.SetActive(true);
    }
	
	public void SetMetronome() {
		metronomeActive = !metronomeActive;
		
		if(metronomeActive) {
			metronomeButton.GetComponent<UnityEngine.UI.RawImage>().texture = metronomeTexture;
		} else {
			metronomeButton.GetComponent<UnityEngine.UI.RawImage>().texture = metronomeTexture_mute;
		}
	}

    // Change view from Play Options to Video
    public void OpenVideo() {
        if (SelectedInstrument != null) {
            videoPopUp.SetActive(true);
            PlayOptions.SetActive(false);
        }
    }

    // Change view from Video to Play Options
    public void CloseVideo() {
        videoPopUp.SetActive(false);
        PlayOptions.SetActive(true);
    }

    /* Record Function
     * This is called to both start and end the recording process
     */
    public void Record() {
        // Cancel any currently playing audio
        stopLayeredAudio();
        playing_recording = false;
        playing_layeredAudio = false;
        setAudioTracks();

        // Ensure instrument is selected
        if (SelectedInstrument != null) {
            // Set Correct View Based on if recording or not
            if (!recording) {
                // populate prompt page container with sheet music for the selected instrument
                switch (SelectedInstrument) {
                    case "guitar":
                        foreach (Texture pageTexture in SelectedSong.guitarPages) {
                            GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                            page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                            page.transform.parent = promptPageContainer.transform;
                            currentPages.Add(page);
                        }
                        break;
                    case "bass":
                        foreach (Texture pageTexture in SelectedSong.bassPages) {
                            GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                            page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                            page.transform.parent = promptPageContainer.transform;
                            currentPages.Add(page);
                        }
                        break;
                    case "piano":
                        foreach (Texture pageTexture in SelectedSong.pianoPages) {
                            GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                            page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                            page.transform.parent = promptPageContainer.transform;
                            currentPages.Add(page);
                        }
                        break;
                    case "drums":
                        foreach (Texture pageTexture in SelectedSong.drumsPages) {
                            GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                            page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                            page.transform.parent = promptPageContainer.transform;
                            currentPages.Add(page);
                        }
                        break;
                    case "voice":
                        foreach (Texture pageTexture in SelectedSong.voicePages) {
                            GameObject page = Instantiate(sheetMusicPage, new Vector3(0, 0, 0), Quaternion.identity);
                            page.GetComponent<UnityEngine.UI.RawImage>().texture = pageTexture;
                            page.transform.parent = promptPageContainer.transform;
                            currentPages.Add(page);
                        }
                        break;
                }
                // Start recording process
                recording = true;
                PlayOptions.SetActive(false);
                recordView.SetActive(true);
                promptScrollBar.GetComponent<Scrollbar>().value = 1;
                playLayeredAudio();
                audioGuideSource.Play();
            } else {
                // remove sheet music pages from prompt container
                foreach (GameObject page in currentPages) {
                    Destroy(page);
                }
                currentPages = new List<GameObject>();
                // End recording process
                recording = false;
                confirmPopUp.SetActive(true);
                recordView.SetActive(false);
                stopLayeredAudio();
                audioGuideSource.Stop();
            }
            // Toggle AudioReader
            audioReader.startRecord = true;
        }
    }

    /* Yes and No on click function for the confirm page
     * Confirm the user wishes to save current recording session
     * Toggle audio clip to students recording
     */
    public void confirmRecording(bool confirm) {
        if (confirm) {
            confirmPopUp.SetActive(false);
            savingPopUp.SetActive(true);
            switch (SelectedInstrument) {
                case "guitar":
                    SelectedSong.recorded_guitarClip = audioReader.audioSource.clip;
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_guitarClip, userAvatar.avatarName, SelectedSong.title, "guitar");
                    SelectedSong.guitarToggleCount = 1;
                    break;
                case "bass":
                    SelectedSong.recorded_bassClip = audioReader.audioSource.clip;
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_bassClip, userAvatar.avatarName, SelectedSong.title, "bass");
                    SelectedSong.bassToggleCount = 1;
                    break;
                case "piano":
                    SelectedSong.recorded_pianoClip = audioReader.audioSource.clip;
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_pianoClip, userAvatar.avatarName, SelectedSong.title, "piano");
                    SelectedSong.pianoToggleCount = 1;
                    break;
                case "drums":
                    SelectedSong.recorded_drumsClip = audioReader.audioSource.clip;
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_drumsClip, userAvatar.avatarName, SelectedSong.title, "drums");
                    SelectedSong.drumsToggleCount = 1;
                    break;
                case "voice":
                    //SelectedSong.recorded_voice = audioReader.recordedInstrument;
                    //SelectedSong.recorded_voiceClip = audioWriter.convertAudio(SelectedSong.recorded_voice);
                    SelectedSong.recorded_voiceClip = audioReader.audioSource.clip;
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_voiceClip, userAvatar.avatarName, SelectedSong.title, "voice");
                    //for testing
                    //string filename5 = "Axe_On_Flesh_Flesh_2";
                    //clipGuitar = (AudioClip)Resources.Load(filename5);
                    //audioClipArrayCombiner.SaveNow(clipGuitar, SelectedSong.title + "voice");
                    SelectedSong.voiceToggleCount = 1;
                    break;
            }
            audioReader.startRecord = false;
        }

        confirmPopUp.SetActive(false);
        savingPopUp.SetActive(false);
        PlayOptions.SetActive(true);

    }


    /* Delete Button On Click Function
     * remove the associated saved audio clip from the SelectedSong object
     * Delete the audio clip file from the persistent data path
     */
    public void DeleteButtonOnClick() {
        switch (SelectedInstrument) {
            case "guitar":
                SelectedSong.recorded_guitarClip = null;
                if (SelectedSong.guitarToggleCount == 1)
                    SelectedSong.guitarToggleCount = 0;
                //delete recorded guitar save file
                audioClipArrayCombiner.DeleteFile(userAvatar.avatarName, SelectedSong.title, "guitar");
                break;
            case "bass":
                SelectedSong.recorded_bassClip = null;
                if (SelectedSong.bassToggleCount == 1)
                    SelectedSong.bassToggleCount = 0;
                //delete recorded bass save file
                audioClipArrayCombiner.DeleteFile(userAvatar.avatarName, SelectedSong.title, "bass");
                break;
            case "piano":
                SelectedSong.recorded_pianoClip = null;
                if (SelectedSong.pianoToggleCount == 1)
                    SelectedSong.pianoToggleCount = 0;
                //delete recorded piano save file
                audioClipArrayCombiner.DeleteFile(userAvatar.avatarName, SelectedSong.title, "piano");
                break;
            case "drums":
                SelectedSong.recorded_drumsClip = null;
                if (SelectedSong.drumsToggleCount == 1)
                    SelectedSong.drumsToggleCount = 0;
                //delete recorded drums save file
                audioClipArrayCombiner.DeleteFile(userAvatar.avatarName, SelectedSong.title, "drums");
                break;
            case "voice":
                SelectedSong.recorded_voiceClip = null;
                if (SelectedSong.voiceToggleCount == 1)
                    SelectedSong.voiceToggleCount = 0;
                //delete recorded voice save file
                audioClipArrayCombiner.DeleteFile(userAvatar.avatarName, SelectedSong.title, "voice");
                break;
        }
    }

    // Toggle the volume of the teacher guide audio
    public void audioGuideMuteToggle() {
        if (audioGuideSource.volume == 1) {
            audioGuideSource.volume = 0;
            guideText.GetComponent<Text>().text = "Mute";
            guideButton.GetComponent<UnityEngine.UI.RawImage>().texture = guideTexture_mute;
        } else {
            audioGuideSource.volume = 1;
            guideText.GetComponent<Text>().text = "Teacher";
            guideButton.GetComponent<UnityEngine.UI.RawImage>().texture = guideTexture;
        }
    }

    /* Combine Audio Button On Click Function
     * call the Combine Files mthod of the Audio Clip Array Combiner script
     */
    public void combineAudioFiles() {
        audioClipArrayCombiner.CombineFiles(userAvatar.avatarName, SelectedSong.title);
    }

    // Stop the recording process and go back to the play options page without saving the recording 
    public void CancelRecording() {
        foreach (GameObject page in currentPages) {
            Destroy(page);
        }
        currentPages = new List<GameObject>();
        recording = false;
        recordView.SetActive(false);
        PlayOptions.SetActive(true);
        stopLayeredAudio();
        audioGuideSource.Stop();
        audioReader.startRecord = true;
    }

    // Update is called once per frame
    void Update()
    {
        if (userAvatar != null) {
            // Set Avatar Texture For Selected Instrument
            switch (SelectedInstrument) {
                case "guitar":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarGuitar;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Guitar";
                    break;
                case "bass":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarBass;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Bass";
                    break;
                case "piano":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarPiano;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Piano";
                    break;
                case "drums":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarDrums;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Drums";
                    break;
                case "voice":
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatarVoice;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": Voice";
                    break;
                default:
                    avatarDisplay.GetComponent<RawImage>().texture = userAvatar.avatar;
                    avatarDisplayText.GetComponent<UnityEngine.UI.Text>().text = userAvatar.avatarName + ": No Instrument";
                    break;
            }
        }

        // Toggle Instrument button text
        if(SelectedSong != null && !playing_recording) {
            switch (SelectedSong.guitarToggleCount) {
                case 0:
                    guitarText.GetComponent<Text>().text = "Teacher";
                    guitarButton.GetComponent<UnityEngine.UI.RawImage>().texture = guitarTexture;
                    break;
                case 1:
                    guitarText.GetComponent<Text>().text = "Student";
                    guitarButton.GetComponent<UnityEngine.UI.RawImage>().texture = guitarTexture;
                    break;
                case 2:
                    guitarText.GetComponent<Text>().text = "Mute";
                    guitarButton.GetComponent<UnityEngine.UI.RawImage>().texture = guitarTexture_mute;
                    break;
            }
            switch (SelectedSong.bassToggleCount) {
                case 0:
                    bassText.GetComponent<Text>().text = "Teacher";
                    bassButton.GetComponent<UnityEngine.UI.RawImage>().texture = bassTexture;
                    break;
                case 1:
                    bassText.GetComponent<Text>().text = "Student";
                    bassButton.GetComponent<UnityEngine.UI.RawImage>().texture = bassTexture;
                    break;
                case 2:
                    bassText.GetComponent<Text>().text = "Mute";
                    bassButton.GetComponent<UnityEngine.UI.RawImage>().texture = bassTexture_mute;
                    break;
            }
            switch (SelectedSong.pianoToggleCount) {
                case 0:
                    pianoText.GetComponent<Text>().text = "Teacher";
                    pianoButton.GetComponent<UnityEngine.UI.RawImage>().texture = pianoTexture;
                    break;
                case 1:
                    pianoText.GetComponent<Text>().text = "Student";
                    pianoButton.GetComponent<UnityEngine.UI.RawImage>().texture = pianoTexture;
                    break;
                case 2:
                    pianoText.GetComponent<Text>().text = "Mute";
                    pianoButton.GetComponent<UnityEngine.UI.RawImage>().texture = pianoTexture_mute;
                    break;
            }
            switch (SelectedSong.drumsToggleCount) {
                case 0:
                    drumsText.GetComponent<Text>().text = "Teacher";
                    drumsButton.GetComponent<UnityEngine.UI.RawImage>().texture = drumsTexture;
                    break;
                case 1:
                    drumsText.GetComponent<Text>().text = "Student";
                    drumsButton.GetComponent<UnityEngine.UI.RawImage>().texture = drumsTexture;
                    break;
                case 2:
                    drumsText.GetComponent<Text>().text = "Mute";
                    drumsButton.GetComponent<UnityEngine.UI.RawImage>().texture = drumsTexture_mute;
                    break;
            }
            switch (SelectedSong.voiceToggleCount) {
                case 0:
                    voiceText.GetComponent<Text>().text = "Teacher";
                    voiceButton.GetComponent<UnityEngine.UI.RawImage>().texture = voiceTexture;
                    break;
                case 1:
                    voiceText.GetComponent<Text>().text = "Student";
                    voiceButton.GetComponent<UnityEngine.UI.RawImage>().texture = voiceTexture;
                    break;
                case 2:
                    voiceText.GetComponent<Text>().text = "Mute";
                    voiceButton.GetComponent<UnityEngine.UI.RawImage>().texture = voiceTexture_mute;
                    break;
            }
        }

        // Revert To Play Button When Audio is Finished Playing Recording or Recording
        if (!playing_recording && !recording) {
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
        }

        // Show Mircrophone Texture on Play Button When Recording
        if (recording) {
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = micTexture;
            promptScrollBar.GetComponent<Scrollbar>().value = 1 - (audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value / audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().maxValue);
        }

        if(SelectedInstrument != null) {
            sheetMusicButton.GetComponent<UnityEngine.UI.RawImage>().texture = sheetMusicButtonActive;
            videoButton.GetComponent<UnityEngine.UI.RawImage>().texture = videoButtonActive;
            switch (SelectedInstrument) {
                // all cases do the same things, only difference is specific for selected instrument
                case "guitar":
                    // Audio bar size equals length of recorded instrument layer
                    audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
                    audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
                    if (playing_recording && !guitarAudioSource.isPlaying) {
                        // Finished playing user recorded track
                        PlayButtonOnClick();
                    } else if (recording && !guitarAudioSource.isPlaying) {
                        // Finished recording
                        Record();
                    }
                    if(SelectedSong.recorded_guitarClip != null) {
                        deleteButton.SetActive(true);
                    } else {
                        deleteButton.SetActive(false);
                    }
                    break;
                case "bass":
                    audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = bassAudioSource.time;
                    audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value = bassAudioSource.time;
                    if (playing_recording && !bassAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !bassAudioSource.isPlaying) {
                        Record();
                    }
                    if (SelectedSong.recorded_bassClip != null) {
                        deleteButton.SetActive(true);
                    } else {
                        deleteButton.SetActive(false);
                    }
                    break;
                case "piano":
                    audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = pianoAudioSource.time;
                    audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value = pianoAudioSource.time;
                    if (playing_recording && !pianoAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !pianoAudioSource.isPlaying) {
                        Record();
                    }
                    if (SelectedSong.recorded_pianoClip != null) {
                        deleteButton.SetActive(true);
                    } else {
                        deleteButton.SetActive(false);
                    }
                    break;
                case "drums":
                    audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = drumsAudioSource.time;
                    audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value = drumsAudioSource.time;
                    if (playing_recording && !drumsAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !drumsAudioSource.isPlaying) {
                        Record();
                    }
                    if (SelectedSong.recorded_drumsClip != null) {
                        deleteButton.SetActive(true);
                    } else {
                        deleteButton.SetActive(false);
                    }
                    break;
                case "voice":
                    audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = voiceAudioSource.time;
                    audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().value = voiceAudioSource.time;
                    if (playing_recording && !voiceAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !voiceAudioSource.isPlaying) {
                        Record();
                    }
                    if (SelectedSong.recorded_voiceClip != null) {
                        deleteButton.SetActive(true);
                    } else {
                        deleteButton.SetActive(false);
                    }
                    break;
            } 
        } else if (SelectedSong != null && SelectedInstrument == null) {
            // Selected Instrument is null
            // fill audio slider based on time passed
            // check for end of song and change play button back
            // can not record without a selected instrument
            sheetMusicButton.GetComponent<UnityEngine.UI.RawImage>().texture = sheetMusicButtonInactive;
            videoButton.GetComponent<UnityEngine.UI.RawImage>().texture = videoButtonInactive;
            deleteButton.SetActive(false);
            if (SelectedSong.recorded_guitarClip != null) {
                audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
            } else if (SelectedSong.recorded_bassClip != null) {
                audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = bassAudioSource.time;
            } else if (SelectedSong.recorded_pianoClip != null) {
                audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = pianoAudioSource.time;
            } else if (SelectedSong.recorded_drumsClip != null) {
                audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = drumsAudioSource.time;
            } else if (SelectedSong.recorded_voiceClip != null) {
                audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().value = voiceAudioSource.time;
            }
        }

        if(SelectedSong != null) {
            if(SelectedSong.recorded_guitarClip == null || SelectedSong.recorded_bassClip == null ||
                SelectedSong.recorded_pianoClip == null || SelectedSong.recorded_drumsClip == null || 
                SelectedSong.recorded_voiceClip == null) {
                combineAudioFilesButton.SetActive(false);
            } else {
                combineAudioFilesButton.SetActive(true);
            }
        }

        if (SelectedSong != null && SelectedSong.recorded_guitarClip == null &&
            SelectedSong.recorded_bassClip == null && SelectedSong.recorded_pianoClip == null &&
            SelectedSong.recorded_drumsClip == null && SelectedSong.recorded_voiceClip == null) {
            playBtn.SetActive(false);
        } else {
            playBtn.SetActive(true);
        }

        if (SelectedInstrument == null || recording) {
            removeInstrumentButton.SetActive(false);
        } else {
            removeInstrumentButton.SetActive(true);
        }
		
		// if (metronomeActive) {
	// 		metronomeButton.GetComponent<UnityEngine.UI.RawImage>().texture = metronomeTexture;
	// 	} else {
	// 		metronomeButton.GetComponent<UnityEngine.UI.RawImage>().texture = metronomeTexture_mute;
	// 	}
			
    }
}
