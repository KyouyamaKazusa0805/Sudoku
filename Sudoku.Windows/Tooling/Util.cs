using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using Sudoku.Drawing.Extensions;
using static System.Reflection.BindingFlags;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Methods for the internal utilities.
	/// </summary>
	internal static class Util
	{
		/// <summary>
		/// Color HLS to RGB.
		/// </summary>
		/// <param name="h">The H value.</param>
		/// <param name="l">The L value.</param>
		/// <param name="s">The S value.</param>
		/// <returns></returns>
		[DllImport("shlwapi.dll")]
		public static extern int ColorHLSToRGB(int h, int l, int s);

		/// <summary>
		/// Get a new color from the specified H, S and L values.
		/// </summary>
		/// <param name="h">The H value.</param>
		/// <param name="s">The S value.</param>
		/// <param name="l">The L value.</param>
		/// <returns>The <see cref="WColor"/>.</returns>
		public static WColor ColorFromHSL(int h, int s, int l)
		{
			byte[] bytes = BitConverter.GetBytes(ColorHLSToRGB(h, l, s));
			return WColor.FromArgb(255, bytes[0], bytes[1], bytes[2]);
		}

		/// <summary>
		/// Converts the <see cref="WColor"/> to the specified hex string.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The result.</returns>
		public static string ToHexString(this WColor @this) => $"#{@this.A:X2}{@this.R:X2}{@this.G:X2}{@this.B:X2}";

		/// <summary>
		/// Get the specified color from the specified hex string such as "<c>#FFFFFF</c>".
		/// </summary>
		/// <param name="hex">The hex string.</param>
		/// <returns>The <see cref="WColor"/>.</returns>
		public static WColor ColorFromHexString(string hex) =>
			WColor.FromRgb(
				Convert.ToByte(hex.Substring(1, 2), 16),
				Convert.ToByte(hex.Substring(3, 2), 16),
				Convert.ToByte(hex.Substring(5, 2), 16));

		/// <summary>
		/// Get the <see cref="BitmapImage"/> from the specified <see cref="BitmapSource"/>.
		/// </summary>
		/// <param name="bitmapSource">The bitmap source.</param>
		/// <returns>The image.</returns>
		public static BitmapImage GetBitmapImage(BitmapSource bitmapSource)
		{
			var encoder = new JpegBitmapEncoder();
			using var memoryStream = new MemoryStream();
			var bImg = new BitmapImage();

			encoder.Frames.Add(BitmapFrame.Create(bitmapSource));
			encoder.Save(memoryStream);

			memoryStream.Position = 0;
			bImg.BeginInit();
			bImg.StreamSource = memoryStream;
			bImg.EndInit();

			return bImg;
		}

		/// <summary>
		/// Get the hue from the specified <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>A <see cref="float"/> value.</returns>
		public static float GetHue(this WColor @this) => DColor.FromArgb(@this.A, @this.R, @this.G, @this.B).GetHue();

		/// <summary>
		/// Get the brightness from the specified <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>A <see cref="float"/> value.</returns>
		public static float GetBrightness(this WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B).GetBrightness();

		/// <summary>
		/// Get the saturation from the specified <see cref="WColor"/>.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>A <see cref="float"/> value.</returns>
		public static float GetSaturation(this WColor @this) =>
			DColor.FromArgb(@this.A, @this.R, @this.G, @this.B).GetSaturation();

		/// <summary>
		/// Get a color from alpha, hue, saturation and brightness.
		/// </summary>
		/// <param name="alpha">The alpha.</param>
		/// <param name="hue">The hue.</param>
		/// <param name="saturation">The saturation.</param>
		/// <param name="brightness">The brightness.</param>
		/// <returns>The <see cref="WColor"/>.</returns>
		public static WColor FromAhsb(int alpha, float hue, float saturation, float brightness)
		{
			if (alpha is < 0 or > 255)
			{
				throw new ArgumentOutOfRangeException(nameof(alpha), alpha, "Value must be within a range of 0 - 255.");
			}
			if (hue is < 0 or > 360F)
			{
				throw new ArgumentOutOfRangeException(nameof(hue), hue, "Value must be within a range of 0 - 360.");
			}
			if (saturation is < 0 or > 1F)
			{
				throw new ArgumentOutOfRangeException(
					nameof(saturation), saturation, "Value must be within a range of 0 - 1.");
			}
			if (brightness is < 0 or > 1F)
			{
				throw new ArgumentOutOfRangeException(
					nameof(brightness), brightness, "Value must be within a range of 0 - 1.");
			}

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
		/// Get all colors especially used for HTML.
		/// </summary>
		/// <returns>All colors.</returns>
		public static IReadOnlyList<WColor> GetWebColors() => (
			from @property in typeof(DColor).GetProperties(Public | Static)
			where @property.PropertyType == typeof(DColor)
			select DColor.FromName(@property.Name).ToWColor()).ToArray();

		/// <summary>
		/// Serialize the specified instance to the specified file.
		/// </summary>
		/// <typeparam name="T">The type of the specified instance.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="filename">The file name.</param>
		public static void SaveToXml<T>(this T @this, string filename) => File.WriteAllText(filename, @this.GetXmlText());

		/// <summary>
		/// Get the text from the specified text.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <returns>The string.</returns>
		internal static string GetXmlText<T>(this T @this)
		{
			using var sw = new StringWriter();
			using var writer = XmlWriter.Create(sw, new() { Indent = true, IndentChars = "    ", NewLineOnAttributes = false });
			new XmlSerializer(typeof(T)).Serialize(writer, @this);
			return sw.ToString();
		}

		/// <summary>
		/// Deserialize the file.
		/// </summary>
		/// <typeparam name="T">The type of the instance.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="filename">The file name.</param>
		/// <returns>The instance.</returns>
		[method: SuppressMessage("Style", "IDE0060:Remove unused parameter", Justification = "<Pending>")]
		internal static T? LoadFromXml<T>(this T? @this, string filename)
		{
			T? result = default;
			if (File.Exists(filename))
			{
				using var sr = new StreamReader(filename);
				using var xr = new XmlTextReader(sr);
				result = (T)new XmlSerializer(typeof(T)).Deserialize(xr);
			}

			return result;
		}
	}
}
