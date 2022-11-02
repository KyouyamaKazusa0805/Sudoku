namespace Sudoku.Communication.Qicq;

/// <summary>
/// Represents a comparison type that compares for two commands, using key value (<see cref="string"/> values).
/// </summary>
public enum CommandComparison
{
	/// <summary>
	/// Indicates the comparison type is struct. Two commands should compare their <see cref="string"/> value.
	/// They are same if and only if they holds a same <see cref="string"/> value.
	/// </summary>
	Strict,

	/// <summary>
	/// Indicates the comparison type is comparing as prefixes. If the target <see cref="string"/> starts with the specified value,
	/// the comparison will be determined as <see langword="true"/>.
	/// </summary>
	Prefix
}
