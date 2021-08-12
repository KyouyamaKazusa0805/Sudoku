using System.Diagnostics.CodeAnalysis;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku.CodeGenerating;

[assembly: AssemblyObsolete]

[assembly: AutoDeconstructExtension(typeof(Image), nameof(Image.Width), nameof(Image.Height), Namespace = "Sudoku.Windows.Extensions")]
[assembly: AutoDeconstructExtension(typeof(Point), nameof(Point.X), nameof(Point.Y), Namespace = "Sudoku.Windows.Extensions")]
[assembly: AutoDeconstructExtension(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B), Namespace = "Sudoku.Windows.Extensions")]
[assembly: AutoDeconstructExtension(typeof(Size), nameof(Size.Width), nameof(Size.Height), Namespace = "Sudoku.Windows.Extensions")]