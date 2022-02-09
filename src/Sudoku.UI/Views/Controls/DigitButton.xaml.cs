namespace Sudoku.UI.Views.Controls;

/// <summary>
/// Defines a user control that indicates a digit button.
/// </summary>
public sealed partial class DigitButton : UserControl
{
	/// <summary>
	/// Initializes a <see cref="DigitButton"/> instance.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public DigitButton() => InitializeComponent();


	/// <summary>
	/// Gets or sets the digit to the view model.
	/// </summary>
	public int Digit { get; set; }
}
