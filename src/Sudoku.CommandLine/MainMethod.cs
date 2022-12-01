#pragma warning disable CA1416

using Sudoku.Presentation.Nodes;

var sudokuPainter = ISudokuPainter.Create(1000, 20)
	.WithPreferenceSettings(static pref => pref.FigurePadding = 20F)
	.WithRenderingCandidates(false)
	.WithGrid(Grid.Empty)
	.WithNodes(
		new FigureViewNode[]
		{
			new SquareViewNode((96, 0, 0, 0), 0),
			new TriangleViewNode((96, 0, 0, 0), 10),
			new CircleViewNode((96, 0, 0, 0), 20),
			new DiamondViewNode((96, 0, 0, 0), 30)
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
