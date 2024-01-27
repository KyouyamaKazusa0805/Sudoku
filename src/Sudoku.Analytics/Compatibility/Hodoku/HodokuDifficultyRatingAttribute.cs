namespace Sudoku.Compatibility.Hodoku;

/// <summary>
/// Defines an attribute that is applied to a field in technique, indicating difficulty rating value defined by Hodoku.
/// </summary>
/// <param name="difficultyRating">Indicates the difficulty rating.</param>
/// <param name="difficultyLevel">Indicates the difficulty level.</param>
[AttributeUsage(AttributeTargets.Field, Inherited = false)]
public sealed partial class HodokuDifficultyRatingAttribute(
	[RecordParameter] int difficultyRating,
	[RecordParameter] HodokuDifficultyLevel difficultyLevel
) : Attribute
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out int difficultyRating, out HodokuDifficultyLevel difficultyLevel)
		=> (difficultyRating, difficultyLevel) = (DifficultyRating, DifficultyLevel);
}
