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

		/// <inheritdoc cref="Enum.HasFlag(Enum)"/>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The current enumeration type instance.</param>
		/// <param name="other">The other instance to check.</param>
		/// <exception cref="ArgumentException">Throws when the used bytes aren't 1, 2 or 4.</exception>
		/// <remarks>
		/// This method is same as <see cref="Enum.HasFlag(Enum)"/>, but without boxing and unboxing.
		/// </remarks>
		/// <seealso cref="Enum.HasFlag(Enum)"/>
		public static unsafe bool HasFlagOf<TEnum>(this TEnum @this, TEnum other) where TEnum : unmanaged, Enum =>
			sizeof(TEnum) switch
			{
				1 or 2 or 4 when Unsafe.As<TEnum, int>(ref other) is var otherValue =>
					(Unsafe.As<TEnum, int>(ref @this) & otherValue) == otherValue,
				8 when Unsafe.As<TEnum, long>(ref other) is var otherValue =>
					(Unsafe.As<TEnum, long>(ref @this) & otherValue) == otherValue,
				_ => throw new ArgumentException("The parameter should be one of the values 1, 2, 4.")
			};
	}
}
