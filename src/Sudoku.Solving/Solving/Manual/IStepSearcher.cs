using Sudoku.Data;

namespace Sudoku.Solving.Manual
{
	/// <summary>
	/// Provides the basic restraint of a <see cref="StepSearcher"/>.
	/// </summary>
	/// <seealso cref="StepSearcher"/>
	public interface IStepSearcher
	{
		/// <summary>
		/// Indicates the step searching options.
		/// </summary>
		SearchingOptions Options { get; set; }


		/// <summary>
		/// Accumulate all technique information instances into the specified accumulator.
		/// </summary>
		/// <param name="accumulator">The accumulator to store technique information.</param>
		/// <param name="grid">The grid to search for techniques.</param>
		void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid);

		/// <summary>
		/// Take a technique step after searched all solving steps.
		/// </summary>
		/// <param name="grid">The grid to search steps.</param>
		/// <returns>A technique information.</returns>
		public StepInfo? GetOne(in SudokuGrid grid)
		{
			var bag = new NotifyChangedList<StepInfo>();
			bag.ElementAdded += static (_, _) => throw new InvalidOperationException(nameof(GetOne));

			try { GetAll(bag, grid); } catch (InvalidOperationException ex) when (ex.Message == nameof(GetOne)) { }

			return bag.FirstOrDefault();
		}
	}
}
