using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
using System.Linq;
using System.Windows;
using Microsoft.Win32;
using Sudoku.Constants;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using Sudoku.Windows.Constants;
using static System.Drawing.Imaging.ImageFormat;
using static System.Drawing.StringAlignment;
using static System.IO.Path;
using static Sudoku.Windows.Constants.Processings;
using DFontStyle = System.Drawing.FontStyle;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>PictureSavingPreferencesWindow.xaml</c>.
	/// </summary>
	public partial class PictureSavingPreferencesWindow : Window
	{
		/// <summary>
		/// Indicates whether the text will be drawn on the final bitmap.
		/// </summary>
		private readonly bool _addText;

		/// <summary>
		/// Indicates the settings in main window.
		/// </summary>
		private readonly Settings _settings;

		/// <summary>
		/// The old collection.
		/// </summary>
		private readonly LayerCollection _oldCollection;

		/// <summary>
		/// Indicates the grid.
		/// </summary>
		private readonly Grid _grid;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="settings">The settings.</param>
		/// <param name="layerCollection">The older layer collection.</param>
		public PictureSavingPreferencesWindow(Grid grid, Settings settings, LayerCollection layerCollection, bool addText)
		{
			InitializeComponent();

			(_settings, _grid, _oldCollection, _addText) = (settings, grid, layerCollection, addText);
			_numericUpDownSize.CurrentValue = (decimal)_settings.SavingPictureSize;
		}


		/// <summary>
		/// Get the encoder information.
		/// </summary>
		/// <param name="imageFormat">The image format.</param>
		/// <returns>The info.</returns>
		private ImageCodecInfo? GetEncoderInfo(ImageFormat imageFormat) =>
			ImageCodecInfo.GetImageEncoders().FirstOrDefault(codec => codec.FormatID == imageFormat.Guid);

		/// <summary>
		/// Draw the extra text.
		/// </summary>
		/// <param name="bitmap">(<see langword="ref"/> parameter) The bitmap on which the text is drawn.</param>
		/// <param name="text">The text that should be drawn on the bitmap.</param>
		/// <returns>The result map.</returns>
		[return: NotNullIfNotNull("text")]
		private Bitmap? DrawText(Bitmap bitmap, string? text)
		{
			if (text is null)
			{
				return null;
			}

			const int fontSize = 16;
			const string fontName = "Times New Roman";
			var result = new Bitmap(bitmap.Width, bitmap.Height + (fontSize << 1));
			using var g = Graphics.FromImage(result);
			using var f = new Font(fontName, fontSize, DFontStyle.Bold);
			using var sf = new StringFormat { Alignment = Center, LineAlignment = Center };
			using var b = Brushes.Black;
			var pt = new PointF(bitmap.Width >> 1, bitmap.Height + (fontSize >> 1));
			g.TextRenderingHint = TextRenderingHint.AntiAlias;
			g.SmoothingMode = SmoothingMode.HighQuality;
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.InterpolationMode = InterpolationMode.HighQualityBicubic;
			g.Clear(Color.White);
			g.DrawImage(bitmap, default(PointF));
			g.DrawString(text, f, b, pt, sf);

			return result;
		}


		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			s((float)_numericUpDownSize.CurrentValue);
			Close();

			bool s(float size)
			{
				var saveFileDialog = new SaveFileDialog
				{
					AddExtension = true,
					DefaultExt = "png",
					Filter = (string)LangSource["PictureSavingFilter"],
					Title = (string)LangSource["PictureSavingSaveDialogTitle"]
				};

				if (!(saveFileDialog.ShowDialog() is true))
				{
					return !(e.Handled = true);
				}

				var pc = new PointConverter(size, size);
				var layerCollection = new LayerCollection
				{
					new BackLayer(pc, _settings.BackgroundColor),
					new GridLineLayer(pc, _settings.GridLineWidth, _settings.GridLineColor),
					new BlockLineLayer(pc, _settings.BlockLineWidth, _settings.BlockLineColor),
					new ValueLayer(
						pc, _settings.ValueScale, _settings.CandidateScale,
						_settings.GivenColor, _settings.ModifiableColor, _settings.CandidateColor,
						_settings.GivenFontName, _settings.ModifiableFontName,
						_settings.CandidateFontName, _grid, _settings.ShowCandidates),
				};

				if (_oldCollection[typeof(CustomViewLayer)] is CustomViewLayer customViewLayer)
				{
					layerCollection.Add(new CustomViewLayer(pc, customViewLayer));
				}

				if (_oldCollection[typeof(ViewLayer)] is ViewLayer viewLayer)
				{
					layerCollection.Add(new ViewLayer(pc, viewLayer));
				}

				Bitmap? bitmap = null;
				try
				{
					bitmap = new Bitmap((int)size, (int)size);

					int selectedIndex = saveFileDialog.FilterIndex;
					string fileName = saveFileDialog.FileName;
					if (selectedIndex >= -1 && selectedIndex <= 3)
					{
						// Normal picture formats.
						layerCollection.IntegrateTo(bitmap);

						bitmap = _addText ? DrawText(bitmap, GetFileNameWithoutExtension(saveFileDialog.FileName)) : bitmap;
						bitmap.Save(
							fileName,
							GetEncoderInfo(
								selectedIndex switch
								{
									-1 => Png,
									0 => Png,
									1 => Jpeg,
									2 => Bmp,
									3 => Gif,
									_ => throw Throwings.ImpossibleCase
								}) ?? throw new NullReferenceException("The return value is null."),
							new EncoderParameters(1) { Param = { [0] = new EncoderParameter(Encoder.Quality, 100L) } });
					}
					else
					{
						// Windows metafile format (WMF).
						using var g = Graphics.FromImage(bitmap);
						using var metaFile = new Metafile(fileName, g.GetHdc());
						using var targetGraphics = Graphics.FromImage(metaFile);
						layerCollection.IntegrateTo(targetGraphics);
						targetGraphics.Save();
					}
				}
				catch (Exception ex)
				{
					Messagings.ShowExceptionMessage(ex);
					return false;
				}
				finally
				{
					bitmap?.Dispose();
				}

				_settings.SavingPictureSize = size;
				return true;
			}
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
	}
}
