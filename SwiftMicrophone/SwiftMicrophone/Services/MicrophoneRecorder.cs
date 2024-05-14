using System;
using System.Linq;
using Concentus.Enums;
using Concentus.Structs;
using Euphelia.SwiftMicrophone.Helpers;
using Euphelia.SwiftMicrophone.Models;
using NAudio.Wave;

namespace Euphelia.SwiftMicrophone.Services
{
	public class MicrophoneRecorder
	{
		public delegate void DataAvailableExceptionDelegate(Exception exception);
		public delegate void DataReceivedDelegate(byte[]              encodedData);
		public delegate void RecordingStoppedDelegate(object          sender, StoppedEventArgs eventArgs);
		
		private readonly MicrophoneRecordConfigurations _configurations;
		private          WaveInEvent                    _waveIn;
		
		public MicrophoneRecorder(MicrophoneRecordConfigurations configurations) => _configurations = configurations;
		
		public event DataReceivedDelegate           DataReceivedEvent;
		public event RecordingStoppedDelegate       RecordingStoppedEvent;
		public event DataAvailableExceptionDelegate IndexOutOfBoundEvent;
		public event DataAvailableExceptionDelegate ExceptionEvent;
		
		public void Start()
		{
			var encoder = OpusEncoder.Create(_configurations.MaxSampleRate, _configurations.Channels, OpusApplication.OPUS_APPLICATION_VOIP);
			encoder.Bitrate = _configurations.Bitrate;
			
			_waveIn = new()
			{
				DeviceNumber       = _configurations.DeviceNumber,
				WaveFormat         = new(_configurations.MaxSampleRate, MicrophoneRecordConfigurations.BITS, _configurations.Channels),
				BufferMilliseconds = _configurations.FrameDuration
			};
			
			_waveIn.DataAvailable += (sender, args) =>
			{
				if (DataReceivedEvent is null || DataReceivedEvent.GetInvocationList().Length == 0)
					return;
				
				try
				{
					var pcmFloats   = AudioConverter.ConvertPcmToFloat(args.Buffer);
					var encodedData = new byte[1275];
					
					var encodedLength = encoder.Encode(pcmFloats,
					                                   0,
					                                   _configurations.FrameSize,
					                                   encodedData,
					                                   0,
					                                   encodedData.Length);
					
					DataReceivedEvent?.Invoke(encodedData.Take(encodedLength).ToArray());
				}
				catch (IndexOutOfRangeException ex)
				{
					IndexOutOfBoundEvent?.Invoke(ex);
				}
				catch (Exception ex)
				{
					ExceptionEvent?.Invoke(ex);
				}
			};
			
			_waveIn.RecordingStopped += (sender, eventArgs) => RecordingStoppedEvent?.Invoke(sender, eventArgs);
			_waveIn.StartRecording();
		}
		
		public void Stop() => _waveIn?.StopRecording();
	}
}