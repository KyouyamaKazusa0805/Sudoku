namespace Sudoku.Analytics;

public partial class Hub
{
	public partial class TechniqueNaming
	{
		/// <summary>
		/// Represents naming rules on regular wings.
		/// </summary>
		public static class RegularWing
		{
			/// <summary>
			/// Make the real name of the regular wing.
			/// </summary>
			/// <param name="size">Indicates the size of the wing.</param>
			/// <param name="isIncomplete">A <see cref="bool"/> value indicating whether the wing is incomplete.</param>
			/// <returns>The real name of the regular wing.</returns>
			/// <exception cref="ArgumentOutOfRangeException">Throws when the argument <paramref name="size"/> isn't between 3 and 9.</exception>
			public static string GetRegularWingEnglishName(Digit size, bool isIncomplete)
				=> size switch
				{
					3 => isIncomplete ? "XY-Wing" : "XYZ-Wing",
					>= 4 and <= 9 when size switch
					{
						4 => "WXYZ-Wing",
						5 => "VWXYZ-Wing",
						6 => "UVWXYZ-Wing",
						7 => "TUVWXYZ-Wing",
						8 => "STUVWXYZ-Wing",
						9 => "RSTUVWXYZ-Wing"
					} is var name => isIncomplete ? $"Incomplete {name}" : name,
					_ => throw new ArgumentOutOfRangeException(nameof(size))
				};

			/// <summary>
			/// Try to fetch corresponding <see cref="Technique"/> instance via the real name representing as a <see cref="string"/> text.
			/// </summary>
			/// <param name="englishName">The real name of the regular wing technique.</param>
			/// <returns>The <see cref="Technique"/> instance.</returns>
			/// <exception cref="InvalidOperationException">Throws when the argument <paramref name="englishName"/> is invalid.</exception>
			public static Technique MakeRegularWingTechniqueCode(string englishName)
				=> englishName switch
				{
					"XY-Wing" => Technique.XyWing,
					"XYZ-Wing" => Technique.XyzWing,
					"WXYZ-Wing" => Technique.WxyzWing,
					"VWXYZ-Wing" => Technique.VwxyzWing,
					"UVWXYZ-Wing" => Technique.UvwxyzWing,
					"TUVWXYZ-Wing" => Technique.TuvwxyzWing,
					"STUVWXYZ-Wing" => Technique.StuvwxyzWing,
					"RSTUVWXYZ-Wing" => Technique.RstuvwxyzWing,
					"Incomplete WXYZ-Wing" => Technique.IncompleteWxyzWing,
					"Incomplete VWXYZ-Wing" => Technique.IncompleteVwxyzWing,
					"Incomplete UVWXYZ-Wing" => Technique.IncompleteUvwxyzWing,
					"Incomplete TUVWXYZ-Wing" => Technique.IncompleteTuvwxyzWing,
					"Incomplete STUVWXYZ-Wing" => Technique.IncompleteStuvwxyzWing,
					"Incomplete RSTUVWXYZ-Wing" => Technique.IncompleteRstuvwxyzWing,
					_ => throw new InvalidOperationException($"The argument {nameof(englishName)} must be valid.")
				};
		}
	}
}
