using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using Sudoku.Data;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Extensions;
using ControlBase = System.Windows.FrameworkElement;
using d = System.Drawing;
using PointConverter = Sudoku.Drawing.PointConverter;
using w = System.Windows;

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

			static void canExecuteRoute(object sender, CanExecuteRoutedEventArgs e) =>
				e.CanExecute = true;
			CommandBindings.Add(
				new CommandBinding(
					CustomCommands.QuitCommand,
					MenuItemFileQuit_Click,
					canExecuteRoute));

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

		/// <summary>
		/// Get the snapshot of a control in WPF.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <returns>The snapshot.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private Bitmap GetSnapshotOf(ControlBase control)
		{
			var (w, h) = ((int)control.Width, (int)control.Height);
			var (x, y) = control.TranslatePoint(new w::Point(0, 0), this);

			var shot = new Bitmap(w, h);
			using var g = Graphics.FromImage(shot);
			g.CompositingQuality = CompositingQuality.HighQuality;
			g.CopyFromScreen(
				(int)x, (int)y, 0, 0, new d::Size(w, h), CopyPixelOperation.SourceCopy);

			return shot;
		}

		/// <summary>
		/// Get the point relative to the whole form <see cref="MainWindow"/>.
		/// </summary>
		/// <returns>The point relative to <see cref="MainWindow"/>.</returns>
		/// <seealso cref="MainWindow"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private w::Point GetMousePoint() => Mouse.GetPosition(this);

		/// <summary>
		/// Get the point relative to the specified control.
		/// </summary>
		/// <returns>The point relative to specified control.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private w::Point GetMousePoint(ControlBase control) => Mouse.GetPosition(control);


		private void MenuItemFileGetSnapshot_Click(object sender, RoutedEventArgs e) =>
			Clipboard.SetImage((BitmapSource)GetSnapshotOf(_imageGrid).ToImageSource());

		private void MenuItemFileQuit_Click(object sender, RoutedEventArgs e) =>
			Close();

		private void MenuItemAboutMe_Click(object sender, RoutedEventArgs e) =>
			new AboutMe().Show();
	}
}
