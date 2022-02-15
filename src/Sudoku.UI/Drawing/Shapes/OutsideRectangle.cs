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
	/// <param name="paneSize">The pane size.</param>
	/// <param name="strokeThickness">The stroke thickness.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public OutsideRectangle(Color strokeColor, double paneSize, double strokeThickness) =>
		_rect = new()
		{
			Width = paneSize,
			Height = paneSize,
			Stroke = new SolidColorBrush(strokeColor),
			StrokeThickness = strokeThickness
		};


	/// <summary>
	/// The size of the rectangle.
	/// </summary>
	public double RectangleSize
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _rect.Width;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => _rect.Width = _rect.Height = value;
	}

	/// <summary>
	/// The stroke thickness of the block line.
	/// </summary>
	public double StrokeThickness
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
	public override bool Equals([NotNullWhen(true)] DrawingElement? other) =>
		other is OutsideRectangle comparer && ReferenceEquals(_rect, comparer._rect);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => HashCode.Combine(nameof(OutsideRectangle), _rect.GetHashCode());

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override Rectangle GetControl() => _rect;
}
