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
global using System.Text.Encodings.Web;
global using System.Text.Json;
global using System.Text.Json.Serialization;
global using System.Threading.Tasks;
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
global using DPoint = System.Drawing.Point;
global using DPointF = System.Drawing.PointF;

#if MUST_DOWNLOAD_TRAINED_DATA
global using System.Net.Http;
global using System.Threading.Tasks;
#endif

[assembly: AutoDeconstructExtension<Color>(nameof(Color.A), nameof(Color.R), nameof(Color.G), nameof(Color.B))]
[assembly: AutoDeconstructExtension<DPoint>(nameof(DPoint.X), nameof(DPoint.Y))]
[assembly: AutoDeconstructExtension<DPointF>(nameof(DPointF.X), nameof(DPointF.Y))]
[assembly: AutoDeconstructExtension<Size>(nameof(Size.Width), nameof(Size.Height))]
[assembly: AutoDeconstructExtension<SizeF>(nameof(SizeF.Width), nameof(SizeF.Height))]
[assembly: AutoDeconstructExtension<RectangleF>(nameof(RectangleF.X), nameof(RectangleF.Y), nameof(RectangleF.Width), nameof(RectangleF.Height))]
[assembly: AutoDeconstructExtension<RotatedRect>(nameof(RotatedRect.Center), nameof(RotatedRect.Size), Namespace = "Emgu.CV.Structure")]

[assembly: AutoDeconstructExtensionLambda<RectangleF, RectangleF_Dap>($".{nameof(RectangleF_Dap.Point)}", nameof(RectangleF.Size))]