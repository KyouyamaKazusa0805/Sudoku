using System;
using System.Threading.Tasks;
using Sudoku.Data;

namespace Sudoku.Solving.Generating
{
	/// <summary>
	/// Provides data for all derived puzzle generators.
	/// </summary>
	public abstract class PuzzleGenerator
	{
		/// <summary>
		/// The random number generator.
		/// </summary>
		protected static readonly Random Rng = new Random();


		/// <summary>
		/// Generates a puzzle.
		/// </summary>
		/// <returns>The puzzle.</returns>
		public abstract IReadOnlyGrid Generate();

		/// <summary>
		/// Generates a puzzle asynchronizedly.
		/// </summary>
		/// <returns>The task.</returns>
		public async Task<IReadOnlyGrid> GenerateAsync() => await Task.Run(Generate);
	}
}
