using System;
using Emgu.CV.Structure;
using Sudoku.CodeGen;

[assembly: CLSCompliant(false)]

[assembly: AutoDeconstructExtension(typeof(RotatedRect), nameof(RotatedRect.Center), nameof(RotatedRect.Size), Namespace = "Sudoku.Recognition.Extensions")]