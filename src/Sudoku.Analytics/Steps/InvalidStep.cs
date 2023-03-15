namespace Sudoku.Analytics.Steps;

/// <summary>
/// The background type that has implemented the type <see cref="InvalidStep"/>.
/// </summary>
/// <seealso cref="InvalidStep"/>
public sealed class InvalidStep() : Step(default!, default)
{
	/// <inheritdoc/>
	public override decimal BaseDifficulty => throw new NotSupportedException();

	/// <inheritdoc/>
	public override string Name => throw new NotSupportedException();

	/// <inheritdoc/>
	public override string Format => throw new NotSupportedException();

	/// <inheritdoc/>
	public override Technique Code => throw new NotSupportedException();

	/// <inheritdoc/>
	public override TechniqueGroup Group => throw new NotSupportedException();

	/// <inheritdoc/>
	public override DifficultyLevel DifficultyLevel => throw new NotSupportedException();

	/// <inheritdoc/>
	public override ExtraDifficultyCase[]? ExtraDifficultyCases => throw new NotSupportedException();

	/// <inheritdoc/>
	public override IReadOnlyDictionary<string, string[]?>? FormatInterpolatedParts => throw new NotSupportedException();
}
