namespace Sudoku.CommandLine;

/// <summary>
/// Represents an argument parser object.
/// </summary>
public abstract class ArgumentParser
{
	/// <summary>
	/// Indicates the number of arguments supported to be recognized.
	/// </summary>
	public abstract int SupportedArgumentsCount { get; }

	/// <summary>
	/// Indicates the aliased character. If unsupported, the value can be <see langword="null"/>.
	/// </summary>
	public abstract char? AliasedCharacter { get; }

	/// <summary>
	/// Indicates the supported name.
	/// </summary>
	public abstract string Name { get; }
}
