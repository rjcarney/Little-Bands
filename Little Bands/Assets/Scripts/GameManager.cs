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
     *  On the record page our instrument buttons will now have two knew functions:
     *  On first click if instrument has not been selected
     *                  then select that instrument
     *  else
     *  On every subsequent click toggle the audio being played from 
     *      0. teacher audio
     *      1. if user has recording for insturment
     *          then users recording
     *      else count = 2
     *      2. mute
     *  
     *  There also be the addition of a new Audio Source to play a teacher guide to play each instrument
     */
    public static GameManager instance = null;
    public AudioRead audioReader;

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
    public GameObject audioSlider;

    private bool playing_recording;
    public GameObject playBtn;
    public GameObject playBtnTxt;
    private bool playing_layeredAudio;
    private bool recording;

    public Texture playTexture;
    public Texture stopTexture;

    public GameObject PlayOptions;

    public GameObject sheetMusicPopUp;
    public GameObject sheetMusicTitle;

    public AudioSource fullAudioSource;
    public AudioSource guitarAudioSource;
    public AudioSource bassAudioSource;
    public AudioSource pianoAudioSource;
    public AudioSource drumsAudioSource;
    public AudioSource voiceAudioSource;

    public Text guitarTrackText;
    public Text bassTrackText;
    public Text pianoTrackText;
    public Text drumsTrackText;
    public Text voiceTrackText;


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

        audioSlider.GetComponent<UnityEngine.UI.Slider>().direction = Slider.Direction.LeftToRight;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().minValue = 0;
        audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = 1;
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
     * selecting a song will
     * 
     * Set SelectedSong
     *     Audio Source for instrument files
     *     These will be based on instrument toggels associated with the selected song
     */
    public void selectSong(GameObject SongListItem) {
        SelectedSong = SongListItem.GetComponent<SongItem>();

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
        audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = fullAudioSource.clip.length;

        // Set Correct Avatar to display

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
        if (!recording) {
            if (!playing_recording) {
                // First Click Select instrument
                if (SelectedInstrument != instrument) {
                    SelectedInstrument = instrument;
                }
                // Every Subsequent Click Toggle Audio
                else {
                    if (instrument == "guitar") {
                        switch (SelectedSong.guitarToggleCount) {
                            case 0:
                                if (SelectedSong.recorded_guitarClip != null) {
                                    guitarAudioSource.clip = SelectedSong.recorded_guitarClip;
                                    SelectedSong.guitarToggleCount = 1;
                                    guitarAudioSource.volume = 1;
                                } else {
                                    guitarAudioSource.clip = SelectedSong.original_guitar;
                                    guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                    SelectedSong.guitarToggleCount = 2;
                                    guitarAudioSource.volume = 0;
                                }
                                break;
                            case 1:
                                guitarAudioSource.clip = SelectedSong.original_guitar;
                                guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.guitarToggleCount = 2;
                                guitarAudioSource.volume = 0;
                                break;
                            case 2:
                                guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                    SelectedSong.bassToggleCount = 1;
                                    bassAudioSource.volume = 1;
                                } else {
                                    bassAudioSource.clip = SelectedSong.original_bass;
                                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                    SelectedSong.bassToggleCount = 2;
                                    bassAudioSource.volume = 0;
                                }
                                break;
                            case 1:
                                bassAudioSource.clip = SelectedSong.original_bass;
                                bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.bassToggleCount = 2;
                                bassAudioSource.volume = 0;
                                break;
                            case 2:
                                bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                    SelectedSong.pianoToggleCount = 1;
                                    pianoAudioSource.volume = 1;
                                } else {
                                    pianoAudioSource.clip = SelectedSong.original_piano;
                                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                    SelectedSong.pianoToggleCount = 2;
                                    pianoAudioSource.volume = 0;
                                }
                                break;
                            case 1:
                                pianoAudioSource.clip = SelectedSong.original_piano;
                                pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.pianoToggleCount = 2;
                                pianoAudioSource.volume = 0;
                                break;
                            case 2:
                                pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                    SelectedSong.drumsToggleCount = 1;
                                    drumsAudioSource.volume = 1;
                                } else {
                                    drumsAudioSource.clip = SelectedSong.original_drums;
                                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                    SelectedSong.drumsToggleCount = 2;
                                    drumsAudioSource.volume = 0;
                                    break;
                                }
                                break;
                            case 1:
                                drumsAudioSource.clip = SelectedSong.original_drums;
                                drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.drumsToggleCount = 2;
                                drumsAudioSource.volume = 0;
                                break;
                            case 2:
                                drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
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
                                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                                    SelectedSong.voiceToggleCount = 1;
                                    voiceAudioSource.volume = 1;
                                } else {
                                    voiceAudioSource.clip = SelectedSong.original_voice;
                                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                    SelectedSong.voiceToggleCount = 2;
                                    voiceAudioSource.volume = 0;
                                }
                                break;
                            case 1:
                                voiceAudioSource.clip = SelectedSong.original_voice;
                                voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                SelectedSong.voiceToggleCount = 2;
                                voiceAudioSource.volume = 0;
                                break;
                            case 2:
                                voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                                voiceAudioSource.volume = 1;
                                SelectedSong.voiceToggleCount = 0;
                                break;
                        }
                    }
                }
            }
        }
    }
    
    //  The Play Button will now only play the users recorded audio
     public void PlayButtonOnClick() {
        if (playing_recording == false) {
            guitarAudioSource.Stop();
            bassAudioSource.Stop();
            pianoAudioSource.Stop();
            drumsAudioSource.Stop();
            voiceAudioSource.Stop();
            playing_layeredAudio = false;

            // Set all recording clips to recorded wav file otherwise set to null
            guitarAudioSource.clip = (SelectedSong.recorded_guitarClip == null) ? null : SelectedSong.recorded_guitarClip;
            bassAudioSource.clip = (SelectedSong.recorded_bassClip == null) ? null : SelectedSong.recorded_bassClip;
            pianoAudioSource.clip = (SelectedSong.recorded_pianoClip == null) ? null : SelectedSong.recorded_pianoClip;
            drumsAudioSource.clip = (SelectedSong.recorded_drumsClip == null) ? null : SelectedSong.recorded_drumsClip;
            voiceAudioSource.clip = (SelectedSong.recorded_voiceClip == null) ? null : SelectedSong.recorded_voiceClip;

            if(guitarAudioSource.clip == null && bassAudioSource.clip == null &&
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
                    guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                } else {
                    guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "None";
                }
                if (bassAudioSource.clip != null) {
                    bassAudioSource.Play();
                    bassAudioSource.volume = 1;
                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                } else {
                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "None";
                }
                if (pianoAudioSource.clip != null) {
                    pianoAudioSource.Play();
                    pianoAudioSource.volume = 1;
                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                } else {
                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "None";
                }
                if (drumsAudioSource.clip != null) {
                    drumsAudioSource.Play();
                    drumsAudioSource.volume = 1;
                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                } else {
                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "None";
                }
                if (voiceAudioSource.clip != null) {
                    voiceAudioSource.Play();
                    voiceAudioSource.volume = 1;
                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                } else {
                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "None";
                }

                playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = stopTexture;
                playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\nStop";
                playing_recording = true;
            }
        } else {
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
    }

    
    public void playLayeredAudio() {
        if (playing_layeredAudio == false) {
            audioSlider.GetComponent<UnityEngine.UI.Slider>().maxValue = SelectedSong.original_full_audio.length;

            guitarAudioSource.Play();
            bassAudioSource.Play();
            pianoAudioSource.Play();
            drumsAudioSource.Play();
            voiceAudioSource.Play();
            playing_layeredAudio = true;
        } else {
            guitarAudioSource.Stop();
            bassAudioSource.Stop();
            pianoAudioSource.Stop();
            drumsAudioSource.Stop();
            voiceAudioSource.Stop();
            playing_layeredAudio = false;
        }
    }

    public void OpenSheetMusic() {
        sheetMusicPopUp.SetActive(true);
        PlayOptions.SetActive(false);
    }

    public void CloseSheetMusic() {
        sheetMusicPopUp.SetActive(false);
        PlayOptions.SetActive(true);
    }

    

    

    public void Record() {
        if (recording == false) {
            recording = true;
            PlayOptions.SetActive(false);
            sheetMusicPopUp.SetActive(true);
        } else {
            recording = false;
            PlayOptions.SetActive(true);
            sheetMusicPopUp.SetActive(false);
        }

        playLayeredAudio();
        audioReader.startRecord = true;

        if (playing_layeredAudio == false)
        {
            switch (SelectedInstrument)
            {
                case "guitar":     
                    SelectedSong.recorded_guitar = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "bass":
                    SelectedSong.recorded_bass = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "piano":
                    SelectedSong.recorded_piano = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "drums":
                    SelectedSong.recorded_drums = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
                case "voice":
                    SelectedSong.recorded_voice = audioReader.recordedClips[0];
                    audioReader.startRecord = false;
                    break;
            }
        }
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
            switch (SelectedSong.guitarToggleCount) {
                case 0:
                    guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    guitarTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.bassToggleCount) {
                case 0:
                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    bassTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.pianoToggleCount) {
                case 0:
                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    pianoTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.drumsToggleCount) {
                case 0:
                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    drumsTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
            switch (SelectedSong.voiceToggleCount) {
                case 0:
                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Teacher";
                    break;
                case 1:
                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Student";
                    break;
                case 2:
                    voiceTrackText.GetComponent<UnityEngine.UI.Text>().text = "Muted";
                    break;
            }
        }

        // Revert To Play Button When Audio is Finished Playing Recording
        if (!playing_recording) {
            playBtn.GetComponent<UnityEngine.UI.RawImage>().texture = playTexture;
            playBtnTxt.GetComponent<UnityEngine.UI.Text>().text = "\n\n\nPlay";
        }


        if(SelectedInstrument != null) {
            switch (SelectedInstrument) {
                case "guitar":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
                    if (playing_recording && !guitarAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !guitarAudioSource.isPlaying) {
                        Record();
                    }
                    break;
                case "bass":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = bassAudioSource.time;
                    if (playing_recording && !bassAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !bassAudioSource.isPlaying) {
                        Record();
                    }
                    break;
                case "piano":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = pianoAudioSource.time;
                    if (playing_recording && !pianoAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !pianoAudioSource.isPlaying) {
                        Record();
                    }
                    break;
                case "drums":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = drumsAudioSource.time;
                    if (playing_recording && !drumsAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !drumsAudioSource.isPlaying) {
                        Record();
                    }
                    break;
                case "voice":
                    audioSlider.GetComponent<UnityEngine.UI.Slider>().value = voiceAudioSource.time;
                    if (playing_recording && !voiceAudioSource.isPlaying) {
                        PlayButtonOnClick();
                    } else if (recording && !voiceAudioSource.isPlaying) {
                        Record();
                    }
                    break;
            } 
        } else {
            audioSlider.GetComponent<UnityEngine.UI.Slider>().value = guitarAudioSource.time;
            if (!guitarAudioSource.isPlaying)
                playing_layeredAudio = false;
        }
    }
}
