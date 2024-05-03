namespace Euphelia.SwiftMicrophone.Models
{
	public readonly struct MicrophoneReadConfigurations
	{
		public int Channels   { get; }
		public int SampleRate { get; }
		
		public MicrophoneReadConfigurations(MicrophoneWriteConfigurations microphoneWriteConfigurations)
		{
			Channels   = microphoneWriteConfigurations.Channels;
			SampleRate = microphoneWriteConfigurations.MaxSampleRate;
		}
	}
}