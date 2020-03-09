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

<<<<<<< HEAD
    void Start() {
=======
    void Start()
    {
>>>>>>> b0fdf84bf1186bde5cf6ad7de898f1c2b46d91cd
        //Set audioSource
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, seconds, frequency);
    }

    void Update() {
        if (Input.GetKeyDown("space") || startRecord) {
            isRecording = !isRecording;
            startRecord = false;

            //Create audio float array
<<<<<<< HEAD
            if (isRecording == false) {
=======
            if (isRecording == false)
            {
>>>>>>> b0fdf84bf1186bde5cf6ad7de898f1c2b46d91cd
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

<<<<<<< HEAD
                recordedInstrument = fullClip;
=======
                recordedInstrument=fullClip;
>>>>>>> b0fdf84bf1186bde5cf6ad7de898f1c2b46d91cd
                audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, frequency, false);
                audioSource.clip.SetData(fullClip, 0);
            }
            //Record
<<<<<<< HEAD
            else {
=======
            else
            {
>>>>>>> b0fdf84bf1186bde5cf6ad7de898f1c2b46d91cd
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = Microphone.Start(null, true, seconds, frequency);
            }
        }
        //Initiate audio playback
        if (Input.GetKeyDown(KeyCode.M))
            playAudio(recordedInstrument);

        //Stop audio from playing
        if (Input.GetKeyDown(KeyCode.N))
            audioSource.Stop();
    }

    //Convert float array to audio
<<<<<<< HEAD
    void playAudio(float[] sound) {
=======
    void playAudio(float[] sound)
    {
>>>>>>> b0fdf84bf1186bde5cf6ad7de898f1c2b46d91cd
        audioSource.Stop();
        int length = sound.Length;
        audioSource.clip = AudioClip.Create("recorded samples", length, 1, frequency, false);
        audioSource.clip.SetData(sound, 0);
        audioSource.loop = true;
        audioSource.Play();
    }
}