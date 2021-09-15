global using System;
global using System.Collections.Generic;
global using System.Diagnostics.CodeAnalysis;
global using System.Drawing;
global using System.Drawing.Drawing2D;
global using System.Drawing.Imaging;
global using System.Drawing.Text;
global using System.IO;
global using System.Linq;
global using System.Numerics;
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
global using Sudoku.Models;
global using static System.Math;
global using static Sudoku.Recognition.Constants;
global using Cv = Emgu.CV.CvInvoke;
global using Field = Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>;

[assembly: AutoDeconstructExtension<Color>(nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]
[assembly: AutoDeconstructExtension<Point>(nameof(Point.X), nameof(Point.Y))]
[assembly: AutoDeconstructExtension<PointF>(nameof(PointF.X), nameof(PointF.Y))]
[assembly: AutoDeconstructExtension<Size>(nameof(Size.Width), nameof(Size.Height))]
[assembly: AutoDeconstructExtension<SizeF>(nameof(SizeF.Width), nameof(SizeF.Height))]
[assembly: AutoDeconstructExtension<RectangleF>(nameof(RectangleF.X), nameof(RectangleF.Y), nameof(RectangleF.Width), nameof(RectangleF.Height))]
[assembly: AutoDeconstructExtension<RotatedRect>(nameof(RotatedRect.Center), nameof(RotatedRect.Size))]