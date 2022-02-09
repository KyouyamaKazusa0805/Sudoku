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


	/// <summary>
	/// Occurs when the control is clicked.
	/// </summary>
	public event RoutedEventHandler? Click;


	/// <summary>
	/// Pass the arguments and trigger the event <see cref="Click"/>.
	/// </summary>
	/// <param name="sender">The object to trigger the event.</param>
	/// <param name="e">The event argument provided.</param>
	/// <seealso cref="Click"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void Button_Click(object sender, RoutedEventArgs e) => Click?.Invoke(sender, e);
}
