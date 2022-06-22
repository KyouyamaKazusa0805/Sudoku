namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines a mark type.
/// </summary>
internal interface IMark
{
	/// <summary>
	/// Indicates the visibility of the shape.
	/// </summary>
	bool ShowMark { get; set; }

	/// <summary>
	/// Indicates the shape kind.
	/// </summary>
	ShapeKind ShapeKind { get; set; }
	
	/// <summary>
	/// Indicates the default margin.
	/// </summary>
	protected static abstract Thickness DefaultMargin { get; }

	/// <summary>
	/// Indicates the default margin value that is applied to a built-in control.
	/// </summary>
	protected static abstract Thickness BuiltinShapeDefaultMargin { get; }


	/// <summary>
	/// Try to get all controls used.
	/// </summary>
	/// <returns>All controls.</returns>
	FrameworkElement[] GetControls();

	/// <summary>
	/// Gets the control via the specified shape kind.
	/// </summary>
	/// <param name="shapeKind">The shape kind.</param>
	/// <returns>The target control. If none found, <see langword="null"/>.</returns>
	private protected abstract FrameworkElement? GetControlViaShapeKind(ShapeKind shapeKind);
}
