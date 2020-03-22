using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using Sudoku.Data.Extensions;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
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
		private UndoableGrid _grid = new UndoableGrid(SudokuGrid.Empty.Clone());

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
		protected override void OnInitialized(EventArgs e)
		{
			// Call the base method.
			base.OnInitialized(e);

			// Define shortcuts.
			AddShortCut(Key.O, ModifierKeys.Control, MenuItemFileOpen_Click);
			AddShortCut(Key.S, ModifierKeys.Control, MenuItemFileSave_Click);
			AddShortCut(Key.F4, ModifierKeys.Alt, MenuItemFileQuit_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control, MenuItemEditFix_Click);
			AddShortCut(Key.OemTilde, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditUnfix_Click);
			AddShortCut(Key.Z, ModifierKeys.Control, MenuItemEditUndo_Click);
			AddShortCut(Key.Y, ModifierKeys.Control, MenuItemEditRedo_Click);
			AddShortCut(Key.C, ModifierKeys.Control, MenuItemEditCopy_Click);
			AddShortCut(Key.C, ModifierKeys.Control | ModifierKeys.Shift, MenuItemEditCopyCurrentGrid_Click);
			AddShortCut(Key.V, ModifierKeys.Control, MenuItemEditPaste_Click);

			// Show title.
			Title = $"{SolutionName} Ver {Version}";

			// Then initialize the layer collection and point converter
			// for later utility.
			_pointConverter = new PointConverter((float)_imageGrid.Width, (float)_imageGrid.Height);
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

			// Update the grid image.
			UpdateImageGrid();
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (MessageBox.Show("Are you sure to quit?", "Info", MessageBoxButton.YesNo) == MessageBoxResult.No)
			{
				e.Cancel = true;
				return;
			}

			base.OnClosing(e);
		}

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// Get all cases for being pressed keys.
			if (e.Key >= Key.D0 && e.Key <= Key.D9)
			{
				// Get the current cell.
				var pt = Mouse.GetPosition(_imageGrid);
				if (IsPointOutOfRange(_imageGrid, pt))
				{
					e.Handled = true;
					return;
				}

				int cell = _pointConverter.GetCellOffset(pt.ToDPointF());
				_grid[cell] = e.Key - Key.D1;

				UpdateImageGrid();
			}
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

		/// <summary>
		/// The internal copy method to process the operation of copying value to clipboard.
		/// </summary>
		/// <param name="format">
		/// The grid format. If the value is <see langword="null"/>, it will call the method
		/// <see cref="SudokuGrid.ToString(string?, IFormatProvider?)"/>; otherwise,
		/// <see cref="SudokuGrid.ToString(string)"/>.
		/// </param>
		/// <seealso cref="SudokuGrid.ToString(string)"/>
		/// <seealso cref="SudokuGrid.ToString(string?, IFormatProvider?)"/>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void InternalCopy(string? format)
		{
			try
			{
				if (format is null)
				{
					Clipboard.SetText(_grid.ToString(null, null));
				}
				else
				{
					Clipboard.SetText(_grid.ToString(format));
				}
			}
			catch (ArgumentNullException ex)
			{
				MessageBox.Show(
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}", "Warning");
			}
		}

		/// <summary>
		/// To load a puzzle with a specified possible puzzle string.
		/// </summary>
		/// <param name="puzzleStr">The puzzle string.</param>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadPuzzle(string puzzleStr)
		{
			try
			{
				_layerCollection.Add(
					new ValueLayer(
						_pointConverter, _settings.ValueScale, _settings.CandidateScale,
						_settings.GivenColor, _settings.ModifiableColor, _settings.CandidateColor,
						_settings.GivenFontName, _settings.ModifiableFontName,
						_settings.CandidateFontName,
						_grid = new UndoableGrid(SudokuGrid.Parse(puzzleStr))));

				_menuItemUndo.IsEnabled = _menuItemRedo.IsEnabled = false;
				UpdateImageGrid();
			}
			catch (ArgumentException)
			{
				MessageBox.Show("The specified puzzle is invalid.", "Warning");
			}
		}

		/// <summary>
		/// To check whether the specified point is out of range of a control.
		/// </summary>
		/// <param name="control">The control.</param>
		/// <param name="point">The point.</param>
		/// <returns>A <see cref="bool"/> value.</returns>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private bool IsPointOutOfRange(FrameworkElement control, w::Point point)
		{
			var (x, y) = point;
			return x < 0 || x > control.Width || y < 0 || y > control.Height;
		}
	}
}
