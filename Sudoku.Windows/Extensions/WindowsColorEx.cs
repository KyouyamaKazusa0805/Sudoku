using System;
using System.Runtime.InteropServices;
using Sudoku.DocComments;
using static System.Convert;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.Extensions
{
	/// <summary>
	/// Provides extension methods on <see cref="WColor"/>.
	/// </summary>
	/// <seealso cref="WColor"/>
	public static class WindowsColorEx
	{
		/// <summary>
		/// Get the hue from the specified <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The color.</param>
		/// <returns>A <see cref="float"/> value.</returns>
		public static float GetHue(this in WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B).GetHue();

		/// <summary>
		/// Get the brightness from the specified <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The color.</param>
		/// <returns>A <see cref="float"/> value.</returns>
		public static float GetBrightness(this in WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B).GetBrightness();

		/// <summary>
		/// Get the saturation from the specified <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The color.</param>
		/// <returns>A <see cref="float"/> value.</returns>
		public static float GetSaturation(this in WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B).GetSaturation();

		/// <inheritdoc cref="DeconstructMethod"/>
		/// <param name="this">(<see langword="this in"/> parameter) The color.</param>
		/// <param name="a">(<see langword="out"/> parameter) The alpha value.</param>
		/// <param name="r">(<see langword="out"/> parameter) The red value.</param>
		/// <param name="g">(<see langword="out"/> parameter) The green value.</param>
		/// <param name="b">(<see langword="out"/> parameter) The blue value.</param>
		public static void Deconstruct(this in WColor @this, out byte a, out byte r, out byte g, out byte b)
		{
			a = @this.A;
			r = @this.R;
			g = @this.G;
			b = @this.B;
		}

		/// <summary>
		/// Convert <see cref="WColor"/> to <see cref="DColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The color.</param>
		/// <returns>The target color.</returns>
		public static DColor ToDColor(this in WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B);

		/// <summary>
		/// Converts the <see cref="WColor"/> to the specified hex string.
		/// </summary>
		/// <param name="this">(<see langword="this in"/> parameter) The color.</param>
		/// <returns>The result.</returns>
		public static string ToHexString(this in WColor @this)
		{
			var (a, r, g, b) = @this;
			return $"#{a:X2}{r:X2}{g:X2}{b:X2}";
		}

		/// <summary>
		/// Get a color from alpha, hue, saturation and brightness.
		/// </summary>
		/// <param name="alpha">The alpha.</param>
		/// <param name="hue">The hue.</param>
		/// <param name="saturation">The saturation.</param>
		/// <param name="brightness">The brightness.</param>
		/// <returns>The <see cref="WColor"/>.</returns>
		/// <exception cref="ArgumentOutOfRangeException">
		/// Throws when:
		/// <list type="bullet">
		/// <item><paramref name="alpha"/> is less than 0 or greater than 255.</item>
		/// <item><paramref name="hue"/> is less than 0 or greater than 360.</item>
		/// <item><paramref name="saturation"/> is less than 0 or greater than 1.</item>
		/// <item><paramref name="brightness"/> is less than 0 or greater than 1.</item>
		/// </list>
		/// </exception>
		public static WColor FromAhsb(int alpha, float hue, float saturation, float brightness)
		{
			_ = alpha is < 0 or > 255 ? throw new ArgumentOutOfRangeException(nameof(alpha), alpha, "Value must be within a range of 0 - 255.") : 0;
			_ = hue is < 0 or > 360F ? throw new ArgumentOutOfRangeException(nameof(hue), hue, "Value must be within a range of 0 - 360.") : 0;
			_ = saturation is < 0 or > 1F ? throw new ArgumentOutOfRangeException(nameof(saturation), saturation, "Value must be within a range of 0 - 1.") : 0;
			_ = brightness is < 0 or > 1F ? throw new ArgumentOutOfRangeException(nameof(brightness), brightness, "Value must be within a range of 0 - 1.") : 0;

			if (saturation == 0)
			{
				return WColor.FromArgb(
					(byte)alpha, (byte)(brightness * 255), (byte)(brightness * 255), (byte)(brightness * 255));
			}

			float fMax, fMid, fMin;
			if (brightness > .5)
			{
				fMax = brightness - brightness * saturation + saturation;
				fMin = brightness + brightness * saturation - saturation;
			}
			else
			{
				fMax = brightness + brightness * saturation;
				fMin = brightness - brightness * saturation;
			}

			int iSextant = (int)Math.Floor(hue / 60F);
			if (hue >= 300F)
			{
				hue -= 360F;
			}
			hue /= 60F;
			hue -= 2F * (float)Math.Floor((iSextant + 1F) % 6F / 2F);
			fMid = iSextant % 2 == 0 ? hue * (fMax - fMin) + fMin : fMin - hue * (fMax - fMin);

			byte iMax = (byte)(fMax * 255), iMid = (byte)(fMid * 255), iMin = (byte)(fMin * 255);
			return iSextant switch
			{
				1 => WColor.FromArgb((byte)alpha, iMid, iMax, iMin),
				2 => WColor.FromArgb((byte)alpha, iMin, iMax, iMid),
				3 => WColor.FromArgb((byte)alpha, iMin, iMid, iMax),
				4 => WColor.FromArgb((byte)alpha, iMid, iMin, iMax),
				5 => WColor.FromArgb((byte)alpha, iMax, iMin, iMid),
				_ => WColor.FromArgb((byte)alpha, iMax, iMid, iMin),
			};
		}

		/// <summary>
		/// Get a new color from the specified H, S and L values.
		/// </summary>
		/// <param name="h">The H value.</param>
		/// <param name="s">The S value.</param>
		/// <param name="l">The L value.</param>
		/// <returns>The <see cref="WColor"/>.</returns>
		public static WColor FromHsl(int h, int s, int l)
		{
			byte[] bytes = BitConverter.GetBytes(hls2Rgb(h, l, s));
			return WColor.FromArgb(255, bytes[0], bytes[1], bytes[2]);

			[DllImport("shlwapi.dll")]
			static extern int hls2Rgb(int h, int l, int s);
		}

		/// <summary>
		/// Get the specified color from the specified hex string such as "<c>#FFFFFF</c>".
		/// </summary>
		/// <param name="hex">The hex string.</param>
		/// <returns>The <see cref="WColor"/>.</returns>
		public static WColor FromHex(string hex) =>
			WColor.FromRgb(ToByte(hex[1..3], 16), ToByte(hex[3..5], 16), ToByte(hex[5..], 16));
	}
}
