namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a diamond mark sign.
/// </summary>
public sealed partial class DiamondMark : GridLayout
{
	/// <summary>
	/// Initializes a <see cref="DiamondMark"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DiamondMark() => InitializeComponent();


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
