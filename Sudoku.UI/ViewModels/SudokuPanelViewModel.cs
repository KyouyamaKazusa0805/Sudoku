using System.Collections.Generic;
using System.ComponentModel;
using Sudoku.Data;
using Sudoku.Drawing;
using Sudoku.UI.Controls;
using Sudoku.UI.ResourceDictionaries;

namespace Sudoku.UI.ViewModels
{
	/// <summary>
	/// Indicates the view model bound by <see cref="SudokuPanel"/>.
	/// </summary>
	/// <seealso cref="SudokuPanel"/>
	public sealed class SudokuPanelViewModel : INotifyPropertyChanged
	{
		/// <summary>
		/// Indicates the inner generator.
		/// </summary>
		private GridImageGenerator _generator = new(
			new(
				(float)(double)TextResources.Current.DefaultSudokuGridRenderingSize,
				(float)(double)TextResources.Current.DefaultSudokuGridRenderingSize
			),
			new()
		);


		/// <inheritdoc/>
		public event PropertyChangedEventHandler? PropertyChanged;


		/// <summary>
		/// Gets or sets the focused cells.
		/// </summary>
		/// <value>The focused cells.</value>
		public Cells FocusedCells
		{
			get => _generator.FocusedCells;

			set
			{
				_generator.FocusedCells = value;

				PropertyChanged?.Invoke(this, new(nameof(FocusedCells)));
			}
		}

		/// <summary>
		/// Gets or sets the grid.
		/// </summary>
		/// <value>The grid.</value>
		public SudokuGrid Grid
		{
			get => _generator.Grid;

			set
			{
				_generator.Grid = value;

				PropertyChanged?.Invoke(this, new(nameof(Grid)));
			}
		}

		/// <summary>
		/// Gets or sets the converter.
		/// </summary>
		/// <value>The converter.</value>
		public DrawingPointConverter Converter
		{
			get => _generator.Converter;

			set
			{
				_generator = new(value, _generator.Preferences);

				PropertyChanged?.Invoke(this, new(nameof(Converter)));
			}
		}

		/// <summary>
		/// Gets or sets the preferences.
		/// </summary>
		/// <value>The preferences.</value>
		public Settings Preferences
		{
			get => _generator.Preferences;

			set
			{
				_generator = new(_generator.Converter, value);

				PropertyChanged?.Invoke(this, new(nameof(Preferences)));
			}
		}

		/// <summary>
		/// Gets or sets the view.
		/// </summary>
		/// <value>The view.</value>
		public PresentationData? View
		{
			get => _generator.View;

			set
			{
				_generator.View = value;

				PropertyChanged?.Invoke(this, new(nameof(View)));
			}
		}

		/// <summary>
		/// Gets or sets the custom view.
		/// </summary>
		/// <value>The custom view.</value>
		public PresentationData? CustomView
		{
			get => _generator.CustomView;

			set
			{
				_generator.CustomView = value;

				PropertyChanged?.Invoke(this, new(nameof(CustomView)));
			}
		}

		/// <summary>
		/// Gets or sets the generator.
		/// </summary>
		/// <value>The generator.</value>
		public GridImageGenerator Generator
		{
			get => _generator;

			set
			{
				_generator = value;

				PropertyChanged?.Invoke(this, new(nameof(Generator)));
			}
		}

		/// <summary>
		/// Gets or sets the conclusions.
		/// </summary>
		/// <value>The conclusions.</value>
		public IEnumerable<Conclusion>? Conclusions
		{
			get => _generator.Conclusions;

			set
			{
				_generator.Conclusions = value;

				PropertyChanged?.Invoke(this, new(nameof(Conclusions)));
			}
		}
	}
}
