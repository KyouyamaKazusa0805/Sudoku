using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using PointConverter = Sudoku.Drawing.PointConverter;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
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
		private Grid _grid = ((Grid)Grid.Empty).Clone();

		/// <summary>
		/// The point converter.
		/// </summary>
		private PointConverter _pointConverter = null!;


		/// <include file='../GlobalDocComments.xml' path='comments/defaultConstructor'/>
		public MainWindow()
		{
			InitializeComponent();

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
		protected override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			var (w, h) = (_imageGrid.Width, _imageGrid.Height);
			_pointConverter = new PointConverter((int)w, (int)h);
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

			var bitmap = new Bitmap((int)w, (int)h);
			_layerCollection.IntegrateTo(bitmap);
			_imageGrid.Source = Imaging.CreateBitmapSourceFromHBitmap(
				bitmap.GetHbitmap(), IntPtr.Zero, Int32Rect.Empty,
				BitmapSizeOptions.FromEmptyOptions());
		}


		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMe().Show();
	}
}
