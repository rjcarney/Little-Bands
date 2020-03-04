using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class AudioRead : MonoBehaviour
{
    public bool startRecord = false;
    public bool isRecording = false;
    public int frequency = 44100;
    public int seconds = 120;
    private AudioSource audioSource;

    public List<float> tempRecording = new List<float>();
    public float[] recordedGuitar;
    public float[] recordedBass;
    public float[] recordedPiano;
    public float[] recordedDrums;
    public float[] recordedVoice;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        audioSource.clip = Microphone.Start(null, true, seconds, frequency);
    }

    void Update()
    {
        if (Input.GetKeyDown("space") || startRecord)
        {
            isRecording = !isRecording;
            startRecord = false;

            if (isRecording == false)
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

                recordedGuitar=fullClip;
                audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, frequency, false);
                audioSource.clip.SetData(fullClip, 0);
            }
            else
            {
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = Microphone.Start(null, true, seconds, frequency);
            }
        }
        if (Input.GetKeyDown(KeyCode.M))
        {
            playAudio(recordedGuitar);
        }
        if (Input.GetKeyDown(KeyCode.N))
            audioSource.Stop();
    }

    void playAudio(float[] sound)
    {
        audioSource.Stop();
        int length = sound.Length;
        audioSource.clip = AudioClip.Create("recorded samples", length, 1, frequency, false);
        audioSource.clip.SetData(sound, 0);
        audioSource.loop = true;
        audioSource.Play();
    }
}