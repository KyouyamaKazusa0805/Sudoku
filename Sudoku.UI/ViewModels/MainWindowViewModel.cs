using System.Collections.Generic;
using System.Drawing;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.UI.Extensions;

namespace Sudoku.UI.ViewModels
{
	/// <summary>
	/// Indicates the view model bound by <see cref="MainWindow"/>.
	/// </summary>
	/// <seealso cref="MainWindow"/>
	public sealed partial class MainWindowViewModel
	{
		/// <summary>
		/// Gets or sets the focused cells.
		/// </summary>
		/// <value>The focused cells.</value>
		public Cells FocusedCells { get => PanelViewModel.FocusedCells; set => PanelViewModel.FocusedCells = value; }

		/// <summary>
		/// Gets or sets the grid.
		/// </summary>
		/// <value>The grid.</value>
		public SudokuGrid Grid { get => PanelViewModel.Grid; set => PanelViewModel.Grid = value; }

		/// <summary>
		/// Gets or sets the converter.
		/// </summary>
		/// <value>The converter.</value>
		public DrawingPointConverter Converter { get => PanelViewModel.Converter; set => PanelViewModel.Generator = new(value, Preferences); }

		/// <summary>
		/// Gets or sets the preferences.
		/// </summary>
		/// <value>The preferences.</value>
		public Settings Preferences { get => PanelViewModel.Preferences; set => PanelViewModel.Generator = new(Converter, value); }

		/// <summary>
		/// Gets or sets the view.
		/// </summary>
		/// <value>The view.</value>
		public PresentationData? View { get => PanelViewModel.View; set => PanelViewModel.View = value; }

		/// <summary>
		/// Gets or sets the custom view.
		/// </summary>
		/// <value>The custom view.</value>
		public PresentationData? CustomView { get => PanelViewModel.CustomView; set => PanelViewModel.CustomView = value; }

		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		public GridImageGenerator Generator { get => PanelViewModel.Generator; set => PanelViewModel.Generator = value; }

		/// <summary>
		/// Indicates the bitmap image to set.
		/// </summary>
		public Bitmap Image { set => SudokuPanelMain.GridDisplayer.Source = value.ToImageSource(); }

		/// <summary>
		/// Indicates the sudoku panel view model.
		/// </summary>
		public SudokuPanelViewModel PanelViewModel => SudokuPanelMain.ViewModel;

		/// <summary>
		/// Gets or sets the conclusions.
		/// </summary>
		/// <value>The conclusions.</value>
		public IEnumerable<Conclusion>? Conclusions { get => PanelViewModel.Conclusions; set => PanelViewModel.Conclusions = value; }
	}
}
