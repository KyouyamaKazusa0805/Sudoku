using Sudoku.UI.Controls;
using System.Windows.Controls;

namespace Sudoku.UI.ViewModels
{
	partial class MainWindowViewModel
	{
		/// <summary>
		/// Indicates the actual width of the sudoku panel.
		/// </summary>
		public float ActualWidth => (float)GridSudokuPanel.ColumnDefinitions[0].ActualWidth;

		/// <summary>
		/// Indicates the actual height of the sudoku panel.
		/// </summary>
		public float ActualHeight => (float)GridSudokuPanel.RowDefinitions[0].ActualHeight;

		/// <summary>
		/// Gets or sets the size.
		/// </summary>
		/// <value>The size.</value>
		public float Size
		{
			get => Generator.Width;

			set
			{
				Generator = new(new(value, value), Generator.Preferences) { Grid = Grid };

				Preferences.GridSize = value;

				Image = Generator.Paint();
			}
		}

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

		/// <summary>
		/// Indicates the <see cref="System.Windows.Controls.Grid"/> instance that stores the main sudoku panel.
		/// </summary>
		/// <seealso cref="System.Windows.Controls.Grid"/>
		public Grid GridSudokuPanel { get; set; } = null!;
	}
}
