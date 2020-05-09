using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Sudoku.Drawing.Extensions;
using WColor = System.Windows.Media.Color;

namespace Sudoku.Windows.Tooling
{
	public delegate void ColorPickerChangedEventHandler(WColor color);

	/// <summary>
	/// Interaction logic for <c>ColorPickerControl.xaml</c>.
	/// </summary>
	public partial class ColorPickerControl : UserControl
	{
		/// <summary>
		/// The color swatch 1.
		/// </summary>
		internal List<ColorSwatchItem> _colorSwatch1 = new List<ColorSwatchItem>();

		/// <summary>
		/// The color swatch 2.
		/// </summary>
		internal List<ColorSwatchItem> _colorSwatch2 = new List<ColorSwatchItem>();

		protected const int NumColorsFirstSwatch = 39;

		protected const int NumColorsSecondSwatch = 112;


		/// <include file='../../../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public ColorPickerControl()
		{
			InitializeComponent();

			Swatch.ColorPickerControl = this;

			// Load from file if possible.
			if (ColorPickerSettings.UsingCustomPalette && File.Exists(ColorPickerSettings.CustomPaletteFilename))
			{
				try
				{
					ColorPalette = ColorPalette.LoadFromXml(ColorPickerSettings.CustomPaletteFilename);
				}
				catch
				{
				}
			}

			if (ColorPalette is null)
			{
				ColorPalette = new ColorPalette();
				ColorPalette.InitializeDefaults();
			}

			_colorSwatch1.AddRange(ColorPalette.BuiltInColors.Take(NumColorsFirstSwatch));
			_colorSwatch2.AddRange(ColorPalette.BuiltInColors.Skip(NumColorsFirstSwatch).Take(NumColorsSecondSwatch));

			Swatch1.SwatchListBox.ItemsSource = _colorSwatch1;
			Swatch2.SwatchListBox.ItemsSource = _colorSwatch2;

			if (ColorPickerSettings.UsingCustomPalette)
			{
				_customColorSwatch.SwatchListBox.ItemsSource = ColorPalette.CustomColors;
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

		public bool IsSettingValues { get; set; } = false;

		/// <summary>
		/// Indicates the selected color.
		/// </summary>
		public WColor? Color { get; set; }


		public event ColorPickerChangedEventHandler? PickingColor;



		internal static ColorPalette? ColorPalette;


		public void SetColor(WColor? color)
		{
			if (color is null)
			{
				return;
			}

			var z = color.Value;
			Color = z;

			_customColorSwatch.CurrentColor = z;

			IsSettingValues = true;

			_rSlider._slider.Value = z.R;
			_gSlider._slider.Value = z.G;
			_bSlider._slider.Value = z.B;
			_aSlider._slider.Value = z.A;

			_sSlider._slider.Value = z.GetSaturation();
			_lSlider._slider.Value = z.GetBrightness();
			_hSlider._slider.Value = z.GetHue();

			ColorDisplayBorder.Background = new SolidColorBrush(z);

			IsSettingValues = false;
			PickingColor?.Invoke(z);
		}

		internal void CustomColorsChanged()
		{
			if (ColorPickerSettings.UsingCustomPalette)
			{
				SaveCustomPalette(ColorPickerSettings.CustomPaletteFilename);
			}
		}

		protected void SampleImageClick(BitmapSource img, Point pos)
		{
			if (Color is null)
			{
				return;
			}

			int stride = (int)img.Width * 4;
			byte[] pixels = new byte[((int)img.Height * stride)];

			img.CopyPixels(pixels, stride, 0);

			int index = (int)pos.Y * stride + 4 * (int)pos.X;
			SetColor(WColor.FromArgb(pixels[index + 3], pixels[index + 2], pixels[index + 1], pixels[index]));
		}

		private void SampleImage_MouseDown(object sender, MouseButtonEventArgs e)
		{
			Mouse.Capture(this);

			MouseMove += ColorPickerControl_MouseMove;
			MouseUp += ColorPickerControl_MouseUp;
		}

		private void ColorPickerControl_MouseMove(object sender, MouseEventArgs e)
		{
			var pos = e.GetPosition(_sampleImage);
			var img = (BitmapSource)_sampleImage.Source;

			if (pos.X > 0 && pos.Y > 0 && pos.X < img.PixelWidth && pos.Y < img.PixelHeight)
			{
				SampleImageClick(img, pos);
			}
		}

		private void ColorPickerControl_MouseUp(object sender, MouseButtonEventArgs e)
		{
			Mouse.Capture(null);
			MouseMove -= ColorPickerControl_MouseMove;
			MouseUp -= ColorPickerControl_MouseUp;
		}

		private void SampleImage2_OnMouseDown(object sender, MouseButtonEventArgs e)
		{
			var pos = e.GetPosition(_sampleImage2);
			var img = (BitmapSource)_sampleImage2.Source;
			SampleImageClick(img, pos);
		}

		private void Swatch_PickColor(WColor color) => SetColor(color);

		private void RSlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}
			
			if (!IsSettingValues)
			{
				var (a, _, g, b) = Color.Value;
				SetColor(Color = WColor.FromArgb(a, (byte)value, g, b));
			}
		}

