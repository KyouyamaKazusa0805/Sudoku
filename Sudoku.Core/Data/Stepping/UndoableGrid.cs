using System;
using System.Collections.Generic;
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
	public sealed class UndoableGrid : IEquatable<UndoableGrid>, IUndoable
	{
		/// <summary>
		/// The undo and redo stack.
		/// </summary>
		private readonly Stack<Step> _undoStack = new(), _redoStack = new();


		/// <summary>
		/// The inner sudoku grid.
		/// </summary>
		internal SudokuGrid _innerGrid;


		/// <summary>
		/// Initializes an instance with the specified mask array.
		/// </summary>
		/// <param name="masks">The mask array.</param>
		public UndoableGrid(short[] masks) => _innerGrid = new(masks);


		/// <summary>
		/// Indicates whether the grid has any undo steps available.
		/// </summary>
		public bool HasUndoSteps => _undoStack.Count != 0;

		/// <summary>
		/// Indicates whether the grid has any redo steps available.
		/// </summary>
		public bool HasRedoSteps => _redoStack.Count != 0;


		/// <inheritdoc/>
		public int this[int offset]
		{
			get => _innerGrid[offset];
			set
			{
				var map = GridMap.Empty;
				foreach (int cell in PeerMaps[offset])
				{
					if (GetStatus(cell) == CellStatus.Empty)
					{
						map.AddAnyway(cell);
					}
				}

				unsafe
				{
					_undoStack.Push(new AssignmentStep(value, offset, _innerGrid._values[offset], map));
				}

				// Do step.
				_innerGrid[offset] = value;
			}
		}

		/// <inheritdoc/>
		public bool this[int offset, int digit]
		{
			get => _innerGrid[offset, digit];
			set
			{
				_undoStack.Push(
					value ? new EliminationStep(digit, offset) : new AntiEliminationStep(digit, offset));

				// Do step.
				_innerGrid[offset, digit] = value;
			}
		}


		/// <inheritdoc/>
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
			unsafe
			{
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
		}

		/// <inheritdoc/>
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
			unsafe
			{
				foreach (int cell in map)
				{
					ref short mask = ref _innerGrid._values[cell];
					mask = (short)((int)CellStatus.Modifiable << 9 | mask & SudokuGrid.MaxCandidatesMask);
				}
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Reset()
		{
			unsafe
			{
				fixed (short* pInitialValues = _innerGrid._initialValues, pValues = _innerGrid._values)
				{
					_undoStack.Push(new ResetStep(pInitialValues, pValues));
				}
			}
			_innerGrid.Reset();
		}

		/// <inheritdoc cref="SudokuGrid.GetStatus(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public CellStatus GetStatus(int cell) => _innerGrid.GetStatus(cell);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void SetStatus(int offset, CellStatus cellStatus)
		{
			_undoStack.Push(new SetStatusStep(offset, GetStatus(offset), cellStatus));
			_innerGrid.SetStatus(offset, cellStatus);
		}

		/// <inheritdoc cref="SudokuGrid.GetMask(int)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public short GetMask(int offset) => _innerGrid.GetMask(offset);

		/// <inheritdoc/>
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
		public bool Equals(UndoableGrid? other) => Equals(other?._innerGrid ?? SudokuGrid.Undefined);

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(in SudokuGrid other) => _innerGrid == other;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => _innerGrid.GetHashCode();


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(UndoableGrid left, UndoableGrid right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in SudokuGrid left, UndoableGrid right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(UndoableGrid left, in SudokuGrid right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(UndoableGrid left, UndoableGrid right) => !(left == right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in SudokuGrid left, UndoableGrid right) => !(left == right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(UndoableGrid left, in SudokuGrid right) => !(left == right);
	}
}
