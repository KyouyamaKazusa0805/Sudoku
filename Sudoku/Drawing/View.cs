using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Drawing
{
	public sealed class View
	{
		public View(
			IReadOnlyList<(int, int)>? cellOffsets, IReadOnlyList<(int, int)>? candidateOffsets,
			IReadOnlyList<(int, int)>? regionOffsets, IReadOnlyList<int>? linkMasks) =>
			(CellOffsets, CandidateOffsets, RegionOffsets, LinkMasks) = (cellOffsets, candidateOffsets, regionOffsets, linkMasks);


		public IReadOnlyList<(int _id, int _cellGlobalOffset)>? CellOffsets { get; }

		public IReadOnlyList<(int _id, int _candidateGlobalOffset)>? CandidateOffsets { get; }

		public IReadOnlyList<(int _id, int _regionGlobalOffset)>? RegionOffsets { get; }

		public IReadOnlyList<int>? LinkMasks { get; }

#if DEBUG
		public override string ToString()
		{
			const string separator = ", ";
			var sb = new StringBuilder();

			if (!(CellOffsets is null))
			{
				sb.AppendLine("Cells:");
				sb.Append("    ");
				foreach (var (_, cellOffset) in CellOffsets)
				{
					sb.Append($"r{cellOffset / 9 + 1}c{cellOffset % 9 + 1}{separator}");
				}
				sb.RemoveFromEnd(separator.Length);
				sb.AppendLine();
			}
			if (!(CandidateOffsets is null))
			{
				sb.AppendLine("Candidates:");
				sb.Append("    ");
				foreach (var (_, candidateOffset) in CandidateOffsets)
				{
					sb.Append($"r{candidateOffset / 81 + 1}{candidateOffset % 81 / 9 + 1}({candidateOffset % 9 + 1}){separator}");
				}
				sb.RemoveFromEnd(separator.Length);
				sb.AppendLine();
			}
			if (!(RegionOffsets is null))
			{
				sb.AppendLine("Regions:");
				sb.Append("    ");
				foreach (var (_, regionOffset) in RegionOffsets)
				{
					sb.Append($@"{(regionOffset / 9) switch
					{
						0 => "b",
						1 => "r",
						2 => "c",
						_ => throw new Exception("Never.")
					}}{regionOffset % 9}{separator}");
				}
				sb.RemoveFromEnd(separator.Length);
				sb.AppendLine();
			}
			if (!(LinkMasks is null))
			{
				sb.AppendLine("Links:");
				sb.Append("    ");
				foreach (int linkMask in LinkMasks)
				{
					int linkType = linkMask >> 20;
					int baseCandidate = linkType % 729, targetCandidate = linkType / 729;

					sb.Append($"r{baseCandidate / 81 + 1}{baseCandidate % 81 / 9 + 1}({baseCandidate % 9 + 1}) -> ");
					sb.Append($"r{targetCandidate / 81 + 1}{targetCandidate % 81 / 9 + 1}({targetCandidate % 9 + 1}){separator}");
				}
				sb.RemoveFromEnd(separator.Length);
				sb.AppendLine();
			}

			return sb.ToString();
		}
#endif
	}
}
