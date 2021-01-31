using System;
using System.Collections.Generic;
using Microsoft.UI.Xaml.Shapes;
using Sudoku.Data;
using Sudoku.Models;

namespace Sudoku.Painting
{
	/// <summary>
	/// Indicates the grid painter.
	/// </summary>
	/// <param name="Translator">Indicates the translator.</param>
	/// <param name="Preferences">Indicates the default preferences that used for painting.</param>
	/// <remarks>
	/// Please note that eliminations will be colored with red, but all assignments won't be colored,
	/// because they will be colored using another method (draw candidates).
	/// </remarks>
	public sealed partial record GridPainter(in PointTranslator Translator, PreferencesBase Preferences)
	{
		/// <summary>
		/// Indicates the length.
		/// </summary>
		private const int Length = 28;

		/// <summary>
		/// The square root of 2.
		/// </summary>
		private const double SqrtOf2 = 1.41421356;

		/// <summary>
		/// The rotate angle (45 degrees, i.e. <c><see cref="Math.PI"/> / 4</c>).
		/// This field is used for rotate the chains if some of them are overlapped.
		/// </summary>
		/// <seealso cref="Math.PI"/>
		private const double RotateAngle = .78539816;


		/// <summary>
		/// Indicates the drawing width.
		/// </summary>
		public double Width => Translator.ControlSize.Width;

		/// <summary>
		/// Indicates the drawing height.
		/// </summary>
		public double Height => Translator.ControlSize.Height;

		/// <summary>
		/// Indicates the focused cells.
		/// </summary>
		public Cells FocusedCells { get; set; }

		/// <summary>
		/// Indicates the sudoku grid used.
		/// </summary>
		public SudokuGrid Grid { get; set; }

		/// <summary>
		/// Indicates the view.
		/// </summary>
		public PresentationData? View { get; set; }

		/// <summary>
		/// Indicates the custom view.
		/// </summary>
		public PresentationData? CustomView { get; set; }

		/// <summary>
		/// Indicates all conclusions.
		/// </summary>
		public IEnumerable<Conclusion>? Conclusions { get; set; }


		/// <summary>
		/// To create a <see cref="Shape"/> collection that draws all elements here.
		/// </summary>
		/// <returns>The <see cref="Shape"/> collection.</returns>
		public IReadOnlyCollection<Shape> Create()
		{
			var shapes = new List<Shape>();

			PaintBackground(shapes);
			PaintGridAndBlockLines(shapes);

			const double offset = 6;
			PaintViewIfNeed(shapes, offset);
			PaintCustomViewIfNeed(shapes, offset);

			if (!FocusedCells.IsEmpty) PaintFocusedCells(shapes, FocusedCells);
			if (Conclusions is not null) PaintEliminations(shapes, Conclusions, offset);
			if (Grid != SudokuGrid.Undefined) PaintValue(shapes, Grid);

			return shapes;
		}

		/// <summary>
		/// Draw the specified view.
		/// </summary>
		/// <param name="controls">The graphics.</param>
		/// <param name="offset">The drawing offset.</param>
		/// <param name="view">The view instance.</param>
		private void PaintViewIfNeedInternal(IList<Shape> controls, double offset, dynamic view)
		{
			if (view.Regions is IEnumerable<DrawingInfo> regions)
			{
				PaintRegions(controls, regions, offset);
			}
			if (view.Cells is IEnumerable<DrawingInfo> cells)
			{
				PaintCells(controls, cells);
			}
			if (view.Candidates is IEnumerable<DrawingInfo> candidates)
			{
				PaintCandidates(controls, candidates, offset);
			}
			if (view.Links is IEnumerable<Link> links)
			{
				PaintLinks(controls, links, offset);
			}
			if (view.DirectLines is IEnumerable<(Cells, Cells)> directLines)
			{
				PaintDirectLines(controls, directLines, offset);
			}
		}


		partial void PaintBackground(IList<Shape> controls);
		partial void PaintGridAndBlockLines(IList<Shape> controls);
		partial void PaintViewIfNeed(IList<Shape> controls, double offset);
		partial void PaintCustomViewIfNeed(IList<Shape> controls, double offset);
		partial void PaintFocusedCells(IList<Shape> controls, in Cells focusedCells);
		partial void PaintEliminations(IList<Shape> controls, IEnumerable<Conclusion> conclusions, double offset);
		partial void PaintValue(IList<Shape> controls, in SudokuGrid grid);

		partial void PaintRegions(IList<Shape> controls, IEnumerable<DrawingInfo> regions, double offset);
		partial void PaintCells(IList<Shape> controls, IEnumerable<DrawingInfo> cells);
		partial void PaintCandidates(IList<Shape> controls, IEnumerable<DrawingInfo> candidates, double offset);
		partial void PaintLinks(IList<Shape> controls, IEnumerable<Link> links, double offset);
		partial void PaintDirectLines(IList<Shape> controls, IEnumerable<(Cells Start, Cells End)> directLines, double offset);
	}
}
