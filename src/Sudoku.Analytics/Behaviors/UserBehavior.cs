namespace Sudoku.Behaviors;

/// <summary>
/// Represents a kind of user behavior.
/// </summary>
/// <remarks><include file="../../global-doc-comments.xml" path="/g/flags-attribute"/></remarks>
/// <seealso cref="FlagsAttribute"/>
/// <completionlist cref="UserBehaviors"/>
[Flags]
public enum UserBehavior : byte
{
	/// <summary>
	/// Indicates a user doesn't contain an explicit behavior.
	/// </summary>
	None = 0,

	/// <summary>
	/// Indicates a user like to finish a puzzle block by block.
	/// </summary>
	BlockFirst = 1 << 0,

	/// <summary>
	/// Indicates a user like to finish a puzzle digit by digit.
	/// </summary>
	DigitFirst = 1 << 1
}
