using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization.Formatters.Binary;
using System.Windows;
using System.Windows.Input;
using System.Xml.Serialization;
using Sudoku.Data.Extensions;
using Sudoku.Data.Stepping;
using Sudoku.Drawing;
using Sudoku.Drawing.Extensions;
using Sudoku.Drawing.Layers;
using Sudoku.Forms.Extensions;
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
		/// Internal layer collection.
		/// </summary>
		private readonly LayerCollection _layerCollection = new LayerCollection();

		/// <summary>
		/// Indicates all focused cells.
		/// </summary>
		private readonly IList<int> _focusedCells = new List<int>();


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
		/// Indicates the settings used.
		/// </summary>
		public Settings Settings { get; private set; } = null!;

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

			// Initializes some controls.
			Title = $"{SolutionName} Ver {Version}";

			// Load configurations if worth.
			LoadConfig();

			// Then initialize the layer collection and point converter
			// for later utility.
			_pointConverter = new PointConverter((float)_imageGrid.Width, (float)_imageGrid.Height);
			_layerCollection.Add(new BackLayer(_pointConverter, Settings.BackgroundColor));
			_layerCollection.Add(
				new GridLineLayer(
					_pointConverter, Settings.GridLineWidth, Settings.GridLineColor));
			_layerCollection.Add(
				new BlockLineLayer(
					_pointConverter, Settings.BlockLineWidth, Settings.BlockLineColor));
			_layerCollection.Add(
				new ValueLayer(
					_pointConverter, Settings.ValueScale, Settings.CandidateScale,
					Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
					Settings.GivenFontName, Settings.ModifiableFontName,
					Settings.CandidateFontName, _grid, Settings.ShowCandidates));

			// Update the grid image.
			UpdateImageGrid();
		}

		/// <inheritdoc/>
		protected override void OnClosing(CancelEventArgs e)
		{
			if (Settings.AskWhileQuitting
				&& MessageBox.Show("Are you sure to quit?", "Info", MessageBoxButton.YesNo) == MessageBoxResult.No)
			{
				e.Cancel = true;
				return;
			}

			SaveConfig();

			base.OnClosing(e);
		}

		/// <inheritdoc/>
		protected override void OnKeyDown(KeyEventArgs e)
		{
			base.OnKeyDown(e);

			// Get all cases for being pressed keys.
			if (e.Key.IsDigit())
			{
				// Get the current cell.
				var pt = Mouse.GetPosition(_imageGrid);
				if (IsPointOutOfRange(_imageGrid, pt))
				{
					e.Handled = true;
					return;
				}

				_grid[_pointConverter.GetCellOffset(pt.ToDPointF())] =
					e.Key.IsDigitUpsideAlphabets() ? e.Key - Key.D1 : e.Key - Key.NumPad1;

				UpdateImageGrid();
			}
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
		/// Save configurations if worth.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void LoadConfig()
		{
			Settings = new Settings();
			if (File.Exists("configurations.scfg"))
			{
				var fs = new FileStream("configurations.scfg", FileMode.Open);
				try
				{
					Settings = (Settings)new BinaryFormatter().Deserialize(fs);
				}
				catch
				{
					// Do nothing.
				}
				finally
				{
					fs.Close();
				}
			}
			else
			{
				Settings.CoverBy(Settings.DefaultSetting);
			}
		}

		/// <summary>
		/// Save configurations.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void SaveConfig()
		{
			var fs = new FileStream("configurations.scfg", FileMode.Create);
			try
			{
				var formatter = new BinaryFormatter();
				formatter.Serialize(fs, Settings);
			}
			catch (Exception ex)
			{
				MessageBox.Show(
					$"The configuration file cannot be saved due to exception throws:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
			finally
			{
				fs.Close();
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
					$"Cannot save text to clipboard due to:{Environment.NewLine}{ex.Message}",
					"Warning");
			}
		}

		/// <summary>
		/// To update the control status.
		/// </summary>
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		private void UpdateControls()
		{
			_menuItemOptionsShowCandidates.IsChecked = Settings.ShowCandidates;
			_menuItemModeSeMode.IsChecked = Settings.SeMode;
			_menuItemModeFastSearch.IsChecked = Settings.FastSearch;

			UpdateImageGrid();
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
						_pointConverter, Settings.ValueScale, Settings.CandidateScale,
						Settings.GivenColor, Settings.ModifiableColor, Settings.CandidateColor,
						Settings.GivenFontName, Settings.ModifiableFontName,
						Settings.CandidateFontName,
						_grid = new UndoableGrid(SudokuGrid.Parse(puzzleStr)), Settings.ShowCandidates));

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
