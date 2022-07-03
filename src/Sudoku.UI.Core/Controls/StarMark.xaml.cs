namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a star sign.
/// </summary>
public sealed partial class StarMark : GridLayout
{
	/// <summary>
	/// Initializes a <see cref="StarMark"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public StarMark() => InitializeComponent();


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
