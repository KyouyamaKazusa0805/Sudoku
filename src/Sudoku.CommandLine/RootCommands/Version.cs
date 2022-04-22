namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Defines the type that stores the version options.
/// </summary>
public sealed class Version : IVersionCommand
{
	/// <inheritdoc/>
	public static string Name => "version";

	/// <inheritdoc/>
	public static string Description => "Displays the version of the current command line project.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "version" };

	/// <inheritdoc/>
	public static IEnumerable<(string CommandLine, string Meaning)>? UsageCommands =>
		new[] { ("""version""", "Gets the version information.") };


	/// <inheritdoc/>
	public void Execute()
	{
		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			throw new CommandLineRuntimeException((int)ErrorCode.AssemblyNameIsNull);
		}

		Terminal.WriteLine(
			$"""
			Project {realName}
			Version {version}
			"""
		);
	}
}
