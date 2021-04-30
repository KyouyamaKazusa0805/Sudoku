using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using Sudoku.CodeGen;

[module: AutoDeconstructExtension(typeof(Image), nameof(Image.Width), nameof(Image.Height))]
[module: AutoDeconstructExtension(typeof(Point), nameof(Point.X), nameof(Point.Y))]
[module: AutoDeconstructExtension(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]
[module: AutoDeconstructExtension(typeof(Size), nameof(Size.Width), nameof(Size.Height))]