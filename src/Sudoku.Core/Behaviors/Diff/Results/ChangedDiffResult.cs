namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a type of digits (given, value or candidate) is changed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
[IntroducedSince(3, 4)]
public abstract class ChangedDiffResult(CandidateMap candidates) : UpdatedDiffResult(candidates)
{
	/// <inheritdoc/>
	public sealed override bool Equals([NotNullWhen(true)] DiffResult? other)
		=> other is ChangedDiffResult comparer && Candidates == comparer.Candidates && EqualityContract == comparer.EqualityContract;

	/// <inheritdoc/>
	public sealed override int GetHashCode() => HashCode.Combine(EqualityContract, Candidates);

	/// <inheritdoc/>
	public sealed override string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		return $"{CellTypeString} digits changed: {converter.CandidateConverter(Candidates)}";
	}
}
