using System;
using System.Collections.Generic;

namespace Euphelia.SwiftMicrophone.Helpers
{
	public class MicrophoneBitConverter
	{
		public static float[] ConvertToFloatArray(IReadOnlyList<byte> buffer)
		{
			// Convert byte array to float array
			var floatArray = new float[buffer.Count / 2];
			
			for (var i = 0; i < floatArray.Length; i++)
			{
				var idx    = i * 2;
				var sample = (short)(buffer[idx + 1] << 8 | buffer[idx] & 0xFF);
				floatArray[i] = sample / 32768f; // Normalize to -1.0 to 1.0 range
			}
			
			return floatArray;
		}
		
		public static byte[] FloatTo16BitPcm(IReadOnlyList<float> input, int length)
		{
			var bytes = new byte[length * 2];
			
			for (var i = 0; i < length; i++)
			{
				var val        = (short)(input[i] * 32767); // Convert float to 16-bit short
				var shortBytes = BitConverter.GetBytes(val);
				Buffer.BlockCopy(shortBytes,
				                 0,
				                 bytes,
				                 i * 2,
				                 2);
			}
			
			return bytes;
		}
	}
}