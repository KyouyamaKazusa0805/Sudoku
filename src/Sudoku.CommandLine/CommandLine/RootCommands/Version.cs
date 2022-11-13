namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines the type that stores the version options.
/// </summary>
[RootCommand("version", "Displays the version of the current command line project.", IsSpecial = true)]
[SupportedArguments("version", "ver")]
[Usage("version", IsPattern = true)]
public sealed class Version : IExecutable
{
	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			throw new CommandLineRuntimeException((int)ErrorCode.AssemblyNameIsNull);
		}

		await Terminal.WriteLineAsync($"Project {realName}\r\nVersion {version}");
	}
}
