using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Enum"/>.
	/// </summary>
	/// <seealso cref="Enum"/>
	public static class EnumEx
	{
		/// <summary>
		/// To get all possible flags from a specified enumeration instance.
		/// </summary>
		/// <typeparam name="TEnum">The type of that enumeration.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The field.</param>
		/// <returns>All flags.</returns>
		public static TEnum[] GetAllFlags<TEnum>(this TEnum @this) where TEnum : unmanaged, Enum
		{
			unsafe
			{
				var buffer = stackalloc TEnum[Enum.GetValues<TEnum>().Length];
				int i = 0;
				foreach (var flag in @this)
				{
					buffer[i++] = flag;
				}

				return new Span<TEnum>(buffer, i).ToArray();
			}
		}

		/// <summary>
		/// Get all possible flags that the current enumeration field set.
		/// </summary>
		/// <typeparam name="TEnum">The type of the enumeration.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The current enumeration type instance.</param>
		/// <returns>All flags.</returns>
		public static IEnumerator<TEnum> GetEnumerator<TEnum>(this TEnum @this) where TEnum : unmanaged, Enum
		{
			unsafe
			{
				return inner(@this, sizeof(TEnum));
			}

			static IEnumerator<TEnum> inner(TEnum @this, int size)
			{
				var array = Enum.GetValues<TEnum>();
				for (int index = 0, length = array.Length; index < length; index++)
				{
					var field = array[index];
					switch (size)
					{
						case 1 or 2 or 4
						when Unsafe.As<TEnum, int>(ref field) is var i && !i.IsPowerOfTwo():
						case 8
						when Unsafe.As<TEnum, long>(ref field) is var l && !l.IsPowerOfTwo():
						{
							// We'll skip the field that keeps the default value (0), or the value isn't a
							// normal flag.
							continue;
						}
					}

					if (@this.Flags(field))
					{
						yield return field;
					}
				}
			}
		}

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
			unsafe
			{
				switch (sizeof(TEnum))
				{
					case 1:
					case 2:
					case 4:
					{
						int otherValue = Unsafe.As<TEnum, int>(ref other);
						return Unsafe.As<TEnum, int>(ref @this).Covers(otherValue);
					}
					case 8:
					{
						long otherValue = Unsafe.As<TEnum, long>(ref other);
						return Unsafe.As<TEnum, long>(ref @this).Covers(otherValue);
					}
					default:
					{
						throw new ArgumentException("The parameter should be one of the values 1, 2, 4.", nameof(@this));
					}
				}
			}
		}
	}
}
