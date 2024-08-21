namespace SudokuStudio.Drawing;

/// <summary>
/// Represents drawing context.
/// </summary>
/// <param name="sudokuPane">Indicates the sudoku pane.</param>
/// <param name="controlAddingActions">Indicates the control adding actions. The collection can be used by playing animation.</param>
[TypeImpl(TypeImplFlag.AllObjectMethods)]
internal readonly ref partial struct DrawingContext(
	[PrimaryConstructorParameter] SudokuPane sudokuPane,
	[PrimaryConstructorParameter] AnimatedResultCollection controlAddingActions
)
{
	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	public void Deconstruct(out SudokuPane pane, out AnimatedResultCollection actions)
		=> (pane, actions) = (SudokuPane, ControlAddingActions);
}
