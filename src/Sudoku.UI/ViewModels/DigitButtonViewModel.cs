using Sudoku.UI.Views.Controls;

namespace Sudoku.UI.ViewModels;

/// <summary>
/// Indicates the view model that binds with the <see cref="DigitButton"/> instances.
/// </summary>
/// <seealso cref="DigitButton"/>
internal sealed class DigitButtonViewModel : NotificationObject
{
	private int _digit;


	/// <summary>
	/// Gets or sets the digit. The digit value must between 0 and 9, 0 is for the empty content.
	/// </summary>
	public int Digit
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _digit;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set => SetProperty(ref _digit, value);
	}
}
