namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Indicates the message type.
	/// </summary>
	public enum MessageType
	{
		/// <summary>
		/// Indicates the normal sending a danmaku (floating comment).
		/// </summary>
		Comment,

		/// <summary>
		/// Indicates the gift sending.
		/// </summary>
		GiftSend,

		/// <summary>
		/// Indicates the gift ranking.
		/// </summary>
		GiftTop,

		/// <summary>
		/// Indicates the information that welcomes a user.
		/// </summary>
		Welcome,

		/// <summary>
		/// Indicates the live is starting.
		/// </summary>
		LiveStart,

		/// <summary>
		/// Indicates the live is end.
		/// </summary>
		LiveEnd,

		/// <summary>
		/// Indicates the welcome information for guard.
		/// </summary>
		WelcomeGuard,

		/// <summary>
		/// Indicates the joining on the guarding (i.e. pinyin: shang chuan).
		/// </summary>
		JoinGuarding,

		/// <summary>
		/// Indicates the super chatting message.
		/// </summary>
		SuperChat,

		/// <summary>
		/// Indicates the interaction message from the user.
		/// </summary>
		Interact,

		/// <summary>
		/// Indicates the warning message from the super room manager.
		/// </summary>
		WarningFromSuperRoomManager,

		/// <summary>
		/// Indicates the information is none of those above.
		/// </summary>
		Unknown
	}
}
