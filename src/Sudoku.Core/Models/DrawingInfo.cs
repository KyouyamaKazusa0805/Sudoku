using System;
using System.Runtime.CompilerServices;
using Sudoku.CodeGen.Deconstruction.Annotations;
using Sudoku.CodeGen.Equality.Annotations;
using Sudoku.DocComments;

namespace Sudoku.Models
{
	/// <summary>
	/// Encapsulates a drawing information pair for highlighting cells,
	/// candidates and regions.
	/// </summary>
	[Obsolete("Please use Sudoku.Drawing.PaintingPair`1 instead.", false)]
	[AutoDeconstruct(nameof(Id), nameof(Value))]
	[AutoEquality(nameof(Id), nameof(Value))]
	public readonly partial struct DrawingInfo : IValueEquatable<DrawingInfo>
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


		/// <inheritdoc cref="object.GetHashCode"/>
		public override int GetHashCode() => (int)((Id * 10000L + Value) % int.MaxValue);

		/// <inheritdoc cref="object.ToString"/>
		public override string ToString() => (Id, Value).ToString();


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in DrawingInfo left, in DrawingInfo right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in DrawingInfo left, in DrawingInfo right) => !(left == right);


		/// <summary>
		/// Implicit cast from <see cref="ValueTuple{T1, T2}"/> to <see cref="DrawingInfo"/>.
		/// </summary>
		/// <param name="pair">The pair value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static implicit operator DrawingInfo(in (long Id, int Value) pair) => new(pair.Id, pair.Value);
	}
}
