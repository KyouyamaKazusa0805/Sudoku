using Sudoku.Constants;

namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a <b>chain</b> technique searcher.
	/// </summary>
	public abstract class ChainingTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Get the region cause with the specified region label.
		/// </summary>
		/// <param name="regionLabel">The region label.</param>
		/// <returns>The region cause.</returns>
		public static Node.Cause GetRegionCause(RegionLabel regionLabel) =>
			regionLabel switch
			{
				RegionLabel.Block => Node.Cause.HiddenSingleBlock,
				RegionLabel.Column => Node.Cause.HiddenSingleColumn,
				_ => Node.Cause.HiddenSingleRow,
			};
	}
}
