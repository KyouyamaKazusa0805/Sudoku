using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Sudoku.Data.Stepping;

namespace Sudoku.Data.Meta
{
	/// <summary>
	/// Provides an undo-able sudoku grid. This data structure is nearly same
	/// as <see cref="Grid"/>, but only add two methods <see cref="Undo"/>
	/// and <see cref="Redo"/>.
	/// </summary>
	/// <seealso cref="Grid"/>
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


		/// <inheritdoc/>
		public override int this[int offset]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => base[offset];
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			set
			{
				var step = new AssignmentStep(value, offset, _masks[offset], new GridMap(offset, false));
				_undoStack.Push(step);
				step.DoStepTo(this);
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
				var step = value
					? (Step)new EliminationStep(digit, offset)
					: new AssignmentStep(digit, offset, _masks[offset], new GridMap(offset, false));
				_undoStack.Push(step);
				step.DoStepTo(this);
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
					map[i] = true;
				}
			}

			var step = new FixStep(map);
			_undoStack.Push(step);
			step.DoStepTo(this);
		}

		/// <inheritdoc/>
		public override void Unfix()
		{
			var map = GridMap.Empty;
			for (int i = 0; i < 81; i++)
			{
				if (GetCellStatus(i) == CellStatus.Given)
				{
					map[i] = true;
				}
			}

			var step = new UnfixStep(map);
			_undoStack.Push(step);
			step.DoStepTo(this);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void Reset()
		{
			var step = new ResetStep(_initialMasks, _masks);
			_undoStack.Push(step);
			step.DoStepTo(this);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SetCellStatus(int offset, CellStatus cellStatus)
		{
			var step = new SetCellStatusStep(offset, GetCellStatus(offset), cellStatus);
			_undoStack.Push(step);
			step.DoStepTo(this);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override void SetMask(int offset, short value)
		{
			var step = new SetMaskStep(offset, GetMask(offset), value);
			_undoStack.Push(step);
			step.DoStepTo(this);
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Redo()
		{
			if (_redoStack.Count != 0)
			{
				var step = _redoStack.Pop();
				_undoStack.Push(step);
				step.DoStepTo(this);
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Undo()
		{
			if (_undoStack.Count != 0)
			{
				var step = _undoStack.Pop();
				_redoStack.Push(step);
				step.UndoStepTo(this);
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public override bool Equals(object? obj) => base.Equals(obj);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public bool Equals(UndoableGrid other) => Equals((Grid)other);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)] 
		public override int GetHashCode() => base.GetHashCode();

		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(UndoableGrid left, UndoableGrid right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(UndoableGrid left, UndoableGrid right) =>
			!(left == right);

		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(Grid left, UndoableGrid right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(Grid left, UndoableGrid right) =>
			!(left == right);

		/// <summary>
		/// Indicates whether two instances have a same value.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator ==(UndoableGrid left, Grid right) =>
			left.Equals(right);

		/// <summary>
		/// Indicates whether two instances have two different values.
		/// </summary>
		/// <param name="left">The left instance.</param>
		/// <param name="right">The right instance.</param>
		/// <returns>A <see cref="bool"/> result indicating that.</returns>
		public static bool operator !=(UndoableGrid left, Grid right) =>
			!(left == right);
	}
}
