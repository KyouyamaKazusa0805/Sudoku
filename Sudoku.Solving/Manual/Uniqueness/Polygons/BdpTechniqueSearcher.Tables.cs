namespace Sudoku.Solving.Manual.Uniqueness.Polygons
{
	partial class BdpTechniqueSearcher
	{
		/// <summary>
		/// All combinations in a block.
		/// </summary>
		private static readonly int[][] Quads = new int[9][]
		{
			new[] { 0, 1, 3, 4 }, new[] { 1, 2, 4, 5 }, new[] { 3, 4, 6, 7 }, new[] { 4, 5, 7, 8 },
			new[] { 0, 2, 3, 5 }, new[] { 3, 5, 6, 8 }, new[] { 0, 1, 6, 7 }, new[] { 1, 2, 7, 8 },
			new[] { 0, 2, 6, 8 }
		};
	}
}
