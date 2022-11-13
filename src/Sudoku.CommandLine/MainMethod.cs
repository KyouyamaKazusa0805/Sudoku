const int statusSuccess = 0;

try
{
	RootCommand.Route(args);
	return statusSuccess;
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
