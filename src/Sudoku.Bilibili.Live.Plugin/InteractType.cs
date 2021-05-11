namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Indicates the interaction message type.
	/// </summary>
	public enum InteractType
	{
		/// <summary>
		/// Indicates the interaction message that is a user entering the live room.
		/// </summary>
		Enter = 1,

		/// <summary>
		/// Indicates the interaction message that he follows you.
		/// </summary>
		Follow,

		/// <summary>
		/// Indicates the interaction message that shares the live room to other platforms.
		/// </summary>
		Share,

		/// <summary>
		/// Indicates the interaction message that is the special following.
		/// </summary>
		SpecialFollow,

		/// <summary>
		/// Indicates the interaction message that is the mutual following.
		/// </summary>
		FollowWithEachOther
	}
}
