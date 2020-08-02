using System;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Models;
using static Sudoku.Data.SymmetryType;

namespace Sudoku.Solving.Generating
{
	/// <summary>
	/// Encapsulates a puzzle generator, which provides the symmetry type constraint
	/// and the maximum clues constraint.
	/// </summary>
	public sealed class BasicPuzzleGenerator : DiggingPuzzleGenerator
	{
		/// <inheritdoc/>
		public override IReadOnlyGrid Generate() => Generate(28, Central, null);

		/// <summary>
		/// Generate a puzzle with the specified information.
		/// </summary>
		/// <param name="max">The maximum hints of the puzzle.</param>
		/// <param name="symmetricalType">
		/// The symmetry type flags. The <see cref="SymmetryType"/> is
		/// a flag type, you can use bit operators to accumulate multiple
		/// symmetrical types such as <c><see cref="AntiDiagonal"/> | <see cref="Diagonal"/></c>,
		/// which means that the solver will generate anti-diagonal type or
		/// diagonal type puzzles.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="globalizationString">The globalization string.</param>
		/// <returns>The grid.</returns>
		/// <seealso cref="SymmetryType"/>
		public IReadOnlyGrid Generate(
			int max, SymmetryType symmetricalType, IProgress<IProgressResult>? progress,
			string? globalizationString = null)
		{
			PuzzleGeneratingProgressResult defaultValue = default;
			var pr = new PuzzleGeneratingProgressResult(default, globalizationString ?? "en-us");
			ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
			progress?.Report(defaultValue);

			var puzzle = new StringBuilder(Grid.EmptyString);
			var solution = new StringBuilder(Grid.EmptyString);
			GenerateAnswerGrid(puzzle, solution);

			// Now we remove some digits from the grid.
			var allTypes = from st in EnumEx.GetValues<SymmetryType>()
						   where st != None && symmetricalType.HasFlag(st)
						   select st;
			int count = allTypes.Count();
			if (count == 0)
			{
				allTypes = new[] { None };
			}

			var tempSb = new StringBuilder(solution.ToString());
			string result;
			do
			{
				var selectedType = allTypes.ElementAt(Rng.Next(count));
				for (int i = 0; i < 81; i++)
				{
					solution[i] = tempSb[i];
				}

				var totalMap = GridMap.Empty;
				do
				{
					int cell;
					do cell = Rng.Next(0, 81); while (totalMap[cell]);

					int r = cell / 9, c = cell % 9;

					// Get new value of 'last'.
					var tempMap = GridMap.Empty;
					foreach (int tCell in selectedType switch
					{
						Central => new[] { r * 9 + c, (8 - r) * 9 + 8 - c },
						Diagonal => new[] { r * 9 + c, c * 9 + r },
						AntiDiagonal => new[] { r * 9 + c, (8 - c) * 9 + 8 - r },
						XAxis => new[] { r * 9 + c, (8 - r) * 9 + c },
						YAxis => new[] { r * 9 + c, r * 9 + 8 - c },
						DiagonalBoth => new[] { r * 9 + c, c * 9 + r, (8 - c) * 9 + 8 - r, (8 - r) * 9 + 8 - c },
						AxisBoth => new[] { r * 9 + c, (8 - r) * 9 + c, r * 9 + 8 - c, (8 - r) * 9 + 8 - c },
						All => new[]
						{
							r * 9 + c, r * 9 + (8 - c), (8 - r) * 9 + c, (8 - r) * 9 + (8 - c),
							c * 9 + r, c * 9 + (8 - r), (8 - c) * 9 + r, (8 - c) * 9 + (8 - r)
						},
						None => new[] { r * 9 + c },
						_ => throw Throwings.ImpossibleCaseWithMessage(
							"You shouldn't add an option that does not contain in the table of symmetrical types.")
					})
					{
						solution[tCell] = '0';
						totalMap.Add(tCell);
						tempMap.Add(tCell);
					}
				} while (81 - totalMap.Count > max);

				if (!(progress is null))
				{
					progressResult.GeneratingTrial++;
					progress.Report(progressResult);
				}
			} while (!FastSolver.CheckValidity(result = solution.ToString(), out _));

			return Grid.Parse(result);
		}

		/// <summary>
		/// Generate a puzzle with the specified information asynchronizedly.
		/// </summary>
		/// <param name="max">The maximum hints of the puzzle.</param>
		/// <param name="symmetricalType">
		/// The symmetry type flags. The <see cref="SymmetryType"/> is
		/// a flag type, you can use bit operators to accumulate multiple
		/// symmetrical types such as <c><see cref="AntiDiagonal"/> | <see cref="Diagonal"/></c>,
		/// which means that the solver will generate anti-diagonal type or
		/// diagonal type puzzles.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="globalzationString">The globalization string.</param>
		/// <returns>The task.</returns>
		/// <seealso cref="SymmetryType"/>
		public async Task<IReadOnlyGrid> GenerateAsync(
			int max, SymmetryType symmetricalType, IProgress<IProgressResult> progress,
			string? globalzationString = null) =>
			await Task.Run(() => Generate(max, symmetricalType, progress, globalzationString));

		/// <inheritdoc/>
		/// <exception cref="NotImplementedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override void CreatePattern(int[] pattern) => throw new NotImplementedException();
	}
}
