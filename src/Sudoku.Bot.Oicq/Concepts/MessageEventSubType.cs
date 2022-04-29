namespace Sudoku.Bot.Oicq.Concepts;

/// <summary>
/// Indicates the subtype of the event that is not defined by <see href="https://onebot.dev/">Onebot</see> protocol.
/// </summary>
public enum MessageEventSubType
{
	/// <summary>
	/// Indicates the subtype is from a friend. The subtype is used for a C2C window.
	/// </summary>
	FRIEND,

	/// <summary>
	/// Indicates the subtype is from a group (temporary message). The subtype is used for a C2C window.
	/// </summary>
	GROUP,

	/// <summary>
	/// Indicates the subtype is from other people. The subtype is used for a C2C window.
	/// </summary>
	OTHER,

	/// <summary>
	/// Indicates the subtype is a normal message. The subtype is used for a QQ group window.
	/// </summary>
	NORMAL,

	/// <summary>
	/// Indicates the subtype is an anonymous message. The subtype is used for a QQ group window.
	/// </summary>
	ANONYMOUS,

	/// <summary>
	/// Indicates the subtype is a noticce message. The subtype is used for a QQ group window.
	/// </summary>
	NOTICE
}
