#pragma warning disable IDE0051

using System;
using System.Collections.Generic;
using System.Extensions;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using System.Xml.Serialization;
using Sudoku.DocComments;
using Sudoku.Windows.Extensions;
using WColor = System.Windows.Media.Color;
using WColorEx = Sudoku.Windows.Extensions.WindowsColorEx;
using WPoint = System.Windows.Point;

namespace Sudoku.Windows.Tooling
{
	/// <summary>
	/// Interaction logic for <c>ColorPickerControl.xaml</c>.
	/// </summary>
	public sealed partial class ColorPickerControl : UserControl
	{
		/// <summary>
		/// The number of first-line swatches.
		/// </summary>
		private const int NumColorsFirstSwatch = 39;

		/// <summary>
		/// The number of second-line swatches.
		/// </summary>
		private const int NumColorsSecondSwatch = 112;


		/// <summary>
		/// The color palette.
		/// </summary>
		private static ColorPalette? _colorPalette;


		/// <summary>
		/// The color swatch 1.
		/// </summary>
		private readonly ICollection<ColorSwatchItem> _colorSwatch1 = new List<ColorSwatchItem>();

		/// <summary>
		/// The color swatch 2.
		/// </summary>
		private readonly ICollection<ColorSwatchItem> _colorSwatch2 = new List<ColorSwatchItem>();

		/// <summary>
		/// Indicates whether the program is modifying the value now.
		/// </summary>
		private bool _isSettingValues = false;


		/// <inheritdoc cref="DefaultConstructor"/>
		public ColorPickerControl() => InitializeComponent();


		/// <summary>
		/// Indicates the selected color.
		/// </summary>
		public WColor? Color { get; set; }


		/// <summary>
		/// The event triggering while picking colors.
		/// </summary>
		public event PickingColorEventHandler? PickingColor;


		public void SaveCustomPalette(string filename)
		{
			_ = _colorPalette is null ? throw new Exception("Color palette is current null.") : 0;

			_colorPalette.CustomColors = _customColorSwatch.GetColors();
			try
			{
				File.WriteAllText(filename, GetXmlText(_colorPalette));
			}
			catch
			{
			}
		}

		public void LoadDefaultCustomPalette() =>
			LoadCustomPalette(
				Path.Combine(
					ColorPickerSettings.CustomColorsDirectory,
					ColorPickerSettings.CustomColorsFilename));

		public void LoadCustomPalette(string filename)
		{
			_ = _colorPalette is null ? throw new Exception("Color palette is current null.") : 0;

			if (File.Exists(filename))
			{
				try
				{
					_colorPalette = LoadFromXml<ColorPalette>(filename);

					_customColorSwatch.SwatchListBox.ItemsSource = _colorPalette!.CustomColors;

					_colorSwatch1.Clear();
					_colorSwatch2.Clear();
					_colorSwatch1.AddRange(_colorPalette.BuiltInColors.Take(NumColorsFirstSwatch));
					_colorSwatch2.AddRange(
						_colorPalette.BuiltInColors.Skip(NumColorsFirstSwatch).Take(NumColorsSecondSwatch));
					_swatch1.SwatchListBox.ItemsSource = _colorSwatch1;
					_swatch2.SwatchListBox.ItemsSource = _colorSwatch2;
				}
				catch
				{
				}
			}
		}

		/// <summary>
		/// Execute when the custom color is changed.
		/// </summary>
		internal void CustomColorsChanged()
		{
			if (ColorPickerSettings.UsingCustomPalette)
			{
				SaveCustomPalette(ColorPickerSettings.CustomPaletteFilename);
			}
		}

		/// <inheritdoc/>
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			Swatch.ColorPickerControl = this;

			// Load from file if possible.
			if (ColorPickerSettings.UsingCustomPalette && File.Exists(ColorPickerSettings.CustomPaletteFilename))
			{
				try
				{
					_colorPalette = LoadFromXml<ColorPalette>(ColorPickerSettings.CustomPaletteFilename);
				}
				catch
				{
				}
			}

			if (_colorPalette is null)
			{
				(_colorPalette = new()).InitializeDefaults();
			}

			_colorSwatch1.AddRange(_colorPalette.BuiltInColors.Take(NumColorsFirstSwatch));
			_colorSwatch2.AddRange(
				_colorPalette.BuiltInColors.Skip(NumColorsFirstSwatch).Take(NumColorsSecondSwatch));

