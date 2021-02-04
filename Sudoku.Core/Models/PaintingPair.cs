using System;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Models
{
	/// <summary>
	/// Encapsulates a painting pair that contains the base color to paint and the value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public readonly struct PaintingPair<T> : IValueEquatable<PaintingPair<T>> where T : unmanaged
	{
		/// <summary>
		/// Initializes an instance with two values.
		/// </summary>
		/// <param name="color">(<see langword="in"/> parameter) The color used.</param>
		/// <param name="value">(<see langword="in"/> parameter) The value used.</param>
		public PaintingPair(in DisplayingColor color, in T value)
		{
			Color = color;
			Value = value;
		}


		/// <summary>
		/// Indicates the displaying color that the current instance held.
		/// </summary>
		public DisplayingColor Color { get; }

		/// <summary>
		/// Indicates the value used.
		/// </summary>
		public T Value { get; }


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is PaintingPair<T> comparer && Equals(comparer);

		/// <inheritdoc/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(in PaintingPair<T> other)
		{
			T l = Value, r = other.Value;
			return Color == other.Color && Unsafe.AreSame(ref l, ref r);
		}

		/// <inheritdoc cref="object.GetHashCode"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => HashCode.Combine(Value, Color);

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => (Color, Value).ToString();


		/// <inheritdoc cref="Operators.operator =="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator ==(in PaintingPair<T> left, in PaintingPair<T> right) =>
			left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool operator !=(in PaintingPair<T> left, in PaintingPair<T> right) =>
			!(left == right);
	}
}
