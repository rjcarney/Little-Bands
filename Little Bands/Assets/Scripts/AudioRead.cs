
﻿using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class AudioRead : MonoBehaviour
{
    public bool startRecord = false;
    public bool isRecording = false;
    private AudioSource audioSource;

    //Controls
    //Space: Records
    //1-9: Sets track and plays recording
    //M: Mixes the recording into one file
    //S: Stop sound

    //temporary audio vector we write to every second while recording is enabled..
    List<float> tempRecording = new List<float>();

    //list of recorded clips...
    public List<float[]> recordedClips = new List<float[]>();
    List<float[]> finishedTrack = new List<float[]>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //set up recording to last a max of 1 seconds and loop over and over
        audioSource.clip = Microphone.Start(null, true, 120, 44100);
        //audioSource.Play();
        //resize our temporary vector every second
        Invoke("ResizeRecording", 1);
    }

    static public byte[] SaveAudioClipToWav(AudioClip audioClip, string filename)
    {
        byte[] buffer;
        FileStream fsWrite = File.Open(filename, FileMode.Create);

        BinaryWriter bw = new BinaryWriter(fsWrite);

        Byte[] header = { 82, 73, 70, 70, 22, 10, 4, 0, 87, 65, 86, 69, 102, 109, 116, 32 };
        bw.Write(header);

        Byte[] header2 = { 16, 0, 0, 0, 1, 0, 1, 0, 68, 172, 0, 0, 136, 88, 1, 0 };
        bw.Write(header2);

        Byte[] header3 = { 2, 0, 16, 0, 100, 97, 116, 97, 152, 9, 4, 0 };

        bw.Write(header3);

        float[] samples = new float[audioClip.samples];
        audioClip.GetData(samples, 0);

        int i = 0;

        while (i < audioClip.samples)
        {
            int sampleInt = (int)(32000.0 * samples[i++]);
            int msb = sampleInt / 256;
            int lsb = sampleInt - (msb * 256);
            bw.Write((Byte)lsb);
            bw.Write((Byte)msb);
        }
        long length = fsWrite.Length;
        int lengthInt = Convert.ToInt32(length);
        buffer = new byte[lengthInt];
        fsWrite.Read(buffer, 0, lengthInt);
        return buffer;

        fsWrite.Close();
    }

    void ResizeRecording()
    {
        if (isRecording)
        {
            //add the next second of recorded audio to temp vector
            int length = 44100;
            float[] clipData = new float[length];
            audioSource.clip.GetData(clipData, 0);
            tempRecording.AddRange(clipData);
            Invoke("ResizeRecording", 1);
        }
    }

    private float ClampToValidRange(float value)
    {
        float min = -1.0f;
        float max = 1.0f;
        return (value < min) ? min : (value > max) ? max : value;
    }

    private float[] MixAndClampFloatBuffers(float[] bufferA, float[] bufferB)
    {
        int maxLength = Math.Min(bufferA.Length, bufferB.Length);
        float[] mixedFloatArray = new float[maxLength];

        for (int i = 0; i < maxLength; i++)
        {
            mixedFloatArray[i] = ClampToValidRange((bufferA[i] + bufferB[i]) / 2);
        }
        return mixedFloatArray;
    }

    void Update()
    {
        //space key triggers recording to start...
        if (Input.GetKeyDown("space") || startRecord)
        {
            startRecord = false;
            isRecording = !isRecording;
            Debug.Log(isRecording == true ? "Is Recording" : "Off");

            if (isRecording == false)
            {
                //stop recording, get length, create a new array of samples
                int length = Microphone.GetPosition(null);

                Microphone.End(null);
                float[] clipData = new float[length];
                audioSource.clip.GetData(clipData, 0);

                //create a larger vector that will have enough space to hold our temporary
                //recording, and the last section of the current recording
                float[] fullClip = new float[clipData.Length + tempRecording.Count];
                for (int i = 0; i < fullClip.Length; i++)
                {
                    //write data all recorded data to fullCLip vector
                    if (i < tempRecording.Count)
                        fullClip[i] = tempRecording[i];
                    else
                        fullClip[i] = clipData[i - tempRecording.Count];
                }

                recordedClips.Add(fullClip);
                audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, 44100, false);
                audioSource.clip.SetData(fullClip, 0);
                audioSource.loop = true;
                print(recordedClips[0].Length);
                //Saves audio file
                //SaveAudioClipToWav(audioSource.clip, "testAudio381.wav");
            }
            else
            {
                //stop audio playback and start new recording...
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = Microphone.Start(null, true, 120, 44100);
                //Invoke("ResizeRecording", 1);
            }


        }

        //use number keys to switch between recorded clips, start from 1!!
        for (int i = 0; i < 10; i++)
        {
            if (Input.GetKeyDown("" + i))
            {
                SwitchClips(i - 1);
            }
        }


        if (Input.GetKeyDown(KeyCode.M))
        {
            float[] temp = MixAndClampFloatBuffers(recordedClips[0], recordedClips[1]);
            audioSource.clip = AudioClip.Create("recorded samples", temp.Length, 1, 44100, false);
            audioSource.clip.SetData(temp, 0);
            SaveAudioClipToWav(audioSource.clip, "testAudio.wav");
        }
        if (Input.GetKeyDown(KeyCode.S))
            audioSource.Stop();

    }

    //chooose which clip to play based on number key..
    void SwitchClips(int index)
    {
        if (index < recordedClips.Count)
        {
            audioSource.Stop();
            int length = recordedClips[0].Length;
            audioSource.clip = AudioClip.Create("recorded samples", length, 1, 44100, false);
            audioSource.clip.SetData(recordedClips[0], 0);
            audioSource.loop = true;
            audioSource.Play();
            print("looping");
        }
    }




}