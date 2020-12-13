using Sudoku.DocComments;

namespace Sudoku.Solving.Manual.Alses
{
	/// <summary>
	/// Encapsulates an <b>almost locked set</b> (ALS) technique searcher.
	/// </summary>
	public abstract class AlsStepSearcher : StepSearcher
	{
		/// <summary>
		/// Indicates whether the ALSes can be overlapped with each other.
		/// </summary>
		protected readonly bool AllowOverlapping;

		/// <summary>
		/// Indicates whether the ALSes shows their regions rather than cells.
		/// </summary>
		protected readonly bool AlsShowRegions;

		/// <summary>
		/// Indicates whether the solver will check ALS cycles.
		/// </summary>
		protected readonly bool AllowAlsCycles;


		/// <inheritdoc cref="DefaultConstructor"/>
		protected AlsStepSearcher()
		{
		}

		/// <summary>
		/// Initializes an instance with the specified information.
		/// </summary>
		/// <param name="allowOverlapping">Indicates whether the ALSes can be overlapped with each other.</param>
		/// <param name="alsShowRegions">Indicates whether the ALSes shows their regions rather than cells.</param>
		/// <param name="allowAlsCycles">Indicates whether the solver will check ALS cycles.</param>
		protected AlsStepSearcher(bool allowOverlapping, bool alsShowRegions, bool allowAlsCycles)
		{
			AllowOverlapping = allowOverlapping;
			AlsShowRegions = alsShowRegions;
			AllowAlsCycles = allowAlsCycles;
		}
	}
}
