using System.ComponentModel;

namespace Sudoku.UI.ViewModels
{
	/// <summary>
	/// The view model that is bound by <see cref="MainWindow"/>.
	/// </summary>
	public sealed class MainWindowViewModel : INotifyPropertyChanged
	{
		/// <inheritdoc/>
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
