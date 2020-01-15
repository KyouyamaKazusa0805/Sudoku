namespace Sudoku.Solving.BruteForces.DancingLinks
{
	internal sealed class ColumnNode<T> : Node<T> where T : notnull
	{
		public ColumnNode(int id) : base(-1) =>
			(Id, Up, Down, ColumnNode) = (id, this, this, this);


		public int Id { get; }

		public int Size { get; private set; }


		public void IncSize() => Size++;

		public void DecSize() => Size--;
	}
}
