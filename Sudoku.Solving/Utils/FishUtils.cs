using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension method used for fish technique.
	/// </summary>
	[DebuggerStepThrough]
	public static class FishUtils
	{
		/// <summary>
		/// All fish names.
		/// </summary>
		private static readonly string[] FishNames =
		{
			string.Empty, "Cyclopsfish", "X-Wing", "Swordfish", "Jellyfish",
			"Squirmbag", "Whale", "Leviathan"
		};


		/// <summary>
		/// Get the fish by its size.
		/// </summary>
		/// <param name="size">The size.</param>
		/// <returns>The name.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetNameBy(int size) => FishNames[size];
	}
}
