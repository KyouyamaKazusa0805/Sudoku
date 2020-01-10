using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Literals;
using Sudoku.Data.Meta;
using Sudoku.Drawing;

namespace Sudoku.Solving.Subsets
{
	public sealed class HiddenSubsetInfo : SubsetInfo
	{
		public HiddenSubsetInfo(
			Conclusion conclusion, ICollection<View> views, Region region, IEnumerable<int> digits, int size)
			: base(conclusion, views, region, digits, size)
		{
		}


		public override decimal Difficulty =>
			 Size switch
			 {
				 2 => 3.4m,
				 3 => 4m,
				 _ => 5.4m
			 };

		public override string Name => $"Hidden {Values.SubsetNames[Size]}";


		public override string ToString() =>
			$"{Name}: {GetValues(Digits)} in {Region} => {Conclusion}";

		private static string GetValues(IEnumerable<int> values)
		{
			const string separator = ", ";
			var sb = new StringBuilder();
			foreach (var value in values)
			{
				sb.Append($"{value + 1}{separator}");
			}

			return sb.RemoveFromLast(separator.Length).ToString();
		}
	}
}
