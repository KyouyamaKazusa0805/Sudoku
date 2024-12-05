namespace Sudoku.Analytics.Construction.Components;

/// <summary>
/// Represents a whip assignment.
/// </summary>
/// <param name="Map">Indicates all candidates used.</param>
/// <param name="Reason">Indicates the reason why the target candidate becomes available.</param>
public abstract record WhipAssignment(ref readonly CandidateMap Map, Technique Reason) :
	IComponent,
	IEqualityOperators<WhipAssignment, WhipAssignment, bool>
{
	/// <inheritdoc/>
	ComponentType IComponent.Type => ComponentType.WhipAssignment;

	/// <inheritdoc/>
	DataStructureType IDataStructure.Type => DataStructureType.None;

	/// <inheritdoc/>
	DataStructureBase IDataStructure.Base => DataStructureBase.None;


	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	protected virtual bool PrintMembers(StringBuilder builder)
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
