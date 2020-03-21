using System;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Extensions;
using d = System.Drawing;
using w = System.Windows;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;
using System.Collections.Generic;

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
		/// Indicates all focused cells.
		/// </summary>
		private readonly IList<int> _focusedCells = new List<int>();

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
		public MainWindow() => InitializeComponent();


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

			// Define a series of hot keys.
			CustomCommands.InputGestures.Add(new KeyGesture(Key.F4, ModifierKeys.Alt));

			// Show title.
			Title = $"{SolutionName} Ver {Version}";

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
		/// Convert the point to <see cref="d.PointF"/>.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns>The target point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SuppressMessage("Style", "IDE0001:Simplify Names", Justification = "<Pending>")]
		private d::PointF ToDrawingPoint(w::Point point) =>
			new d::PointF((float)point.X, (float)point.Y);

		/// <summary>
		/// Convert the point to <see cref="d.PointF"/>.
		/// </summary>
		/// <param name="point">The point.</param>
		/// <returns>The target point.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		[SuppressMessage("Style", "IDE0001:Simplify Names", Justification = "<Pending>")]
		private w::Point ToWindowPoint(d::PointF point) =>
			new w::Point(point.X, point.Y);


		private void Window_KeyDown(object sender, KeyEventArgs e)
		{
			if (sender == this)
			{
				// Get the current cell.
				var pt = Mouse.GetPosition(_imageGrid);
				var (x, y) = pt;
				if (x < 0 || x > _imageGrid.Width || y < 0 || y > _imageGrid.Height)
				{
					e.Handled = true;
					return;
				}

				// Get all cases for being pressed keys.
				if (e.Key >= Key.D0 && e.Key <= Key.D9)
				{
					int cell = _pointConverter.GetCellOffset(ToDrawingPoint(pt));
					_grid[cell] = e.Key - Key.D1;

					// Re-paint the control now.
					var bitmap = new Bitmap((int)_imageGrid.Width, (int)_imageGrid.Height);
					_layerCollection.IntegrateTo(bitmap);
					_imageGrid.Source = bitmap.ToImageSource();
				}
			}
		}
	}
}
