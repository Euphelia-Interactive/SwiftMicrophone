using System;

namespace Euphelia.SwiftMicrophone.Helpers
{
	public static class AudioConverter
	{
		public static float[] ConvertPcmToFloat(byte[] pcmBuffer)
		{
			var sampleCount  = pcmBuffer.Length / 2;
			var floatSamples = new float[sampleCount];
			
			for (var i = 0; i < sampleCount; i++)
			{
				var pcmValue = BitConverter.ToInt16(pcmBuffer, i * 2);
				floatSamples[i] = pcmValue / 32768f;
			}
			
			return floatSamples;
		}
		
		public static byte[] ConvertFloatToPcm(float[] floatSamples, int sampleCount)
		{
			var pcmBytes = new byte[sampleCount * sizeof(short)];
			
			for (var i = 0; i < sampleCount; i++)
			{
				var pcmValue = (short)(floatSamples[i] * 32767);
				BitConverter.GetBytes(pcmValue).CopyTo(pcmBytes, i * sizeof(short));
			}
			
			return pcmBytes;
		}
	}
}