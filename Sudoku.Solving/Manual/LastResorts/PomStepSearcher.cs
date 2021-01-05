#pragma warning disable IDE0055

using System.Collections.Generic;
using System.Extensions;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Drawing;
using static Sudoku.Solving.Manual.FastProperties;

namespace Sudoku.Solving.Manual.LastResorts
{
	/// <summary>
	/// Encapsulates a <b>pattern overlay method</b> (POM) technique searcher.
	/// </summary>
	[DirectSearcher]
	public sealed class PomStepSearcher : LastResortStepSearcher
	{
		/// <inheritdoc cref="SearchingProperties"/>
		public static TechniqueProperties Properties { get; } = new(55, nameof(TechniqueCode.Pom))
		{
			DisplayLevel = 3,
			IsEnabled = false,
			DisabledReason = DisabledReason.LastResort
		};


		/// <inheritdoc/>
		public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid)
		{
			var templates = GetInvalidPos(grid);
			for (int digit = 0; digit < 9; digit++)
			{
				var template = templates[digit];
				if (template.IsEmpty)
				{
					continue;
				}

				var conclusions = new Conclusion[template.Count];
				int i = 0;
				foreach (int cell in template)
				{
					conclusions[i++] = new(ConclusionType.Elimination, cell, digit);
				}

				accumulator.Add(new PomStepInfo(conclusions, new View[] { new() }));
			}
		}

		/// <summary>
		/// Get all invalid positions.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <returns>The 9 maps for invalid positions of each digit.</returns>
		private static Cells[] GetInvalidPos(in SudokuGrid grid)
		{
			var result = new Cells[9];
			var invalidPos = new Cells[9];
			var mustPos = new Cells[9];
			for (int digit = 0; digit < 9; digit++)
			{
				for (int cell = 0; cell < 81; cell++)
				{
					if (!grid.GetCandidates(cell).ContainsBit(digit))
					{
						invalidPos[digit].AddAnyway(cell);
					}
					else if (grid[cell] == digit)
					{
						mustPos[digit].AddAnyway(cell);
					}
				}
			}

			for (int digit = 0; digit < 9; digit++)
			{
				foreach (var map in GetTemplates())
				{
					if (mustPos[digit] > map || invalidPos[digit].Overlaps(map))
					{
						continue;
					}

					result[digit] |= map;
				}

				result[digit] = CandMaps[digit] - result[digit];
			}

			return result;
		}

		/// <summary>
		/// Get templates.
		/// </summary>
		/// <returns>The templates.</returns>
		private static IEnumerable<Cells> GetTemplates()
		{
			for (int i1 = 0; i1 < 9; i1++)
			for (int i2 = 0; i2 < 9; i2++)
			if (i2 / 3 != i1 / 3)
			for (int i3 = 0; i3 < 9; i3++)
			if (i3 / 3 != i1 / 3 && i3 / 3 != i2 / 3)
			for (int i4 = 0; i4 < 9; i4++)
			if (i4 != i1 && i4 != i2 && i4 != i3)
			for (int i5 = 0; i5 < 9; i5++)
			if (i5 != i1 && i5 != i2 && i5 != i3 && i5 / 3 != i4 / 3)
			for (int i6 = 0; i6 < 9; i6++)
			if (i6 != i1 && i6 != i2 && i6 != i3 && i6 / 3 != i4 / 3 && i6 / 3 != i5 / 3)
			for (int i7 = 0; i7 < 9; i7++)
			if (i7 != i1 && i7 != i2 && i7 != i3 && i7 != i4 && i7 != i5 && i7 != i6)
			for (int i8 = 0; i8 < 9; i8++)
			if (i8 != i1 && i8 != i2 && i8 != i3 && i8 != i4 && i8 != i5 && i8 != i6 && i8 / 3 != i7 / 3)
			for (int i9 = 0; i9 < 9; i9++)
			if (i9 != i1 && i9 != i2 && i9 != i3 && i9 != i4 && i9 != i5 && i9 != i6 && i9 / 3 != i7 / 3 && i9 / 3 != i8 / 3)
			{
				yield return new(
					1 << i1 | 1 << (i2 + 9) | 1 << (i3 + 18),
					1 << i4 | 1 << (i5 + 9) | 1 << (i6 + 18),
					1 << i7 | 1 << (i8 + 9) | 1 << (i9 + 18));
			}
		}
	}
}
