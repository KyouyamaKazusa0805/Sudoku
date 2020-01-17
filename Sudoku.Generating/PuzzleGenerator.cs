using System.Threading.Tasks;
using Sudoku.Data.Meta;

namespace Sudoku.Generating
{
	/// <summary>
	/// Provides data for all derived puzzle generators.
	/// </summary>
	public abstract class PuzzleGenerator
	{
		/// <summary>
		/// Generates a puzzle.
		/// </summary>
		/// <returns>The puzzle.</returns>
		public abstract Grid Generate();

		/// <summary>
		/// Generate a puzzle asynchronizedly.
		/// </summary>
		/// <param name="continueOnCapturedContext">
		/// <c>true</c> to attempt to marshal the continuation back to
		/// the original context captured; otherwise, <c>false</c>.
		/// </param>
		/// <returns>The task of generating puzzle.</returns>
		public virtual async Task<Grid> GenerateAsync(bool continueOnCapturedContext = false)
		{
			return continueOnCapturedContext
				? await Task.Run(Generate)
				: await Task.Run(Generate).ConfigureAwait(false);
		}
	}
}
