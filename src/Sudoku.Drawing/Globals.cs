global using System.Diagnostics.CodeAnalysis;
global using System.Drawing;
global using System.Drawing.Imaging;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using Emgu.CV;
global using Emgu.CV.CvEnum;
global using Emgu.CV.OCR;
global using Emgu.CV.Structure;
global using Emgu.CV.Util;
global using Sudoku.CodeGenerating;
global using Sudoku.Data;
global using static Sudoku.Recognition.Constants;
global using Cv = Emgu.CV.CvInvoke;
global using Field = Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>;
global using d = System.Drawing;

[assembly: SuppressMessage("Style", "IDE0001:Simplify Names", Justification = "<Pending>")]

[assembly: AutoDeconstructExtension(typeof(d::Color), nameof(d::Color.A), nameof(d::Color.R), nameof(d::Color.G), nameof(d::Color.B))]
[assembly: AutoDeconstructExtension(typeof(d::Point), nameof(d::Point.X), nameof(d::Point.Y))]
[assembly: AutoDeconstructExtension(typeof(d::PointF), nameof(d::PointF.X), nameof(d::PointF.Y))]
[assembly: AutoDeconstructExtension(typeof(d::Size), nameof(d::Size.Width), nameof(d::Size.Height))]
[assembly: AutoDeconstructExtension(typeof(d::SizeF), nameof(d::SizeF.Width), nameof(d::SizeF.Height))]
[assembly: AutoDeconstructExtension(typeof(d::RectangleF), nameof(d::RectangleF.X), nameof(d::RectangleF.Y), nameof(d::RectangleF.Width), nameof(d::RectangleF.Height))]
[assembly: AutoDeconstructExtension(typeof(RotatedRect), nameof(RotatedRect.Center), nameof(RotatedRect.Size))]