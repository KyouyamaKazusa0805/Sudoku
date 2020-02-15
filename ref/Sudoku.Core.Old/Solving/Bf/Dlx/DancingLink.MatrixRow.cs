namespace Sudoku.Solving.Bf.Dlx
{
	public partial class DancingLink
	{
		private struct MatrixRow
		{
			public MatrixRow(DataNode position, DataNode row, DataNode column, DataNode block) =>
				(Position, Row, Column, Block) = (position, row, column, block);


			public DataNode Position { get; set; }

			public DataNode Row { get; set; }

			public DataNode Column { get; set; }

			public DataNode Block { get; set; }
		}
	}
}
