namespace Sudoku.Generating;

/// <summary>
/// Provides a puzzle generator with the technique filter.
/// </summary>
public sealed class TechniqueFilteringPuzzleGenerator : HardPatternPuzzleGenerator
{
	/// <summary>
	/// Indicates the default filter.
	/// </summary>
	private static readonly TechniqueCodeFilter DefaultFilter = new(
		Technique.LastDigit, Technique.FullHouse,
		Technique.HiddenSingleRow, Technique.HiddenSingleColumn,
		Technique.HiddenSingleBlock, Technique.NakedSingle,
		Technique.NakedPair, Technique.NakedPairPlus,
		Technique.HiddenPair, Technique.LockedPair,
		Technique.NakedTriple, Technique.NakedTriplePlus,
		Technique.HiddenTriple, Technique.LockedTriple,
		Technique.NakedQuadruple, Technique.NakedQuadruplePlus,
		Technique.HiddenQuadruple
	);

	/// <summary>
	/// The default manual solver.
	/// </summary>
	private static readonly ManualSolver ManualSolver = new();


	/// <inheritdoc/>
	public override SudokuGrid? Generate() => Generate(DefaultFilter, null, CountryCode.Default);


	/// <summary>
	/// To generate a puzzle that contains the specified technique code.
	/// </summary>
	/// <param name="techniqueCodeFilter">
	/// The technique codes to filter. If the parameter is <see langword="null"/>,
	/// the process will use the default filter.
	/// </param>
	/// <param name="progress">The progress.</param>
	/// <param name="countryCode">The country code.</param>
	/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
	/// <returns>The puzzle.</returns>
	/// <exception cref="OperationCanceledException">Throws when the operation is cancelled.</exception>
	public SudokuGrid? Generate(
		TechniqueCodeFilter? techniqueCodeFilter, IProgress<IProgressResult>? progress,
		CountryCode countryCode = CountryCode.Default, CancellationToken? cancellationToken = null)
	{
		PuzzleGeneratingProgressResult
			defaultValue = default,
			pr = new(0, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
		ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
		progress?.Report(progressResult);

		techniqueCodeFilter ??= DefaultFilter;

		while (true)
		{
			var puzzle = Generate(-1, progress, countryCode: countryCode, cancellationToken: cancellationToken);
			if (puzzle is null)
			{
				continue;
			}

			if (ManualSolver.Solve(puzzle.Value).Any(step => techniqueCodeFilter.Contains(step.TechniqueCode)))
			{
				return puzzle;
			}
		}
	}

	/// <summary>
	/// To generate a puzzle that contains the specified technique code asynchronously.
	/// </summary>
	/// <param name="techniqueCodeFilter">
	/// The technique codes to filter. If the parameter is <see langword="null"/>,
	/// the process will use the default filter.
	/// </param>
	/// <param name="progress">The progress.</param>
	/// <param name="countryCode">The globalization string.</param>
	/// <param name="cancellationToken">The cancellation token used for cancelling an operation.</param>
	/// <returns>The task.</returns>
	public async Task<SudokuGrid?> GenerateAsync(
		TechniqueCodeFilter? techniqueCodeFilter, IProgress<IProgressResult>? progress,
		CountryCode countryCode = CountryCode.Default, CancellationToken? cancellationToken = null)
	{
		return await (cancellationToken is { } t ? Task.Run(innerGenerate, t) : Task.Run(innerGenerate));

		SudokuGrid? innerGenerate()
		{
			try
			{
				return Generate(techniqueCodeFilter, progress, countryCode, cancellationToken);
			}
			catch (OperationCanceledException)
			{
				return null;
			}
		}
	}
}
