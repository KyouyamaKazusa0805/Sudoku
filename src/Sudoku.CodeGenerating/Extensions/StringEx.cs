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
	}
}
