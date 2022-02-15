using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.UI;

namespace Sudoku.UI.Drawing.Shapes;

/// <summary>
/// Defines an outside rectangle.
/// </summary>
public sealed class OutsideRectangle : DrawingElement
{
	/// <summary>
	/// The inner rectangle.
	/// </summary>
	private readonly Rectangle _rect;


	/// <summary>
	/// Initializes an <see cref="OutsideRectangle"/> instance via the specified details.
	/// </summary>
	/// <param name="strokeColor">The stroke color.</param>
	/// <param name="width">The width.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OutsideRectangle(Color strokeColor, double width) =>
		_rect = new()
		{
			Stroke = new SolidColorBrush(strokeColor),
			StrokeThickness = width
		};


	/// <summary>
	/// The stroke width of the block line.
	/// </summary>
	public double Width
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _rect.StrokeThickness;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _rect.StrokeThickness = value;
	}

	/// <summary>
	/// The stroke color of the block line.
	/// </summary>
	public Color StrokeColor
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ((SolidColorBrush)_rect.Stroke).Color;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _rect.Stroke = new SolidColorBrush(value);
	}


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Rectangle GetControl() => _rect;
}
