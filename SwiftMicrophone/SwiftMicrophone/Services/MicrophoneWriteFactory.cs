using System;
using Concentus;
using Concentus.Enums;
using Euphelia.SwiftMicrophone.Helpers;
using Euphelia.SwiftMicrophone.Models;
using NAudio.Wave;

namespace Euphelia.SwiftMicrophone.Services
{
	public class MicrophoneWriteFactory
	{
		public delegate void DataReceivedDelegate(byte[]     dgram,  int              bytes);
		public delegate void RecordingStoppedDelegate(object sender, StoppedEventArgs e);
		
		private readonly MicrophoneWriteConfigurations _configurations;
		private          WaveInEvent                   _waveIn;
		
		public MicrophoneWriteFactory(MicrophoneWriteConfigurations configurations) => _configurations = configurations;
		
		public event DataReceivedDelegate     DataReceivedEvent;
		public event RecordingStoppedDelegate RecordingStoppedEvent;
		
		public void Start()
		{
			var encoder = OpusCodecFactory.CreateEncoder(_configurations.MaxSampleRate, _configurations.Channels, OpusApplication.OPUS_APPLICATION_VOIP);
			encoder.Bitrate = _configurations.Bitrate;
			
			_waveIn = new()
			{
				DeviceNumber       = _configurations.DeviceNumber,
				WaveFormat         = new(_configurations.MaxSampleRate, MicrophoneWriteConfigurations.BITS, _configurations.Channels),
				BufferMilliseconds = _configurations.FrameDuration
			};
			
			_waveIn.DataAvailable += (sender, e) =>
			{
				if (DataReceivedEvent is null || DataReceivedEvent.GetInvocationList().Length == 0)
					return;
				
				var pcmFloats = MicrophoneBitConverter.ConvertToFloatArray(e.Buffer);
				// Allocate buffer for encoded data with a reasonable maximum size
				var maxPacketSize = 1275 * _configurations.Channels; // Assuming the maximum packet size per frame and number of channels
				var encoded       = new byte[maxPacketSize];
				
				// Encode the PCM data to Opus format
				var length = encoder.Encode(new ReadOnlySpan<float>(pcmFloats, 0, _configurations.FrameSize), _configurations.FrameSize, new(encoded), encoded.Length);
				
				// Invoke the DataReceivedEvent with the encoded data and its actual length
				DataReceivedEvent?.Invoke(encoded, length);
			};
			
			_waveIn.RecordingStopped += (sender, stoppedEventArgs) => RecordingStoppedEvent?.Invoke(sender, stoppedEventArgs);
			_waveIn.StartRecording();
		}
		
		public void Stop() => _waveIn?.StopRecording();
	}
}