using System.Runtime.CompilerServices;

namespace Sudoku.Diagnostics.CodeAnalysis.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="string"/>.
	/// </summary>
	/// <seealso cref="string"/>
	public static class StringEx
	{
		/// <summary>
		/// Converts the current name into the camel case.
		/// </summary>
		/// <param name="this">The name.</param>
		/// <returns>The result name.</returns>
		public static unsafe string ToCamelCase(this string @this)
		{
			char* ptr = stackalloc char[@this.Length];
			fixed (char* pString = @this)
			{
				Unsafe.CopyBlock(ptr, pString, (uint)(sizeof(char) * @this.Length));
			}

			ptr[0] = (char)(ptr[0] + ' ');

			return new string(ptr);
		}
	}
}
