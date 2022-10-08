using System.Text.Json;
using static Sudoku.SolutionWideReadOnlyFields;

var crosshatch = Crosshatch.Create(3, in CellsMap[9], HousesMap[3] & HousesMap[18]);
Console.WriteLine(crosshatch.ToString());

var json = JsonSerializer.Serialize(crosshatch, new JsonSerializerOptions { WriteIndented = true });
Console.WriteLine(json);

var newInstance = JsonSerializer.Deserialize<Crosshatch>(json);
Console.WriteLine(newInstance.ToString());
Console.WriteLine(crosshatch == newInstance);

#if false
try
{
	// Routes the command line arguments to the target root command to be executed.
	RootCommand.Route(args);

	// If succeed, return 0.
	return 0;
}
catch (CommandLineException ex)
{
	Terminal.WriteLine(
		$"""
		The parsing or runtime operation is unexpected.
		
		{ex}
		""",
		ConsoleColor.Red
	);

	return -ex.ErrorCode;
}
catch (Exception ex)
{
	Terminal.WriteLine(
		$"""
		An unexpected error has been encountered.
		
		{ex}
		""",
		ConsoleColor.Red
	);

	return -(int)ErrorCode.OtherRuntimeError;
}

#endif