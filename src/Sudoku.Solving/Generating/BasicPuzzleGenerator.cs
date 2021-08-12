namespace Sudoku.Generating;

/// <summary>
/// Encapsulates a puzzle generator, which provides the symmetry type constraint
/// and the maximum clues constraint.
/// </summary>
public sealed class BasicPuzzleGenerator : DiggingPuzzleGenerator
{
	/// <inheritdoc/>
	public override SudokuGrid? Generate() =>
		Generate(28, SymmetryType.Central, null, CountryCode.Default, null);

	/// <summary>
	/// Generate a puzzle with the specified information.
	/// </summary>
	/// <param name="max">The maximum hints of the puzzle.</param>
	/// <param name="symmetricalType">
	/// The symmetry type flags. The <see cref="SymmetryType"/> is
	/// a flag type, you can use bit operators to accumulate multiple
	/// symmetrical types such as
	/// <c><see cref="SymmetryType.AntiDiagonal"/> | <see cref="SymmetryType.Diagonal"/></c>,
	/// which means that the solver will generate anti-diagonal type or
	/// diagonal type puzzles.
	/// </param>
	/// <param name="progress">The progress.</param>
	/// <param name="countryCode">The country code.</param>
	/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
	/// <returns>The grid.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
	/// <seealso cref="SymmetryType"/>
	public SudokuGrid Generate(
		int max, SymmetryType symmetricalType, IProgress<IProgressResult>? progress,
		CountryCode countryCode = CountryCode.Default, CancellationToken? cancellationToken = null)
	{
		PuzzleGeneratingProgressResult
			defaultValue = default,
			pr = new(countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
		ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
		progress?.Report(defaultValue);

		StringBuilder puzzle = new(SudokuGrid.EmptyString), solution = new(SudokuGrid.EmptyString);
		GenerateAnswerGrid(puzzle, solution);

		// Now we remove some digits from the grid.
		var allTypes = symmetricalType.GetAllFlags() ?? new[] { SymmetryType.None };
		int count = allTypes.Length;
		var tempSb = new ValueStringBuilder(solution.ToString());
		string result;
		do
		{
			var selectedType = allTypes[IPuzzleGenerator.Rng.Next(count)];
			tempSb.CopyTo(solution);

			var totalMap = Cells.Empty;
			do
			{
				int cell;
				do cell = IPuzzleGenerator.Rng.Next(0, 81); while (totalMap.Contains(cell));

				int r = cell / 9, c = cell % 9;

				// Get new value of 'last'.
				var tempMap = Cells.Empty;
				foreach (int tCell in selectedType switch
				{
					SymmetryType.Central => stackalloc[] { r * 9 + c, (8 - r) * 9 + 8 - c },
					SymmetryType.Diagonal => stackalloc[] { r * 9 + c, c * 9 + r },
					SymmetryType.AntiDiagonal => stackalloc[] { r * 9 + c, (8 - c) * 9 + 8 - r },
					SymmetryType.XAxis => stackalloc[] { r * 9 + c, (8 - r) * 9 + c },
					SymmetryType.YAxis => stackalloc[] { r * 9 + c, r * 9 + 8 - c },
					SymmetryType.DiagonalBoth => stackalloc[]
					{
						r * 9 + c,
						c * 9 + r,
						(8 - c) * 9 + 8 - r,
						(8 - r) * 9 + 8 - c
					},
					SymmetryType.AxisBoth => stackalloc[]
					{
						r * 9 + c,
						(8 - r) * 9 + c,
						r * 9 + 8 - c,
						(8 - r) * 9 + 8 - c
					},
					SymmetryType.All => stackalloc[]
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
					SymmetryType.None => stackalloc[] { r * 9 + c }
				})
				{
					solution[tCell] = '0';
					totalMap.AddAnyway(tCell);
					tempMap.AddAnyway(tCell);
				}
			} while (81 - totalMap.Count > max);

			if (cancellationToken is { IsCancellationRequested: true })
			{
				throw new OperationCanceledException();
			}

			if (progress is not null)
			{
				progressResult.GeneratingTrial++;
				progress.Report(progressResult);
			}
		} while (!FastSolver.CheckValidity(result = solution.ToString()));

		return SudokuGrid.Parse(result);
	}

	/// <summary>
	/// Generate a puzzle with the specified information asynchronously.
	/// </summary>
	/// <param name="max">The maximum hints of the puzzle.</param>
	/// <param name="symmetricalType">
	/// The symmetry type flags. The <see cref="SymmetryType"/> is
	/// a flag type, you can use bit operators to accumulate multiple
	/// symmetrical types such as
	/// <c><see cref="SymmetryType.AntiDiagonal"/> | <see cref="SymmetryType.Diagonal"/></c>,
	/// which means that the solver will generate anti-diagonal type or
	/// diagonal type puzzles.
	/// </param>
	/// <param name="progress">The progress.</param>
	/// <param name="countryCode">The country code.</param>
	/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
	/// <returns>The task.</returns>
	/// <seealso cref="SymmetryType"/>
	public async Task<SudokuGrid?> GenerateAsync(
		int max, SymmetryType symmetricalType, IProgress<IProgressResult> progress,
		CountryCode countryCode = CountryCode.Default, CancellationToken? cancellationToken = null)
	{
		return await (cancellationToken is { } t ? Task.Run(innerGenerate, t) : Task.Run(innerGenerate));

		SudokuGrid? innerGenerate()
		{
			try
			{
				return Generate(max, symmetricalType, progress, countryCode, cancellationToken);
			}
			catch (OperationCanceledException)
			{
				return null;
			}
		}
	}

	/// <inheritdoc/>
	/// <exception cref="NotImplementedException">Always throws.</exception>
	[EditorBrowsable(EditorBrowsableState.Never)]
	protected override void CreatePattern(ref Span<int> pattern) => throw new NotImplementedException();
}
