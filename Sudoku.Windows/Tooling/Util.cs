using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using Sudoku.Drawing.Extensions;
using DColor = System.Drawing.Color;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.Tooling
{
	internal static class Util
	{
		[DllImport("shlwapi.dll")]
		public static extern int ColorHLSToRGB(int H, int L, int S);

		public static WColor ColorFromHSL(int H, int S, int L)
		{
			int colorInt = ColorHLSToRGB(H, L, S);
			byte[] bytes = BitConverter.GetBytes(colorInt);
			return WColor.FromArgb(255, bytes[0], bytes[1], bytes[2]);
		}

		/// <summary>
		/// Converts the <see cref="WColor"/> to the specified hex string.
		/// </summary>
		/// <param name="this">(<see langword="this"/> parameter) The color.</param>
		/// <returns>The result.</returns>
		public static string ToHexString(this WColor @this) => $"#{@this.A:X2}{@this.R:X2}{@this.G:X2}{@this.B:X2}";

		public static WColor ColorFromHexString(string hex)
		{
			return WColor.FromRgb(
			   Convert.ToByte(hex.Substring(1, 2), 16),
			   Convert.ToByte(hex.Substring(3, 2), 16),
			   Convert.ToByte(hex.Substring(5, 2), 16));
		}

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

		public static float GetHue(this WColor c) => DColor.FromArgb(c.A, c.R, c.G, c.B).GetHue();

		public static float GetBrightness(this WColor c) => DColor.FromArgb(c.A, c.R, c.G, c.B).GetBrightness();

		public static float GetSaturation(this WColor c) => DColor.FromArgb(c.A, c.R, c.G, c.B).GetSaturation();

		public static WColor FromAhsb(int alpha, float hue, float saturation, float brightness)
		{
			if (0 > alpha || 255 < alpha)
			{
				throw new ArgumentOutOfRangeException(
					nameof(alpha), alpha, "Value must be within a range of 0 - 255.");
			}
			if (hue < 0 || hue > 360F)
			{
				throw new ArgumentOutOfRangeException(
					nameof(hue), hue, "Value must be within a range of 0 - 360.");
			}
			if (saturation < 0 || saturation > 1F)
			{
				throw new ArgumentOutOfRangeException(
					nameof(saturation), saturation, "Value must be within a range of 0 - 1.");
			}
			if (brightness < 0 ||  brightness > 1F)
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
				fMax = brightness - (brightness * saturation) + saturation;
				fMin = brightness + (brightness * saturation) - saturation;
			}
			else
			{
				fMax = brightness + (brightness * saturation);
				fMin = brightness - (brightness * saturation);
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

		public static List<WColor> GetWebColors()
		{
			var list = new List<WColor>();
			foreach (var info in typeof(DColor).GetProperties(BindingFlags.Public | BindingFlags.Static))
			{
				list.Add(DColor.FromName(info.Name).ToWColor());
			}

			return list;
		}


		public static void SaveToXml<T>(this T @this, string filename)
		{
			var xml = @this.GetXmlText();

			File.WriteAllText(filename, xml);
		}

		public static string GetXmlText<T>(this T obj)
		{
			using var sw = new StringWriter();
			using var writer = XmlWriter.Create(sw, new XmlWriterSettings()
			{
				Indent = true,
				IndentChars = "    ",
				NewLineOnAttributes = false
			});
			new XmlSerializer(typeof(T)).Serialize(writer, obj);
			return sw.ToString();
		}

		[return: MaybeNull]
		public static T LoadFromXml<T>(this T @this, string filename)
		{
			var result = default(T);
			if (File.Exists(filename))
			{
				using var sr = new StreamReader(filename);
				using var xr = new XmlTextReader(sr);
				result = (T)new XmlSerializer(typeof(T)).Deserialize(xr);
			}

			return result;
		}

		[return: MaybeNull]
		public static T LoadFromXmlText<T>(this T @this, string xml)
		{
			var result = default(T);
			if (!string.IsNullOrEmpty(xml))
			{
				using var xr = XmlReader.Create(new StringReader(xml));
				result = (T)new XmlSerializer(typeof(T)).Deserialize(xr);
			}

			return result;
		}
	}
}
