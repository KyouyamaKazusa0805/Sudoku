global using Microsoft.UI.Xaml;
global using Microsoft.UI.Xaml.Controls;
global using static Sudoku.Runtime.AnalysisServices.CommonReadOnlies;
global using static Sudoku.UI.StringResource;
global using GridLayout = Microsoft.UI.Xaml.Controls.Grid;
global using Grid = Sudoku.Concepts.Collections.Grid;
using System.Diagnostics.CodeGen;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

[assembly: AutoExtensionDeconstruction(typeof(Line), nameof(Line.X1), nameof(Line.X2), nameof(Line.Y1), nameof(Line.Y2))]
[assembly: AutoExtensionDeconstruction(typeof(Point), nameof(Point.X), nameof(Point.Y))]