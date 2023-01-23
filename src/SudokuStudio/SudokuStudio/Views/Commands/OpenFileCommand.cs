namespace SudokuStudio.Views.Commands;

/// <summary>
/// Defines an open-file command.
/// </summary>
public sealed class OpenFileCommand : ButtonCommand
{
	/// <inheritdoc/>
	public override async void Execute(object? parameter)
	{
		if (parameter is not AnalyzePage page)
		{
			return;
		}

		await page.OpenFileInternalAsync();
	}
}
