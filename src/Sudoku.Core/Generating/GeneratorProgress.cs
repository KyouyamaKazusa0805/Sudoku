namespace Sudoku.Generating;

/// <summary>
/// Represents a progress used by <see cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)"/>.
/// </summary>
/// <param name="Count">The number of puzzles generated currently.</param>
/// <seealso cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)"/>
public readonly record struct GeneratorProgress(int Count) : IProgressDataProvider<GeneratorProgress>
{
	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	string IProgressDataProvider<GeneratorProgress>.ToDisplayString() => Count.ToString();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	static GeneratorProgress IProgressDataProvider<GeneratorProgress>.Create(int count, int succeeded) => new(count);
}
