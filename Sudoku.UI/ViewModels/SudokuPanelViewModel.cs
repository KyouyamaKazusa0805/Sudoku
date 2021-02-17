using System;
using System.Collections.Generic;
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
	public sealed class SudokuPanelViewModel
	{
		/// <summary>
		/// Indicates the default rendering size.
		/// </summary>
		private static readonly float RenderingSize = (float)(double)App.DefaultSudokuGridRenderingSize;


		/// <summary>
		/// The back field of the property <see cref="Generator"/>.
		/// </summary>
		/// <seealso cref="Generator"/>
		private GridImageGenerator _generator = new(new(RenderingSize, RenderingSize), new());


		/// <summary>
		/// Gets or sets the focused cells.
		/// </summary>
		/// <value>The focused cells.</value>
		public Cells FocusedCells
		{
			get => Generator.FocusedCells;

			set
			{
				Generator.FocusedCells = value;

				FocusedCellsChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the grid.
		/// </summary>
		/// <value>The grid.</value>
		public SudokuGrid Grid
		{
			get => Generator.Grid;

			set
			{
				Generator.Grid = value;

				GridChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the converter.
		/// </summary>
		/// <value>The converter.</value>
		public DrawingPointConverter Converter
		{
			get => Generator.Converter;

			set
			{
				Generator = new(value, Generator.Preferences);

				ConverterChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the preferences.
		/// </summary>
		/// <value>The preferences.</value>
		public Settings Preferences
		{
			get => Generator.Preferences;

			set
			{
				Generator = new(Generator.Converter, value);

				PreferencesChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the view.
		/// </summary>
		/// <value>The view.</value>
		public PresentationData? View
		{
			get => Generator.View;

			set
			{
				Generator.View = value;

				ViewChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the custom view.
		/// </summary>
		/// <value>The custom view.</value>
		public PresentationData? CustomView
		{
			get => Generator.CustomView;

			set
			{
				Generator.CustomView = value;

				CustomViewChanged?.Invoke(this, value);
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

				GeneratorChanged?.Invoke(this, value);
			}
		}

		/// <summary>
		/// Gets or sets the conclusions.
		/// </summary>
		/// <value>The conclusions.</value>
		public IEnumerable<Conclusion>? Conclusions
		{
			get => Generator.Conclusions;

			set
			{
				Generator.Conclusions = value;

				ConclusionsChanged?.Invoke(this, value);
			}
		}


		/// <summary>
		/// Indicates the current application.
		/// </summary>
		private static dynamic App => TextResources.Current;


		/// <summary>
		/// Indicates the event to trigger when the focused cells instance has been changed.
		/// </summary>
		public event EventHandler<Cells>? FocusedCellsChanged;

		/// <summary>
		/// Indicates the event to trigger when the grid instance has been changed.
		/// </summary>
		public event EventHandler<SudokuGrid>? GridChanged;

		/// <summary>
		/// Indicates the event to trigger when the converter instance has been changed.
		/// </summary>
		public event EventHandler<DrawingPointConverter>? ConverterChanged;

		/// <summary>
		/// Indicates the event to trigger when the preferences instance has been changed.
		/// </summary>
		public event EventHandler<Settings>? PreferencesChanged;

		/// <summary>
		/// Indicates the event to trigger when the view instance has been changed.
		/// </summary>
		public event EventHandler<PresentationData?>? ViewChanged;

		/// <summary>
		/// Indicates the event to trigger when the custom view instance has been changed.
		/// </summary>
		public event EventHandler<PresentationData?>? CustomViewChanged;

		/// <summary>
		/// Indicates the event to trigger when the generator instance has been changed.
		/// </summary>
		public event EventHandler<GridImageGenerator>? GeneratorChanged;

		/// <summary>
		/// Indicates the event to trigger when the conclusions instance has been changed.
		/// </summary>
		public event EventHandler<IEnumerable<Conclusion>?>? ConclusionsChanged;
	}
}
