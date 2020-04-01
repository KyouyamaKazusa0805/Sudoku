using System;
using System.Linq;
using System.Text;
using Sudoku.Data;
using Sudoku.Data.Extensions;
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
		public override IReadOnlyGrid Generate() => Generate(28, Central);

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
		/// <returns>The grid.</returns>
		/// <seealso cref="SymmetryType"/>
		public IReadOnlyGrid Generate(int max, SymmetryType symmetricalType)
		{
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
					do
					{
						cell = Rng.Next(0, 81);
					} while (totalMap[cell]);

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
						DiagonalBoth => new[]
						{
							r * 9 + c, c * 9 + r, (8 - c) * 9 + 8 - r, (8 - r) * 9 + 8 - c
						},
						AxisBoth => new[]
						{
							r * 9 + c, (8 - r) * 9 + c, r * 9 + 8 - c, (8 - r) * 9 + 8 - c
						},
						All => new[]
						{
							r * 9 + c, r * 9 + (8 - c), (8 - r) * 9 + c, (8 - r) * 9 + (8 - c),
							c * 9 + r, c * 9 + (8 - r), (8 - c) * 9 + r, (8 - c) * 9 + (8 - r)
						},
						None => new[] { r * 9 + c },
						_ => throw Throwing.ImpossibleCaseWithMessage("You should not add an option that does not contain in the table of symmetrical types.")
					})
					{
						solution[tCell] = '0';
						totalMap.Add(tCell);
						tempMap.Add(tCell);
					}
				} while (81 - totalMap.Count > max);
			} while (!FastSolver.CheckValidity(result = solution.ToString(), out _));

			return Grid.Parse(result);
		}

		/// <inheritdoc/>
		/// <exception cref="NotImplementedException">
		/// Throws always.
		/// </exception>
		protected override void CreatePattern(int[] pattern) => throw new NotImplementedException();
	}
}
