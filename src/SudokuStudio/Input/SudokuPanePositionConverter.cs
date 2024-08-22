namespace SudokuStudio.Input;

/// <summary>
/// Defines a converter instance that calculates for cursor pointers.
/// </summary>
/// <param name="Grid">Indicates the grid layout.</param>
[TypeImpl(TypeImplFlag.Object_GetHashCode)]
internal readonly partial record struct SudokuPanePositionConverter([property: HashCodeMember] GridLayout Grid) : IPointCalculator
{
	/// <summary>
	/// Indicates the first cell top-left position.
	/// </summary>
	public Point FirstCellTopLeftPosition
		=> Grid switch
		{
			{
				RowDefinitions: [{ ActualHeight: var h }, { ActualHeight: var ho }, ..],
				ColumnDefinitions: [{ ActualWidth: var w }, { ActualWidth: var wo }, ..]
			} => new(w + wo, h + ho),
			_ => default
		};

	/// <summary>
	/// Indicates the candidate size.
	/// </summary>
	public Size CandidateSize => new(CellSize.Width / 3, CellSize.Height / 3);

	/// <summary>
	/// Indicates the cell size.
	/// </summary>
	public Size CellSize
		=> Grid switch
		{
			{ RowDefinitions: [_, _, { ActualHeight: var h }, ..], ColumnDefinitions: [_, _, { ActualWidth: var w }, ..] } => new(w, h),
			_ => default
		};

	/// <summary>
	/// Indicates the block size.
	/// </summary>
	public Size BlockSize => new(CellSize.Width * 3, CellSize.Height * 3);

	/// <summary>
	/// Indicates the grid size.
	/// </summary>
	public Size GridSize => new(CellSize.Width * 9, CellSize.Height * 9);

	/// <summary>
	/// Indicates the grid points.
	/// </summary>
	private Point[,] GridPoints
	{
		get
		{
			const int length = 28;
			var (ox, oy) = FirstCellTopLeftPosition;
			var (cw, ch) = CandidateSize;
			var result = new Point[length, length];
			for (var i = 0; i < length; i++)
			{
				for (var j = 0; j < length; j++)
				{
					result[i, j] = new(cw * i + ox, ch * j + oy);
				}
			}
			return result;
		}
	}

	/// <inheritdoc/>
	float IPointCalculator.Width => (float)Grid.ActualWidth;

	/// <inheritdoc/>
	float IPointCalculator.Height => (float)Grid.ActualHeight;

	/// <inheritdoc/>
	float IPointCalculator.Padding => 0;

	/// <inheritdoc/>
	(float Width, float Height) IPointCalculator.ControlSize => ((float)Grid.ActualWidth, (float)Grid.ActualHeight);

	/// <inheritdoc/>
	(float Width, float Height) IPointCalculator.GridSize => ((float)GridSize.Width, (float)GridSize.Height);

	/// <inheritdoc/>
	(float Width, float Height) IPointCalculator.CellSize => ((float)CellSize.Width, (float)CellSize.Height);

	/// <inheritdoc/>
	(float Width, float Height) IPointCalculator.CandidateSize => ((float)CandidateSize.Width, (float)CandidateSize.Height);

	/// <inheritdoc/>
	(float X, float Y)[,] IPointCalculator.GridPoints => from pt in GridPoints select ((float)pt.X, (float)pt.Y);


	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Point firstCellTopLeftPosition, out Point[,] gridPoints)
		=> (firstCellTopLeftPosition, gridPoints) = (FirstCellTopLeftPosition, GridPoints);

	/// <include file="../../global-doc-comments.xml" path="g/csharp7/feature[@name='deconstruction-method']/target[@name='method']"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Deconstruct(out Size candidateSize, out Size cellSize, out Size blockSize, out Size gridSize)
		=> (candidateSize, cellSize, blockSize, gridSize) = (CandidateSize, CellSize, BlockSize, GridSize);

	/// <inheritdoc/>
	public bool Equals(SudokuPanePositionConverter other) => GridSize == other.GridSize;

	/// <summary>
	/// Try to get the position <see cref="Point"/> of the target candidate.
	/// </summary>
	/// <param name="candidate">The candidate.</param>
	/// <param name="position">
	/// <para>Indicates the position that output <see cref="Point"/> value describes for.</para>
	/// <para>The default value is <see cref="Position.Center"/>.</para>
	/// </param>
	/// <returns>The target position transformed.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="position"/> value is not defined in its base type.
	/// </exception>
	/// <seealso cref="Point"/>
	/// <seealso cref="Position"/>
	public Point GetPosition(Candidate candidate, Position position = Position.Center)
	{
		const double offset = 1;
		var (cw, ch) = CandidateSize;
		var (cell, digit) = (candidate / 9, candidate % 9);
		var @base = GridPoints[cell % 9 * 3 + digit % 3, cell / 9 * 3 + digit / 3];
		var extraWidth = candidate / 9 % 9 / 3 * offset;
		var extraHeight = candidate / 9 / 9 / 3 * offset;
		var result = position switch
		{
			Position.TopLeft => @base,
			Position.TopRight => @base with { X = @base.X + cw },
			Position.BottomLeft => @base with { Y = @base.Y + ch },
			Position.BottomRight => new(@base.X + cw, @base.Y + ch),
			Position.Center => new(@base.X + cw / 2, @base.Y + ch / 2),
			_ => throw new ArgumentOutOfRangeException(nameof(position))
		};

		// Fix the extra border stroke thickness.
		result.X += extraWidth;
		result.Y += extraHeight;
		return result;
	}

	/// <include
	///     file="../../global-doc-comments.xml"
	///     path="/g/csharp9/feature[@name='records']/target[@name='method' and @cref='PrintMembers']"/>
	private bool PrintMembers(StringBuilder builder)
	{
		builder.Append($"GridWidth = {(int)GridSize.Width}");
		builder.Append(", ");
		builder.Append($"GridHeight = {(int)GridSize.Height}");
		return true;
	}
}
