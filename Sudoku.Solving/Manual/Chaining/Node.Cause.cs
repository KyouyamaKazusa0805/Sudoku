namespace Sudoku.Solving.Manual.Chaining
{
	public sealed partial class Node
	{
		/// <summary>
		/// The cause.
		/// </summary>
		public enum Cause : byte
		{
			/// <summary>
			/// Indicates the cause is a naked single.
			/// </summary>
			NakedSingle,

			/// <summary>
			/// Indicates the cause is a hidden single in row.
			/// </summary>
			HiddenSingleRow,

			/// <summary>
			/// Indicates the cause is a hidden single in column.
			/// </summary>
			HiddenSingleColumn,

			/// <summary>
			/// Indicates the cause is a hidden single in block.
			/// </summary>
			HiddenSingleBlock,

			/// <summary>
			/// Indicates the cause is advanced.
			/// </summary>
			Advanced,
		}
	}
}
