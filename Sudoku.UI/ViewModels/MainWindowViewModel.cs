using System.ComponentModel;

namespace Sudoku.UI.ViewModels
{
	/// <summary>
	/// The view model of the window <see cref="MainWindow"/>.
	/// </summary>
	/// <seealso cref="MainWindow"/>
	public sealed class MainWindowViewModel : INotifyPropertyChanging, INotifyPropertyChanged
	{
		/// <inheritdoc/>
		public event PropertyChangingEventHandler? PropertyChanging;

		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;
	}
}
