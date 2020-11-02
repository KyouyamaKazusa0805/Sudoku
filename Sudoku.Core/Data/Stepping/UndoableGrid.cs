using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;
using static Sudoku.Constants.Processings;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Provides an undoable sudoku grid. This data structure is nearly same
	/// as <see cref="SudokuGrid"/>, but only add two methods <see cref="Undo"/>
	/// and <see cref="Redo"/>.
	/// </summary>
	/// <seealso cref="SudokuGrid"/>
	/// <seealso cref="Undo"/>
	/// <seealso cref="Redo"/>
#if DEBUG
	[DebuggerDisplay("{ToString(\"#.\")}")]
#endif
	public sealed unsafe class UndoableGrid : IEquatable<UndoableGrid>, IFormattable, IUndoable
	{
		/// <summary>
		/// The inner sudoku grid.
		/// </summary>
		internal SudokuGrid _innerGrid;

		/// <summary>
		/// The undo and redo stack.
		/// </summary>
		private readonly Stack<Step> _undoStack = new(), _redoStack = new();


		/// <summary>
		/// Initializes an instance with the specified mask array.
		/// </summary>
		/// <param name="masks">The mask array.</param>
		public UndoableGrid(short[] masks) => _innerGrid = new(masks);

		/// <summary>
		/// Initializes an instance with the specified sudoku grid (value type).
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		private UndoableGrid(in SudokuGrid grid) => _innerGrid = grid;


		/// <summary>
		/// Indicates whether the grid has any undo steps available.
		/// </summary>
		public bool HasUndoSteps => _undoStack.Count != 0;

		/// <summary>
		/// Indicates whether the grid has any redo steps available.
		/// </summary>
		public bool HasRedoSteps => _redoStack.Count != 0;

		/// <inheritdoc cref="SudokuGrid.HasSolved"/>
		public bool HasSolved => _innerGrid.HasSolved;

		/// <inheritdoc cref="SudokuGrid.GivensCount"/>
		public int GivensCount => _innerGrid.GivensCount;


		/// <inheritdoc cref="SudokuGrid.this[int]"/>
		public int this[int cell]
		{
			get => _innerGrid[cell];
			set
			{
				var map = GridMap.Empty;
				foreach (int peerCell in PeerMaps[cell])
				{
					if (GetStatus(peerCell) == CellStatus.Empty)
					{
						map.AddAnyway(peerCell);
					}
				}

				_undoStack.Push(new AssignmentStep(value, cell, _innerGrid._values[cell], map));

				// Do step.
				_innerGrid[cell] = value;
			}
		}

		/// <inheritdoc cref="SudokuGrid.this[int, int]"/>
		public bool this[int cell, int digit]
		{
			get => _innerGrid[cell, digit];
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
			var map = GridMap.Empty;
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Modifiable)
				{
					map.AddAnyway(i);
				}
			}

			_undoStack.Push(new FixStep(map));
			foreach (int cell in map)
			{
				ref short mask = ref _innerGrid._values[cell];
				mask = (short)((int)CellStatus.Given << 9 | mask & SudokuGrid.MaxCandidatesMask);
			}

			fixed (short* pInitialValues = _innerGrid._initialValues, pValues = _innerGrid._values)
			{
				SudokuGrid.InternalCopy(pInitialValues, pValues);
			}
		}

		/// <inheritdoc cref="SudokuGrid.Unfix"/>
		public void Unfix()
		{
			var map = GridMap.Empty;
			for (int i = 0; i < 81; i++)
			{
				if (GetStatus(i) == CellStatus.Given)
				{
					map.AddAnyway(i);
				}
			}

			_undoStack.Push(new UnfixStep(map));
			foreach (int cell in map)
			{
				ref short mask = ref _innerGrid._values[cell];
				mask = (short)((int)CellStatus.Modifiable << 9 | mask & SudokuGrid.MaxCandidatesMask);
			}
		}

		/// <inheritdoc cref="SudokuGrid.Reset"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			fixed (short* pInitialValues = _innerGrid._initialValues, pValues = _innerGrid._values)
			{
				_undoStack.Push(new ResetStep(pInitialValues, pValues));
			}
			_innerGrid.Reset();
		}

		/// <inheritdoc cref="SudokuGrid.RefreshingCandidates"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void RecomputeCandidates() => SudokuGrid.RefreshingCandidates(ref _innerGrid);

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

		/// <inheritdoc cref="SudokuGrid.GetCandidateMask(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short GetCandidateMask(int cell) => _innerGrid.GetCandidateMask(cell);

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
			_ = HasRedoSteps ? 0 : throw new InvalidOperationException("The redo stack is already empty.");

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
			_ = HasUndoSteps ? 0 : throw new InvalidOperationException("The undo stack is already empty.");

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
		public bool Equals(in SudokuGrid other) => _innerGrid == other;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => _innerGrid.GetHashCode();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => _innerGrid.ToString();

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string? format) => _innerGrid.ToString(format);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			_innerGrid.ToString(format, formatProvider);


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(UndoableGrid left, UndoableGrid right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in SudokuGrid left, UndoableGrid right) => right.Equals(left);

		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(UndoableGrid left, in SudokuGrid right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(UndoableGrid left, UndoableGrid right) => !(left == right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in SudokuGrid left, UndoableGrid right) => !(left == right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(UndoableGrid left, in SudokuGrid right) => !(left == right);


		/// <summary>
		/// Explicit cast from <see cref="UndoableGrid"/> to <see cref="SudokuGrid"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <remarks>The cast won't use neither box nor unbox operations.</remarks>
		public static explicit operator SudokuGrid(UndoableGrid grid) => grid._innerGrid;

		/// <summary>
		/// Implicit cast from <see cref="SudokuGrid"/> to <see cref="UndoableGrid"/>.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		/// <remarks>The cast won't use neither box nor unbox operations.</remarks>
		public static implicit operator UndoableGrid(in SudokuGrid grid) => new(grid);
	}
}
