using System.Diagnostics;
using System.Text;

namespace Sudoku.Data.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="StringBuilder"/>.
	/// </summary>
	/// <seealso cref="StringBuilder"/>
	[DebuggerStepThrough]
	public static class StringBuilderEx
	{
		/// <summary>
		/// Reverse all characters in a specified string builder instance.
		/// </summary>
		/// <param name="this">The instance to reverse all characters.</param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder Reverse(this StringBuilder @this)
		{
			for (int i = 0, length = @this.Length >> 1; i < length; i++)
			{
				int z = i + 1;
#if !LAZY_CODE
				checked
				{
					@this[i] ^= @this[^z] ^= @this[i] ^= @this[^z];
				}
#else
				char temp = @this[i];
				@this[i] = @this[^z];
				@this[^z] = temp;
#endif
			}

			return @this;
		}

		/// <summary>
		/// Remove the specified number of characters from the end of the string builder
		/// instance.
		/// </summary>
		/// <param name="this">The instance to remove characters.</param>
		/// <param name="length">The number of characters you want to remove.</param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder RemoveFromEnd(this StringBuilder @this, int length) =>
			@this.Remove(@this.Length - length, length);

		/// <summary>
		/// Append a character to the end of the specified string builder instance,
		/// and then append a <see cref="System.Environment.NewLine"/>.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="value">The character you want to append.</param>
		/// <returns>The reference of the current instance.</returns>
		/// <remarks>
		/// The extension method is used in order to avoid implicit conversion from
		/// <see cref="char"/> to <see cref="int"/>. If you want to append everything,
		/// please use the method <see cref="AppendLine(StringBuilder, object)"/>.
		/// </remarks>
		/// <seealso cref="AppendLine(StringBuilder, object)"/>
		public static StringBuilder AppendLine(this StringBuilder @this, char value)
		{
			@this.Append(value);
			@this.AppendLine();

			return @this;
		}

		/// <summary>
		/// Append a <see cref="string"/> representation of an object
		/// to the end of the specified string builder instance,
		/// and then append a <see cref="System.Environment.NewLine"/>.
		/// </summary>
		/// <param name="this">The instance.</param>
		/// <param name="obj">
		/// The <see cref="string"/> representation of an object you want to append.
		/// </param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder AppendLine(this StringBuilder @this, object? obj) =>
			@this.AppendLine(obj.NullableToString());
	}
}
