using System.Diagnostics.CodeGen;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;

[assembly: AutoExtensionDeconstruction(typeof(Line), nameof(Line.X1), nameof(Line.X2), nameof(Line.Y1), nameof(Line.Y2))]
[assembly: AutoExtensionDeconstruction(typeof(Point), nameof(Point.X), nameof(Point.Y))]
