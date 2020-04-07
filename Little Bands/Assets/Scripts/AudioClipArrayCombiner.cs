using System;
using System.IO;
using System.Text;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

// Part of this script, basically the exporting math bits, were derived from Gregorio Zanon's script
// http://forum.unity3d.com/threads/119295-Writing-AudioListener.GetOutputData-to-wav-problem?p=806734&viewfull=1#post806734
// https://gist.github.com/darktable/2317063
//
// The rest is provided by S.F. Bay Studios, Inc (http://www.InfinityPBR.com).

public class AudioClipArrayCombiner : MonoBehaviour
{
	[ContextMenuItem("Export Combined Audio Clips", "SaveNow")]
	public string outputName;
	public AudioLayer[] audioLayers;
	[HideInInspector]
	public bool displayInspector = false;

	static float rescaleFactor = 32767; //to convert float to Int16

	const int HEADER_SIZE = 44;

	public class AudioLayer
	{
		//audioLayers[0]= ;
		public AudioClip[] clip;
		[HideInInspector]
		public bool expanded = false;
		[HideInInspector]
		public int clipNumber = 0;
		[HideInInspector]
		public string name = "No Clips Added";
		public float volume = 1;
		public float delay;
		[HideInInspector]
		public Int16[] samples;
		[HideInInspector]
		public Byte[] bytes;
		[HideInInspector]
		public int sampleCount;
		[HideInInspector]
		public int delayCount;
		[HideInInspector]
		public int onClip = 0;

		public void GetSamples(int clipNumber)
		{
			samples = GetSamplesFromClip(clip[clipNumber], volume);
			delayCount = (int)(delay * clip[clipNumber].frequency * clip[clipNumber].channels);
			sampleCount = delayCount + samples.Length;
		}
	}

	public void DeleteFile(string userFolder, string songFolder, string foldername)
	{
		DirectoryInfo dataDir = new DirectoryInfo(Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + foldername);
		dataDir.Delete(true);
	}

