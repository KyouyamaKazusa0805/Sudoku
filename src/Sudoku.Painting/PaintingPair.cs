using System;
using System.Drawing;
using System.Runtime.CompilerServices;
using Sudoku.DocComments;

namespace Sudoku.Painting
{
	/// <summary>
	/// Encapsulates a painting pair that contains the base color to paint and the value.
	/// </summary>
	/// <typeparam name="T">The type of the value.</typeparam>
	public readonly struct PaintingPair<T> : IValueEquatable<PaintingPair<T>> where T : unmanaged
	{
		/// <summary>
		/// Initializes an instance with the color palette index and the value.
		/// </summary>
		/// <param name="paletteIndex">The palette index.</param>
		/// <param name="value">(<see langword="in"/> parameter) The value used.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PaintingPair(int paletteIndex, in T value) : this()
		{
			UsePaletteColor = true;
			PaletteColorIndex = paletteIndex;
			Value = value;
		}

		/// <summary>
		/// Initializes an instance with the color and the value.
		/// </summary>
		/// <param name="color">(<see langword="in"/> parameter) The color used.</param>
		/// <param name="value">(<see langword="in"/> parameter) The value used.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public PaintingPair(in Color color, in T value) : this()
		{
			UsePaletteColor = false;
			Color = color;
			Value = value;
		}


		/// <summary>
		/// Indicates the palette color index.
		/// </summary>
		/// <remarks>
		/// The property contains value if <see cref="UsePaletteColor"/> is <see langword="true"/>.
		/// </remarks>
		public int PaletteColorIndex { get; }

		/// <summary>
		/// Indicates whether the program uses the palette color to draw and render.
		/// </summary>
		/// <remarks>
		/// If <see langword="true"/>, we won't assign the property <see cref="Color"/>.
		/// </remarks>
		public bool UsePaletteColor { get; }

		/// <summary>
		/// Indicates the displaying color that the current instance held.
		/// </summary>
		/// <remarks>
		/// The property contains value if <see cref="UsePaletteColor"/> is <see langword="false"/>.
		/// </remarks>
		public Color Color { get; }

		/// <summary>
		/// Indicates the value used.
		/// </summary>
		public T Value { get; }


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="usePaletteColor">
		/// (<see langword="out"/> parameter) Indicates whether the instance stores the index
		/// rather than color value.
		/// </param>
		/// <param name="paletteIndex">
		/// (<see langword="out"/> parameter) The palette color index. The value should be valid
		/// to use when the value <paramref name="usePaletteColor"/> is <see langword="true"/>.
		/// </param>
		/// <param name="color">
		/// (<see langword="out"/> parameter) The color. The palette color index. The value should be valid
		/// to use when the value <paramref name="usePaletteColor"/> is <see langword="false"/>.
		/// </param>
		/// <param name="value">(<see langword="out"/> parameter) The value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out bool usePaletteColor, out int paletteIndex, out Color color, out T value)
		{
			usePaletteColor = UsePaletteColor;
			paletteIndex = PaletteColorIndex;
			color = Color;
			value = Value;
		}

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
