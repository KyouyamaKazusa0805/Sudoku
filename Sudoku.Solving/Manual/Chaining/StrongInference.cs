namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a strong inference.
	/// </summary>
	public sealed class StrongInference : Inference
	{
		/// <summary>
		/// Initializes an instance with two nodes.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="end">The end node.</param>
		public StrongInference(Node start, Node end) : base(start, false, end, true)
		{
		}
	}
}
