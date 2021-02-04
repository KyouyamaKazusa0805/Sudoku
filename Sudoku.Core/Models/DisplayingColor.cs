using System;
using System.Extensions;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Sudoku.DocComments;

namespace Sudoku.Models
{
	/// <summary>
	/// Indicates a color that is used for displaying a conclsion, a drawing pair instance.
	/// </summary>
	public readonly struct DisplayingColor : IFormattable, IValueEquatable<DisplayingColor>
	{
		/// <summary>
		/// Indicates the alpha value.
		/// </summary>
		public byte A { get; init; }

		/// <summary>
		/// Indicates the red value.
		/// </summary>
		public byte R { get; init; }

		/// <summary>
		/// Indicates the green value.
		/// </summary>
		public byte G { get; init; }

		/// <summary>
		/// Indicates the blue value.
		/// </summary>
		public byte B { get; init; }

		/// <summary>
		/// Indicates whether the current color is a light color.
		/// </summary>
		[JsonIgnore]
		public bool IsLightColor
		{
			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			get => (.299 * R + .587 * G + .114 * B) / 255 > .5;
		}


		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="a">(<see langword="out"/> parameter) The alpha value.</param>
		/// <param name="r">(<see langword="out"/> parameter) The red value.</param>
		/// <param name="g">(<see langword="out"/> parameter) The green value.</param>
		/// <param name="b">(<see langword="out"/> parameter) The blue value.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public void Deconstruct(out byte a, out byte r, out byte g, out byte b)
		{
			a = A;
			r = R;
			g = G;
			b = B;
		}

		/// <inheritdoc/>
		[CLSCompliant(false)]
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public bool Equals(in DisplayingColor other) =>
			A == other.A && R == other.R && G == other.G && B == other.B;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override bool Equals(object? obj) => obj is DisplayingColor comparer && Equals(comparer);

		/// <inheritdoc cref="object.GetHashCode"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override int GetHashCode() => A << 24 | R << 16 | G << 8 | B;

		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public override string ToString() => ToString(null, null);

		/// <inheritdoc cref="Formattable.ToString(string?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string format) => ToString(format, null);

		/// <inheritdoc/>
		/// <remarks>
		/// All available format strings are:
		/// <list type="table">
		/// <item>
		/// <term><c><see langword="null"/></c> or <c><see cref="string.Empty"/></c></term>
		/// <description>
		/// A quadruple that contains <see cref="A"/>, <see cref="R"/>, <see cref="G"/>
		/// and <see cref="B"/> value.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c>"x"</c> or <c>"x"</c></term>
		/// <description>HTML color notation. For example: <c>#FFFFFFFF</c>.</description>
		/// </item>
		/// <item>
		/// <term><c>"!x"</c> or <c>"!x"</c></term>
		/// <description>
		/// HTML color notation without <see cref="A"/> value. For example: <c>#FFFFFF</c>.
		/// </description>
		/// </item>
		/// <item>
		/// <term><c>"V"</c> or <c>"v"</c></term>
		/// <description>An integer value that represents the current instance.</description>
		/// </item>
		/// </list>
		/// </remarks>
		/// <exception cref="FormatException">Throws when the format is invalid.</exception>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public string ToString(string? format, IFormatProvider? formatProvider) =>
			format?.ToLower() switch
			{
				null or "" => (A, R, G, B).ToString(),
				"x" => $"#{A.ToString("X2")}{R.ToString("X2")}{G.ToString("X2")}{B.ToString("X2")}",
				"!x" => $"#{R.ToString("X2")}{G.ToString("X2")}{B.ToString("X2")}",
				"v" => GetHashCode().ToString(),
				_ when formatProvider.HasFormatted(this, format, out string? result) => result,
				_ => throw new FormatException("The format string is invalid.")
			};


		/// <summary>
		/// Determine whether the specified value is a valid Win32 color value.
		/// </summary>
		/// <param name="value">The value to check.</param>
		/// <param name="result">
		/// (<see langword="out"/> parameter) The result color that created. If the value is greater than
		/// the maximum value of the Win32 color limit, it'll be truncated.
		/// </param>
		/// <returns>A <see cref="bool"/> value indicating that.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static bool IsWin32Color(int value, out DisplayingColor result)
		{
			byte b = (byte)(value & 255);
			byte g = (byte)(value >> 8 & 255);
			byte r = (byte)(value >> 16 & 255);
			byte a = (byte)(value >> 24 & 255);

			result = new() { A = a, R = r, G = g, B = b };

			return value > (a << 24 | r << 16 | g << 8 | b);
		}

		/// <summary>
		/// Creates a <see cref="DisplayingColor"/> instance using the specified red, green and blue values.
		/// </summary>
		/// <param name="red">The red value.</param>
		/// <param name="green">The green value.</param>
		/// <param name="blue">The blue value.</param>
		/// <returns>The instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DisplayingColor FromArgb(byte red, byte green, byte blue) =>
			new() { A = 255, R = red, G = green, B = blue };

		/// <summary>
		/// Creates a <see cref="DisplayingColor"/> instance using the specified alpha, red, green
		/// and blue values.
		/// </summary>
		/// <param name="alpha">The alpha value.</param>
		/// <param name="red">The red value.</param>
		/// <param name="green">The green value.</param>
		/// <param name="blue">The blue value.</param>
		/// <returns>The instance.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		public static DisplayingColor FromArgb(byte alpha, byte red, byte green, byte blue) =>
			new() { A = alpha, R = red, G = green, B = blue };


		/// <inheritdoc cref="Operators.operator =="/>
		public static bool operator ==(in DisplayingColor left, in DisplayingColor right) => left.Equals(right);

		/// <inheritdoc cref="Operators.operator !="/>
		public static bool operator !=(in DisplayingColor left, in DisplayingColor right) => !(left == right);
	}
}
