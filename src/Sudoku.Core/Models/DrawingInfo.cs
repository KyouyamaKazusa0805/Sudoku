using System;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Models
{
	/// <summary>
	/// Encapsulates a drawing information pair for highlighting cells,
	/// candidates and regions.
	/// </summary>
	[Obsolete("Please use Sudoku.Drawing.PaintingPair`1 instead.")]
	public readonly struct DrawingInfo : IValueEquatable<DrawingInfo>
	{
		/// <summary>
		/// Initializes an instance with ID and value.
		/// </summary>
		/// <param name="id">The ID.</param>
		/// <param name="value">The value.</param>
		public DrawingInfo(long id, int value)
		{
			Id = id;
			Value = value;
		}


		/// <summary>
		/// Indicates the ID value. The ID is the unique key corresponding to one color.
		/// </summary>
		public long Id { get; }

		/// <summary>
		/// Indicates the value.
		/// </summary>
		public int Value { get; }


		/// <inheritdoc cref="object.Equals(object?)"/>
		public override bool Equals(object? obj) => obj is DrawingInfo comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		public bool Equals(in DrawingInfo other) => Id == other.Id && Value == other.Value;

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => (int)((Id * 10000L + Value) % int.MaxValue);

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => (Id, Value).ToString();

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="id">(<see langword="out"/> parameter) The ID.</param>
		/// <param name="value">(<see langword="out"/> parameter) The value.</param>
		public void Deconstruct(out long id, out int value)
		{
			id = Id;
			value = Value;
		}


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in DrawingInfo left, in DrawingInfo right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in DrawingInfo left, in DrawingInfo right) => !(left == right);


		/// <summary>
		/// Implicit cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="DrawingInfo"/>.
		/// </summary>
		/// <param name="pair">(<see langword="in"/> parameter) The pair value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DrawingInfo(in (long Id, int Value) pair) => new(pair.Id, pair.Value);
	}
}
