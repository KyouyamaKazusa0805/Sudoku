// This project is copied and modified from the repo https://github.com/VoBilyk/SudokuSolverOCR
// Original author: VoBilyk https://github.com/VoBilyk
// Copyright (c) VoBilyk 2018
//
// This project obeys LGPLv2.1 open-source license.
// For more information, please visit the file.

global using System;
global using System.Diagnostics.CodeAnalysis;
global using System.Diagnostics.CodeGen;
global using System.Drawing;
global using System.Drawing.Imaging;
global using System.IO;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Versioning;
global using Emgu.CV;
global using Emgu.CV.CvEnum;
global using Emgu.CV.OCR;
global using Emgu.CV.Structure;
global using Emgu.CV.Util;
global using Sudoku.Concepts.Collections;
global using static System.Math;
global using static Sudoku.Recognition.Constants;
global using Cv = Emgu.CV.CvInvoke;
global using Field = Emgu.CV.Image<Emgu.CV.Structure.Bgr, byte>;
