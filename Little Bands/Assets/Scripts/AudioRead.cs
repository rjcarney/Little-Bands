using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;

public class AudioRead : MonoBehaviour
{

    public bool isRecording = false;
    public bool startRecord = false;

    private AudioSource audioSource;
    public AudioClip produce;

    //temporary audio vector we write to every second while recording is enabled..
    List<float> tempRecording = new List<float>();

    //list of recorded clips...
    List<float[]> recordedClips = new List<float[]>();

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        //set up recording to last a max of 1 seconds and loop over and over
        audioSource.clip = Microphone.Start(null, true, 1, 44100);
        audioSource.Play();
        //resize our temporary vector every second
        Invoke("ResizeRecording", 1);

    }



    static public byte[] SaveAudioClipToWav(AudioClip audioClip, string filename)
    {
        print("working");
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

    void Update()
    {
        //space key triggers recording to start...
        if (startRecord)
        {
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
                produce = audioSource.clip = AudioClip.Create("recorded samples", fullClip.Length, 1, 44100, false);
                //produce = audioSource.clip;
                audioSource.clip.SetData(fullClip, 0);
                audioSource.loop = true;
                //Saves audio file
                SaveAudioClipToWav(audioSource.clip, "testAudio381.wav");
            }
            else
            {
                //stop audio playback and start new recording...
                audioSource.Stop();
                tempRecording.Clear();
                Microphone.End(null);
                audioSource.clip = Microphone.Start(null, true, 1, 44100);
                Invoke("ResizeRecording", 1);
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

    }

    //chooose which clip to play based on number key..
    void SwitchClips(int index)
    {
        if (index < recordedClips.Count)
        {
            audioSource.Stop();
            int length = recordedClips[index].Length;
            audioSource.clip = AudioClip.Create("recorded samples", length, 1, 44100, false);
            audioSource.clip.SetData(recordedClips[index], 0);
            audioSource.loop = true;
            audioSource.Play();

        }
    }




}