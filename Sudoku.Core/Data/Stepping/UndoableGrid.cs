using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data.Extensions;

namespace Sudoku.Data.Stepping
{
	/// <summary>
	/// Provides an undoable sudoku grid. This data structure is nearly same
	/// as <see cref="Grid"/>, but only add two methods <see cref="Undo"/>
	/// and <see cref="Redo"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
	/// <seealso cref="Undo"/>
	/// <seealso cref="Redo"/>
	public sealed class UndoableGrid : Grid, IEquatable<UndoableGrid>, IUndoable
	{
		/// <summary>
		/// The undo stack.
		/// </summary>
		private readonly Stack<Step> _undoStack = new Stack<Step>();

		/// <summary>
		/// The redo stack.
		/// </summary>
		private readonly Stack<Step> _redoStack = new Stack<Step>();


		/// <inheritdoc/>
		public UndoableGrid(short[] masks) : base(masks)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified grid (to convert to
		/// an undoable grid).
		/// </summary>
		/// <param name="grid">The grid.</param>
		public UndoableGrid(Grid grid) : this((short[])grid._masks.Clone())
		{
		}

		/// <summary>
		/// Initializes an instance with the specified read only grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <exception cref="ArgumentException">
		/// Throws when the type is invalid (invalid grid type).
		/// </exception>
		public UndoableGrid(IReadOnlyGrid grid)
			: this((short[])(
				(grid as Grid)?._masks.Clone()
				?? throw new ArgumentException("The type is invalid.", nameof(grid))))
		{
		}


		/// <summary>
		/// Indicates whether the grid has any undo steps available.
		/// </summary>
		public bool HasUndoSteps => _undoStack.Count != 0;

		/// <summary>
		/// Indicates whether the grid has any redo steps available.
		/// </summary>
		public bool HasRedoSteps => _redoStack.Count != 0;


		/// <inheritdoc/>
		public override int this[int offset]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => base[offset];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				var map = GridMap.Empty;
				foreach (int cell in GridProcessings.Peers[offset])
				{
					if (cell == offset || GetCellStatus(cell) != CellStatus.Empty)
					{
						continue;
					}

					map.Add(cell);
				}
				_undoStack.Push(new AssignmentStep(value, offset, _masks[offset], map));

				// Do step.
				base[offset] = value;
			}
		}

		/// <inheritdoc/>
		public override bool this[int offset, int digit]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => base[offset, digit];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				_undoStack.Push(
					value
						? (Step)new EliminationStep(digit, offset)
						: new AntiEliminationStep(digit, offset));

				// Do step.
				base[offset, digit] = value;
			}
		}


		/// <inheritdoc/>
		public override void Fix()
		{
			var map = GridMap.Empty;
			for (int i = 0; i < 81; i++)
			{
				if (GetCellStatus(i) == CellStatus.Modifiable)
				{
					map.Add(i);
				}
			}

			_undoStack.Push(new FixStep(map));
			foreach (int cell in map.Offsets)
			{
				ref short mask = ref _masks[cell];
				mask = (short)((int)CellStatus.Given << 9 | mask & 511);
			}

			Array.Copy(_masks, _initialMasks, 81);
		}

		/// <inheritdoc/>
		public override void Unfix()
		{
			var map = GridMap.Empty;
			for (int i = 0; i < 81; i++)
			{
				if (GetCellStatus(i) == CellStatus.Given)
				{
					map.Add(i);
				}
			}

			_undoStack.Push(new UnfixStep(map));
			foreach (int cell in map.Offsets)
			{
				ref short mask = ref _masks[cell];
				mask = (short)((int)CellStatus.Modifiable << 9 | mask & 511);
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Reset()
		{
			_undoStack.Push(new ResetStep(_initialMasks, _masks));
			base.Reset();
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SetCellStatus(int offset, CellStatus cellStatus)
		{
			_undoStack.Push(new SetCellStatusStep(offset, GetCellStatus(offset), cellStatus));
			base.SetCellStatus(offset, cellStatus);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SetMask(int offset, short value)
		{
			_undoStack.Push(new SetMaskStep(offset, GetMask(offset), value));
			base.SetMask(offset, value);
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
		public override bool Equals(object? obj) =>
			obj is UndoableGrid comparer && Equals(comparer);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(UndoableGrid other) => Equals(other.ToMutable());

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => base.GetHashCode();


		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(UndoableGrid left, UndoableGrid right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Grid left, UndoableGrid right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(UndoableGrid left, Grid right) =>
			left.Equals(right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(UndoableGrid left, UndoableGrid right) =>
			!(left == right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Grid left, UndoableGrid right) =>
			!(left == right);

		/// <include file='../GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(UndoableGrid left, Grid right) =>
			!(left == right);
	}
}
