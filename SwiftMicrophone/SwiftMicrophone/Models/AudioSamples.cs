using System.Collections.Generic;

namespace Euphelia.SwiftMicrophone.Models
{
	public readonly struct AudioSamples
	{
		public const int                       OFFSET = 0;
		public       IReadOnlyCollection<byte> PcmBytes { get; }
		public       int                       Length   { get; }
		
		public AudioSamples(IReadOnlyCollection<byte> pcmBytes, int length)
		{
			PcmBytes = pcmBytes;
			Length   = length;
		}
	}
}