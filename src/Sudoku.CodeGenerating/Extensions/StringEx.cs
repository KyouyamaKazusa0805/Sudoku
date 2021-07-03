using System;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Sudoku.CodeGenerating.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringEx
	{
		/// <summary>
		/// Count how many specified characters are in the current string.
		/// </summary>
		/// <param name="this">The current string.</param>
		/// <param name="character">The character to count.</param>
		/// <returns>The number of characters found.</returns>
		public static unsafe int CountOf(this string @this, char character)
		{
			int count = 0;
			fixed (char* pThis = @this)
			{
				for (char* p = pThis; *p != '\0'; p++)
				{
					if (*p == character)
					{
						count++;
					}
				}
			}

			return count;
		}

		/// <summary>
		/// Converts the current string into the camel case.
		/// </summary>
		/// <param name="this">The string.</param>
		/// <returns>The result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string ToCamelCase(this string @this)
		{
			@this = @this.TrimStart('_');
			return @this.Substring(0, 1).ToLowerInvariant() + @this.Substring(1);
		}

		/// <summary>
		/// Separate the attribute string representation into multiple elements.
		/// The attribute string will be like <c>Type({value1, value2, value3})</c>.
		/// </summary>
		/// <param name="this">The attribute string.</param>
		/// <param name="tokenStartIndex">The token start index.</param>
		/// <returns>The array of separated values.</returns>
		internal static string[]? GetMemberValues(this string @this, int tokenStartIndex)
		{
			string[] result = (
				from parameterValue in @this.Substring(
					tokenStartIndex,
					@this.Length - tokenStartIndex - 2
				).Split(new string[] { ", " }, StringSplitOptions.RemoveEmptyEntries)
				select parameterValue.Substring(1, parameterValue.Length - 2)
			).ToArray();

			if (result.Length == 0)
			{
				return null;
			}

			result[0] = result[0].Substring(2);
			return result;
		}
	}
}
