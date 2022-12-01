#pragma warning disable

using Sudoku.Presentation;
using Sudoku.Presentation.Nodes.Shapes;

Identifier identifier = (192, 0, 0, 0);
var sudokuPainter = ISudokuPainter.Create(1000, 20)
	.WithGrid((Grid)"...71...33.9........19..6..6..8.....1...4...9.....6..5..3..52........5.77...69...")
	.WithRenderingCandidates(false)
	.WithPreferenceSettings(static pref => pref.QuadrupleMaxArrowSize = 20F)
	.WithNodes(
		new QuadrupleMaxArrowViewNode[]
		{
			new(identifier, 5, Direction.TopRight),
			new(identifier, 6, Direction.TopLeft),
			new(identifier, 13, Direction.TopRight),
			new(identifier, 16, Direction.BottomLeft),
			new(identifier, 22, Direction.TopLeft),
			new(identifier, 25, Direction.TopLeft),
			new(identifier, 32, Direction.BottomLeft),
			new(identifier, 33, Direction.TopRight),
			new(identifier, 37, Direction.TopRight),
			new(identifier, 38, Direction.TopLeft),
			new(identifier, 45, Direction.BottomLeft),
			new(identifier, 48, Direction.TopRight),
			new(identifier, 54, Direction.BottomLeft),
			new(identifier, 57, Direction.BottomRight),
			new(identifier, 64, Direction.TopLeft),
			new(identifier, 65, Direction.TopLeft)
		}
	);

var desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
sudokuPainter.SaveTo($"""{desktop}\temp.png""");

return 0;

// Registers the resource fetching.
R.AddExternalResourceFetecher(typeof(Program).Assembly, static key => Resources.ResourceManager.GetString(key));

// Parse and route commands.
try
{
	await RootCommand.RouteAsync(args);
	return 0;
}
catch (CommandLineException ex)
{
	Terminal.WriteLine($"The parsing or runtime operation is unexpected.\r\n\r\n{ex}", ConsoleColor.Red);
	return -ex.ErrorCode;
}
catch (Exception ex)
{
	Terminal.WriteLine($"An unexpected error has been encountered.\r\n\r\n{ex}", ConsoleColor.Red);
	return -(int)ErrorCode.OtherRuntimeError;
}
