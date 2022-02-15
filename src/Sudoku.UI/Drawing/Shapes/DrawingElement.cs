namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a drawing element that represents a sudoku information.
/// </summary>
public abstract class DrawingElement
{
	/// <summary>
	/// Provides a way to assign the inner properties using the reflection via the specified parameters.
	/// </summary>
	/// <param name="objectHandler">The handler that checks and changes the inner value.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void DynamicAssign(Action<dynamic> objectHandler) => objectHandler(this);

	/// <summary>
	/// Gets the inner control.
	/// </summary>
	/// <returns>The control.</returns>
	public abstract UIElement GetControl();
}
