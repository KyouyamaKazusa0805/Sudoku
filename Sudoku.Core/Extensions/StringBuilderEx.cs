using System;
using System.Diagnostics;
using System.Text;

namespace Sudoku.Extensions
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
		/// <param name="this">
		/// (<see langword="this"/> parameter) The instance to reverse all characters.
		/// </param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder Reverse(this StringBuilder @this)
		{
			for (int i = 0, length = @this.Length >> 1; i < length; i++)
			{
				int z = i + 1;
				char temp = @this[i];
				@this[i] = @this[^z];
				@this[^z] = temp;
			}

			return @this;
		}

		/// <summary>
		/// Remove the specified number of characters from the end of the string builder
		/// instance.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The instance to remove characters.</param>
		/// <param name="length">The number of characters you want to remove.</param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder RemoveFromEnd(this StringBuilder @this, int length) =>
			@this.Remove(@this.Length - length, length);

		/// <summary>
		/// Append a character to the end of the specified string builder instance,
		/// and then append a <see cref="Environment.NewLine"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
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
		/// and then append a <see cref="Environment.NewLine"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="obj">
		/// The <see cref="string"/> representation of an object you want to append.
		/// </param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder AppendLine(this StringBuilder @this, object? obj) =>
			@this.AppendLine(obj.NullableToString());

		/// <summary>
		/// Copy the specified string builder to the specified target.
		/// </summary>
		/// <param name="this">The base string builder.</param>
		/// <param name="to">The target builder.</param>
		/// <exception cref="ArgumentException">
		/// Throws if the target instance does not have enough space to store all characters
		/// from the base one.
		/// </exception>
		public static void CopyTo(this StringBuilder @this, StringBuilder @to)
		{
			if (@this.Length > @to.Length)
			{
				throw new ArgumentException(
					"The specified string builder does not have enough space to copy.");
			}

			for (int i = 0; i < @this.Length; i++)
				@to[i] = @this[i];
		}
	}
}
