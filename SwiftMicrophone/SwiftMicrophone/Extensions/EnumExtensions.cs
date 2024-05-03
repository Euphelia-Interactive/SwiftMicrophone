using System;

namespace Euphelia.SwiftMicrophone.Extensions
{
	public static class EnumExtensions
	{
		/// <summary>
		///     Checks if the specified enum value is defined in its enum type.
		/// </summary>
		/// <typeparam name="TEnum">The enum type.</typeparam>
		/// <param name="value">The enum value to check.</param>
		/// <returns>True if the value is defined, otherwise false.</returns>
		public static bool IsValidEnumValue<TEnum>(this TEnum value) where TEnum : struct, Enum => Enum.IsDefined(typeof(TEnum), value);
		
		public static void ThrowInvalidEnumValue<TEnum>(this TEnum value, string customMessage = null) where TEnum : struct, Enum
		{
			if (value.IsValidEnumValue())
				throw new Exception(customMessage ?? "Invalid enum value passed.");
		}
	}
}