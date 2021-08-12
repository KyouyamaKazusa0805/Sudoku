using Sudoku.Data;

namespace Sudoku.Generating
{
	/// <summary>
	/// Provides data for all derived puzzle generators.
	/// </summary>
	public interface IPuzzleGenerator
	{
		/// <summary>
		/// The random number generator.
		/// </summary>
		protected static readonly Random Rng = new();


		/// <summary>
		/// Generates a puzzle.
		/// </summary>
		/// <returns>
		/// The puzzle. If the operation is cancelled, the return value will be <see langword="null"/>.
		/// </returns>
		SudokuGrid? Generate();

		/// <summary>
		/// Generates a puzzle asynchronously.
		/// </summary>
		/// <returns>The task.</returns>
		public async Task<SudokuGrid?> GenerateAsync() => await Task.Run(Generate);
	}
}
