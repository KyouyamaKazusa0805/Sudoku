namespace Sudoku.Behaviors.Diff.Results;

/// <summary>
/// Represents a difference that describes a type of digits (given, value or candidate) is added.
/// </summary>
/// <param name="candidates">
/// <inheritdoc cref="UpdatedDiffResult(CandidateMap)" path="/param[@name='candidates']"/>
/// </param>
/// <param name="areCorrect">Indicates whether the digits are correct to be added.</param>
public abstract partial class AddDiffResult(CandidateMap candidates, [Property] bool areCorrect) : UpdatedDiffResult(candidates)
{
	/// <inheritdoc/>
	public sealed override bool Equals([NotNullWhen(true)] DiffResult? other)
		=> other is AddDiffResult comparer
		&& Candidates == comparer.Candidates && AreCorrect == comparer.AreCorrect
		&& EqualityContract == comparer.EqualityContract;

	/// <inheritdoc/>
	public sealed override int GetHashCode() => HashCode.Combine(EqualityContract, Candidates, AreCorrect);

	/// <inheritdoc/>
	public sealed override string ToString(IFormatProvider? formatProvider)
	{
		var converter = CoordinateConverter.GetInstance(formatProvider);
		var areCorrectedString = AreCorrect ? string.Empty : ", incorrect";
		return $"{CellTypeString} digits added: {converter.CandidateConverter(Candidates)}{areCorrectedString}";
	}
}
