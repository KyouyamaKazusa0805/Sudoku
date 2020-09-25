using System;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Enum"/>.
	/// </summary>
	/// <seealso cref="Enum"/>
	public static class EnumEx
	{
		/// <summary>
		/// Get all enumeration fields.
		/// </summary>
		/// <typeparam name="TEnum">The type of enumeration type.</typeparam>
		/// <returns>The fields.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TEnum[] GetValues<TEnum>() where TEnum : Enum => (TEnum[])Enum.GetValues(typeof(TEnum));

		/// <summary>
		/// Get the total length of the specified enumeration type (how many fields there are).
		/// </summary>
		/// <typeparam name="TEnum">The type of the enmeration.</typeparam>
		/// <returns>The <see cref="int"/> number indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static int LengthOf<TEnum>() where TEnum : Enum => typeof(TEnum).GetFields().Length;
	}
}
