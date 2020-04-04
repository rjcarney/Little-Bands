using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class AudioRead : MonoBehaviour {
    public bool startRecord = false;
    public bool isRecording = false;
    public int frequency = 44100;
    public int seconds = 120;
    private AudioSource audioSource;

    public List<float> tempRecording = new List<float>();
    public float[] recordedInstrument;

    void Start() {
        //Set audioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, seconds, frequency);
    }

    void Update() {
        if (Input.GetKeyDown("space") || startRecord) {
            print("recording: " + isRecording);
            isRecording = !isRecording;
            startRecord = false;

            //Create audio float array
            if (isRecording == false) {
                int length = Microphone.GetPosition(null);
                Microphone.End(null);
                float[] clipData = new float[length];
                audioSource.clip.GetData(clipData, 0);

                float[] fullClip = new float[clipData.Length + tempRecording.Count];
                for (int i = 0; i < fullClip.Length; i++) {
                    if (i < tempRecording.Count)
                        fullClip[i] = tempRecording[i];
                    else
                        fullClip[i] = clipData[i - tempRecording.Count];
                }

                recordedInstrument = fullClip;
                audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, frequency, false);
                audioSource.clip.SetData(fullClip, 0);
            }
            //Record
            else {
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = Microphone.Start(null, true, seconds, frequency);
            }
        }
        //testing
        if (Input.GetKeyDown(KeyCode.M))
            playAudio(recordedInstrument);
        if (Input.GetKeyDown(KeyCode.N))
        {
            AudioWrite x = new AudioWrite();
            x.SaveAudioClipToWav(x.convertAudio(audioSource,recordedInstrument), "Test420.wav");
        }
        if (Input.GetKeyDown(KeyCode.B))       
            audioSource.Stop();
        
    }

    //Convert float array to audio
    void playAudio(float[] sound) {
        audioSource.Stop();
        int length = sound.Length;
        audioSource.clip = AudioClip.Create("recorded samples", length, 1, frequency, false);
        audioSource.clip.SetData(sound, 0);
        audioSource.loop = true;
        audioSource.Play();
    }
}