namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a progress data type that is nearly same as <see cref="GeneratorProgress"/>, but with filtered data.
/// </summary>
/// <param name="Count">The number of checked puzzles.</param>
/// <param name="Succeeded">The number of succeeded puzzles.</param>
/// <seealso cref="GeneratorProgress"/>
public readonly record struct FilteredGeneratorProgress(int Count, int Succeeded) : IProgressDataProvider<FilteredGeneratorProgress>
{
	/// <summary>
	/// Indicates the percentage.
	/// </summary>
	public double Percentage => Succeeded / (double)Count;


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	string IProgressDataProvider<FilteredGeneratorProgress>.ToDisplayString() => $"{Succeeded}/{Count} ({Percentage:P2})";


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static FilteredGeneratorProgress IProgressDataProvider<FilteredGeneratorProgress>.Create(int count, int succeeded) => new(count, succeeded);
}
