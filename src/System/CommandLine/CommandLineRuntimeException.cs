namespace System.CommandLine;

/// <summary>
/// Defines an exception type that will be thrown when an error has been encountered while command line handling.
/// </summary>
/// <param name="errorCode">The error code.</param>
public sealed class CommandLineRuntimeException(int errorCode) : CommandLineException(errorCode);