		private void GSlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}

			if (!IsSettingValues)
			{
				var (a, r, _, b) = Color.Value;
				SetColor(Color = WColor.FromArgb(a, r, (byte)value, b));
			}
		}

		private void BSlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}

			if (!IsSettingValues)
			{
				var (a, r, g, _) = Color.Value;
				SetColor(Color = WColor.FromArgb(a, r, g, (byte)value));
			}
		}

		private void ASlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}

			if (!IsSettingValues)
			{
				var (_, r, g, b) = Color.Value;
				SetColor(WColor.FromArgb((byte)value, r, g, b));
			}
		}

		private void HSlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}
			
			if (!IsSettingValues)
			{
				var z = Color.Value;
				SetColor(
					Util.FromAhsb((int)_aSlider._slider.Value, (float)value, z.GetSaturation(), z.GetBrightness()));
			}
		}

		private void SSlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}

			if (!IsSettingValues)
			{
				var z = Color.Value;
				SetColor(Color = Util.FromAhsb((int)_aSlider._slider.Value, z.GetHue(), (float)value, z.GetBrightness()));
			}
		}

		private void LSlider_ValueChanged(double value)
		{
			if (Color is null)
			{
				return;
			}

			if (!IsSettingValues)
			{
				var z = Color.Value;
				SetColor(
					Color = Util.FromAhsb((int)_aSlider._slider.Value, z.GetHue(), z.GetSaturation(), (float)value));
			}
		}


		private void PickerHueSlider_OnValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e) =>
			UpdateImageForHSV();

		private void UpdateImageForHSV()
		{
			//var hueChange = (int)((PickerHueSlider.Value / 360D) * 240);
			float sliderHue = (float)_pickerHueSlider.Value;

			//var colorPickerImage = Path.Combine(Environment.CurrentDirectory, @"/Resources/ColorPalette.png");
			var img =
				new BitmapImage(
					new Uri(
						"pack://application:,,,/Sudoku.Windows;component/Resources/ColorSample.png",
						UriKind.RelativeOrAbsolute));
			if (sliderHue <= 0 || sliderHue >= 360F)
			{
				// No hue change just return
				_sampleImage2.Source = img;
				return;
			}

			var writableImage = BitmapFactory.ConvertToPbgra32Format(img);
			using (var context = writableImage.GetBitmapContext())
			{
				long numPixels = img.PixelWidth * img.PixelHeight;

				for (int x = 0; x < img.PixelWidth; x++)
				{
					for (int y = 0; y < img.PixelHeight; y++)
					{
						var pixel = writableImage.GetPixel(x, y);

						float newHue = sliderHue + pixel.GetHue();
						if (newHue >= 360)
						{
							newHue -= 360;
						}

						var color = Util.FromAhsb(255, newHue, pixel.GetSaturation(), pixel.GetBrightness());

						writableImage.SetPixel(x, y, color);
					}
				}
			}

			_sampleImage2.Source = writableImage;
		}

		public void SaveCustomPalette(string filename)
		{
			if (ColorPalette is null)
			{
				throw new Exception("Color palette is current null.");
			}

			var colors = _customColorSwatch.GetColors();
			ColorPalette.CustomColors = colors;

			try
			{
				ColorPalette.SaveToXml(filename);
			}
			catch
			{
			}
		}

		public void LoadCustomPalette(string filename)
		{
			if (ColorPalette is null)
			{
				throw new Exception("Color palette is current null.");
			}

			if (File.Exists(filename))
			{
				try
				{
					ColorPalette = ColorPalette.LoadFromXml(filename);

					_customColorSwatch.SwatchListBox.ItemsSource = ColorPalette!.CustomColors;

					_colorSwatch1.Clear();
					_colorSwatch2.Clear();
					_colorSwatch1.AddRange(ColorPalette.BuiltInColors.Take(NumColorsFirstSwatch));
					_colorSwatch2.AddRange(
						ColorPalette.BuiltInColors.Skip(NumColorsFirstSwatch).Take(NumColorsSecondSwatch));
					Swatch1.SwatchListBox.ItemsSource = _colorSwatch1;
					Swatch2.SwatchListBox.ItemsSource = _colorSwatch2;
				}
				catch
				{
				}
			}
		}

		public void LoadDefaultCustomPalette()
		{
			LoadCustomPalette(
				Path.Combine(ColorPickerSettings.CustomColorsDirectory, ColorPickerSettings.CustomColorsFilename));
		}
	}
}
