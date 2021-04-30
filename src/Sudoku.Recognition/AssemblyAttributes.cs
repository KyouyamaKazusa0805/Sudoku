using System;
using Emgu.CV.Structure;
using Sudoku.CodeGen;

[assembly: CLSCompliant(false)]

[module: AutoDeconstructExtension(typeof(RotatedRect), nameof(RotatedRect.Center), nameof(RotatedRect.Size))]