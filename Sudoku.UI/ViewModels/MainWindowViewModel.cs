using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace Sudoku.UI.ViewModels
{
	/// <summary>
	/// Indicates the view model bound by <see cref="MainWindow"/>.
	/// </summary>
	/// <seealso cref="MainWindow"/>
	public sealed class MainWindowViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Indicates the sudoku panel view model.
		/// </summary>
		private SudokuPanelViewModel? _sudokuPanelViewModel;


		/// <summary>
		/// Gets or sets the sudoku panel view model.
		/// </summary>
		/// <value>The view model.</value>
		[DisallowNull]
		public SudokuPanelViewModel? SudokuPanelViewModel
		{
			get => _sudokuPanelViewModel;

			set
			{
				_sudokuPanelViewModel = value;

				PropertyChanged?.Invoke(this, new(nameof(SudokuPanelViewModel)));
			}
		}


		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
