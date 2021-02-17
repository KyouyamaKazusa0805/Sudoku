using System.ComponentModel;
using Sudoku.UI.ResourceDictionaries;
using Sudoku.UI.Windows;

namespace Sudoku.UI.ViewModels
{
	/// <summary>
	/// Indicates the view model bound by <see cref="MessageDialog"/>.
	/// </summary>
	/// <seealso cref="MessageDialog"/>
	public sealed class MessageDialogViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// The back field of the property <see cref="Text"/>.
		/// </summary>
		private string _text = string.Empty;

		/// <summary>
		/// The back field of the property <see cref="Title"/>.
		/// </summary>
		/// <seealso cref="Title"/>
		private string _title = TextResources.Current.MessageDialogTitle;


		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;


		/// <summary>
		/// Gets or sets the base text.
		/// </summary>
		/// <value>The value you want to set.</value>
		public string Text
		{
			get => _text;

			set
			{
				_text = value;

				PropertyChanged?.Invoke(this, new(nameof(Text)));
			}
		}

		/// <summary>
		/// Gets or sets the title of the window.
		/// </summary>
		/// <value>The value you want to set.</value>
		public string Title
		{
			get => _title;

			set
			{
				_title = value;

				PropertyChanged?.Invoke(this, new(nameof(Title)));
			}
		}
	}
}