	public void CombineFiles(string userFolder, String songFolder)
	{

		Debug.Log(Application.persistentDataPath);
		int newLength = 5;
		AudioLayer myobj1 = new AudioLayer();
		AudioLayer myobj2 = new AudioLayer();
		AudioLayer myobj3 = new AudioLayer();
		AudioLayer myobj4 = new AudioLayer();
		AudioLayer myobj5 = new AudioLayer();
		audioLayers = new AudioLayer[newLength];                    // Create a new array

		audioLayers[0] = myobj1;
		audioLayers[1] = myobj2;
		audioLayers[2] = myobj3;
		audioLayers[3] = myobj4;
		audioLayers[4] = myobj5;

		myobj1.clip = new AudioClip[1];
		myobj2.clip = new AudioClip[1];
		myobj3.clip = new AudioClip[1];
		myobj4.clip = new AudioClip[1];
		myobj5.clip = new AudioClip[1];

		// Find total number of exports
		int totalExports = 1;                                                // Start at 1...

        string folderName1 = "guitar";
        string filename1 = userFolder + "_" + songFolder + "_guitar.wav";
		string folderName2 = "bass";
		string filename2 = userFolder + "_" + songFolder + "_bass.wav";
		string folderName3 = "piano";
		string filename3 = userFolder + "_" + songFolder + "_piano.wav";
		string folderName4 = "drums";
		string filename4 = userFolder + "_" + songFolder + "_drums.wav";
		string folderName5 = "voice";
		string filename5 = userFolder + "_" + songFolder + "_voice.wav";

		string filename = "";
		//var filename1 = "song1.wav";
		//var filename2 = "song2.wav";
		//var filename1 = "Axe_On_Flesh_Axe_1.wav";
		//var filename2 = "Axe_On_Flesh_Axe_2.wav";
		//var filename1 = "song1.wav";
		//var filename2 = "song2.wav";
		//var filename1 = "Axe_On_Flesh_Axe_1.wav";
		//var filename2 = "Axe_On_Flesh_Axe_2.wav";
		//var filename3 = "Axe_On_Flesh_Flesh_1.wav";
		//var filename4 = "Axe_On_Flesh_Flesh_2.wav";
		//var filename5 = "Axe_On_Flesh_Flesh_3.wav";

		//used to load files from persistentDataPath 
		AudioClip clip1 = ToAudioClip(Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName1 + "/" + filename1);
		AudioClip clip2 = ToAudioClip(Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName2 + "/" + filename2);
		AudioClip clip3 = ToAudioClip(Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName3 + "/" + filename3);
		AudioClip clip4 = ToAudioClip(Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName4 + "/" + filename4);
		AudioClip clip5 = ToAudioClip(Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName5 + "/" + filename5);

		//used to load files from Asset folder in Unity
		//AudioClip clip1 = (AudioClip)Resources.Load(filename1);
		//AudioClip clip2 = (AudioClip)Resources.Load(filename2);
		//AudioClip clip3 = (AudioClip)Resources.Load(filename3);
		//AudioClip clip4 = (AudioClip)Resources.Load(filename4);
		//AudioClip clip5 = (AudioClip)Resources.Load(filename5);

		myobj1.clip[0] = clip1;
		myobj2.clip[0] = clip2;
		myobj3.clip[0] = clip3;
		myobj4.clip[0] = clip4;
		myobj5.clip[0] = clip5;

		for (int n = 0; n < audioLayers.Length; n++)
		{
			totalExports *= audioLayers[n].clip.Length;                     // Multiply by the number of clips in each layer
		}

		if (totalExports > 0)
		{
			float progressPercent = 0.0f;
			int clipCount = 0;
			string[] combinations;                                                  // Start an array of all combinations
			combinations = new string[totalExports];                                // Set the number of entries to the number of exports

			// Reset the onClip value for each layer
			for (int r = 0; r < audioLayers.Length; r++)
			{
				audioLayers[r].onClip = 0;
			}

			for (int l = 0; l < audioLayers.Length; l++)
			{                           // For each layer...
				int exportsLeft = 1;                                                // Start at 1...
				for (int i = l; i < audioLayers.Length; i++)
				{                           // For each layer left in the list (don't compute those we've already done)
					exportsLeft *= audioLayers[i].clip.Length;                  // Find out how many exports are left if it were just those layers
				}

				int entriesPerValue = exportsLeft / audioLayers[l].clip.Length; // Compute how many entires per value, if the total entries were exportsLeft
				int entryCount = 0;                                                 // Set entryCount to 0

				for (int e = 0; e < combinations.Length; e++)
				{                       // For all combinations
					if (l != 0)                                                     // If this isn't the first layer
						combinations[e] = combinations[e] + ",";                    // Append a "," to the String
					combinations[e] = combinations[e] + audioLayers[l].onClip;  // Append the "onClip" value to the string
					entryCount++;                                                   // increase entryCount
					if (entryCount >= entriesPerValue)
					{                           // if we've done all the entires for that "onClip" value...
						audioLayers[l].onClip++;                                    // increase onClip by 1
						entryCount = 0;                                             // Reset entryCount
						if (audioLayers[l].onClip >= audioLayers[l].clip.Length)    // if we've also run out of clips for this layer
							audioLayers[l].onClip = 0;                              // Reset onClip count
					}
				}
			}

			int number = 0;                                                         // for the file name
																					// For each combination, save a .wav file with those clip numbers.
			foreach (var combination in combinations)
			{
				clipCount++;
				//progressPercent = clipCount / totalExports * 1.0f;
				progressPercent = clipCount / (float)totalExports;
				//Debug.Log ("progressPercent: " + progressPercent);
				string[] clipsAsString = combination.Split(","[0]);
				SaveClip(userFolder, songFolder, filename, number, clipsAsString, audioLayers);
				number++;
			}
		}
		else
		{
			Debug.Log("Nothing To Export! (or maybe a layer is missing clips?)");
		}
	}

