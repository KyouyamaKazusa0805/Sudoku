namespace Sudoku.Platforms.QQ.Data.Parsing;

/// <summary>
/// Represents a module argument or module hint attribute.
/// </summary>
[AttributeUsage(AttributeTargets.Property, Inherited = false)]
public sealed class HintAttribute : CommandLineParsingItemAttribute
{
	/// <summary>
	/// Initializes a <see cref="HintAttribute"/> instance via the specified hint text.
	/// </summary>
	/// <param name="hint">The hint.</param>
	public HintAttribute(string hint) => Hint = hint;


	/// <summary>
	/// Indicates the hint text.
	/// </summary>
	public string Hint { get; }
}
