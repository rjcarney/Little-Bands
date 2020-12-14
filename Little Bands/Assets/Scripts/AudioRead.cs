using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class AudioRead : MonoBehaviour
{
    public bool startRecord = false;
    public bool isRecording = false;
    public bool finishedRecording;
    public int frequency = 44100;
    public int seconds = 120;
    public AudioSource audioSource;
    public AudioSource tempSource;

    private bool tempFull = false;

    private GameManager gm;
    private List<float> tempRecording = new List<float>();
    private float[] recordedInstrument;

    void Start()
    {
        //Set audioSource
        //audioSource = GetComponent<AudioSource>();
        //audioSource.clip = Microphone.Start(null, true, seconds, frequency);
        //tempSource = new AudioSource();
    }

    void Update()
    {
        //Determine state of recording
        if (startRecord)
        {
            isRecording = !isRecording;
            startRecord = false;

            //Create audio clip
            if (isRecording == false)
                RecordAudio();
            //Stop recording
            else
                PauseRecording();
        }

        //Combine and clear tempSource
        if (!isRecording)
        {
            if (tempFull)
                Combine();
            else
                tempSource.clip = audioSource.clip;
        }

        if (Input.GetKeyDown("space"))
            startRecord = !startRecord;
        if (Input.GetKeyDown("z"))
            audioSource.Play();
        if (Input.GetKeyDown("x"))
            tempSource.Play();
    }

    private void RecordAudio()
    {
        int length = Microphone.GetPosition(null);
        Microphone.End(null);
        float[] clipData = new float[length];
        audioSource.clip.GetData(clipData, 0);

        float[] fullClip = new float[clipData.Length + tempRecording.Count];
        for (int i = 0; i < fullClip.Length; i++)
        {
            if (i < tempRecording.Count)
                fullClip[i] = tempRecording[i];
            else
                fullClip[i] = clipData[i - tempRecording.Count];
        }

        //Set Audio
        recordedInstrument = fullClip;
        audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, frequency, false);

        audioSource.clip.SetData(fullClip, 0);

    }

    public void PauseRecording()
    {
        audioSource.Stop();
        //tempRecording.Clear();
        Microphone.End(null);
        audioSource.clip = Microphone.Start(null, true, seconds, frequency);
        print("Pause Recording");
        tempFull = true;
    }


    private void Combine()
    {
        if (tempFull)
        {
            print("Combining");
            //AudioClip[] clips = new AudioClip[] { audioSource.clip, tempSource.clip };
            Combine(audioSource.clip, tempSource.clip);
            RemoveClip(tempSource);
            tempFull = false;
        }
    }

    private void RemoveClip(AudioSource audio)
    {
        audio.clip = Microphone.Start(null, true, seconds, frequency);
        audio.clip = null;
    }

    public void ClearAudio()
    {
        audioSource.clip = null;
        tempSource.clip = null;
    }

    private void Combine(AudioClip x, AudioClip y)
    {
        if (x != null && y != null)
        {
            float[] first = new float[x.samples * x.channels];
            float[] second = new float[y.samples * y.channels];
            float[] final = new float[first.Length + second.Length];

            x.GetData(first, 0);
            y.GetData(second, 0);

            final = second.Concat(first).ToArray();

            audioSource.clip = AudioClip.Create("recorded samples", final.Length, 1, frequency, false);
            audioSource.clip.SetData(final, 0);
            tempSource.clip.SetData(final, 0);
        }
    }
}