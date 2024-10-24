namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a type that is used by whip chaining, describing the target candidate, and why it can become available.
/// </summary>
/// <param name="Candidate">Indicates the target candidate.</param>
/// <param name="Reason">Indicates the reason why the target candidate becomes available.</param>
public readonly record struct WhipAssignment(Candidate Candidate, Technique Reason)
{
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	private bool PrintMembers(StringBuilder builder)
	{
		var converter = CoordinateConverter.InvariantCultureInstance;
		builder.Append($"{nameof(Candidate)} = ");
		builder.Append(converter.CandidateConverter([Candidate]));
		builder.Append($", {nameof(Reason)} = ");
		builder.Append(Reason.GetName(null));
		return true;
	}
}
