using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Data;
using Sudoku.DocComments;
using Sudoku.Solving.Manual;

namespace Sudoku.Solving.Manual.Tracing
{
	/// <summary>
	/// Encapsulates a sudoku grid that all steps can be traced.
	/// </summary>
	/// <remarks>
	/// Note that this data structure doesn't check any causes in equality methods.
	/// </remarks>
	public sealed class TraceableGrid : IEquatable<TraceableGrid?>, IFormattable, ITraceable
	{
		/// <summary>
		/// Indicates the iner sudoku grid.
		/// </summary>
		private SudokuGrid _innerGrid;

		/// <summary>
		/// Indictaes the table that holds reasons why the candidate has been eliminated.
		/// </summary>
		private readonly StepInfo?[] _causes;


		/// <summary>
		/// Initializes an instance with the specified iner sudoku grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private TraceableGrid(in SudokuGrid grid)
		{
			_innerGrid = grid;
			_causes = new StepInfo[729];
		}


		/// <inheritdoc/>
		[IndexerName("Cause")]
		public StepInfo? this[int candidate]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _causes[candidate];
		}

		/// <inheritdoc/>
		[IndexerName("Cause")]
		public StepInfo? this[int cell, int digit]
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => _causes[cell * 9 + digit];
		}


		/// <inheritdoc/>
		public void BindStep(StepInfo stepInfo)
		{
			foreach (var (t, c, d) in stepInfo.Conclusions)
			{
				switch (t)
				{
					case ConclusionType.Assignment:
					{
						_innerGrid[c] = d;
						break;
					}
					case ConclusionType.Elimination:
					{
						_innerGrid[c, d] = false;
						_causes[c * 9 + d] = stepInfo;

						break;
					}
				}
			}
		}

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is TraceableGrid comparer && Equals(comparer);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(TraceableGrid? other) => other is not null && _innerGrid == other._innerGrid;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => _innerGrid.GetHashCode();

		/// <inheritdoc cref="SudokuGrid.GetPinnableReference"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public ref readonly short GetPinnableReference() => ref _innerGrid.GetPinnableReference();

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			_innerGrid.ToString(format, formatProvider);

		/// <summary>
		/// Get the reason why the candidate has been eliminated.
		/// </summary>
		/// <param name="candidate">The candidate.</param>
		/// <returns>
		/// The reason to cause the elimination. If the candidate isn't eliminated,
		/// the return value will be <see langword="null"/>.
		/// </returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public StepInfo? GetCause(int candidate) => _causes[candidate];

		/// <inheritdoc/>
		public IEnumerator<(int Candidate, StepInfo Info)> GetEnumerator()
		{
			for (int i = 0; i < 729; i++)
			{
				if (_causes[i] is { } cause)
				{
					yield return (i, cause);
				}
			}
		}


		/// <inheritdoc cref="SudokuGrid.TryParse(string, out SudokuGrid)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool TryParse(string str, [NotNullWhen(true)] out TraceableGrid? result)
		{
			if (SudokuGrid.TryParse(str, out var tempGrid))
			{
				result = tempGrid;
				return true;
			}
			else
			{
				result = null;
				return false;
			}
		}

		/// <summary>
		/// Parse a string code representing a possible sudoku grid, and convert it to
		/// <see cref="TraceableGrid"/> instance.
		/// </summary>
		/// <param name="str">The string code.</param>
		/// <returns>The instance parsed.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TraceableGrid Parse(string str) => SudokuGrid.Parse(str);


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(TraceableGrid left, in SudokuGrid right) => left._innerGrid == right;

		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in SudokuGrid left, TraceableGrid right) => left == right._innerGrid;

		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(TraceableGrid left, TraceableGrid right) =>
			left._innerGrid == right._innerGrid;

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(TraceableGrid left, in SudokuGrid right) => left._innerGrid != right;

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in SudokuGrid left, TraceableGrid right) => left != right._innerGrid;

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(TraceableGrid left, TraceableGrid right) =>
			left._innerGrid != right._innerGrid;

		/// <summary>
		/// To bind a step information with a candidate, and returns the reference of <paramref name="grid"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="stepInfo">The step information.</param>
		/// <returns>The reference of <paramref name="grid"/>.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static TraceableGrid operator +(TraceableGrid grid, StepInfo stepInfo)
		{
			grid.BindStep(stepInfo);
			return grid;
		}


		/// <summary>
		/// Implicit cast from <see cref="SudokuGrid"/> to <see cref="TraceableGrid"/>.
		/// </summary>
		/// <param name="grid">(<see langword="in"/> parameter) The grid.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator TraceableGrid(in SudokuGrid grid) => new(grid);

		/// <summary>
		/// Explicit cast from <see cref="TraceableGrid"/> to <see cref="SudokuGrid"/>.
		/// </summary>
		/// <param name="grid">The grid.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static explicit operator SudokuGrid(TraceableGrid grid) => grid._innerGrid;
	}
}