	public void SaveNow(AudioClip clipToSave, string userFolder, string songFolder, string filename)
	{

		Debug.Log(Application.persistentDataPath);
		int newLength = 1;
		AudioLayer myobj1 = new AudioLayer();
		audioLayers = new AudioLayer[newLength];                    // Create a new array

		audioLayers[0] = myobj1;

		myobj1.clip = new AudioClip[1];

		// Find total number of exports
		int totalExports = 1;                                                // Start at 1...

		//used to load files from persistentDataPath 
		AudioClip clip1 = clipToSave;

		myobj1.clip[0] = clip1;

		for (int n = 0; n < audioLayers.Length; n++)
		{
			totalExports *= audioLayers[n].clip.Length;                     // Multiply by the number of clips in each layer
		}

		if (totalExports > 0)
		{
			float progressPercent = 0.0f;
			int clipCount = 0;
			string[] combinations;                                                  // Start an array of all combinations
			combinations = new string[totalExports];                                // Set the number of entries to the number of exports

			// Reset the onClip value for each layer
			for (int r = 0; r < audioLayers.Length; r++)
			{
				audioLayers[r].onClip = 0;
			}

			for (int l = 0; l < audioLayers.Length; l++)
			{                           // For each layer...
				int exportsLeft = 1;                                                // Start at 1...
				for (int i = l; i < audioLayers.Length; i++)
				{                           // For each layer left in the list (don't compute those we've already done)
					exportsLeft *= audioLayers[i].clip.Length;                  // Find out how many exports are left if it were just those layers
				}

				int entriesPerValue = exportsLeft / audioLayers[l].clip.Length; // Compute how many entires per value, if the total entries were exportsLeft
				int entryCount = 0;                                                 // Set entryCount to 0

				for (int e = 0; e < combinations.Length; e++)
				{                       // For all combinations
					if (l != 0)                                                     // If this isn't the first layer
						combinations[e] = combinations[e] + ",";                    // Append a "," to the String
					combinations[e] = combinations[e] + audioLayers[l].onClip;  // Append the "onClip" value to the string
					entryCount++;                                                   // increase entryCount
					if (entryCount >= entriesPerValue)
					{                           // if we've done all the entires for that "onClip" value...
						audioLayers[l].onClip++;                                    // increase onClip by 1
						entryCount = 0;                                             // Reset entryCount
						if (audioLayers[l].onClip >= audioLayers[l].clip.Length)    // if we've also run out of clips for this layer
							audioLayers[l].onClip = 0;                              // Reset onClip count
					}
				}
			}

			int number = 0;                                                         // for the file name
																					// For each combination, save a .wav file with those clip numbers.
			foreach (var combination in combinations)
			{
				clipCount++;
				//progressPercent = clipCount / totalExports * 1.0f;
				progressPercent = clipCount / (float)totalExports;
				//Debug.Log ("progressPercent: " + progressPercent);
				string[] clipsAsString = combination.Split(","[0]);
				SaveClip(userFolder, songFolder, filename, number, clipsAsString, audioLayers);
				number++;
			}
		}
		else
		{
			Debug.Log("Nothing To Export! (or maybe a layer is missing clips?)");
		}
	}


	public static bool SaveClip(string userFolder, string songFolder, string filename, int exportNumber, string[] clipsAsString, AudioLayer[] audioLayers)
	{
		//Debug.Log ("Doing Export " + exportNumber);
		var filepath="";
        if (filename.Length <= 0)                                                           // If the name hasn't been set
		{
			filename = userFolder + "_" + songFolder;                                      // Use a default name
			string folderName = filename;
			filename += ".wav";
			filepath = Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName + "/" + filename;
		}
		else
		{                                                                               // else
			string folderName = filename;
			filename = userFolder + "_" + songFolder + "_" + filename;                  // Use the chosen name plus the number
			filename += ".wav";
			filepath = Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName + "/" + filename;
		}
		//filename += ".wav";                                                                 // add the .wav extension

		//USED IF SAVED WITHIN ASSET FOLDER
		//var filepath	= "Assets/SFBayStudios/Exported Audio Files/" + filename;           // Set the file path
		//var filepath = Application.persistentDataPath + "/" + userFolder + "/" + songFolder + "/" + folderName + "/" + filename;                     // Set the file path

		// Make sure directory exists if user is saving to sub dir.
		Directory.CreateDirectory(Path.GetDirectoryName(filepath));

		using (var fileStream = CreateEmpty(filepath))                                      // Create an empty file
		{
			int sampleCount = ConvertAndWrite(fileStream, clipsAsString, audioLayers);

			//	 ClIP NUMBER CHANGE HERE
			WriteHeader(fileStream, audioLayers[0].clip[0], sampleCount);
		}
		//USED IF AUDIO FILE IS SAVED IN ASSET FOLDER IN UNITY
		//AssetDatabase.ImportAsset(filepath);
		return true; // TODO: return false if there's a failure saving the file
	}

