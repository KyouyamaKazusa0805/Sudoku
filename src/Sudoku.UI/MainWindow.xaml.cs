using System;
using System.IO;
using System.Windows;
using Sudoku.Data;
using Sudoku.UI.Extensions;

namespace Sudoku.UI
{
	/// <summary>
	/// Interaction logic for <see cref="MainWindow"/>.
	/// </summary>
	public partial class MainWindow : Window
	{
		/// <summary>
		/// Initializes a <see cref="MainWindow"/> instance with the default instantiation behavior.
		/// </summary>
		public MainWindow()
		{
			InitializeComponent();
			InitializeValue();
			InitializeEvent();

			RefreshPicture();
		}


		/// <inheritdoc/>
		protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
		{
			base.OnRenderSizeChanged(sizeInfo);

			if (sizeInfo is { WidthChanged: true } or { HeightChanged: true })
			{
				ViewModel.Size = MathF.Min(ViewModel.ActualWidth, ViewModel.ActualHeight);
			}
		}

		/// <summary>
		/// Initializes events.
		/// </summary>
		private void InitializeEvent()
		{
			var model = ViewModel.PanelViewModel;

			model.ConclusionsChanged += () => RefreshPicture();
			model.ConverterChanged += () => RefreshPicture();
			model.CustomViewChanged += () => RefreshPicture();
			model.FocusedCellsChanged += () => RefreshPicture();
			model.GeneratorChanged += () => RefreshPicture();
			model.PreferencesChanged += () => RefreshPicture();
			model.ViewChanged += () => RefreshPicture();
			model.GridChanged += () => { ViewModel.Description = null; RefreshPicture(); };
		}

		/// <summary>
		/// Set the references that XAML page can't set.
		/// </summary>
		private void InitializeValue()
		{
			ViewModel.TextBoxDescription = TextBoxDescription;
			ViewModel.SudokuPanelMain = SudokuPanelMain;
			ViewModel.GridSudokuPanel = GridSudokuPanel;
		}

		/// <summary>
		/// Refresh the control, and re-paint the picture, then display the picture.
		/// </summary>
		private void RefreshPicture() => ViewModel.Image = ViewModel.Generator.Paint();


		/// <summary>
		/// Triggers when the menu item that opens a file is clicked.
		/// </summary>
		/// <param name="sender">The sender triggers the event.</param>
		/// <param name="e">The event arguments provided.</param>
		private async void MenuItemFileOpen_ClickAsync(object sender, RoutedEventArgs e)
		{
			var dialog = OpenFileDialogs.PuzzleLoading;
			if (dialog.ShowDialog() is not true)
			{
				return;
			}

			string path = dialog.FileName;
			if (string.IsNullOrEmpty(path))
			{
				return;
			}

			string sudokuGridCode = await File.ReadAllTextAsync(path);

			if (SudokuGrid.TryParse(sudokuGridCode, out var grid))
			{
				// Load successful.
				ViewModel.Grid = grid;
			}
			else
			{
				// Load failed.
				MessageDialogs.PuzzleFileLoadFailed.ShowDialog();
			}
		}
	}
}