			_swatch1.SwatchListBox.ItemsSource = _colorSwatch1;
			_swatch2.SwatchListBox.ItemsSource = _colorSwatch2;

			if (ColorPickerSettings.UsingCustomPalette)
			{
				_customColorSwatch.SwatchListBox.ItemsSource = _colorPalette.CustomColors;
			}
			else
			{
				_customColorsLabel.Visibility = Visibility.Collapsed;
				_customColorSwatch.Visibility = Visibility.Collapsed;
			}

			_rSlider._slider.Maximum = 255;
			_gSlider._slider.Maximum = 255;
			_bSlider._slider.Maximum = 255;
			_aSlider._slider.Maximum = 255;
			_hSlider._slider.Maximum = 360;
			_sSlider._slider.Maximum = 1;
			_lSlider._slider.Maximum = 1;

			_rSlider._label.Content = "R";
			_rSlider._slider.TickFrequency = 1;
			_rSlider._slider.IsSnapToTickEnabled = true;
			_gSlider._label.Content = "G";
			_gSlider._slider.TickFrequency = 1;
			_gSlider._slider.IsSnapToTickEnabled = true;
			_bSlider._label.Content = "B";
			_bSlider._slider.TickFrequency = 1;
			_bSlider._slider.IsSnapToTickEnabled = true;

			_aSlider._label.Content = "A";
			_aSlider._slider.TickFrequency = 1;
			_aSlider._slider.IsSnapToTickEnabled = true;

			_hSlider._label.Content = "H";
			_hSlider._slider.TickFrequency = 1;
			_hSlider._slider.IsSnapToTickEnabled = true;
			_sSlider._label.Content = "S";
			_lSlider._label.Content = "V";

			SetColor(Color);
		}

		/// <summary>
		/// Set the current color.
		/// </summary>
		/// <param name="color">The color.</param>
		private void SetColor(WColor? color)
		{
			color ??= default;

			var z = color.Value;
			Color = z;

			_customColorSwatch.CurrentColor = z;

			_isSettingValues = true;

			_rSlider._slider.Value = z.R;
			_gSlider._slider.Value = z.G;
			_bSlider._slider.Value = z.B;
			_aSlider._slider.Value = z.A;
			_sSlider._slider.Value = z.GetSaturation();
			_lSlider._slider.Value = z.GetBrightness();
			_hSlider._slider.Value = z.GetHue();

			_colorDisplayBorder.Background = new SolidColorBrush(z);

			_isSettingValues = false;
			PickingColor?.Invoke(z);
		}

