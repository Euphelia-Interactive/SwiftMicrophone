namespace Euphelia.SwiftMicrophone.Models
{
	public readonly struct MicrophoneReadConfigurations
	{
		public MicrophoneReadConfigurations(int sampleRate, int channels)
		{
			SampleRate    = sampleRate;
			Channels = channels;
		}
		
		public int Channels   { get; }
		public int SampleRate { get; }
	}
}