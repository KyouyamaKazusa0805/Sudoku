using System.Runtime.CompilerServices;

namespace System.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="char"/>.
	/// </summary>
	/// <seealso cref="char"/>
	public static class CharEx
	{
		/// <summary>
		/// Checks whether the specified character is a punctuation that is in the range ASCII.
		/// </summary>
		/// <param name="this">The character.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsAsciiPuncuation(this char @this) => @this is '!' or '?' or ',' or '.' or ';' or ':';
	}
}
