using System.Diagnostics.CodeAnalysis;
using System.Windows.Controls;
using Sudoku.Data;
using Sudoku.Data.Stepping;
using Sudoku.DocComments;
using Sudoku.Drawing;
using Sudoku.Windows.Extensions;

namespace Sudoku.Windows.CustomControls
{
	/// <summary>
	/// Interaction logic for <c>SudokuGridPanel.xaml</c>.
	/// </summary>
	public partial class SudokuGridPanel : UserControl
	{
		#region Fields
		/// <summary>
		/// Indicates the base painter.
		/// </summary>
		private GridPainter _painter;
		#endregion


		#region Constructors
		/// <inheritdoc cref="DefaultConstructor"/>
		public SudokuGridPanel() : this(null)
		{
		}

		/// <summary>
		/// Initializes an instance with the specified settings.
		/// </summary>
		/// <param name="settings">The settings.</param>
		public SudokuGridPanel(Settings? settings)
		{
			InitializeComponent();

			Settings = settings ?? new();
			Painter = new(new((float)_mainGrid.Width, (float)_mainGrid.Height), Settings, SudokuGrid.Empty);
		}
		#endregion


		#region Properties
		/// <summary>
		/// Gets or sets the main grid that this control used.
		/// </summary>
		/// <returns>The grid.</returns>
		/// <value>The grid to assign.</value>
		public UndoableGrid MainGrid
		{
			get => _painter.Grid;

			set => Painter = new(Converter, Settings, value);
		}

		/// <summary>
		/// Gets or sets the painter used.
		/// </summary>
		/// <value>The painter used.</value>
		[MemberNotNull(nameof(_painter))]
		public GridPainter Painter
		{
#nullable disable
			get => _painter;
#nullable restore

			set
			{
				_painter = value;

				Redraw();
			}
		}

		/// <summary>
		/// Indicates the inner point converter used.
		/// </summary>
		public PointConverter Converter
		{
			get => _painter.Converter;

			set => Painter = new(value, Settings, MainGrid);
		}

		/// <summary>
		/// Indicates the base settings.
		/// </summary>
		public Settings Settings { get; }
		#endregion


		#region Methods
		/// <summary>
		/// To re-draw the grid onto the control.
		/// </summary>
		private void Redraw() => _mainGrid.Source = _painter.Draw().ToImageSource();
		#endregion
	}
}
