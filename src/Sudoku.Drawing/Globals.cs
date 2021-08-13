global using System.Drawing;
global using System.Drawing.Drawing2D;
global using System.Drawing.Text;
global using System.Numerics;
global using System.Runtime.CompilerServices;
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using Sudoku.CodeGenerating;
global using Sudoku.Data;
global using Sudoku.Models;
global using static System.Math;
global using DPoint = System.Drawing.Point;
global using DPointF = System.Drawing.PointF;

[assembly: AutoDeconstructExtension(typeof(Color), nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]
[assembly: AutoDeconstructExtension(typeof(DPoint), nameof(DPoint.X), nameof(DPoint.Y))]
[assembly: AutoDeconstructExtension(typeof(DPointF), nameof(DPointF.X), nameof(DPointF.Y))]
[assembly: AutoDeconstructExtension(typeof(Size), nameof(Size.Width), nameof(Size.Height))]
[assembly: AutoDeconstructExtension(typeof(SizeF), nameof(SizeF.Width), nameof(SizeF.Height))]
[assembly: AutoDeconstructExtension(typeof(RectangleF), nameof(RectangleF.X), nameof(RectangleF.Y), nameof(RectangleF.Width), nameof(RectangleF.Height))]