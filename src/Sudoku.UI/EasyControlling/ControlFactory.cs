namespace Sudoku.UI;

/// <summary>
/// Defines a control factory.
/// </summary>
internal static class ControlFactory
{
	/// <summary>
	/// Creates a <see cref="Microsoft.UI.Xaml.Controls.InfoBar"/> instance.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <returns>The <see cref="Microsoft.UI.Xaml.Controls.InfoBar"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static InfoBar InfoBar(InfoBarSeverity severity) =>
		new()
		{
			Title = StringResource.Get(
				severity switch
				{
					InfoBarSeverity.Informational => "SudokuPage_InfoBar_SeverityInfo",
					InfoBarSeverity.Success => "SudokuPage_InfoBar_SeveritySuccess",
					InfoBarSeverity.Warning => "SudokuPage_InfoBar_SeverityWarning",
					InfoBarSeverity.Error => "SudokuPage_InfoBar_SeverityError"
				}
			),
			Severity = severity,
			IsOpen = false,
			Margin = new(25, 30, 30, 0),
			IsClosable = true,
			IsIconVisible = true
		};
}
