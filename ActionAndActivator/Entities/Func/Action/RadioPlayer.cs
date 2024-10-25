using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR;

namespace Assets.Scripts.Entities.Func.Action
{
	public readonly struct PcmData
	{
		#region Public types & data

		public float[] Value { get; }
		public int Length { get; }
		public int Channels { get; }
		public int SampleRate { get; }

		#endregion

		#region Constructors & Finalizer

		private PcmData(float[] value, int channels, int sampleRate)
		{
			Value = value;
			Length = value.Length;
			Channels = channels;
			SampleRate = sampleRate;
		}



		#endregion
		#region Public Methods

		public static PcmData FromBytes(byte[] bytes)
		{
			if(bytes == null)
			{
				throw new ArgumentNullException(nameof(bytes));
			}

			PcmHeader pcmHeader = PcmHeader.FromBytes(bytes);
			if(pcmHeader.BitDepth != 16 && pcmHeader.BitDepth != 32 && pcmHeader.BitDepth != 8)
			{
				throw new ArgumentOutOfRangeException(nameof(pcmHeader.BitDepth), pcmHeader.BitDepth, "Supported values are: 8, 16, 32");
			}

			float[] samples = new float[pcmHeader.AudioSampleCount];
			for(int i = 0; i < samples.Length; ++i)
			{
				int byteIndex = pcmHeader.AudioStartIndex + i * pcmHeader.AudioSampleSize;
				float rawSample;
				switch(pcmHeader.BitDepth)
				{
					case 8:
						rawSample = bytes[byteIndex];
						break;

					case 16:
						rawSample = BitConverter.ToInt16(bytes, byteIndex);
						break;

					case 32:
						rawSample = BitConverter.ToInt32(bytes, byteIndex);
						break;

					default: throw new ArgumentOutOfRangeException(nameof(pcmHeader.BitDepth), pcmHeader.BitDepth, "Supported values are: 8, 16, 32");
				}

				samples[i] = pcmHeader.NormalizeSample(rawSample); // normalize sample between [-1f, 1f]
			}

			return new PcmData(samples, pcmHeader.Channels, pcmHeader.SampleRate);
		}

		#endregion
	}

	public readonly struct PcmHeader
	{
		#region Public types & data

		public int BitDepth { get; }
		public int AudioSampleSize { get; }
		public int AudioSampleCount { get; }
		public ushort Channels { get; }
		public int SampleRate { get; }
		public int AudioStartIndex { get; }
		public int ByteRate { get; }
		public ushort BlockAlign { get; }

		#endregion

		#region Constructors & Finalizer

		private PcmHeader(int bitDepth,
			int audioSize,
			int audioStartIndex,
			ushort channels,
			int sampleRate,
			int byteRate,
			ushort blockAlign)
		{
			BitDepth = bitDepth;
			_negativeDepth = Mathf.Pow(2f, BitDepth - 1f);
			_positiveDepth = _negativeDepth - 1f;

			AudioSampleSize = bitDepth / 8;
			AudioSampleCount = Mathf.FloorToInt(audioSize / (float)AudioSampleSize);
			AudioStartIndex = audioStartIndex;

			Channels = channels;
			SampleRate = sampleRate;
			ByteRate = byteRate;
			BlockAlign = blockAlign;
		}

		#endregion

		#region Public Methods

		public static PcmHeader FromBytes(byte[] pcmBytes)
		{
			using var memoryStream = new MemoryStream(pcmBytes);
			return FromStream(memoryStream);
		}

		public static PcmHeader FromStream(Stream pcmStream)
		{
			pcmStream.Position = SizeIndex;
			using BinaryReader reader = new BinaryReader(pcmStream);

			int headerSize = reader.ReadInt32();  // 16
			ushort audioFormatCode = reader.ReadUInt16(); // 20

			string audioFormat = GetAudioFormatFromCode(audioFormatCode);
			if(audioFormatCode != 1 && audioFormatCode == 65534)
			{
				// Only uncompressed PCM wav files are supported.
				throw new ArgumentOutOfRangeException(nameof(pcmStream),
													  $"Detected format code '{audioFormatCode}' {audioFormat}, but only PCM and WaveFormatExtensible uncompressed formats are currently supported.");
			}

			ushort channelCount = reader.ReadUInt16(); // 22
			int sampleRate = reader.ReadInt32();  // 24
			int byteRate = reader.ReadInt32();  // 28
			ushort blockAlign = reader.ReadUInt16(); // 32
			ushort bitDepth = reader.ReadUInt16(); //34

			pcmStream.Position = SizeIndex + headerSize + 2 * sizeof(int); // Header end index
			int audioSize = reader.ReadInt32();                            // Audio size index

			return new PcmHeader(bitDepth, audioSize, (int)pcmStream.Position, channelCount, sampleRate, byteRate, blockAlign); // audio start index
		}

		public float NormalizeSample(float rawSample)
		{
			float sampleDepth = rawSample < 0 ? _negativeDepth : _positiveDepth;
			return rawSample / sampleDepth;
		}

		#endregion

		#region Private Methods

		private static string GetAudioFormatFromCode(ushort code)
		{
			switch(code)
			{
				case 1: return "PCM";
				case 2: return "ADPCM";
				case 3: return "IEEE";
				case 7: return "?-law";
				case 65534: return "WaveFormatExtensible";
				default: throw new ArgumentOutOfRangeException(nameof(code), code, "Unknown wav code format.");
			}
		}

		#endregion

		#region Private types & Data

		private const int SizeIndex = 16;

		private readonly float _positiveDepth;
		private readonly float _negativeDepth;

		#endregion
	}

