using Sudoku.UI.Controls;
using System.Windows.Controls;

namespace Sudoku.UI.ViewModels
{
	partial class MainWindowViewModel
	{
		/// <summary>
		/// Gets or sets the description of the window (using the text box to display).
		/// If <see langword="null"/>, the control will clear its value.
		/// </summary>
		/// <value>The description.</value>
		public string? Description
		{
			get => TextBoxDescription.Text;

			set
			{
				if (value is null)
				{
					TextBoxDescription.ClearValue(TextBox.TextProperty);
				}
				else
				{
					TextBoxDescription.Text = value;
				}
			}
		}

		/// <summary>
		/// Indicates the base panel.
		/// </summary>
		public SudokuPanel SudokuPanelMain { get; set; } = null!;

		/// <summary>
		/// Indicates the text box that stores the description.
		/// </summary>
		public TextBox TextBoxDescription { get; set; } = null!;
	}
}
