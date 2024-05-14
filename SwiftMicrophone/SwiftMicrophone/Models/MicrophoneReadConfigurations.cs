namespace Euphelia.SwiftMicrophone.Models
{
	public readonly struct MicrophoneReadConfigurations
	{
		public MicrophoneReadConfigurations(int sampleRate, int channels, int frameSize)
		{
			SampleRate = sampleRate;
			Channels   = channels;
			FrameSize  = frameSize;
		}
		
		public int Channels   { get; }
		public int FrameSize  { get; }
		public int SampleRate { get; }
	}
}