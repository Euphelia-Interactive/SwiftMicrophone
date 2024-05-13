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
			
			_waveIn = new WaveInEvent
			{
				DeviceNumber       = _configurations.DeviceNumber,
				WaveFormat         = new WaveFormat(_configurations.MaxSampleRate, MicrophoneWriteConfigurations.BITS, _configurations.Channels),
				BufferMilliseconds = _configurations.FrameDuration
			};
			
			_waveIn.DataAvailable += (sender, e) =>
			{
				// if (DataReceivedEvent is null || DataReceivedEvent.GetInvocationList().Length == 0)
				// 	return;
				
				var pcmFloats = MicrophoneBitConverter.ConvertToFloatArray(e.Buffer);
				var encoded   = new byte[1275]; // Maximum possible Opus packet size
				var length    = encoder.Encode(new ReadOnlySpan<float>(pcmFloats, 0, _configurations.FrameSize), _configurations.FrameSize, new Span<byte>(encoded), encoded.Length);
				DataReceivedEvent?.Invoke(encoded, length);
			};
			_waveIn.RecordingStopped += (sender, stoppedEventArgs) => RecordingStoppedEvent?.Invoke(sender, stoppedEventArgs);
			_waveIn.StartRecording();
		}
		
		public void Stop() => _waveIn?.StopRecording();
	}
}