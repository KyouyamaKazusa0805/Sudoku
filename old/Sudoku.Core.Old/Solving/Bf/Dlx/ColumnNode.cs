namespace Sudoku.Solving.Bf.Dlx
{
	public sealed class ColumnNode : DataNode
	{
		public ColumnNode(int id) : base(id, null)
		{
			Column = this;
			Size = 0;
		}

		public int Size { get; set; }
	}
}
