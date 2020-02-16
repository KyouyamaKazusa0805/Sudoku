using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Subsets
{
	public class NakedSubsetInfo : SubsetInfo
	{
		public NakedSubsetInfo(
			Conclusion conclusion, ICollection<View> views, Region region, IEnumerable<int> digits, int size)
			: base(conclusion, views, region, digits, size)
		{
		}


		public override decimal Difficulty
		{
			get
			{
				return Size switch
				{
					2 => 3m,
					3 => 3.6m,
					_ => 5m
				};
			}
		}

		public override string Name => $"Naked {Values.SubsetNames[Size]}";


		public override string ToString() =>
			$"{Name}: {GetValues(Digits)} in {Region} => {Conclusion}";

		private static string GetValues(IEnumerable<int> values)
		{
			const string separator = ", ";
			var sb = new StringBuilder();
			foreach (int value in values)
			{
				sb.Append($"{value + 1}{separator}");
			}

			return sb.RemoveFromLast(separator.Length).ToString();
		}
	}
}
