using System.Diagnostics;

namespace Sudoku.Data.Collections
{
	/// <summary>
	/// Provides methods for region offsets.
	/// </summary>
	[DebuggerStepThrough]
	public readonly struct Region
	{
		/// <summary>
		/// Get the name of the region.
		/// </summary>
		/// <param name="region">The region.</param>
		/// <returns>The name.</returns>
		public static string GetName(int region)
		{
			return stackalloc[] { 'B', 'l', 'o', 'c', 'k', 'R', 'o', 'w', 'C', 'o', 'l', 'u', 'm', 'n' }[
				(region / 9) switch
				{
					0 => 0..5,
					1 => 5..8,
					2 => 8..,
					_ => throw Throwing.ImpossibleCase
				}].ToString();
		}
	}
}
