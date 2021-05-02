using System;
using System.Drawing;
using Sudoku.CodeGen;

[assembly: CLSCompliant(false)]

[assembly: AutoDeconstructExtension(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B), Namespace = "Sudoku.Painting.Extensions")]
[assembly: AutoDeconstructExtension(typeof(Point), nameof(Point.X), nameof(Point.Y), Namespace = "Sudoku.Painting.Extensions")]
[assembly: AutoDeconstructExtension(typeof(PointF), nameof(PointF.X), nameof(PointF.Y), Namespace = "Sudoku.Painting.Extensions")]
[assembly: AutoDeconstructExtension(typeof(Size), nameof(Size.Width), nameof(Size.Height), Namespace = "Sudoku.Painting.Extensions")]
[assembly: AutoDeconstructExtension(typeof(SizeF), nameof(SizeF.Width), nameof(SizeF.Height), Namespace = "Sudoku.Painting.Extensions")]
[assembly: AutoDeconstructExtension(typeof(RectangleF), nameof(RectangleF.X), nameof(RectangleF.Y), nameof(RectangleF.Width), nameof(RectangleF.Height), Namespace = "Sudoku.Painting.Extensions")]