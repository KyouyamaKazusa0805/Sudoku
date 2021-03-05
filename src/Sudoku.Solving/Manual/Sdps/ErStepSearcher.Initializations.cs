namespace Sudoku.Solving.Manual.Sdps
{
	partial class ErStepSearcher
	{
		/// <summary>
		/// Indicates all regions iterating on the specified block
		/// forming an empty rectangle.
		/// </summary>
		private static readonly int[,] LinkIds =
		{
			{ 12, 13, 14, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
			{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
			{ 12, 13, 14, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
			{ 9, 10, 11, 15, 16, 17, 21, 22, 23, 24, 25, 26 },
			{ 9, 10, 11, 15, 16, 17, 18, 19, 20, 24, 25, 26 },
			{ 9, 10, 11, 15, 16, 17, 18, 19, 20, 21, 22, 23 },
			{ 9, 10, 11, 12, 13, 14, 21, 22, 23, 24, 25, 26 },
			{ 9, 10, 11, 12, 13, 14, 18, 19, 20, 24, 25, 26 },
			{ 9, 10, 11, 12, 13, 14, 18, 19, 20, 21, 22, 23 }
		};
	}
}
