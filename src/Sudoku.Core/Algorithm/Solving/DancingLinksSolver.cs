namespace Sudoku.Algorithm.Solving;

/// <summary>
/// Defines a solver that uses the dancing links algorithm.
/// </summary>
public sealed class DancingLinksSolver : ISolver
{
	/// <summary>
	/// Indicates the stack that stores the raw data for the solutions.
	/// </summary>
	private readonly Stack<DancingLinkNode> _answerNodesStack = [];


	/// <summary>
	/// indicates the number of all found solutions.
	/// </summary>
	private int _solutionCount;

	/// <summary>
	/// Indicates the found solution.
	/// </summary>
	private Grid _solution;

	/// <summary>
	/// Indicates the root node of the full link map.
	/// </summary>
	private ColumnNode? _root;


	/// <inheritdoc/>
	public static string UriLink => "https://en.wikipedia.org/wiki/Dancing_Links";


	/// <inheritdoc/>
	public bool? Solve(scoped ref readonly Grid grid, out Grid result)
	{
		Unsafe.SkipInit(out result);

		var gridValue = grid.ToArray();
		var dlx = new DancingLink(new(-1));
		_root = dlx.CreateLinkedList(gridValue);

		try
		{
			Search();

			result = _solution;
			return true;
		}
		catch (InvalidOperationException ex)
		{
			return ex.Message.Contains("multiple") ? false : null;
		}
	}

	/// <summary>
	/// Try to search the full dancing link map and get the possible solution.
	/// </summary>
	/// <exception cref="InvalidOperationException">Throws when the puzzle has multiple solutions.</exception>
	private void Search()
	{
		if (_solutionCount > 1)
		{
			throw new InvalidOperationException("The puzzle has multiple solutions.");
		}

		Debug.Assert(_root is not null);
		if (ReferenceEquals(_root.Right, _root))
		{
			// All columns were removed!
			_solutionCount++;
			RecordSolution(_answerNodesStack, out _solution);
		}
		else
		{
			var c = ChooseNextColumn();
			Cover(c);

			for (var r = c.Down; !ReferenceEquals(r, c); r = r.Down)
			{
				_answerNodesStack.Push(r);
				for (var j = r.Right; !ReferenceEquals(j, r); j = j.Right)
				{
					Cover(j.Column!);
				}

				Search();
				r = _answerNodesStack.Pop();
				c = r.Column!;

				for (var j = r.Left; !ReferenceEquals(j, r); j = j.Left)
				{
					Uncover(j.Column!);
				}
			}

			Uncover(c);
		}
	}

	/// <summary>
	/// Cover the nodes for the specified column.
	/// </summary>
	/// <param name="column">The column.</param>
	private void Cover(DancingLinkNode column)
	{
		column.Right.Left = column.Left;
		column.Left.Right = column.Right;
		for (var i = column.Down; !ReferenceEquals(i, column); i = i.Down)
		{
			for (var j = i.Right; !ReferenceEquals(j, i); j = j.Right)
			{
				j.Down.Up = j.Up;
				j.Up.Down = j.Down;
				j.Column!.Size--;
			}
		}
	}

	/// <summary>
	/// Uncover the nodes for the specified column.
	/// </summary>
	/// <param name="column">The column.</param>
	private void Uncover(DancingLinkNode column)
	{
		for (var i = column.Up; !ReferenceEquals(i, column); i = i.Up)
		{
			for (var j = i.Left; !ReferenceEquals(j, i); j = j.Left)
			{
				j.Column!.Size++;
				j.Down.Up = j;
				j.Up.Down = j;
			}
		}
		column.Right.Left = column;
		column.Left.Right = column;
	}

	/// <summary>
	/// Try to gather all possible solutions, and determine whether the puzzle is valid.
	/// </summary>
	/// <param name="answer">The answers found.</param>
	/// <param name="result">The solution if the puzzle is unique.</param>
	/// <exception cref="InvalidOperationException">
	/// Throws when the puzzle has no possible solutions.
	/// </exception>
	private void RecordSolution(Stack<DancingLinkNode> answer, out Grid result)
	{
		var idList = (from k in answer orderby k.Id select k.Id).ToList();
		var grid = Grid.Create(from id in idList select id % 9 + 1, GridCreatingOption.MinusOne);
		result = grid.IsValid ? grid : throw new InvalidOperationException("The puzzle has no possible solutions.");
	}

