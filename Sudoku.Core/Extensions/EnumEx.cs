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

#if false
		public static unsafe bool HasFlag<TEnum>(this TEnum @this, TEnum other) where TEnum : unmanaged, Enum =>
#if NON_VS_IDE
			@this.HasFlag(other);
#else
			sizeof(TEnum) switch
			{
				1 or 2 or 4 when __refvalue(__makeref(other), int) is var otherValue =>
					(__refvalue(__makeref(@this), int) & otherValue) == otherValue,
				8 when __refvalue(__makeref(other), long) is var otherValue =>
					(__refvalue(__makeref(@this), long) & otherValue) == otherValue,
				_ => throw new ArgumentException("The parameter should be one of the values 1, 2, 4.")
			};
#endif
#endif
	}
}
