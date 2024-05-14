using System.Linq;
using Euphelia.SwiftMicrophone.Enums;
using Euphelia.SwiftMicrophone.Extensions;
using Euphelia.SwiftMicrophone.Helpers.Global;
using NAudio;
using NAudio.Wave;

namespace Euphelia.SwiftMicrophone.Models
{
	public readonly struct MicrophoneRecordConfigurations
	{
		public const  int                            BITS = 16;
		public static MicrophoneRecordConfigurations Default => new(0, Enums.FrameDuration.Normal, Enums.Bitrate.Normal, null);
		
		public MicrophoneRecordConfigurations
		(
				int           deviceNumber,
				FrameDuration duration,
				Bitrate       bitrate,
				Channels?     channels
		)
		{
			duration.ThrowInvalidEnumValue($"Invalid ${nameof(duration)} passed to {nameof(MicrophoneRecordConfigurations)}.");
			bitrate.ThrowInvalidEnumValue($"Invalid ${nameof(bitrate)} passed to {nameof(MicrophoneRecordConfigurations)}.");
			
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
				throw new($"Invalid {deviceNumber} passed to {nameof(MicrophoneRecordConfigurations)}.");
			}
		}
		
		private static int GetChannels(Channels? channels, WaveInCapabilities capabilities)
		{
			if (channels is null)
				return capabilities.Channels;
			
			channels.Value.ThrowInvalidEnumValue($"Invalid ${nameof(channels)} passed to {nameof(MicrophoneRecordConfigurations)}.");
			var channelsInt = (int)channels;
			if (channelsInt > capabilities.Channels)
				throw new($"Invalid ${nameof(channels)} passed to {nameof(MicrophoneRecordConfigurations)}. Maximum amount is {capabilities.Channels}");
			
			return channelsInt;
		}
		
		private static int GetMaxSupportedSampleRate
				(WaveInCapabilities capabilities, int channels) => EnumHelper.GetAllValues<SampleRate>().Where(rate => IsSupportedSampleRate(rate, capabilities, channels)).Select(e => (int)e).Max();
		
		private static bool IsSupportedSampleRate
				(SampleRate rate, WaveInCapabilities capabilities, int channels) => IsMono(channels) ? IsSupportedMonoSampleRate(rate, capabilities) : IsSupportedStereoSampleRate(rate, capabilities);
		
		private static bool IsSupportedStereoSampleRate(SampleRate rate, WaveInCapabilities capabilities) => rate switch
		{
			SampleRate.Rate8K => capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48S08),
			SampleRate.Rate12K =>
					// Opus supports 12K, but NAudio might not explicitly, so check closest available formats
					capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1S08),
			SampleRate.Rate16K => capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1S16),
			SampleRate.Rate24K =>
					// Opus supports 24K, but NAudio might not explicitly, so check closest available formats
					capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_2S16),
			SampleRate.Rate48K => capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48S16),
			_                  => false
		};
		
		private static bool IsSupportedMonoSampleRate(SampleRate rate, WaveInCapabilities capabilities) => rate switch
		{
			SampleRate.Rate8K => capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48M08),
			SampleRate.Rate12K =>
					// Opus supports 12K, but NAudio might not explicitly, so check closest available formats
					capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1M08),
			SampleRate.Rate16K => capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_1M16),
			SampleRate.Rate24K =>
					// Opus supports 24K, but NAudio might not explicitly, so check closest available formats
					capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_2M16),
			SampleRate.Rate48K => capabilities.SupportsWaveFormat(SupportedWaveFormat.WAVE_FORMAT_48M16),
			_                  => false
		};
		
		private static bool IsMono(int channels) => (Channels)channels == Enums.Channels.Mono;
		
		private static int CalculateFrameSize(int maxSampleRate, int frameDuration) => maxSampleRate / 1000 * frameDuration;
	}
}