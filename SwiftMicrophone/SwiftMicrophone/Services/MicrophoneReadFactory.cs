using Concentus;
using Euphelia.SwiftMicrophone.Helpers;
using Euphelia.SwiftMicrophone.Models;

namespace Euphelia.SwiftMicrophone.Services
{
	public class MicrophoneReadFactory
	{
		private readonly MicrophoneReadConfigurations _configurations;
		private readonly IOpusDecoder                 _decoder;
		
		public MicrophoneReadFactory(MicrophoneReadConfigurations microphoneReadConfigurations)
		{
			_configurations = microphoneReadConfigurations;
			_decoder        = OpusCodecFactory.CreateDecoder(_configurations.SampleRate, _configurations.Channels);
		}
		
		public AudioSamples GetAudioSamples(byte[] dgram)
		{
			// The minimum safe buffer size is 5760 samples
			var decoded = new float[5760];
			var length  = _decoder.Decode(dgram, decoded, decoded.Length / _configurations.Channels);
			
			// Convert float PCM to 16-bit PCM
			var pcmBytes = MicrophoneBitConverter.FloatTo16BitPcm(decoded, length * _configurations.Channels);
			
			return new AudioSamples(pcmBytes, pcmBytes.Length);
		}
	}
}