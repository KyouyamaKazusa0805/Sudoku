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
