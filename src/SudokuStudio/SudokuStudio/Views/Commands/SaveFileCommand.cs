namespace SudokuStudio.Views.Commands;

/// <summary>
/// Defines a save-file command.
/// </summary>
public sealed class SaveFileCommand : ButtonCommand
{
	/// <inheritdoc/>
	public override async void Execute(object? parameter)
	{
		if (parameter is not AnalyzePage page)
		{
			return;
		}

		await page.SaveFileInternalAsync();
	}
}
