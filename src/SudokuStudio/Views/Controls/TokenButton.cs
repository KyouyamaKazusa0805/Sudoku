using Microsoft.UI.Xaml.Controls.Primitives;

namespace SudokuStudio.Views.Controls;

/// <summary>
/// Represents for a button used in a token view.
/// </summary>
public sealed class TokenButton : ToggleButton
{
	/// <summary>
	/// Initializes a <see cref="TokenButton"/> instance.
	/// </summary>
	public TokenButton() : base()
	{
		CornerRadius = new(ActualHeight != 0 ? ActualHeight / 2 : 4);
		IsThreeState = false;
	}
}
