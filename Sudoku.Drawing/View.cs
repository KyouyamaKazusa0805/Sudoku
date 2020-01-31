using System;
using System.Collections.Generic;
using System.Text;
using Sudoku.Data.Extensions;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a view when displaying the information on forms.
	/// </summary>
	public sealed class View
	{
		/// <summary>
		/// Initializes an instance with information.
		/// </summary>
		/// <param name="cellOffsets">
		/// The list of pairs of identifier and cell offset.
		/// </param>
		/// <param name="candidateOffsets">
		/// The list of pairs of identifier and candidate offset.
		/// </param>
		/// <param name="regionOffsets">
		/// The list of pairs of identifier and region offset.
		/// </param>
		/// <param name="linkMasks">The list of link masks.</param>
		public View(
			IReadOnlyList<(int, int)>? cellOffsets, IReadOnlyList<(int, int)>? candidateOffsets,
			IReadOnlyList<(int, int)>? regionOffsets, IReadOnlyList<int>? linkMasks) =>
			(CellOffsets, CandidateOffsets, RegionOffsets, LinkMasks) = (cellOffsets, candidateOffsets, regionOffsets, linkMasks);


		/// <summary>
		/// All cell offsets.
		/// </summary>
		/// <remarks>
		/// This property is a list of pairs of identifier and cell offsets,
		/// where the identifier is an <see cref="int"/> value that can tell
		/// all cell offsets' colors.
		/// </remarks>
		public IReadOnlyList<(int _id, int _cellOffset)>? CellOffsets { get; }

		/// <summary>
		/// All candidate offsets.
		/// </summary>
		/// <remarks>
		/// This property is a list of pairs of identifier and candidate offsets,
		/// where the identifier is an <see cref="int"/> value that can tell
		/// all cell offsets' colors.
		/// </remarks>
		public IReadOnlyList<(int _id, int _candidateOffset)>? CandidateOffsets { get; }

		/// <summary>
		/// All region offsets.
		/// </summary>
		/// <remarks>
		/// This property is a list of pairs of identifier and region offsets,
		/// where the identifier is an <see cref="int"/> value that can tell
		/// all cell offsets' colors.
		/// </remarks>
		public IReadOnlyList<(int _id, int _regionOffset)>? RegionOffsets { get; }

		/// <summary>
		/// All link masks.
		/// </summary>
		/// <remarks>
		/// This property is a list of masks.
		/// All mask is regarded as 22 bits, 10 bits for base candidate offset,
		/// another 10 bits for target candidate offset and 2 bits for link type,
		/// where the value is:
		/// <list type="table">
		/// <item>
		/// <term>0b00</term><description><see langword="false"/> -&gt; <see langword="false"/>.</description>
		/// <term>0b01</term><description><see langword="false"/> -&gt; <see langword="true"/>.</description>
		/// <term>0b10</term><description><see langword="true"/> -&gt; <see langword="false"/>.</description>
		/// <term>0b11</term><description><see langword="true"/> -&gt; <see langword="true"/>.</description>
		/// </item>
		/// </list>
		/// </remarks>
		public IReadOnlyList<int>? LinkMasks { get; }

		/// <inheritdoc/>
		public override string ToString()
		{
#if DEBUG
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
#else
		return base.ToString() ?? string.Empty;
#endif
	}
}