	public class RadioPlayer : MonoBehaviour
	{
		private AudioSource _audioSource;
		public string URL;

		private void Start()
		{
			_audioSource = GetComponent<AudioSource>();
			StartCoroutine(StartPlayer("http://stream.zeno.fm/c6mgmtfg8e9uv"));
		}

		IEnumerator DownloadAndPlay(string url)
		{
			using(UnityWebRequest www = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.AUDIOQUEUE))
			{
				yield return www.SendWebRequest();

				if(www.result == UnityWebRequest.Result.ConnectionError)
				{
					Debug.Log(www.error);
				}
				else
				{
					AudioClip myClip = DownloadHandlerAudioClip.GetContent(www);
					_audioSource.clip = myClip;
					_audioSource.Play();
				}
			}
		}

		private IEnumerator StartPlayer(string url)
		{
			using(var webRequest = UnityWebRequestMultimedia.GetAudioClip(url, AudioType.WAV))
			{
				((DownloadHandlerAudioClip)webRequest.downloadHandler).streamAudio = true;

				webRequest.SendWebRequest();
				while(!webRequest.isNetworkError && webRequest.downloadedBytes < 16096)
				{
					Debug.Log(webRequest.downloadProgress);
					yield return new WaitForFixedUpdate();
				}

				if(webRequest.isNetworkError)
				{
					Debug.LogError(webRequest.error);
					yield break;
				}

				Debug.Log($"size: {webRequest.downloadedBytes}");


				var brs = webRequest.downloadHandler.data;

				using(var file= File.Open("test.wav", FileMode.OpenOrCreate))
				{
					file.Write(brs, 0, brs.Length);
				}

				BytesToAudioClip(brs);

				//var clip = ((DownloadHandlerAudioClip)webRequest.downloadHandler).audioClip;
				//_audioSource.clip = audioClip;
				_audioSource.Play();

			}
		}

		public int position = 0;
		public int samplerate = 44100;
		public float frequency = 320;

		void OnAudioRead(float[] data)
		{
			int count = 0;
			while(count < data.Length)
			{
				data[count] = Mathf.Sin(2 * Mathf.PI * frequency * position / samplerate);
				position++;
				count++;
			}
		}

		void OnAudioSetPosition(int newPosition)
		{
			position = newPosition;
		}

		public void BytesToAudioClip(byte[] data)
		{
			//var clip = WavUtility.ToAudioClip(data);
			//_audioSource.clip = clip;

			//var pcmData = PcmData.FromBytes(data);
			//audioClip.SetData(pcmData.Value, 0);

			//_audioSource.clip = AudioClip.Create("audioClip", samplerate * 2, 1, samplerate, true, OnAudioRead, OnAudioSetPosition);
			float[] _clipData = bytesToFloat(data);
			var audioClip = AudioClip.Create("test", _clipData.Length, 2, 44100, false);
			_audioSource.clip = audioClip;
			_audioSource.clip.SetData(_clipData, 0);
		}
		public static float[] bytesToFloat(byte[] byteArray)// byte [] array converts to ADioClip readable float [] type
		{
			float[] sounddata = new float[byteArray.Length / 2];
			for(int i = 0; i < sounddata.Length; i++)
			{
				sounddata[i] = bytesToFloat(byteArray[i * 2], byteArray[i * 2 + 1]);
			}
			return sounddata;
		}
		static float bytesToFloat(byte firstByte, byte secondByte)
		{
			// Small end and large end order to be adjusted
			short s;
			if(BitConverter.IsLittleEndian)
				s = (short)((secondByte << 8) | firstByte);
			else
				s = (short)((firstByte << 8) | secondByte);
			return s / 32768.0F;
		}
	}
}
