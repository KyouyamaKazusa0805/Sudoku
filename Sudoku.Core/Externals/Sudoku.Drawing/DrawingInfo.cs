using System;
using Sudoku.DocComments;

namespace Sudoku.Drawing
{
	/// <summary>
	/// Encapsulates a drawing information pair for highlighting cells,
	/// candidates and regions.
	/// </summary>
	public readonly struct DrawingInfo : IEquatable<DrawingInfo>
	{
		/// <summary>
		/// Initializes an instance with ID and value.
		/// </summary>
		/// <param name="id">The ID.</param>
		/// <param name="value">The value.</param>
		public DrawingInfo(int id, int value) => (Id, Value) = (id, value);


		/// <summary>
		/// Indicates the ID value. The ID is the unique key corresponding to one color.
		/// </summary>
		public int Id { get; }

		/// <summary>
		/// Indicates the value.
		/// </summary>
		public int Value { get; }


		/// <inheritdoc cref="object.Equals(object?)"/>
		public override bool Equals(object? obj) => obj is DrawingInfo comparer && Equals(comparer);

		/// <inheritdoc/>
		public bool Equals(DrawingInfo other) => (Id, Value) == (other.Id, other.Value);

		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => Id * 10000 + Value;

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => (Id, Value).ToString();

		/// <inheritdoc cref="DeconstructMethod"/>
		public void Deconstruct(out int id, out int value) => (id, value) = (Id, Value);


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in DrawingInfo left, in DrawingInfo right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in DrawingInfo left, in DrawingInfo right) => !(left == right);


		/// <summary>
		/// Implicit cast from <see cref="DrawingInfo"/> to <see cref="ValueTuple{T1, T2}"/>.
		/// </summary>
		/// <param name="info">The drawing information instance.</param>
		public static explicit operator (int Id, int Value)(DrawingInfo info) => (info.Id, info.Value);
	}
}
