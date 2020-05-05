using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="bool"/>.
	/// </summary>
	/// <seealso cref="bool"/>
	[DebuggerStepThrough]
	public static class BooleanEx
	{
		/// <summary>
		/// Flip the current <see cref="bool"/> value (i.e. <see langword="true"/>
		/// -&gt; <see langword="false"/>, <see langword="false"/> -&gt; <see langword="true"/>),
		/// which is equivalent to code '<c>b = !b</c>' or '<c>b ^= true</c>'.
		/// </summary>
		/// <param name="this">(<see langword="this ref"/> parameter) The value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void Flip(this ref bool @this) => @this = !@this;
	}
}