	static int ConvertAndWrite(FileStream fileStream, String[] clipsAsString, AudioLayer[] audioLayers)
	{
		int mostSamples = 0;                                                                // Set this to 0
																							//Debug.Log("audioLayers Lenght: " + audioLayers.Length);
		for (int c = 0; c < audioLayers.Length; c++)
		{                                       // For each Layer
			int clipNumber = int.Parse(clipsAsString[c]);                                   // Get the clip number as an int
			audioLayers[c].GetSamples(clipNumber);                                          // Run this function from the class
			mostSamples = Mathf.Max(mostSamples, audioLayers[c].sampleCount);               // Set mostSamples to the greatest one
		}



		Int16[] finalSamples = new Int16[mostSamples];                                      // The exported clip will have the mostSamples
		float[] sampleValues = new float[mostSamples];

		for (int i = 0; i < mostSamples; i++)                                               // for each sample
		{
			float sampleValue = 0;                                                          // Set variable for exported clip
			int sampleCount = 0;                                                            // Set variable

			foreach (var audioLayer in audioLayers)                                         // For each layer....
			{
				if (i > audioLayer.delayCount && i < audioLayer.sampleCount)                    // if we are not in the delay range and we are under the samplecount for the clip
				{
					// Add the value from this layer to the final (sampleValue)
					sampleValue += (audioLayer.samples[i - audioLayer.delayCount] / rescaleFactor);
					sampleCount++;
				}
			}

			sampleValues[i] += sampleValue;

			//if(sampleCount!=0)																// If we have done some samples (keep from dividing by 0)
			//    sampleValue /= sampleCount;													// compute sampleValue

		}


		float highSample = 0.0f;                                                                            // Variable for the highest sample
		float lowSample = 0.0f;                                                                             // Variable for the lowest sample
		for (int h = 0; h < mostSamples; h++)
		{                                                               // For each sample...
			highSample = Mathf.Max(highSample, sampleValues[h]);                                            // Compute the highest sample
			lowSample = Mathf.Min(lowSample, sampleValues[h]);                                          // Compute the lowest sample
		}
		float parameter = Mathf.InverseLerp(0.0f, Mathf.Max(highSample, lowSample * -1), 1.0f);             // Find the amount we need to multiply each sample by, based on the most extreme sample (high or low)

		for (int p = 0; p < mostSamples; p++)
		{                                                               // For each sample...
			sampleValues[p] *= parameter;                                                                   // Multiply the value by the parameter value															// Adjust the volume
		}

		for (int i2 = 0; i2 < mostSamples; i2++)
		{                                                           // For each sample...
			finalSamples[i2] = (short)(sampleValues[i2] * rescaleFactor);                                   // Finalize the value
		}
		sampleValues = new float[0];                                                                        // Clear this data

		Byte[] bytesData = ConvertSamplesToBytes(finalSamples);
		fileStream.Write(bytesData, 0, bytesData.Length);

		return mostSamples;
	}


	static Byte[] ConvertSamplesToBytes(Int16[] samples)
	{
		Byte[] bytesData = new Byte[samples.Length * 2];
		for (int i = 0; i < samples.Length; i++)
		{
			Byte[] byteArr = new Byte[2];
			byteArr = BitConverter.GetBytes(samples[i]);
			byteArr.CopyTo(bytesData, i * 2);
		}
		return bytesData;
	}


	static Int16[] GetSamplesFromClip(AudioClip clip, float volume = 1)
	{
		//Debug.Log ("Getting Samples from clip " + clip.name);
		var samples = new float[clip.samples * clip.channels];
		//Debug.Log ("Samples: " + samples.Length);

		clip.GetData(samples, 0);

		Int16[] intData = new Int16[samples.Length];

		for (int i = 0; i < samples.Length; i++)
		{
			intData[i] = (short)(samples[i] * volume * rescaleFactor);
		}
		return intData;
	}



