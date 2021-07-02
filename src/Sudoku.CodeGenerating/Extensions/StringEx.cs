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
