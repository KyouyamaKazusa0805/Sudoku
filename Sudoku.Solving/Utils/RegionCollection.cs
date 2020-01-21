using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sudoku.Solving.Utils
{
	/// <summary>
	/// Provides extension methods of region collection.
	/// </summary>
	public static class RegionCollection
	{
		/// <summary>
		/// Get a string consists of all texts of region offsets.
		/// </summary>
		/// <param name="regionOffsets">The region offsets.</param>
		/// <returns>The string text.</returns>
		public static string ToString(IEnumerable<int> regionOffsets)
		{
			var sb = new StringBuilder();

			foreach (var regionGroup in from regionOffset in regionOffsets
										orderby regionOffset switch
										{
											_ when regionOffset >= 0 && regionOffset < 9 => regionOffset % 9 + 18,
											_ when regionOffset >= 9 && regionOffset < 18 => regionOffset % 9,
											_ when regionOffset >= 18 && regionOffset < 27 => regionOffset % 9 + 9,
											_ => throw new Exception("Invalid region offset value.")
										}
										group regionOffset by regionOffset / 9)
			{
				sb.Append(regionGroup.Key switch
				{
					0 => 'b',
					1 => 'r',
					2 => 'c',
					_ => throw new Exception("Invalid region offset value.")
				});
				foreach (int region in regionGroup)
				{
					sb.Append(region % 9 + 1);
				}
			}

			return sb.ToString();
		}
	}
}
