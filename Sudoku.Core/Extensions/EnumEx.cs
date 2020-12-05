using System;
using static System.Runtime.CompilerServices.Unsafe;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Enum"/>.
	/// </summary>
	/// <seealso cref="Enum"/>
	public static class EnumEx
	{
		/// <inheritdoc cref="Enum.HasFlag(Enum)"/>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The current enumeration type instance.</param>
		/// <param name="other">The other instance to check.</param>
		/// <exception cref="ArgumentException">Throws when the used bytes aren't 1, 2 or 4.</exception>
		/// <remarks>
		/// This method is same as <see cref="Enum.HasFlag(Enum)"/>, but without boxing and unboxing operations.
		/// </remarks>
		/// <seealso cref="Enum.HasFlag(Enum)"/>
		public static bool Flags<TEnum>(this TEnum @this, TEnum other) where TEnum : unmanaged, Enum
		{
			int size;
			unsafe
			{
				size = sizeof(TEnum);
			}

			switch (size)
			{
				case 1 or 2 or 4:
				{
					int otherValue = As<TEnum, int>(ref other);
					return (As<TEnum, int>(ref @this) & otherValue) == otherValue;
				}
				case 8:
				{
					long otherValue = As<TEnum, long>(ref other);
					return (As<TEnum, long>(ref @this) & otherValue) == otherValue;
				}
				default:
				{
					throw new ArgumentException("The parameter should be one of the values 1, 2, 4.");
				}
			}
		}
	}
}
