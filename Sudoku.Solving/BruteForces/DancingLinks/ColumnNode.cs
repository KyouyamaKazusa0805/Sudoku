namespace Sudoku.Solving.BruteForces.DancingLinks
{
	/// <summary>
	/// Provides a column node.
	/// </summary>
	/// <typeparam name="T">The element type.</typeparam>
	internal sealed class ColumnNode<T> : Node<T> where T : notnull
	{
		/// <summary>
		/// Initializes an instance with an ID.
		/// </summary>
		/// <param name="id">The ID.</param>
		public ColumnNode(int id) : base(-1) =>
			(Id, Up, Down, ColumnNode) = (id, this, this, this);


		/// <summary>
		/// The ID.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// The size of this column.
		/// </summary>
		public int Size { get; private set; }


		/// <summary>
		/// Increase the property <see cref="Size"/>.
		/// </summary>
		public void IncSize() => Size++;

		/// <summary>
		/// Decrease the property <see cref="Size"/>.
		/// </summary>
		public void DecSize() => Size--;
	}
}
