using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Windows;
using Microsoft.Win32;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Windows
{
	/// <summary>
	/// Interaction logic for <c>PictureSavingPreferencesWindow.xaml</c>.
	/// </summary>
	public partial class PictureSavingPreferencesWindow : Window
	{
		/// <summary>
		/// Indicates the settings in main window.
		/// </summary>
		private readonly Settings _settings;

		/// <summary>
		/// Indicates the grid.
		/// </summary>
		private readonly Grid _grid;


		/// <summary>
		/// Initializes an instance with the specified size.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="settings">The settings.</param>
		public PictureSavingPreferencesWindow(Grid grid, Settings settings)
		{
			InitializeComponent();

			(_settings, _grid) = (settings, grid);
		}

		private void ButtonSave_Click(object sender, RoutedEventArgs e)
		{
			if (checkValidity(out float size) && savePicture(size))
			{
				Close();
			}

			bool checkValidity(out float size)
			{
				if (!float.TryParse(_textBoxSize.Text, out size))
				{
					MessageBox.Show("Please check your input. The input is invalid.", "Info");

					size = default;
					return !(e.Handled = true);
				}

				return true;
			}

			bool savePicture(float size)
			{
				var saveFileDialog = new SaveFileDialog
				{
					AddExtension = true,
					DefaultExt = "png",
					Filter = "PNG files|*.png|JPG files|*.jpg|BMP files|*.bmp|GIF files|*.gif",
					Title = "Save picture..."
				};

				if (!(saveFileDialog.ShowDialog() is true))
				{
					return !(e.Handled = true);
				}

				var pc = new PointConverter(size, size);
				var layerCollection = new LayerCollection
				{
					new BackLayer(pc, _settings.BackgroundColor),
					new GridLineLayer(
						pc, _settings.GridLineWidth, _settings.GridLineColor),
					new BlockLineLayer(
						pc, _settings.BlockLineWidth, _settings.BlockLineColor),
					new ValueLayer(
						pc, _settings.ValueScale, _settings.CandidateScale,
						_settings.GivenColor, _settings.ModifiableColor, _settings.CandidateColor,
						_settings.GivenFontName, _settings.ModifiableFontName,
						_settings.CandidateFontName, _grid, _settings.ShowCandidates),
				};

				Bitmap? bitmap = null;
				try
				{
					bitmap = new Bitmap((int)size, (int)size);
					layerCollection.IntegrateTo(bitmap);
					bitmap.Save(saveFileDialog.FileName, saveFileDialog.FilterIndex switch
					{
						-1 => ImageFormat.Png,
						0 => ImageFormat.Png,
						1 => ImageFormat.Jpeg,
						2 => ImageFormat.Bmp,
						3 => ImageFormat.Gif,
						_ => throw Throwing.ImpossibleCase
					});
				}
				catch (Exception ex)
				{
					MessageBox.Show($"Failed to save the file because: {ex.Message}.", "Info");

					return false;
				}
				finally
				{
					bitmap?.Dispose();
				}

				return true;
			}
		}

		private void ButtonCancel_Click(object sender, RoutedEventArgs e) => Close();
	}
}
