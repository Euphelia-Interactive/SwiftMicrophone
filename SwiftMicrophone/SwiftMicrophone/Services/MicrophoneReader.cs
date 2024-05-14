using Concentus.Structs;
using Euphelia.SwiftMicrophone.Helpers;
using Euphelia.SwiftMicrophone.Models;

namespace Euphelia.SwiftMicrophone.Services
{
	public class MicrophoneReader
	{
		private readonly MicrophoneReadConfigurations _configurations;
		private readonly OpusDecoder                  _decoder;
		
		public MicrophoneReader(MicrophoneReadConfigurations microphoneReadConfigurations)
		{
			_configurations = microphoneReadConfigurations;
			_decoder        = OpusDecoder.Create(_configurations.SampleRate, _configurations.Channels);
		}
		
		public byte[] DecodeToPcm(byte[] encodedSamples)
		{
			var decodedFloats = new float[_configurations.FrameSize * _configurations.Channels];
			var decodedSampleCount = _decoder.Decode(encodedSamples,
			                                         0,
			                                         encodedSamples.Length,
			                                         decodedFloats,
			                                         0,
			                                         _configurations.FrameSize);
			
			return AudioConverter.ConvertFloatToPcm(decodedFloats, decodedSampleCount * _configurations.Channels);
		}
	}
}