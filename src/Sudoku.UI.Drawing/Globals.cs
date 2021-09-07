global using System.ComponentModel;
global using System.Diagnostics.CodeAnalysis;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using Microsoft.UI;
global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using Microsoft.UI.Xaml.Media;
global using Microsoft.UI.Xaml.Shapes;
global using Sudoku.CodeGenerating;
global using Sudoku.Data;
global using Sudoku.Models;
global using Sudoku.UI.Drawing.Xaml.Converters;
global using Windows.Foundation;
global using Windows.UI;
global using Windows.UI.Text;
global using static System.Math;
global using static Sudoku.Constants.Tables;
global using Grid = Microsoft.UI.Xaml.Controls.Grid;
global using Path = Microsoft.UI.Xaml.Shapes.Path;
global using SudokuGrid = Sudoku.Data.Grid;
global using ChainLinkType = Sudoku.Data.LinkType;

[assembly: AutoDeconstructExtension(typeof(Point), nameof(Point.X), nameof(Point.Y))]
[assembly: AutoDeconstructExtension(typeof(Size), nameof(Size.Width), nameof(Size.Height))]
[assembly: AutoDeconstructExtension(typeof(Color), nameof(Color.R), nameof(Color.G), nameof(Color.B))]
[assembly: AutoDeconstructExtension(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]