		/// <summary>
		/// Execute when the sample image is clicked.
		/// </summary>
		/// <param name="img">The image.</param>
		/// <param name="pos">The point.</param>
		private void SampleImageClick(BitmapSource img, WPoint pos)
		{
			int stride = (int)img.Width * 4;
			byte[] pixels = new byte[((int)img.Height * stride)];

			img.CopyPixels(pixels, stride, 0);

			int index = (int)pos.Y * stride + 4 * (int)pos.X;
			SetColor(WColor.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]));
		}

		/// <inheritdoc cref="Events.MouseDown(object?, EventArgs)"/>
		private void SampleImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Mouse.Capture(this);

			MouseMove += ColorPickerControl_MouseMove;
			MouseUp += ColorPickerControl_MouseUp;
		}

		/// <inheritdoc cref="Events.MouseMove(object?, EventArgs)"/>
		private void ColorPickerControl_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(_sampleImage);
			var img = (BitmapSource)_sampleImage.Source;

			var (x, y) = pos;
			if (x > 0 && y > 0 && x < img.PixelWidth && y < img.PixelHeight)
			{
				SampleImageClick(img, pos);
			}
		}

		/// <inheritdoc cref="Events.MouseUp(object?, EventArgs)"/>
		private void ColorPickerControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Mouse.Capture(null);

			MouseMove -= ColorPickerControl_MouseMove;
			MouseUp -= ColorPickerControl_MouseUp;
		}

		/// <inheritdoc cref="Events.MouseDown(object?, EventArgs)"/>
		private void SampleImage2_MouseDown(object sender, MouseButtonEventArgs e) =>
			SampleImageClick((BitmapSource)_sampleImage2.Source, e.GetPosition(_sampleImage2));

		/// <inheritdoc cref="PickingColorEventHandler"/>
		private void Swatch_PickColor(WColor color) => SetColor(color);

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void RSlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var (a, _, g, b) = Color.GetValueOrDefault();
				SetColor(WColor.FromArgb(a, (byte)value, g, b));
			}
		}

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void GSlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var (a, r, _, b) = Color.GetValueOrDefault();
				SetColor(WColor.FromArgb(a, r, (byte)value, b));
			}
		}

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void BSlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var (a, r, g, _) = Color.GetValueOrDefault();
				SetColor(WColor.FromArgb(a, r, g, (byte)value));
			}
		}

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void ASlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var (_, r, g, b) = Color.GetValueOrDefault();
				SetColor(WColor.FromArgb((byte)value, r, g, b));
			}
		}

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void HSlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var z = Color.GetValueOrDefault();
				SetColor(
					WColorEx.FromAhsb(
						(int)_aSlider._slider.Value, (float)value, z.GetSaturation(), z.GetBrightness()));
			}
		}

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void SSlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var z = Color.GetValueOrDefault();
				Color = WColorEx.FromAhsb(
					(int)_aSlider._slider.Value, z.GetHue(), (float)value, z.GetBrightness());

				SetColor(Color);
			}
		}

		/// <inheritdoc cref="ValueChangedEventHandler"/>
		private void LSlider_ValueChanged(double value)
		{
			if (!_isSettingValues)
			{
				var z = Color.GetValueOrDefault();
				Color = WColorEx.FromAhsb(
					(int)_aSlider._slider.Value, z.GetHue(), z.GetSaturation(), (float)value);

				SetColor(Color);
			}
		}

		/// <inheritdoc cref="Events.ValueChanged(object?, EventArgs)"/>
		private void PickerHueSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
			UpdateImageForHSV();

		/// <summary>
		/// Update the image for HSV.
		/// </summary>
		private void UpdateImageForHSV()
		{
			var img =
				new BitmapImage(
					new(
						"pack://application:,,,/Sudoku.Windows;component/Resources/ColorSample.png",
						UriKind.RelativeOrAbsolute));
			float sliderHue = (float)_pickerHueSlider.Value;
			if (sliderHue is <= 0 or >= 360F)
			{
				// No hue change just return.
				_sampleImage2.Source = img;
				return;
			}

			var writableImage = BitmapFactory.ConvertToPbgra32Format(img);
			using var context = writableImage.GetBitmapContext();
			for (int x = 0; x < img.PixelWidth; x++)
			{
				for (int y = 0; y < img.PixelHeight; y++)
				{
					var pixel = writableImage.GetPixel(x, y);
					float newHue = sliderHue + pixel.GetHue();
					newHue = newHue >= 360 ? newHue - 360 : newHue;
					writableImage.SetPixel(
						x, y, WColorEx.FromAhsb(255, newHue, pixel.GetSaturation(), pixel.GetBrightness()));
				}
			}

			_sampleImage2.Source = writableImage;
		}


		/// <summary>
		/// Deserialize the file.
		/// </summary>
		/// <typeparam name="T">The type of the instance.</typeparam>
		/// <param name="this">(<see langword="this"/> parameter) The instance.</param>
		/// <param name="filename">The file name.</param>
		/// <returns>The instance.</returns>
		private static T? LoadFromXml<T>(string filename)
		{
			if (File.Exists(filename))
			{
				using var sr = new StreamReader(filename);
				using var xr = new XmlTextReader(sr);
				return new XmlSerializer(typeof(T)).Deserialize(xr) is T r ? r : default;
			}
			else
			{
				return default;
			}
		}

		/// <summary>
		/// Get the text from the specified text.
		/// </summary>
		/// <typeparam name="T">The type of the element.</typeparam>
		/// <param name="this">The instance.</param>
		/// <returns>The string.</returns>
		private static string GetXmlText<T>(T @this)
		{
			using var sw = new StringWriter();
			using var xmlWriter = XmlWriter.Create(
				sw,
				new()
				{
					Indent = true,
					IndentChars = "    ",
					NewLineOnAttributes = false
				});

			new XmlSerializer(typeof(T)).Serialize(xmlWriter, @this);
			return sw.ToString();
		}
	}
}
