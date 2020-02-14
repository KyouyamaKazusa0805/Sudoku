namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of rating a chain.
	/// </summary>
	public static class ChainingDifficultyRatingUtils
	{
		/// <summary>
		/// Get extra difficulty rating.
		/// </summary>
		/// <param name="length">The length.</param>
		/// <returns>The difficulty.</returns>
		public static decimal GetExtraDifficultyByLength(int length)
		{
			return length switch
			{
				_ when length > 0 && length < 3 => .0m,
				_ when length >= 3 && length < 5 => .1m,
				_ when length >= 5 && length < 7 => .2m,
				_ when length >= 7 && length < 9 => .3m,
				_ when length >= 9 && length < 13 => .4m,
				_ when length >= 13 && length < 17 => .5m,
				_ when length >= 17 && length < 25 => .6m,
				_ when length >= 25 && length < 37 => .7m,
				_ when length >= 37 && length < 49 => .8m,
				_ when length >= 49 && length < 73 => .9m,
				_ when length >= 73 && length < 97 => 1m,
				_ when length >= 97 && length < 145 => 1.1m,
				_ when length >= 145 && length < 193 => 1.2m,
				_ when length >= 193 && length < 289 => 1.3m,
				_ when length >= 289 && length < 577 => 1.4m,
				_ => 1.5m
			};
		}
	}
}
