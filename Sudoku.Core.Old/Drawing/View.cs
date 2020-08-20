using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;
using Sudoku.Data.Meta;

namespace Sudoku.Drawing
{
	public sealed partial class View
	{
		public View(
			IEnumerable<(Id, Cell)>? cells, IEnumerable<(Id, Candidate)>? candidates,
			IEnumerable<(Id, Region)>? regions, IEnumerable<Inference>? inferences)
		{
			if (cells is not null)
			{
				Cells = new HashSet<(Id, Cell)>(cells, new Comparer<Cell>());
			}
			if (candidates is not null)
			{
				Candidates = new HashSet<(Id, Candidate)>(candidates, new Comparer<Candidate>());
			}
			if (regions is not null)
			{
				Regions = new HashSet<(Id, Region)>(regions, new Comparer<Region>());
			}
			if (inferences is not null)
			{
				Inferences = new HashSet<Inference>(inferences);
			}
		}


		public ISet<Inference>? Inferences { get; }

		public ISet<(Id id, Cell cell)>? Cells { get; }

		public ISet<(Id id, Candidate candidate)>? Candidates { get; }

		public ISet<(Id id, Region region)>? Regions { get; }


		public override int GetHashCode() => GetType().GetHashCode() ^ ToString().GetHashCode(StringComparison.Ordinal);

		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			if (Cells is not null)
			{
				sb.AppendLine("Cells:");
				foreach (var (_, cell) in Cells)
				{
					sb.Append($"{cell}{separator}");
				}
				sb.RemoveFromLast(separator.Length).AppendLine();
			}

			if (Candidates is not null)
			{
				sb.AppendLine("Candidates:");
				foreach (var (_, candidate) in Candidates)
				{
					sb.Append($"{candidate}, ");
				}
				sb.RemoveFromLast(separator.Length).AppendLine();
			}
			if (Regions is not null)
			{
				sb.AppendLine("Regions:");
				foreach (var (_, region) in Regions)
				{
					sb.Append($"{region}, ");
				}
				sb.RemoveFromLast(separator.Length).AppendLine();
			}
			if (Inferences is not null)
			{
				sb.AppendLine("Inferences:");
				foreach (var inference in Inferences)
				{
					sb.Append($"{inference}, ");
				}
				sb.RemoveFromLast(separator.Length).AppendLine();
			}

			return sb.ToString();
		}
	}
}
