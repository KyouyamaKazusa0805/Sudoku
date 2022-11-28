#pragma warning disable CA1416

using Sudoku.Presentation;
using Sudoku.Presentation.Nodes.Shapes;

// To be updated.
Identifier color = (128, 0, 0, 0);

var sudokuPainter = ISudokuPainter.Create(1000, 20)
	.WithRenderingCandidates(false)
	.WithGrid((Grid)"..8.........2...........7....1..........3..........1....7...........2.........6..")
	.WithPreferenceSettings(static pref => { pref.NeighborSignCellPadding = 8F; pref.NeighborSignsWidth = 6F; })
	.WithNodes(
		new NeighborSignViewNode[]
		{
			new(color, 1, true),
			new(color, 11, true),
			new(color, 16, false),
			new(color, 19, false),
			new(color, 21, true),
			new(color, 25, false),
			new(color, 26, true),
			new(color, 28, true),
			new(color, 30, false),
			new(color, 34, false),
			new(color, 36, false),
			new(color, 37, false),
			new(color, 38, false),
			new(color, 43, true),
			new(color, 45, true),
			new(color, 46, false),
			new(color, 50, false),
			new(color, 59, true),
			new(color, 62, false),
			new(color, 66, false),
			new(color, 69, true),
			new(color, 72, true),
			new(color, 74, true),
			new(color, 76, false)
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
