namespace Sudoku.Generating;

/// <summary>
/// Represents a progress used by <see cref="IGenerator{TResult}.Generate(IProgress{GeneratorProgress}?, CancellationToken)"/>.
/// </summary>
/// <param name="Count">The number of puzzles generated currently.</param>
/// <seealso cref="IGenerator{TResult}.Generate(IProgress{GeneratorProgress}?, CancellationToken)"/>
public readonly record struct GeneratorProgress(int Count) : IProgressDataProvider<GeneratorProgress>
{
	/// <inheritdoc/>
	string IProgressDataProvider<GeneratorProgress>.ToDisplayString() => Count.ToString();


	/// <inheritdoc/>
	static GeneratorProgress IProgressDataProvider<GeneratorProgress>.Create(int count, int succeeded) => new(count);
}
