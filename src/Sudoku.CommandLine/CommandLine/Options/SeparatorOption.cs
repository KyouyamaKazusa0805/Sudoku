namespace Sudoku.CommandLine.Options;

/// <summary>
/// Provides with separator option.
/// </summary>
internal sealed class SeparatorOption : Option<string>, IOption<string>
{
	/// <summary>
	/// Indicates the special characters.
	/// </summary>
	private static readonly Dictionary<string, string> SpecialCharacterNames =
		new() { { "tab", "\t" }, { "space", " " }, { "crlf", "\r\n" }, { "lf", "\n" } };


	/// <summary>
	/// Initializes a <see cref="SeparatorOption"/> instance.
	/// </summary>
	public SeparatorOption() : base(
		["--separator", "-S"],
		ParseArgument,
		false,
		"Specifies the separator; special characters: 'tab' -> '\\t', 'space' -> ' ', 'crlf' -> '\\r\\n', 'lf' -> '\\n'"
	)
	{
		Arity = ArgumentArity.ExactlyOne;
		IsRequired = false;
		SetDefaultValue("\t");
	}


	/// <inheritdoc/>
	public static string ParseArgument(ArgumentResult result)
	{
		if (result.Tokens is not [{ Value: var characterString }])
		{
			result.ErrorMessage = "Argument expected.";
			return default!;
		}

		foreach (var kvp in SpecialCharacterNames)
		{
			if (kvp.Key.Equals(characterString, StringComparison.OrdinalIgnoreCase))
			{
				return kvp.Value;
			}
		}
		return characterString;
	}
}
