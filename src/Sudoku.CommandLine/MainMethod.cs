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
