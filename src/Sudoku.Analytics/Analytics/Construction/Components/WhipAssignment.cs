namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a whip assignment.
/// </summary>
/// <param name="Map">Indicates all candidates used.</param>
/// <param name="Reason">Indicates the reason why the target candidate becomes available.</param>
public abstract record WhipAssignment(ref readonly CandidateMap Map, Technique Reason) :
	IEqualityOperators<WhipAssignment, WhipAssignment, bool>
{
	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	protected abstract bool PrintMembers(StringBuilder builder);
}
