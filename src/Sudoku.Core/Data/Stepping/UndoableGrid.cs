using static Sudoku.Data.SudokuGrid;

namespace Sudoku.Data.Stepping;

/// <summary>
/// Provides an undoable sudoku grid.
/// </summary>
#if DEBUG
[DebuggerDisplay($@"{{{nameof(ToString)}(""#."")}}")]
#endif
[Obsolete("In the future, this class won't be used.", false)]
public sealed unsafe class UndoableGrid : IEquatable<UndoableGrid>, ISimpleFormattable, IUndoable
{
	/// <summary>
	/// The undo and redo stack.
	/// </summary>
	private readonly Stack<IStep> _undoStack = new(), _redoStack = new();


	/// <summary>
	/// The inner sudoku grid.
	/// </summary>
	private SudokuGrid _innerGrid;


	/// <summary>
	/// Initializes an instance with the specified mask array.
	/// </summary>
	/// <param name="masks">The mask array.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public UndoableGrid(short[] masks) => _innerGrid = new(masks);

	/// <summary>
	/// Initializes an instance with the specified sudoku grid (value type).
	/// </summary>
	/// <param name="grid">The grid.</param>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	private UndoableGrid(in SudokuGrid grid) => _innerGrid = grid;


	/// <summary>
	/// Indicates whether the grid has any undo steps available.
	/// </summary>
	public bool HasUndoSteps
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _undoStack.Count != 0;
	}

	/// <summary>
	/// Indicates whether the grid has any redo steps available.
	/// </summary>
	public bool HasRedoSteps
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _redoStack.Count != 0;
	}

	/// <summary>
	/// Indicates whether the grid has been already solved.
	/// </summary>
	public bool IsSolved
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _innerGrid.IsSolved;
	}

	/// <summary>
	/// Indicates the number of given cells.
	/// </summary>
	public int GivensCount
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _innerGrid.GivensCount;
	}

	/// <summary>
	/// Indicates the inner grid.
	/// </summary>
	public ref SudokuGrid InnerGrid
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => ref _innerGrid;
	}


	/// <summary>
	/// Get the result digit of a specified cell, or set the specified digit to a cell.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <value>The digit you want to set.</value>
	/// <returns>The result digit.</returns>
	public int this[int cell]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _innerGrid[cell];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_undoStack.Push(new AssignmentStep(cell, value, _innerGrid));

			// Do step.
			_innerGrid[cell] = value;
		}
	}

	/// <summary>
	/// Get whether the specified candidate is currently set, or set the status to a candidate.
	/// </summary>
	/// <param name="cell">The cell.</param>
	/// <param name="digit">The digit.</param>
	/// <value>
	/// The <see cref="bool"/> status you want to set. If <see langword="false"/>, the candidate
	/// should be deleted.
	/// </value>
	/// <returns>The <see cref="bool"/> result indicating whether the candidate is set.</returns>
	public bool this[int cell, int digit]
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _innerGrid[cell, digit];

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		set
		{
			_undoStack.Push(value ? new EliminationStep(digit, cell) : new AntiEliminationStep(digit, cell));

			// Do step.
			_innerGrid[cell, digit] = value;
		}
	}


	/// <inheritdoc cref="SudokuGrid.Fix"/>
	public void Fix()
	{
		var map = Cells.Empty;
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Modifiable)
			{
				map.AddAnyway(i);
			}
		}

		_undoStack.Push(new FixStep(map));
		fixed (short* pGrid = _innerGrid)
		{
			foreach (int cell in map)
			{
				pGrid[cell] = (short)(GivenMask | pGrid[cell] & MaxCandidatesMask);
			}
		}

		_innerGrid.UpdateInitialMasks();
	}

	/// <inheritdoc cref="SudokuGrid.Unfix"/>
	public void Unfix()
	{
		var map = Cells.Empty;
		for (int i = 0; i < 81; i++)
		{
			if (GetStatus(i) == CellStatus.Given)
			{
				map.AddAnyway(i);
			}
		}

		_undoStack.Push(new UnfixStep(map));
		fixed (short* pGrid = _innerGrid)
		{
			foreach (int cell in map)
			{
				pGrid[cell] = (short)(ModifiableMask | pGrid[cell] & MaxCandidatesMask);
			}
		}
	}

	/// <inheritdoc cref="SudokuGrid.Reset"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Reset()
	{
		fixed (short* pGrid = _innerGrid, pInitial = &_innerGrid.GetPinnableReference(PinnedItem.InitialGrid))
		{
			_undoStack.Push(new ResetStep((IntPtr)pInitial, (IntPtr)pGrid));
		}
		_innerGrid.Reset();
	}

	/// <inheritdoc cref="RefreshingCandidates"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void RecomputeCandidates() => RefreshingCandidates(ref _innerGrid);

	/// <inheritdoc cref="SudokuGrid.GetStatus(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public CellStatus GetStatus(int cell) => _innerGrid.GetStatus(cell);

	/// <inheritdoc cref="SudokuGrid.SetStatus(int, CellStatus)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetStatus(int offset, CellStatus cellStatus)
	{
		_undoStack.Push(new SetStatusStep(offset, GetStatus(offset), cellStatus));
		_innerGrid.SetStatus(offset, cellStatus);
	}

	/// <inheritdoc cref="SudokuGrid.GetMask(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short GetMask(int offset) => _innerGrid.GetMask(offset);

	/// <inheritdoc cref="SudokuGrid.GetCandidates(int)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public short GetCandidates(int cell) => _innerGrid.GetCandidates(cell);

	/// <summary>
	/// Returns a reference to the element of the <see cref="UndoableGrid"/> at index zero.
	/// </summary>
	/// <returns>A reference to the element of the <see cref="UndoableGrid"/> at index zero.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[EditorBrowsable(EditorBrowsableState.Never)]
	public ref readonly short GetPinnableReference()
	{
		fixed (short* pThis = _innerGrid)
		{
			return ref *pThis;
		}
	}

	/// <summary>
	/// Returns a reference to the element of the <see cref="SudokuGrid"/> at index zero.
	/// </summary>
	/// <param name="pinnedItem">The item you want to fix. If </param>
	/// <returns>A reference to the element of the <see cref="SudokuGrid"/> at index zero.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public ref readonly short GetPinnableReference(PinnedItem pinnedItem) =>
		ref _innerGrid.GetPinnableReference(pinnedItem);

	/// <inheritdoc cref="SudokuGrid.SetMask(int, short)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void SetMask(int offset, short value)
	{
		_undoStack.Push(new SetMaskStep(offset, GetMask(offset), value));
		_innerGrid.SetMask(offset, value);
	}

	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">
	/// Throws when the redo stack is empty.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Redo()
	{
		if (!HasRedoSteps)
		{
			throw new InvalidOperationException("The redo stack is already empty.");
		}

		var step = _redoStack.Pop();
		_undoStack.Push(step);
		step.DoStepTo(this);
	}

	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">
	/// Throws when the undo stack is empty.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void Undo()
	{
		if (!HasUndoSteps)
		{
			throw new InvalidOperationException("The undo stack is already empty.");
		}

		var step = _undoStack.Pop();
		_redoStack.Push(step);
		step.UndoStepTo(this);
	}

	/// <summary>
	/// To clear step stacks.
	/// </summary>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public void ClearStack()
	{
		_undoStack.Clear();
		_redoStack.Clear();
	}

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override bool Equals(object? obj) => Equals(obj as UndoableGrid);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(UndoableGrid? other) => other is { _innerGrid: var grid } && Equals(grid);

	/// <inheritdoc cref="IValueEquatable{TStruct}.Equals(in TStruct)"/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(in SudokuGrid other) => _innerGrid == other;

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override int GetHashCode() => _innerGrid.GetHashCode();

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public override string ToString() => _innerGrid.ToString(null);

	/// <summary>
	/// Returns a string that represents the current object with the specified format string.
	/// </summary>
	/// <param name="format">The format. If available, the parameter can be <see langword="null"/>.</param>
	/// <returns>The string result.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public string ToString(string? format) => _innerGrid.ToString(format);


	/// <summary>
	/// Determines whether two <see cref="UndoableGrid"/>s hold a same sudoku grid as inner.
	/// </summary>
	/// <param name="left">The first sudoku grid to compare.</param>
	/// <param name="right">The second sudoku grid to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result. <see langword="true"/> is for same value; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UndoableGrid left, UndoableGrid right) =>
		left._innerGrid == right._innerGrid;

	/// <summary>
	/// Determines whether a <see cref="UndoableGrid"/> and a <see cref="SudokuGrid"/>
	/// hold a same sudoku grid as inner.
	/// </summary>
	/// <param name="left">The first sudoku grid to compare.</param>
	/// <param name="right">The second sudoku grid to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result. <see langword="true"/> is for same value; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(in SudokuGrid left, UndoableGrid right) => left == right._innerGrid;

	/// <summary>
	/// Determines whether a <see cref="UndoableGrid"/> and a <see cref="SudokuGrid"/>
	/// hold a same sudoku grid as inner.
	/// </summary>
	/// <param name="left">The first sudoku grid to compare.</param>
	/// <param name="right">The second sudoku grid to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result. <see langword="true"/> is for same value; otherwise,
	/// <see langword="false"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator ==(UndoableGrid left, in SudokuGrid right) => left._innerGrid == right;

	/// <summary>
	/// Determines whether two <see cref="UndoableGrid"/>s don't hold the totally same sudoku grid as inner.
	/// </summary>
	/// <param name="left">The first sudoku grid to compare.</param>
	/// <param name="right">The second sudoku grid to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result. <see langword="false"/> is for same value; otherwise,
	/// <see langword="true"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UndoableGrid left, UndoableGrid right) =>
		left._innerGrid != right._innerGrid;

	/// <summary>
	/// Determines whether a <see cref="UndoableGrid"/> and a <see cref="SudokuGrid"/>
	/// don't hold the totally same sudoku grid as inner.
	/// </summary>
	/// <param name="left">The first sudoku grid to compare.</param>
	/// <param name="right">The second sudoku grid to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result. <see langword="false"/> is for same value; otherwise,
	/// <see langword="true"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(in SudokuGrid left, UndoableGrid right) => left != right._innerGrid;

	/// <summary>
	/// Determines whether a <see cref="UndoableGrid"/> and a <see cref="SudokuGrid"/>
	/// don't hold the totally same sudoku grid as inner.
	/// </summary>
	/// <param name="left">The first sudoku grid to compare.</param>
	/// <param name="right">The second sudoku grid to compare.</param>
	/// <returns>
	/// A <see cref="bool"/> result. <see langword="false"/> is for same value; otherwise,
	/// <see langword="true"/>.
	/// </returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static bool operator !=(UndoableGrid left, in SudokuGrid right) => left._innerGrid != right;


	/// <summary>
	/// Explicit cast from <see cref="UndoableGrid"/> to <see cref="SudokuGrid"/>.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <remarks>The cast won't use neither box nor unbox operations.</remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static explicit operator SudokuGrid(UndoableGrid grid) => grid._innerGrid;

	/// <summary>
	/// Implicit cast from <see cref="SudokuGrid"/> to <see cref="UndoableGrid"/>.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <remarks>The cast won't use neither box nor unbox operations.</remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static implicit operator UndoableGrid(in SudokuGrid grid) => new(grid);
}
