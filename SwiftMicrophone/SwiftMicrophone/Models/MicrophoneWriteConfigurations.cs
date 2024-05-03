using System;
using System.Linq;
using Euphelia.SwiftMicrophone.Enums;
using Euphelia.SwiftMicrophone.Extensions;
using Euphelia.SwiftMicrophone.Helpers.Global;
using NAudio;
using NAudio.Wave;

namespace Euphelia.SwiftMicrophone.Models
{
	public readonly struct MicrophoneWriteConfigurations
	{
		public const int BITS = 16;
		
		public MicrophoneWriteConfigurations
		(
				int           deviceNumber = 0,
				FrameDuration duration     = Enums.FrameDuration.Normal,
				Bitrate       bitrate      = Enums.Bitrate.Normal,
				Channels?     channels     = null
		)
		{
			duration.ThrowInvalidEnumValue($"Invalid ${nameof(duration)} passed to {nameof(MicrophoneWriteConfigurations)}.");
			bitrate.ThrowInvalidEnumValue($"Invalid ${nameof(bitrate)} passed to {nameof(MicrophoneWriteConfigurations)}.");
			
			DeviceNumber  = deviceNumber;
			FrameDuration = (int)duration;
			Bitrate       = (int)bitrate;
			
			var capabilities = GetCapabilities(deviceNumber);
			Channels      = GetChannels(channels, capabilities);
			MaxSampleRate = GetMaxSupportedSampleRate(capabilities, Channels);
			FrameSize     = CalculateFrameSize(MaxSampleRate, FrameDuration);
		}
		
		public int DeviceNumber  { get; }
		public int FrameDuration { get; }
		public int Bitrate       { get; }
		public int Channels      { get; }
		public int MaxSampleRate { get; }
		public int FrameSize     { get; }
		
		
		private static WaveInCapabilities GetCapabilities(int deviceNumber)
		{
			try
			{
				return WaveIn.GetCapabilities(deviceNumber);
			}
			catch (MmException)
			{
				throw new Exception($"Invalid {deviceNumber} passed to {nameof(MicrophoneWriteConfigurations)}.");
			}
		}
		
		private static int GetChannels(Channels? channels, WaveInCapabilities capabilities)
		{
			if (channels is null)
				return capabilities.Channels;
			
			channels.Value.ThrowInvalidEnumValue($"Invalid ${nameof(channels)} passed to {nameof(MicrophoneWriteConfigurations)}.");
			var channelsInt = (int)channels;
			if (channelsInt > capabilities.Channels)
				throw new Exception($"Invalid ${nameof(channels)} passed to {nameof(MicrophoneWriteConfigurations)}. Maximum amount is {capabilities.Channels}");
			
			return channelsInt;
		}
		
		private static int GetMaxSupportedSampleRate
				(WaveInCapabilities capabilities, int channels) => EnumHelper.GetAllValues<SampleRate>().Where(rate => IsSupportedSampleRate(rate, capabilities, channels)).Select(e => (int)e).Max();
		
		private static bool IsSupportedSampleRate
				(SampleRate rate, WaveInCapabilities capabilities, int channels) => IsMono(channels) ? IsSupportedMonoSampleRate(rate, capabilities) : IsSupportedStereoSampleRate(rate, capabilities);
		
		private static bool IsSupportedStereoSampleRate(SampleRate rate, WaveInCapabilities capabilities)
		{
			switch (rate)
			{
				case SampleRate.Rate8K:
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48S08);
				case SampleRate.Rate12K:
					// Opus supports 12K, but NAudio might not explicitly, so check closest available formats
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1S08);
				case SampleRate.Rate16K:
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1S16);
				case SampleRate.Rate24K:
					// Opus supports 24K, but NAudio might not explicitly, so check closest available formats
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_2S16);
				case SampleRate.Rate48K:
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48S16);
				default:
					return false;
			}
		}
		
		private static bool IsSupportedMonoSampleRate(SampleRate rate, WaveInCapabilities capabilities)
		{
			switch (rate)
			{
				case SampleRate.Rate8K:
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48M08);
				case SampleRate.Rate12K:
					// Opus supports 12K, but NAudio might not explicitly, so check closest available formats
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1M08);
				case SampleRate.Rate16K:
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1M16);
				case SampleRate.Rate24K:
					// Opus supports 24K, but NAudio might not explicitly, so check closest available formats
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_2M16);
				case SampleRate.Rate48K:
					return capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48M16);
				default:
					return false;
			}
		}
		
		private static bool IsMono(int channels) => (Channels)channels == Enums.Channels.Mono;
		
		private static int CalculateFrameSize(int maxSampleRate, int frameDuration) => maxSampleRate / 1000 * frameDuration;
	}
}