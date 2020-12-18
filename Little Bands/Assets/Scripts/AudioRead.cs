using UnityEngine;
using System.Collections.Generic;
using System.IO;
using System;
using System.Linq;

public class AudioRead : MonoBehaviour
{
	//Audio Controls
	public bool startRecord = false;
	public int frequency = 44100;
	public int seconds = 120;
	public AudioSource audioSource;
	[Range(0, 100)]
	public int recordRate = 100;

	//Tempory hold befor music request
	private float[] tempRecording = new float[0];
	//Helps build the switch that makes it so isRecording only needs
	//to be called once to turn on and off.
	private bool isRecording;

	private void Start()
	{
		audioSource = GetComponent<AudioSource>();
		audioSource.clip = Microphone.Start(null, true, seconds, frequency);
	}

	//Called every Frame
	void Update()
	{
		//Determine state of recording
		if (startRecord)
		{
			isRecording = !isRecording;
			startRecord = false;
			if (isRecording == false)
				RecordAudio();
			else
				audioSource.clip = Microphone.Start(null, true, seconds, frequency);
		}

		if (Input.GetKeyDown("space"))
		{
			startRecord = !startRecord;
		}
		if (Input.GetKeyDown("z"))
		{
			FinalizeRecording();
			audioSource.Play();
		}

	}

	//Records Audio, converts to float, and Adds to tempRecording when done
	private void RecordAudio()
	{
		int length = Microphone.GetPosition(null);
		Microphone.End(null);
		float[] clipData = new float[length];
		audioSource.clip.GetData(clipData, 0);
		float[] fullClip = new float[clipData.Length + tempRecording.Length];
		//Add new recorded data to previous data
		for (int i = 0; i < fullClip.Length; i++)
		{
			if (i < tempRecording.Length)
				fullClip[i] = tempRecording[i];
			else
				fullClip[i] = clipData[i - tempRecording.Length];
		}
		tempRecording = fullClip;
	}

	//Clears tempRecording in order to start a different recording
	public void ClearAudio()
	{
		tempRecording = new float[0];
	}

	//If called, sets tempRecording to a clip in audioSource
	public void FinalizeRecording()
	{
		audioSource.clip = AudioClip.Create("recorded samples", tempRecording.Length * 2, 1, frequency, false);
		audioSource.clip.SetData(tempRecording, 0);
	}

	//TODO
	private float[] editRate(float[] array)
	{
		if (recordRate == 50)
		{
			float[] partialData = new float[array.Length / 2];
			for (int i = 0; i < partialData.Length; i++)
				partialData[i] = array[i * 2];
			return partialData;
		}
		else if (recordRate == 25 && array.Length > 3)
		{
			float[] partialData = new float[array.Length / 4];
			for (int i = 0; i < partialData.Length; i++)
				partialData[i] = array[i * 2];
			return partialData;
		}
		return array;
	}
}

//Update could probably be removed and the functions called accordingly by the GameManager.