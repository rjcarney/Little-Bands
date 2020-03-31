using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    /*  Notes for UI update:
     *  We now need to start off on an Avatar Select Page
     *  The user will select either Red, Green or Blue as their avatar
     * 
     *  We can remove the InstrumentSelectPage and go directly to the RecordPage
     *  
     *  On the record page our instrument buttons will now have two new functions:
     *  On first click if instrument has not been selected
     *                  then select that instrument
     *  else
     *  On every subsequent click toggle the audio being played from 
     *      0. teacher audio
     *      1. if user has recording for insturment
     *          then users recording
     *          else count = 2
     *      2. mute
     *  
     *  There also be the addition of a new Audio Source to play a teacher guide to play each instrument
     */
    public static GameManager instance = null;
    public AudioRead audioReader;
    public AudioWrite audioWriter;

    // Saves individual files to persistentDataPath. Also combines files and saves to persistentDataPath
    public AudioClipArrayCombiner audioClipArrayCombiner;

    // Avatar Select Page Variables
    public GameObject AvatarSelecetPage;
    private Avatar userAvatar;
    
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
    public GameObject playBtnTxt;
    public GameObject audioSlider_playOptions;
    private bool playing_layeredAudio;

    public GameObject combineAudioFilesButton;
    public GameObject deleteButton;

    // RecordView Content
    private bool recording;
    public GameObject recordView;
    public GameObject audioSlider_recordView;
    public GameObject confirmPopUp;

    // SheetMusic Content
    public GameObject sheetMusicPopUp;
    public GameObject sheetMusicTitle;

    // Video Content
    public GameObject videoPopUp;
    public GameObject videoTitle;

    // Audio Sources
    public AudioSource fullAudioSource;
    public AudioSource guitarAudioSource;
    public AudioSource bassAudioSource;
    public AudioSource pianoAudioSource;
    public AudioSource drumsAudioSource;
    public AudioSource voiceAudioSource;
    public AudioSource audioGuideSource;

    public Text guitarTrackText_playOptions;
    public Text bassTrackText_playOptions;
    public Text pianoTrackText_playOptions;
    public Text drumsTrackText_playOptions;
    public Text voiceTrackText_playOptions;

    public Text guitarTrackText_recordView;
    public Text bassTrackText_recordView;
    public Text pianoTrackText_recordView;
    public Text drumsTrackText_recordView;
    public Text voiceTrackText_recordView;

    public AudioClip clipGuitar;


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

        SelectedSong = null;
        playing_recording = false;
        playing_layeredAudio = false;
        recording = false;

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
        //Here is where we will call harris's load method

        // ! UNCOMMENT ONCE LOAD FUNTIONALITY IS ADDED ! 
        //AvatarSelecetPage.SetActive(false);
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

        // Change View
        SongListPage.SetActive(false);
        RecordPage.SetActive(true);
    }


    public void backToSongs() {
        //Deselect song and audio
        SelectedSong = null;
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
    }

    /* RECORDING PAGE
     * After selecting a song the user will be need to select in instrument before they are able to record
     * 
     */

    // New Record page instrument buttons
    public void InstrumentButtonOnClick(string instrument) {
        if (!playing_recording && !recording) {
            // First Click Select instrument
            if (SelectedInstrument != instrument) {
                SelectedInstrument = instrument;
                sheetMusicTitle.GetComponent<UnityEngine.UI.Text>().text = SelectedSong.title + ": " + SelectedInstrument;
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
                                guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                SelectedSong.guitarToggleCount = 1;
                                guitarAudioSource.volume = 1;
                            } else {
                                // No student track, mute
                                guitarAudioSource.clip = SelectedSong.original_guitar;
                                guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.guitarToggleCount = 2;
                                guitarAudioSource.volume = 0;
                            }
                            break;
                        case 1:  // Student
                            // Mute
                            guitarAudioSource.clip = SelectedSong.original_guitar;
                            guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            SelectedSong.guitarToggleCount = 2;
                            guitarAudioSource.volume = 0;
                            break;
                        case 2:  // Muted
                            // Change to teacher
                            guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                            guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                SelectedSong.bassToggleCount = 1;
                                bassAudioSource.volume = 1;
                            } else {
                                bassAudioSource.clip = SelectedSong.original_bass;
                                bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.bassToggleCount = 2;
                                bassAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            bassAudioSource.clip = SelectedSong.original_bass;
                            bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            SelectedSong.bassToggleCount = 2;
                            bassAudioSource.volume = 0;
                            break;
                        case 2:
                            bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                            bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                SelectedSong.pianoToggleCount = 1;
                                pianoAudioSource.volume = 1;
                            } else {
                                pianoAudioSource.clip = SelectedSong.original_piano;
                                pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.pianoToggleCount = 2;
                                pianoAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            pianoAudioSource.clip = SelectedSong.original_piano;
                            pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            SelectedSong.pianoToggleCount = 2;
                            pianoAudioSource.volume = 0;
                            break;
                        case 2:
                            pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                            pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                SelectedSong.drumsToggleCount = 1;
                                drumsAudioSource.volume = 1;
                            } else {
                                drumsAudioSource.clip = SelectedSong.original_drums;
                                drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.drumsToggleCount = 2;
                                drumsAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            drumsAudioSource.clip = SelectedSong.original_drums;
                            drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            SelectedSong.drumsToggleCount = 2;
                            drumsAudioSource.volume = 0;
                            break;
                        case 2:
                            drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                            drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                SelectedSong.voiceToggleCount = 1;
                                voiceAudioSource.volume = 1;
                            } else {
                                voiceAudioSource.clip = SelectedSong.original_voice;
                                voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.voiceToggleCount = 2;
                                voiceAudioSource.volume = 0;
                            }
                            break;
                        case 1:
                            voiceAudioSource.clip = SelectedSong.original_voice;
                            voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                            SelectedSong.voiceToggleCount = 2;
                            voiceAudioSource.volume = 0;
                            break;
                        case 2:
                            voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                            voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                            voiceAudioSource.volume = 1;
                            SelectedSong.voiceToggleCount = 0;
                            break;
                    }
                }
            }
        }
    }
    
    //  The Play Button will now only play the users recorded audio
     public void PlayButtonOnClick() {
        if (!recording) {
            if (playing_recording == false) {
                // Stop all curently playing audio
                guitarAudioSource.Stop();
                bassAudioSource.Stop();
                pianoAudioSource.Stop();
                drumsAudioSource.Stop();
                voiceAudioSource.Stop();
                playing_layeredAudio = false;

                // Set all recording clips to associated recorded wav file otherwise set to null
                guitarAudioSource.clip = (SelectedSong.recorded_guitarClip == null) ? null : SelectedSong.recorded_guitarClip;
                bassAudioSource.clip = (SelectedSong.recorded_bassClip == null) ? null : SelectedSong.recorded_bassClip;
                pianoAudioSource.clip = (SelectedSong.recorded_pianoClip == null) ? null : SelectedSong.recorded_pianoClip;
                drumsAudioSource.clip = (SelectedSong.recorded_drumsClip == null) ? null : SelectedSong.recorded_drumsClip;
                voiceAudioSource.clip = (SelectedSong.recorded_voiceClip == null) ? null : SelectedSong.recorded_voiceClip;

                if (guitarAudioSource.clip == null && bassAudioSource.clip == null &&
                    pianoAudioSource.clip == null && drumsAudioSource.clip == null &&
                    voiceAudioSource.clip == null) {
                    // All Recorded Wav Files Null Nothing to Play
                    // Maybe play avatar voice saying I need to record something first
                } else {
                    // There are recorded wav Files
                    // Play what files the user has recorded
                    if (guitarAudioSource.clip != null) {
                        guitarAudioSource.Play();
                        guitarAudioSource.volume = 1;
                        guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    } else {
                        guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "None";
                    }
                    if (bassAudioSource.clip != null) {
                        bassAudioSource.Play();
                        bassAudioSource.volume = 1;
                        bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    } else {
                        bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "None";
                    }
                    if (pianoAudioSource.clip != null) {
                        pianoAudioSource.Play();
                        pianoAudioSource.volume = 1;
                        pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    } else {
                        pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "None";
                    }
                    if (drumsAudioSource.clip != null) {
                        drumsAudioSource.Play();
                        drumsAudioSource.volume = 1;
                        drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    } else {
                        drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "None";
                    }
                    if (voiceAudioSource.clip != null) {
                        voiceAudioSource.Play();
                        voiceAudioSource.volume = 1;
                        voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    } else {
                        voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "None";
                    }

                    playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
                    playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\nStop";
                    playing_recording = true;
                }
            } else {
                // Stop All Audio
                guitarAudioSource.Stop();
                bassAudioSource.Stop();
                pianoAudioSource.Stop();
                drumsAudioSource.Stop();
                voiceAudioSource.Stop();

                // Reset Audio Track to toggled Selection
                // Reset guitar track
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
                // Reset bass track
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
                // Reset piano track
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
                // Reset drums track
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
                // Reset voice track
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

                playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
                playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\nPlay";
                playing_recording = false;
            }
        } else {
            // Audio is recoring this button will display the microphone texture at this time
            // This will be happening in the update method
        }
    }

    
    public void playLayeredAudio() {
        if (playing_recording)
            PlayButtonOnClick();

        if (playing_layeredAudio == false) {
            audioSlider_playOptions.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;
            audioSlider_recordView.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;

            guitarAudioSource.Play();
            bassAudioSource.Play();
            pianoAudioSource.Play();
            drumsAudioSource.Play();
            voiceAudioSource.Play();
            audioGuideSource.Play();
            playing_layeredAudio = true;
        } else {
            guitarAudioSource.Stop();
            bassAudioSource.Stop();
            pianoAudioSource.Stop();
            drumsAudioSource.Stop();
            voiceAudioSource.Stop();
            audioGuideSource.Stop();
            playing_layeredAudio = false;
        }
    }

    // Change view from Play Options to Sheet Music
    public void OpenSheetMusic() {
        sheetMusicPopUp.SetActive(true);
        PlayOptions.SetActive(false);
    }

    // Change view from Sheet Music to Play Options
    public void CloseSheetMusic() {
        sheetMusicPopUp.SetActive(false);
        PlayOptions.SetActive(true);
    }

    // Change view from Play Options to Video
    public void OpenVideo() {
        videoPopUp.SetActive(true);
        PlayOptions.SetActive(false);
    }

    // Change view from Video to Play Options
    public void CloseVideo() {
        videoPopUp.SetActive(false);
        PlayOptions.SetActive(true);
    }

    public void Record() {
        // Cancel any currently playing audio
        if (playing_recording)
            PlayButtonOnClick();

        // Ensure instrument is selected
        if (SelectedInstrument != null) {
            // Set Correct View Based on if recording or not
            if (recording == false) {
                recording = true;
                PlayOptions.SetActive(false);
                recordView.SetActive(true);
            } else {
                recording = false;
                confirmPopUp.SetActive(true);
                recordView.SetActive(false);
            }

            // Toggle Audio and AudioReader start
            playLayeredAudio();
            audioReader.startRecord = true;
        } else {
            // Prompt user to select an instrument
        }
    }

    /* Confirm the user wishes to save current recording session
     * Toggle audio clip to students recording
     */
    

    public void confirmRecording(bool confirm) {
        if (confirm) {
            switch (SelectedInstrument) {
                case "guitar":
                    SelectedSong.recorded_guitar = audioReader.recordedInstrument;
                    SelectedSong.recorded_guitarClip = audioWriter.convertAudio(SelectedSong.recorded_guitar);
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_guitarClip, SelectedSong.title + "guitar");
                    //string filename1 = "song1";
                    //clipGuitar = (AudioClip)Resources.Load(filename1);
                    //SelectedSong.recorded_guitarClip = audioClipArrayCombiner.ToAudioClip(Application.persistentDataPath + "/" + filename1);
                    //audioClipArrayCombiner.SaveNow(clipGuitar, "guitar");
                    SelectedSong.guitarToggleCount = 1;
                    break;
                case "bass":
                    SelectedSong.recorded_bass = audioReader.recordedInstrument;
                    SelectedSong.recorded_bassClip = audioWriter.convertAudio(SelectedSong.recorded_bass);
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_guitarClip, SelectedSong.title + "bass");
                    SelectedSong.bassToggleCount = 1;
                    break;
                case "piano":
                    SelectedSong.recorded_piano = audioReader.recordedInstrument;
                    SelectedSong.recorded_pianoClip = audioWriter.convertAudio(SelectedSong.recorded_piano);
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_guitarClip, SelectedSong.title + "piano");
                    SelectedSong.pianoToggleCount = 1;
                    break;
                case "drums":
                    SelectedSong.recorded_drums = audioReader.recordedInstrument;
                    SelectedSong.recorded_drumsClip = audioWriter.convertAudio(SelectedSong.recorded_drums);
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_guitarClip, SelectedSong.title + "drums");
                    SelectedSong.drumsToggleCount = 1;
                    break;
                case "voice":
                    SelectedSong.recorded_voice = audioReader.recordedInstrument;
                    SelectedSong.recorded_voiceClip = audioWriter.convertAudio(SelectedSong.recorded_voice);
                    audioClipArrayCombiner.SaveNow(SelectedSong.recorded_guitarClip, SelectedSong.title + "voice");
                    SelectedSong.voiceToggleCount = 1;
                    break;
            }
            audioReader.startRecord = false;
        }

        confirmPopUp.SetActive(false);
        PlayOptions.SetActive(true);

    }

    public void DeleteButtonOnClick() {
        switch (SelectedInstrument) {
            case "guitar":
                SelectedSong.recorded_guitar = new float[0];
                SelectedSong.recorded_guitarClip = null;
                if (SelectedSong.guitarToggleCount == 1)
                    SelectedSong.guitarToggleCount = 0;
                //delete recorded guitar save file
                break;
            case "bass":
                SelectedSong.recorded_bass = new float[0];
                SelectedSong.recorded_bassClip = null;
                if (SelectedSong.bassToggleCount == 1)
                    SelectedSong.bassToggleCount = 0;
                //delete recorded bass save file
                break;
            case "piano":
                SelectedSong.recorded_piano = new float[0];
                SelectedSong.recorded_pianoClip = null;
                if (SelectedSong.pianoToggleCount == 1)
                    SelectedSong.pianoToggleCount = 0;
                //delete recorded piano save file
                break;
            case "drums":
                SelectedSong.recorded_drums = new float[0];
                SelectedSong.recorded_drumsClip = null;
                if (SelectedSong.drumsToggleCount == 1)
                    SelectedSong.drumsToggleCount = 0;
                //delete recorded drums save file
                break;
            case "voice":
                SelectedSong.recorded_voice = new float[0];
                SelectedSong.recorded_voiceClip = null;
                if (SelectedSong.voiceToggleCount == 1)
                    SelectedSong.voiceToggleCount = 0;
                //delete recorded voice save file
                break;
        }
    }

    // Toggle the teacher guide audio
    public void audioGuideMuteToggle() {
        if (audioGuideSource.volume == 1)
            audioGuideSource.volume = 0;
        else
            audioGuideSource.volume = 1;
    }

    public void combineAudioFiles() {
        audioClipArrayCombiner.CombineFiles();
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

        // Toggle Track Text
        if (SelectedSong != null && !playing_recording) {
            // playing_recording reffers to if the player is playing the audio they recorded using the play button
            switch (SelectedSong.guitarToggleCount) {
                case 0:
                    guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    guitarTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    guitarTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.bassToggleCount) {
                case 0:
                    bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    bassTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    bassTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.pianoToggleCount) {
                case 0:
                    pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    pianoTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    pianoTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.drumsToggleCount) {
                case 0:
                    drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    drumsTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    drumsTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.voiceToggleCount) {
                case 0:
                    voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    voiceTrackText_playOptions.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    voiceTrackText_recordView.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
        }

        // Revert To Play Button When Audio is Finished Playing Recording or Recording
        if (!playing_recording && !recording) {
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\nPlay";
        }

        // Show Mircrophone Texture on Play Button When Recording
        if (recording) {
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = micTexture;
            playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\nRecording";
        }

        if(SelectedInstrument != null) {
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
        } else

        if(SelectedSong != null && SelectedSong.recorded_guitarClip != null &&
            SelectedSong.recorded_bassClip != null && SelectedSong.recorded_pianoClip != null &&
            SelectedSong.recorded_drumsClip != null && SelectedSong.recorded_voiceClip != null) {
            combineAudioFilesButton.SetActive(true);
        } else {
            combineAudioFilesButton.SetActive(false);
        }
    }
}
