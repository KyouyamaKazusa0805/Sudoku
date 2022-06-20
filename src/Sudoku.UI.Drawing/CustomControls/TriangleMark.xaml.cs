namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a star sign.
/// </summary>
public sealed partial class TriangleMark : GridLayout
{
	/// <summary>
	/// Initializes a <see cref="TriangleMark"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public TriangleMark() => InitializeComponent();


	/// <inheritdoc cref="Shape.Fill"/>
	public Brush Fill
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _cPath.Fill;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			if (Fill == value)
			{
				return;
			}

			_cPath.Fill = value;
		}
	}
}
