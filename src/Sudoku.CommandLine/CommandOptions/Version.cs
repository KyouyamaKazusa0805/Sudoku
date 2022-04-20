namespace Sudoku.CommandLine.CommandOptions;

/// <summary>
/// Defines the type that stores the version options.
/// </summary>
public sealed class Version : IVersionCommand<ErrorCode>
{
	/// <inheritdoc/>
	public static string Name => "version";

	/// <inheritdoc/>
	public static string Description => "Displays the version of the current command line project.";

	/// <inheritdoc/>
	public static string[] SupportedCommands => new[] { "version" };

	/// <inheritdoc/>
	public static IEnumerable<IRootCommand<ErrorCode>>? UsageCommands => new[] { new Version() };


	/// <inheritdoc/>
	public ErrorCode Execute()
	{
		if (typeof(Version).Assembly.GetName() is not { Name: { } realName, Version: var version })
		{
			// Returns an error that cannot fetch the assembly name correctly.
			return ErrorCode.AssemblyNameIsNull;
		}

		Console.WriteLine(
			$"""
			Project {realName}
			Version {version}
			"""
		);
		return ErrorCode.None;
	}
}