	static void WriteHeader(FileStream fileStream, AudioClip clip, int sampleCount)
	{
		var frequency = clip.frequency;
		var channelCount = clip.channels;

		fileStream.Seek(0, SeekOrigin.Begin);

		Byte[] riff = System.Text.Encoding.UTF8.GetBytes("RIFF");
		fileStream.Write(riff, 0, 4);

		Byte[] chunkSize = BitConverter.GetBytes(fileStream.Length - 8);
		fileStream.Write(chunkSize, 0, 4);

		Byte[] wave = System.Text.Encoding.UTF8.GetBytes("WAVE");
		fileStream.Write(wave, 0, 4);

		Byte[] fmt = System.Text.Encoding.UTF8.GetBytes("fmt ");
		fileStream.Write(fmt, 0, 4);

		Byte[] subChunk1 = BitConverter.GetBytes(16);
		fileStream.Write(subChunk1, 0, 4);

		//UInt16 two = 2;
		UInt16 one = 1;

		Byte[] audioFormat = BitConverter.GetBytes(one);
		fileStream.Write(audioFormat, 0, 2);

		Byte[] numChannels = BitConverter.GetBytes(channelCount);
		fileStream.Write(numChannels, 0, 2);

		Byte[] sampleRate = BitConverter.GetBytes(frequency);
		fileStream.Write(sampleRate, 0, 4);

		Byte[] byteRate = BitConverter.GetBytes(frequency * channelCount * 2); // sampleRate * bytesPerSample*number of channels, here 44100*2*2
		fileStream.Write(byteRate, 0, 4);

		UInt16 blockAlign = (ushort)(channelCount * 2);
		fileStream.Write(BitConverter.GetBytes(blockAlign), 0, 2);

		UInt16 bps = 16;
		Byte[] bitsPerSample = BitConverter.GetBytes(bps);
		fileStream.Write(bitsPerSample, 0, 2);

		Byte[] datastring = System.Text.Encoding.UTF8.GetBytes("data");
		fileStream.Write(datastring, 0, 4);

		//Byte[] subChunk2 = BitConverter.GetBytes(sampleCount * channelCount * 2);
		Byte[] subChunk2 = BitConverter.GetBytes(sampleCount * channelCount * 1);
		fileStream.Write(subChunk2, 0, 4);

		//		fileStream.Close();
	}

	static FileStream CreateEmpty(string filepath)
	{
		var fileStream = new FileStream(filepath, FileMode.Create);
		byte emptyByte = new byte();

		for (int i = 0; i < HEADER_SIZE; i++) //preparing the header
		{
			fileStream.WriteByte(emptyByte);
		}

		return fileStream;
	}











	// Force save as 16-bit .wav
	//const int BlockSize_16Bit = 2;

	/// <summary>
	/// Load PCM format *.wav audio file (using Unity's Application data path) and convert to AudioClip.
	/// </summary>
	/// <returns>The AudioClip.</returns>
	/// <param name="filePath">Local file path to .wav file</param>
	public AudioClip ToAudioClip(string filePath)
	{
		if (!filePath.StartsWith(Application.persistentDataPath) && !filePath.StartsWith(Application.dataPath))
		{
			Debug.LogWarning("This only supports files that are stored using Unity's Application data path. \nTo load bundled resources use 'Resources.Load(\"filename\") typeof(AudioClip)' method. \nhttps://docs.unity3d.com/ScriptReference/Resources.Load.html");
			return null;
		}
		byte[] fileBytes = File.ReadAllBytes(filePath);
		return ToAudioClip(fileBytes, 0);
	}

	public static AudioClip ToAudioClip(byte[] fileBytes, int offsetSamples = 0, string name = "wav")
	{
		//string riff = Encoding.ASCII.GetString (fileBytes, 0, 4);
		//string wave = Encoding.ASCII.GetString (fileBytes, 8, 4);
		int subchunk1 = BitConverter.ToInt32(fileBytes, 16);
		UInt16 audioFormat = BitConverter.ToUInt16(fileBytes, 20);

		// NB: Only uncompressed PCM wav files are supported.
		string formatCode = FormatCode(audioFormat);
		Debug.AssertFormat(audioFormat == 1 || audioFormat == 65534, "Detected format code '{0}' {1}, but only PCM and WaveFormatExtensable uncompressed formats are currently supported.", audioFormat, formatCode);

		UInt16 channels = BitConverter.ToUInt16(fileBytes, 22);
		int sampleRate = BitConverter.ToInt32(fileBytes, 24);
		//int byteRate = BitConverter.ToInt32 (fileBytes, 28);
		//UInt16 blockAlign = BitConverter.ToUInt16 (fileBytes, 32);
		UInt16 bitDepth = BitConverter.ToUInt16(fileBytes, 34);

		int headerOffset = 16 + 4 + subchunk1 + 4;
		int subchunk2 = BitConverter.ToInt32(fileBytes, headerOffset);
		//Debug.LogFormat ("riff={0} wave={1} subchunk1={2} format={3} channels={4} sampleRate={5} byteRate={6} blockAlign={7} bitDepth={8} headerOffset={9} subchunk2={10} filesize={11}", riff, wave, subchunk1, formatCode, channels, sampleRate, byteRate, blockAlign, bitDepth, headerOffset, subchunk2, fileBytes.Length);

		float[] data;
		switch (bitDepth)
		{
			case 8:
				data = Convert8BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			case 16:
				data = Convert16BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			case 24:
				data = Convert24BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			case 32:
				data = Convert32BitByteArrayToAudioClipData(fileBytes, headerOffset, subchunk2);
				break;
			default:
				throw new Exception(bitDepth + " bit depth is not supported.");
		}

		AudioClip audioClip = AudioClip.Create(name, data.Length / channels, (int)channels, sampleRate, false);
		audioClip.SetData(data, 0);
		return audioClip;
	}

