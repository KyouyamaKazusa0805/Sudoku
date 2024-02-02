namespace Sudoku.Concepts.ObjectModel;

/// <summary>
/// Represents an element that is the unit data structure of a <see cref="SolvingPath"/>.
/// </summary>
/// <param name="steppingGrid">Indicates the stepping grid.</param>
/// <param name="step">Indicates the current step.</param>
/// <seealso cref="SolvingPath"/>
public readonly partial struct SolvingPathElement([PrimaryConstructorParameter] scoped ref readonly Grid steppingGrid, [PrimaryConstructorParameter] Step step)
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Grid steppingGrid, out Step step) => (steppingGrid, step) = (SteppingGrid, Step);
}
