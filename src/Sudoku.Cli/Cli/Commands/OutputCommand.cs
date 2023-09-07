using System.CommandLine;
using Sudoku.Cli.Converters;
using Sudoku.Cli.Options;
using Sudoku.Drawing;

namespace Sudoku.Cli.Commands;

/// <summary>
/// Indicates a command that output puzzles to the local path.
/// </summary>
public sealed class OutputCommand : Command, ICommand<OutputCommand>
{
	/// <summary>
	/// Creates a <see cref="OutputCommand"/> instance.
	/// </summary>
	public OutputCommand() : base("output", "Output the puzzle into the local path.")
	{
		var gridOption = IOption<GridOption, Grid, GridArgumentConverter>.CreateOption();
		var pathOption = IOption<PathOption, string>.CreateOption();
		AddOption(gridOption);
		AddOption(pathOption);
		this.SetHandler(static async (grid, path) =>
		{
			if (File.Exists(path))
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Sorry. The path '{path}' already exists. Please check the path and retry later.");
				Console.ResetColor();
				return;
			}

			var parentDirectory = Path.GetDirectoryName(path);
			if (parentDirectory is null)
			{
				Console.ForegroundColor = ConsoleColor.Red;
				Console.WriteLine($"Sorry. The path '{path}' is invalid. Please check the path and retry later.");
				Console.ResetColor();
				return;
			}

			if (!Directory.Exists(parentDirectory))
			{
				Directory.CreateDirectory(parentDirectory);
			}

			switch (Path.GetExtension(path))
			{
				case ".txt":
				{
					await File.WriteAllTextAsync(path, grid.ToString());
					break;
				}
				case ".jpg" or ".jpeg" or ".png" or ".bmp" or ".gif" or ".wmf":
				{
					ISudokuPainter.Create(1000, 20)
						.WithGrid(grid)
						.WithFontScale(1, .4M)
						.SaveTo(path);

					break;
				}
				case var extension:
				{
					Console.ForegroundColor = ConsoleColor.Red;
					Console.WriteLine(
						$"""
						Sorry. The extension '{extension}' is not supported. All supported extensions are: 
						*.jpg, *.jpeg, *.png, *.gif, *.bmp, *.txt and *.wmf.
						""".RemoveLineEndings()
					);
					Console.ResetColor();
					return;
				}
			}
		}, gridOption, pathOption);
	}
}
