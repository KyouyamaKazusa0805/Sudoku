using System;
using System.Text;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="StringBuilder"/>.
	/// </summary>
	/// <seealso cref="StringBuilder"/>
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
		/// please use the method <see cref="AppendLine{T}(StringBuilder, T)"/>.
		/// </remarks>
		/// <seealso cref="Environment.NewLine"/>
		/// <seealso cref="AppendLine{T}(StringBuilder, T)"/>
		public static StringBuilder AppendLine(this StringBuilder @this, char value) => @this.Append(value).AppendLine();

		/// <summary>
		/// Append a <see cref="string"/> representation of an object
		/// to the end of the specified string builder instance,
		/// and then append a <see cref="Environment.NewLine"/>.
		/// </summary>
		/// <typeparam name="T">The type of the instance to add.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="obj">
		/// The <see cref="string"/> representation of an object you want to append.
		/// </param>
		/// <returns>The reference of the current instance.</returns>
		/// <remarks>
		/// This method can solve the problem of boxing and unboxing.
		/// </remarks>
		public static StringBuilder AppendLine<T>(this StringBuilder @this, T obj) =>
			@this.AppendLine(obj.NullableToString());

		/// <summary>
		/// Append several lines into the <see cref="StringBuilder"/> instance.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="lines">The lines you want to add.</param>
		/// <returns>The reference of the parameter <paramref name="this"/>.</returns>
		public static StringBuilder AppendLines(this StringBuilder @this, int lines)
		{
			for (int i = 0; i < lines; i++)
			{
				@this.AppendLine();
			}

			return @this;
		}

		/// <summary>
		/// Append the text into the tail of the <see cref="StringBuilder"/> object if
		/// the text is not <see langword="null"/>; otherwise, do nothing.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The string builder.</param>
		/// <param name="text">The text to add.</param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder NullableAppend(this StringBuilder @this, string? text) =>
			text is null ? @this : @this.Append(text);

		/// <summary>
		/// Append the text into the tail of the <see cref="StringBuilder"/> object if
		/// the text is not <see langword="null"/>, and then add a terminator at the tail;
		/// otherwise, do nothing.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The string builder.</param>
		/// <param name="text">The text to add.</param>
		/// <returns>The reference of the current instance.</returns>
		public static StringBuilder NullableAppendLine(this StringBuilder @this, string? text) =>
			text is null ? @this : @this.AppendLine(text);

		/// <summary>
		/// Copy the specified string builder to the specified target.
		/// </summary>
		/// <param name="this">The base string builder.</param>
		/// <param name="to">The target builder.</param>
		/// <exception cref="ArgumentException">
		/// Throws if the target instance doesn't have enough space to store all characters
		/// from the base one.
		/// </exception>
		public static void CopyTo(this StringBuilder @this, StringBuilder @to)
		{
			if (@this.Length > @to.Length)
			{
				throw new ArgumentException("The specified string builder doesn't have enough space to copy.");
			}

			for (int i = 0; i < @this.Length; i++)
			{
				@to[i] = @this[i];
			}
		}
	}
}
