using System.Runtime.CompilerServices;

namespace Sudoku.Solving.Utils
{
	public static class FishUtils
	{
		private static readonly string[] FishNames =
		{
			string.Empty, "Cyclopsfish", "X-Wing", "Swordfish", "Jellyfish",
			"Squirmbag", "Whale", "Leviathan"
		};


		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static string GetNameBy(int size) => FishNames[size];
	}
}
