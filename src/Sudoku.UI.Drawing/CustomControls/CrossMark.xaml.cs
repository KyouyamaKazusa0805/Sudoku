namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a cross mark sign.
/// </summary>
public sealed partial class CrossMark : GridLayout
{
	/// <summary>
	/// Initializes a <see cref="CrossMark"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CrossMark() => InitializeComponent();


	/// <inheritdoc cref="Shape.StrokeThickness"/>
	public double StrokeThickness
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cPath.StrokeThickness;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (StrokeThickness.NearlyEquals(value, 1E-2))
			{
				return;
			}

			_cPath.StrokeThickness = value;
		}
	}

	/// <inheritdoc cref="Shape.Stroke"/>
	public Brush Stroke
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cPath.Stroke;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (Stroke == value)
			{
				return;
			}

			_cPath.Stroke = value;
		}
	}
}
