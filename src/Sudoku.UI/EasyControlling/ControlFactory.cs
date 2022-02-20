namespace Sudoku.UI;

/// <summary>
/// Defines a control factory.
/// </summary>
internal static class ControlFactory
{
	/// <summary>
	/// Creates a <see cref="InfoBar"/> instance.
	/// </summary>
	/// <param name="severity">The severity of the info bar.</param>
	/// <param name="baseStackPanel">The base stack panel.</param>
	/// <returns>The <see cref="InfoBar"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static InfoBar CreateInfoBar(InfoBarSeverity severity, StackPanel? baseStackPanel = null)
	{
		// Create the target control.
		var targetControl = new InfoBar
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

		// Add handler that removes the control after close the info bar.
		if (baseStackPanel is not null)
		{
			baseStackPanel.Children.Insert(0, targetControl);
			targetControl.Closed +=
				(s, e) => _ = e.Reason == InfoBarCloseReason.CloseButton && baseStackPanel.Children.Remove(s);
		}

		return targetControl;
	}
}
