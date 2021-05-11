namespace Sudoku.Bilibili.Live.Plugin
{
	/// <summary>
	/// Indicates the guard level.
	/// </summary>
	public enum GuardLevel : byte
	{
		/// <summary>
		/// Indicates the user is none of all guardian levels.
		/// </summary>
		None,

		/// <summary>
		/// Indicates the user is a governer (i.e. pinyin: zong du).
		/// </summary>
		Governer,

		/// <summary>
		/// Indicates the user is prefector (i.e. pinyin: ti du).
		/// </summary>
		Prefector,

		/// <summary>
		/// Indicates the user is captain (i.e. pinyin: jian zhang).
		/// </summary>
		Captain
	}
}
