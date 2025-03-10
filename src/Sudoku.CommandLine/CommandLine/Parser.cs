namespace Sudoku.CommandLine;

/// <summary>
/// Provides an entry to parse command line raw string, separating them into multiple <see cref="string"/> values in an easy way.
/// </summary>
public static partial class Parser
{
	[GeneratedRegex(@"""((?:[^""\\]|\\.)*)""|'((?:[^'\\]|\\.)*)'|([^\s""']+)", RegexOptions.Compiled)]
	public static partial Regex CommandLineArgsPattern { get; }


	/// <summary>
	/// Parses the specified command line raw values, and return a list of <see cref="string"/> values parsed.
	/// </summary>
	/// <param name="commandLine">The command line.</param>
	/// <returns>A list of <see cref="string"/> values parsed.</returns>
	/// <remarks>
	/// Example:
	/// <code>cmd "hello \"world" 'it\'s me' foo</code>
	/// This command line will return an array of 4 values:
	/// <list type="bullet">
	/// <item><c><![CDATA[cmd]]></c></item>
	/// <item><c><![CDATA[hello \"world]]></c></item>
	/// <item><c><![CDATA[it\'s me]]></c></item>
	/// <item><c><![CDATA[foo]]></c></item>
	/// </list>
	/// You can use quotes <c>"</c> and <c>'</c> to include spaces as valid characters inside an argument value,
	/// and escape sequences <c>\"</c> and <c>\'</c> to include quotes as valid characters.
	/// </remarks>
	public static ReadOnlySpan<string> Parse(string commandLine)
		=>
		from element in CommandLineArgsPattern.Matches(commandLine)
		select element.Value into element
		select element[element[0] is '"' or '\'' ? 1..^1 : ..];
}
