using System;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Globalization;
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
		public override SudokuGrid Generate() => Generate(28, Central, null, CountryCode.Default);

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
		/// <param name="countryCode">The country code.</param>
		/// <returns>The grid.</returns>
		/// <seealso cref="SymmetryType"/>
		[SkipLocalsInit]
		public SudokuGrid Generate(
			int max, SymmetryType symmetricalType, IProgress<IProgressResult>? progress,
			CountryCode countryCode = CountryCode.Default)
		{
			PuzzleGeneratingProgressResult defaultValue = default;
			var pr = new PuzzleGeneratingProgressResult(
				default, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
			ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
			progress?.Report(defaultValue);

			var puzzle = new StringBuilder(SudokuGrid.EmptyString);
			var solution = new StringBuilder(SudokuGrid.EmptyString);
			GenerateAnswerGrid(puzzle, solution);

			// Now we remove some digits from the grid.
			var allTypes = from @type in EnumEx.GetValues<SymmetryType>()
						   where @type != None && symmetricalType.Flags(@type)
						   select @type;
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
				tempSb.CopyTo(solution);

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
						Central => stackalloc[] { r * 9 + c, (8 - r) * 9 + 8 - c },
						Diagonal => stackalloc[] { r * 9 + c, c * 9 + r },
						AntiDiagonal => stackalloc[] { r * 9 + c, (8 - c) * 9 + 8 - r },
						XAxis => stackalloc[] { r * 9 + c, (8 - r) * 9 + c },
						YAxis => stackalloc[] { r * 9 + c, r * 9 + 8 - c },
						DiagonalBoth => stackalloc[]
						{
							r * 9 + c,
							c * 9 + r,
							(8 - c) * 9 + 8 - r,
							(8 - r) * 9 + 8 - c
						},
						AxisBoth => stackalloc[]
						{
							r * 9 + c,
							(8 - r) * 9 + c,
							r * 9 + 8 - c,
							(8 - r) * 9 + 8 - c
						},
						All => stackalloc[]
						{
							r * 9 + c,
							r * 9 + (8 - c),
							(8 - r) * 9 + c,
							(8 - r) * 9 + (8 - c),
							c * 9 + r,
							c * 9 + (8 - r),
							(8 - c) * 9 + r,
							(8 - c) * 9 + (8 - r)
						},
						None => stackalloc[] { r * 9 + c }
					})
					{
						solution[tCell] = '0';
						totalMap.AddAnyway(tCell);
						tempMap.AddAnyway(tCell);
					}
				} while (81 - totalMap.Count > max);

				if (progress is not null)
				{
					progressResult.GeneratingTrial++;
					progress.Report(progressResult);
				}
			} while (!FastSolver.CheckValidity(result = solution.ToString()));

			return SudokuGrid.Parse(result);
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
		/// <param name="countryCode">The country code.</param>
		/// <returns>The task.</returns>
		/// <seealso cref="SymmetryType"/>
		public async Task<SudokuGrid> GenerateAsync(
			int max, SymmetryType symmetricalType, IProgress<IProgressResult> progress,
			CountryCode countryCode = CountryCode.Default) =>
			await Task.Run(() => Generate(max, symmetricalType, progress, countryCode));

		/// <inheritdoc/>
		/// <exception cref="NotImplementedException">Always throws.</exception>
		[EditorBrowsable(EditorBrowsableState.Never)]
		protected override void CreatePattern(int[] pattern) => throw new NotImplementedException();
	}
}
