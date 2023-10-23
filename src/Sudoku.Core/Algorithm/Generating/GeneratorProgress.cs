namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a progress used by <see cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)"/>.
/// </summary>
/// <param name="Count">The number of puzzles generated currently.</param>
/// <seealso cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)"/>
public readonly record struct GeneratorProgress(int Count);
