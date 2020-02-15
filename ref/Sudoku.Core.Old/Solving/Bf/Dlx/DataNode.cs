using System.Diagnostics.Contracts;

namespace Sudoku.Solving.Bf.Dlx
{
	public class DataNode
	{
		public DataNode(int id, ColumnNode? column) =>
			(Id, Column, Left, Right, Up, Down) = (id, column, this, this, this, this);


		public int Id { get; set; }

		public ColumnNode? Column { get; set; }

		public DataNode Left { get; set; }

		public DataNode Right { get; set; }

		public DataNode Up { get; set; }

		public DataNode Down { get; set; }


		public void Cover()
		{
			Right.Left = Left;
			Left.Right = Right;
			for (var i = Down; i != this; i = i.Down)
			{
				for (var j = i.Right; j != i; j = j.Right)
				{
					Contract.Assume(!(j.Column is null));

					j.Down.Up = j.Up;
					j.Up.Down = j.Down;
					j.Column.Size--;
				}
			}
		}

		public void Uncover()
		{
			for (var i = Up; i != this; i = i.Up)
			{
				for (var j = i.Left; j != i; j = j.Left)
				{
					Contract.Assume(!(j.Column is null));

					j.Column.Size++;
					j.Down.Up = j;
					j.Up.Down = j;
				}
			}
			Right.Left = this;
			Left.Right = this;
		}
	}
}
