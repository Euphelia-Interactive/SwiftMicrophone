using System;
using System.Collections.Generic;
using System.Linq;

namespace Euphelia.SwiftMicrophone.Helpers
{
	public static class MicrophoneBitConverter
	{
		public static float[] ConvertToFloatArray(ICollection<byte> buffer) => Enumerable.Range(0, buffer.Count / 2)
		                                                                                 .Select(i => BitConverter.ToInt16(buffer.ToArray(), i * 2) / 32768f)
		                                                                                 .ToArray();
		
		public static byte[] FloatTo16BitPcm(IEnumerable<float> input, int length) => input
		                                                                              .Take(length)
		                                                                              .SelectMany(f => BitConverter.GetBytes((short)(f * 32767)))
		                                                                              .ToArray();
	}
}