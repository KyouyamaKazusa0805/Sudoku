try
{
	// Routes the command line arguments to the target root command to be executed.
	RootCommand.Route(args);

	// If succeed, return 0.
	return 0;
}
catch (Exception ex)
{
	// If any unhandled exceptions is thrown, the program will output the error message,
	// with a non-zero return value.
	Terminal.WriteLine(
		$"""
		An error has been encountered.
		
		Error info:
		{ex}
		""",
		ConsoleColor.Red
	);

	return ex switch
	{
		// This exception will only be thrown if the command line arguments is empty.
		InvalidOperationException => -(int)ErrorCode.EmptyCommandLineArguments,

		// This exception will be thrown when the parser has encountered an error
		// that is not related to the runtime.
		CommandLineParserException { ErrorCode: var code } => -(int)code,

		// This exception will be thrown when the routing and exeuction operation has encountered an error.
		CommandLineException { ErrorCode: var code } => -code,

		// Other errors. Just mark this as a "runtime error" and return the corresponding value.
		_ => -(int)ErrorCode.OtherRuntimeError
	};
}
