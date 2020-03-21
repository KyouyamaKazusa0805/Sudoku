using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Extensions;
using ControlBase = System.Windows.FrameworkElement;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;
using w = System.Windows;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Indicates the custom commands list, which is used for binding onto the
		/// <see cref="MenuItem"/>s.
		/// </summary>
		/// <seealso cref="MenuItem"/>
		public static RoutedCommand CustomCommands = new RoutedCommand();


		/// <summary>
		/// Internal layer collection.
		/// </summary>
		private readonly LayerCollection _layerCollection = new LayerCollection();

		/// <summary>
		/// Internal settings.
		/// </summary>
		private readonly Settings _settings = Settings.DefaultSetting.Clone();


		/// <summary>
		/// The grid.
		/// </summary>
		[SuppressMessage("Style", "IDE0044:Add readonly modifier", Justification = "<Pending>")]
		private SudokuGrid _grid = ((SudokuGrid)SudokuGrid.Empty).Clone();

		/// <summary>
		/// The point converter.
		/// </summary>
		private PointConverter _pointConverter = null!;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public MainWindow()
		{
			InitializeComponent();

			// Define a series of hot keys.
			CustomCommands.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));

			Title = $"{SolutionName} Ver {Version}";
		}


		/// <summary>
		/// Indicates the name of this solution.
		/// </summary>
		private string SolutionName
		{
			get
			{
				return Assembly
					.GetExecutingAssembly()
					.GetCustomAttribute<AssemblyProductAttribute>()
					is AssemblyProductAttribute attr
					? attr.Product
					: "Sunnie's Sudoku Solution";
			}
		}

		/// <summary>
		/// Indicates the version.
		/// </summary>
		private string Version
		{
			get
			{
				return Assembly
					.GetExecutingAssembly()
					.GetName()
					.Version
					.NullableToString("Unknown version");
			}
		}


		/// <inheritdoc/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var (w, h) = ((int)_imageGrid.Width, (int)_imageGrid.Height);
			_pointConverter = new PointConverter(w, h);
			_layerCollection.Add(new BackLayer(_pointConverter, _settings.BackgroundColor));
			_layerCollection.Add(
				new GridLineLayer(
					_pointConverter, _settings.GridLineWidth, _settings.GridLineColor));
			_layerCollection.Add(
				new BlockLineLayer(
					_pointConverter, _settings.BlockLineWidth, _settings.BlockLineColor));
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, _settings.ValueScale, _settings.CandidateScale,
					_settings.GivenColor, _settings.ModifiableColor, _settings.CandidateColor,
					_settings.GivenFontName, _settings.ModifiableFontName,
					_settings.CandidateFontName, _grid));

			var bitmap = new Bitmap(w, h);
			_layerCollection.IntegrateTo(bitmap);
			_imageGrid.Source = bitmap.ToImageSource();
		}


		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e)
		{
			try
			{
				Clipboard.SetImage((BitmapSource)_imageGrid.Source);
			}
			catch (Exception ex)
			{
				MessageBox.Show($"Save failed due to:{Environment.NewLine}{ex.Message}.", "Warning");
			}
		}

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMe().Show();

		private void MenuItemFileQuit_Executed(object sender, ExecutedRoutedEventArgs e) =>
			Close();

		private void ImageGrid_MouseMove(object sender, MouseEventArgs e)
		{
			if (sender is w::Controls.Image imageControl)
			{
				var (x, y) = e.GetPosition(imageControl);
				_textBoxInfo.Text = $"{(int)x}, {(int)y}";
			}
		}

		private void ImageGrid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
		{

		}
	}
}
