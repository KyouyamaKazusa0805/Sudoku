namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a type that is used by whip chaining, describing the target candidate, and why it can become available.
/// </summary>
/// <param name="Candidate">Indicates the target candidate.</param>
/// <param name="Reason"><inheritdoc cref="WhipAssignment.Reason" path="/summary"/></param>
public sealed record NormalWhipAssignment(Candidate Candidate, Technique Reason) : WhipAssignment(Candidate.AsCandidateMap(), Reason)
{
	/// <inheritdoc/>
	protected override bool PrintMembers(StringBuilder builder)
	{
		var converter = CoordinateConverter.InvariantCultureInstance;
		if (Reason == Technique.None)
		{
			builder.Append(converter.CandidateConverter(Map));
		}
		else
		{
			builder.Append($"{nameof(Candidate)} = ");
			builder.Append(converter.CandidateConverter(Map));
			builder.Append($", {nameof(Reason)} = ");
			builder.Append(Reason == Technique.None ? "<none>" : Reason.GetName(null));
		}
		return true;
	}
}
