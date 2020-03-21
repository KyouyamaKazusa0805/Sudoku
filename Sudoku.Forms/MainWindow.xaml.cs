using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sudoku.Data.Extensions;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Extensions;
using PointConverter = Sudoku.Drawing.PointConverter;
using SudokuGrid = Sudoku.Data.Grid;

namespace Sudoku.Forms
{
	/// <summary>
	/// Interaction logic for <c>MainWindow.xaml</c>.
	/// </summary>
	public partial class MainWindow : Window
	{
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
			AddShortCut(Key.F4, ModifierKeys.Alt, MenuItemFileQuit_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control, MenuItemFileFix_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control | ModifierKeys.Shift, MenuItemFileUnfix_Click);

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
		/// Bind a shortcut to a method (mounted to an event) to execute.
		/// </summary>
		/// <param name="key">The key.</param>
		/// <param name="modifierKeys">The modifiers.</param>
		/// <param name="executed">The execution.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void AddShortCut(Key key, ModifierKeys modifierKeys, ExecutedRoutedEventHandler executed)
		{
			var routedCommand = new RoutedCommand();
			routedCommand.InputGestures.Add(new KeyGesture(key, modifierKeys));
			CommandBindings.Add(new CommandBinding(routedCommand, executed));
		}

		/// <summary>
		/// Repaint the <see cref="_imageGrid"/> to show the newer grid image.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateImageGrid()
		{
			var bitmap = new Bitmap((int)_imageGrid.Width, (int)_imageGrid.Height);
			_layerCollection.IntegrateTo(bitmap);
			_imageGrid.Source = bitmap.ToImageSource();

			GC.Collect();
		}

		private void OnShortcutRoute(object sender, ExecutedRoutedEventArgs e)
		{

		}


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
					int cell = _pointConverter.GetCellOffset(pt.ToDPointF());
					_grid[cell] = e.Key - Key.D1;

					UpdateImageGrid();
				}
			}
		}
	}
}
