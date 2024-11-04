namespace Sudoku.Behaviors.DiffResults;

/// <summary>
/// Represents a difference that describes a type of digits (given, value or candidate) is removed.
/// </summary>
/// <param name="candidates"><inheritdoc path="/param[@name='candidates']"/></param>
public abstract class RemoveDiffResult(ref readonly CandidateMap candidates) : UpdatedDiffResult(in candidates)
{
	/// <inheritdoc/>
	public sealed override bool Equals([NotNullWhen(true)] DiffResult? other)
		=> other is RemoveDiffResult comparer && Candidates == comparer.Candidates && EqualityContract == comparer.EqualityContract;

	/// <inheritdoc/>
	public sealed override int GetHashCode() => HashCode.Combine(EqualityContract, Candidates);

	/// <inheritdoc/>
	public sealed override string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		return $"{CellTypeString} digits removed: {converter.CandidateConverter(Candidates)}";
	}
}
