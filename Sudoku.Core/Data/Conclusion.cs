using System;
using System.Diagnostics;
using Sudoku.DocComments;

namespace Sudoku.Data
{
	/// <summary>
	/// Encapsulates a conclusion representation while solving in logic.
	/// </summary>
	[DebuggerStepThrough]
	public readonly struct Conclusion : IEquatable<Conclusion>
	{
		/// <summary>
		/// Initializes an instance with a conclusion type, a cell offset and a digit.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="cellOffset">The cell offset.</param>
		/// <param name="digit">The digit.</param>
		public Conclusion(ConclusionType conclusionType, int cellOffset, int digit) =>
			(ConclusionType, CellOffset, Digit) = (conclusionType, cellOffset, digit);

		/// <summary>
		/// Initializes an instance with a conclusion type and a candidate offset.
		/// </summary>
		/// <param name="conclusionType">The conclusion type.</param>
		/// <param name="candidateOffset">The candidate offset.</param>
		public Conclusion(ConclusionType conclusionType, int candidateOffset)
			: this(conclusionType, candidateOffset / 9, candidateOffset % 9)
		{
		}


		/// <summary>
		/// The cell offset.
		/// </summary>
		public int CellOffset { get; }

		/// <summary>
		/// The digit.
		/// </summary>
		public int Digit { get; }

		/// <summary>
		/// The conclusion type to control the action of applying.
		/// If the type is <see cref="ConclusionType.Assignment"/>,
		/// this conclusion will be set value (Set a digit into a cell);
		/// otherwise, a candidate will be removed.
		/// </summary>
		public ConclusionType ConclusionType { get; }


		/// <summary>
		/// Put this instance into the specified grid.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <exception cref="InvalidOperationException">
		/// Throws when the specified conclusion type is neither <see cref="ConclusionType.Assignment"/>
		/// nor <see cref="ConclusionType.Elimination"/>.
		/// </exception>
		public void ApplyTo(Grid grid)
		{
			switch (ConclusionType)
			{
				case ConclusionType.Assignment:
				{
					grid[CellOffset] = Digit;
					break;
				}
				case ConclusionType.Elimination:
				{
					grid[CellOffset, Digit] = true;
					break;
				}
				default:
				{
					throw new InvalidOperationException("Cannot apply to grid due to invalid conclusion type.");
				}
			}
		}

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="conclusionType">(<see langword="out"/> parameter) The type of this conclusion.</param>
		/// <param name="candidate">(<see langword="out"/> parameter) The candidate.</param>
		public void Deconstruct(out ConclusionType conclusionType, out int candidate) =>
			(conclusionType, candidate) = (ConclusionType, CellOffset * 9 + Digit);

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="conclusionType">(<see langword="out"/> parameter) The type of this conclusion.</param>
		/// <param name="cell">(<see langword="out"/> parameter) The cell.</param>
		/// <param name="digit">(<see langword="out"/> parameter) The digit.</param>
		public void Deconstruct(out ConclusionType conclusionType, out int cell, out int digit) =>
			(conclusionType, cell, digit) = (ConclusionType, CellOffset, Digit);

		/// <inheritdoc/>
		public override bool Equals(object? obj) => obj is Conclusion comparer && Equals(comparer);

		/// <inheritdoc cref="IEquatable{T}.Equals(T)"/>
		public bool Equals(Conclusion other) => GetHashCode() == other.GetHashCode();

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => ((int)ConclusionType + 1) * (CellOffset * 9 + Digit);

		/// <inheritdoc cref="object.ToString"/>
		/// <exception cref="InvalidOperationException">
		/// Throws when the current conclusion type is invalid (neither <see cref="ConclusionType.Assignment"/>
		/// nor <see cref="ConclusionType.Elimination"/>.
		/// </exception>
		/// <seealso cref="ConclusionType"/>
		public override string ToString() =>
			$@"r{CellOffset / 9 + 1}c{CellOffset % 9 + 1} {ConclusionType switch
			{
				ConclusionType.Assignment => "=",
				ConclusionType.Elimination => "<>",
				_ => throw new InvalidOperationException("Cannot get the string due to invalid conclusion type.")
			}} {Digit + 1}";


		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Equality"]'/>
		public static bool operator ==(Conclusion left, Conclusion right) => left.Equals(right);

		/// <include file='..\GlobalDocComments.xml' path='comments/operator[@name="op_Inequality"]'/>
		public static bool operator !=(Conclusion left, Conclusion right) => !(left == right);
	}
}