	/// <summary>
	/// Try to choose the next column node.
	/// </summary>
	/// <returns>The chosen next column node.</returns>
	[MemberNotNull(nameof(_root))]
	private ColumnNode ChooseNextColumn()
	{
		Debug.Assert(_root is not null);

		var size = int.MaxValue;
		var nextColumn = new ColumnNode(-1);
		var j = _root.Right.Column;
		while (!ReferenceEquals(j, _root))
		{
			if (j!.Size < size)
			{
				nextColumn = j;
				size = j.Size;
			}

			j = j.Right.Column;
		}
		return nextColumn;
	}
}

/// <summary>
/// Represents as a dancing link.
/// </summary>
/// <param name="root">The root node.</param>
file sealed class DancingLink(ColumnNode root)
{
	/// <summary>
	/// Indicates the root node.
	/// </summary>
	public ColumnNode Root { get; set; } = root;


	/// <summary>
	/// Creates the links.
	/// </summary>
	/// <param name="gridArray">The grid array.</param>
	/// <returns>The column node for the root node.</returns>
	public ColumnNode CreateLinkedList(Digit[] gridArray)
	{
		var columns = new List<ColumnNode>();
		for (var columnIndex = 0; columnIndex < 324; columnIndex++)
		{
			var col = new ColumnNode(columnIndex) { Right = Root, Left = Root.Left };
			Root.Left.Right = col;
			Root.Left = col;
			columns.Add(col);
		}

		for (var i = 0; i < 81; i++)
		{
			var x = i / 9;
			var y = i % 9;
			if (gridArray[i] == 0)
			{
				// The cell is empty.
				for (var d = 0; d < 9; d++)
				{
					FormLinks(columns, x, y, d);
				}
			}
			else
			{
				// The cell is given.
				var d = gridArray[i] - 1;
				FormLinks(columns, x, y, d);
			}
		}

		return Root;
	}

	/// <summary>
	/// Links the row.
	/// </summary>
	/// <param name="d">The matrix row instance.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LinkRow(scoped ref MatrixRow d)
	{
		d.Cell.Right = d.Column;
		d.Cell.Left = d.Block;
		d.Column.Right = d.Row;
		d.Column.Left = d.Cell;
		d.Row.Right = d.Block;
		d.Row.Left = d.Column;
		d.Block.Right = d.Cell;
		d.Block.Left = d.Row;
	}

	/// <summary>
	/// Links the row to the column.
	/// </summary>
	/// <param name="section">The section.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void LinkRowToColumn(DancingLinkNode section)
	{
		if (section.Column is { } col)
		{
			col.Size++;
			section.Down = col;
			section.Up = col.Up;
			col.Up.Down = section;
			col.Up = section;
		}
	}

	/// <summary>
	/// To form the links via the specified columns, the cell index and the digit used.
	/// </summary>
	/// <param name="columns">The columns having been stored.</param>
	/// <param name="x">The current row index.</param>
	/// <param name="y">The current column index.</param>
	/// <param name="d">The current digit.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private void FormLinks(List<ColumnNode> columns, int x, int y, Digit d)
	{
		var cell = new DancingLinkNode(x * 81 + y * 9 + d, columns[x * 9 + y]);
		var row = new DancingLinkNode(x * 81 + y * 9 + d, columns[81 + x * 9 + d]);
		var column = new DancingLinkNode(x * 81 + y * 9 + d, columns[162 + y * 9 + d]);
		var block = new DancingLinkNode(x * 81 + y * 9 + d, columns[243 + (3 * (x / 3) + y / 3) * 9 + d]);
		var matrixRow = new MatrixRow(cell, row, column, block);

		LinkRow(ref matrixRow);
		LinkRowToColumn(matrixRow.Cell);
		LinkRowToColumn(matrixRow.Row);
		LinkRowToColumn(matrixRow.Column);
		LinkRowToColumn(matrixRow.Block);
	}
}
