namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>almost locked set</b> (ALS) technique searcher.
	/// </summary>
	public abstract class AlsTechniqueSearcher : TechniqueSearcher
	{
		/// <summary>
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </summary>
		protected readonly bool _allowOverlapping;

		/// <summary>
		/// Indicates whether the ALSes shows their regions rather than cells.
		/// </summary>
		protected readonly bool _alsShowRegions;

		/// <summary>
		/// Indicates whether the solver will check ALS cycles.
		/// </summary>
		protected readonly bool _allowAlsCycles;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		protected AlsTechniqueSearcher()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">Indicates whether the ALSes can be overlapped with each other.</param>
		/// <param name="alsShowRegions">Indicates whether the ALSes shows their regions rather than cells.</param>
		/// <param name="allowAlsCycles">Indicates whether the solver will check ALS cycles.</param>
		protected AlsTechniqueSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles) =>
			(_allowOverlapping, _alsShowRegions, _allowAlsCycles) = (allowOverlapping, alsShowRegions, allowAlsCycles);
	}
}
