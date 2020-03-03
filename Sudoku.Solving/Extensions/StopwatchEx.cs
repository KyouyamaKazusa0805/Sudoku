using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="Stopwatch"/>.
	/// </summary>
	/// <seealso cref="Stopwatch"/>
	[DebuggerStepThrough]
	public static class StopwatchEx
	{
		/// <summary>
		/// To stop the stopwatch anyway. If the stopwatch is running at present,
		/// this method will stop it; otherwise, do nothing.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The stopwatch.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static void StopAnyway(this Stopwatch @this)
		{
			if (@this.IsRunning)
			{
				@this.Stop();
			}
		}
	}
}
