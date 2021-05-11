namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Provides the commands that is displayed in the danmaku JSON being received.
	/// </summary>
	public static class DanmakuJsonCommands
	{
		/// <summary>
		/// Indicates the live command.
		/// </summary>
		public const string Live = "LIVE";

		/// <summary>
		/// Indicates the preparing command.
		/// </summary>
		public const string Preparing = "PREPARING";

		/// <summary>
		/// Indicats the danmaku message command.
		/// </summary>
		public const string DanmakuMessage = "DANMU_MSG";

		/// <summary>
		/// Indicates the sending gift command.
		/// </summary>
		public const string SendingGift = "SEND_GIFT";

		/// <summary>
		/// Indicates the sending gift with combo command.
		/// </summary>
		public const string ComboSending = "COMBO_SEND";

		/// <summary>
		/// Indicates the gift ranking command.
		/// </summary>
		public const string GiftRanking = "GIFT_TOP";

		/// <summary>
		/// Indicates the welcome command.
		/// </summary>
		public const string Welcome = "WELCOME";

		/// <summary>
		/// Indicates the guard-welcome command.
		/// </summary>
		public const string WelcomeGuard = "WELCOME_GUARD";

		/// <summary>
		/// Indicates the entry effect command.
		/// </summary>
		public const string EntryEffect = "ENTRY_EFFECT";

		/// <summary>
		/// Indicates the guard buy command.
		/// </summary>
		public const string GuardBuy = "GUARD_BUY";

		/// <summary>
		/// Indicates the super chat command.
		/// </summary>
		public const string SuperChatMessage = "SUPER_CHAT_MESSAGE";

		/// <summary>
		/// Indicates the super chat (JP) command.
		/// </summary>
		public const string SuperChatMessageJp = "SUPER_CHAT_MESSAGE_JP";

		/// <summary>
		/// Indicates the interaction command.
		/// </summary>
		public const string Interact = "INTERACT_WORD";

		/// <summary>
		/// Indicates the warning message command that is from a super room manager.
		/// </summary>
		public const string WarningFromSuperRoomManager = "WARNING";

		/// <summary>
		/// Indicates the cut-off (live ending) message.
		/// </summary>
		public const string LiveEnd = "CUT_OFF";
	}
}