	#region wav file bytes to Unity AudioClip conversion methods

	private static float[] Convert8BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 8-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		float[] data = new float[wavSize];

		sbyte maxValue = sbyte.MaxValue;

		int i = 0;
		while (i < wavSize)
		{
			data[i] = (float)source[i] / maxValue;
			++i;
		}

		return data;
	}

	private static float[] Convert16BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		//int wavSize = BitConverter.ToInt32(source, headerOffset);
		int wavSize = source.Length;
		headerOffset += sizeof(int);
		int x = sizeof(Int16); // block size = 2
		int convertedSize = (wavSize / x) - headerOffset;

		float[] data = new float[convertedSize];

		Int16 maxValue = Int16.MaxValue;

		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			data[i] = (float)BitConverter.ToInt16(source, offset) / maxValue;
			++i;
		}

		Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	private static float[] Convert24BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 24-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = 3; // block size = 3
		int convertedSize = wavSize / x;

		int maxValue = Int32.MaxValue;

		float[] data = new float[convertedSize];

		byte[] block = new byte[sizeof(int)]; // using a 4 byte block for copying 3 bytes, then copy bytes with 1 offset

		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			Buffer.BlockCopy(source, offset, block, 1, x);
			data[i] = (float)BitConverter.ToInt32(block, 0) / maxValue;
			++i;
		}

		Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	private static float[] Convert32BitByteArrayToAudioClipData(byte[] source, int headerOffset, int dataSize)
	{
		int wavSize = BitConverter.ToInt32(source, headerOffset);
		headerOffset += sizeof(int);
		Debug.AssertFormat(wavSize > 0 && wavSize == dataSize, "Failed to get valid 32-bit wav size: {0} from data bytes: {1} at offset: {2}", wavSize, dataSize, headerOffset);

		int x = sizeof(float); //  block size = 4
		int convertedSize = wavSize / x;

		Int32 maxValue = Int32.MaxValue;

		float[] data = new float[convertedSize];

		int offset = 0;
		int i = 0;
		while (i < convertedSize)
		{
			offset = i * x + headerOffset;
			data[i] = (float)BitConverter.ToInt32(source, offset) / maxValue;
			++i;
		}

		Debug.AssertFormat(data.Length == convertedSize, "AudioClip .wav data is wrong size: {0} == {1}", data.Length, convertedSize);

		return data;
	}

	#endregion

	/// <summary>
	/// Calculates the bit depth of an AudioClip
	/// </summary>
	/// <returns>The bit depth. Should be 8 or 16 or 32 bit.</returns>
	/// <param name="audioClip">Audio clip.</param>
	public static UInt16 BitDepth(AudioClip audioClip)
	{
		UInt16 bitDepth = Convert.ToUInt16(audioClip.samples * audioClip.channels * audioClip.length / audioClip.frequency);
		Debug.AssertFormat(bitDepth == 8 || bitDepth == 16 || bitDepth == 32, "Unexpected AudioClip bit depth: {0}. Expected 8 or 16 or 32 bit.", bitDepth);
		return bitDepth;
	}

	private static int BytesPerSample(UInt16 bitDepth)
	{
		return bitDepth / 8;
	}

	private static int BlockSize(UInt16 bitDepth)
	{
		switch (bitDepth)
		{
			case 32:
				return sizeof(Int32); // 32-bit -> 4 bytes (Int32)
			case 16:
				return sizeof(Int16); // 16-bit -> 2 bytes (Int16)
			case 8:
				return sizeof(sbyte); // 8-bit -> 1 byte (sbyte)
			default:
				throw new Exception(bitDepth + " bit depth is not supported.");
		}
	}

	private static string FormatCode(UInt16 code)
	{
		switch (code)
		{
			case 1:
				return "PCM";
			case 2:
				return "ADPCM";
			case 3:
				return "IEEE";
			case 7:
				return "μ-law";
			case 65534:
				return "WaveFormatExtensable";
			default:
				Debug.LogWarning("Unknown wav code format:" + code);
				return "";
		}
	}



}