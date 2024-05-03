using System;

namespace Euphelia.SwiftMicrophone.Helpers.Global
{
	public class EnumHelper
	{
		/// <summary>
		///     Retrieves all values of the specified enum type.
		/// </summary>
		/// <typeparam name="TEnum">The enum type.</typeparam>
		/// <returns>An array containing all values of the enum.</returns>
		public static TEnum[] GetAllValues<TEnum>() where TEnum : Enum => (TEnum[])Enum.GetValues(typeof(TEnum));
	}
}