namespace System.CommandLine;

/// <summary>
/// Defines a meta parser that parses the raw string in a single line, separating them into multiple arguments using spaces and quotes.
/// </summary>
/// <remarks>
/// This type uses a method by regular expressions to parse strings, referenced from StackOverflow.
/// For more information please visit <see href="https://stackoverflow.com/a/59638741/13613782">this link</see>.
/// </remarks>
public static partial class MetaParser
{
	/// <summary>
	/// Parses a line of command, separating the command line into multiple <see cref="string"/> arguments using spaces and quotes.
	/// </summary>
	/// <param name="commandLine">The command line.</param>
	/// <returns>Parsed arguments, represented as an array of <see cref="string"/> values.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static string[] Parse(string commandLine)
		=> from match in CommandArgumentsPattern().Matches(commandLine) select match.Value.Trim('"');

	[GeneratedRegex("""[\""].+?[\""]|[^ ]+""", RegexOptions.Singleline | RegexOptions.Compiled, 5000)]
	private static partial Regex CommandArgumentsPattern();
}
