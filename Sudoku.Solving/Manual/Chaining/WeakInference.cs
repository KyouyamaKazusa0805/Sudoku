namespace Sudoku.Solving.Manual.Chaining
{
	/// <summary>
	/// Encapsulates a weak inference.
	/// </summary>
	public sealed class WeakInference : Inference
	{
		/// <summary>
		/// Initializes an instance with two nodes.
		/// </summary>
		/// <param name="start">The start node.</param>
		/// <param name="end">The end node.</param>
		public WeakInference(Node start, Node end) : base(start, true, end, false)
		{ 
		}
	}
}
