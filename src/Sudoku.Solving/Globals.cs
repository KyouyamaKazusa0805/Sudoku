global using System;
global using System.Collections.Generic;
global using System.Collections.Immutable;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Numerics;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Runtime.Serialization;
global using System.Text;
global using System.Threading;
global using System.Threading.Tasks;
global using Microsoft.CSharp.RuntimeBinder;
global using Sudoku.Data;
global using Sudoku.Data.Collections;
global using Sudoku.Diagnostics.CodeGen;
global using Sudoku.Globalization;
global using Sudoku.Models;
global using Sudoku.Resources;
global using Sudoku.Solving.BruteForces;
global using Sudoku.Solving.Collections;
global using Sudoku.Solving.Manual;
global using Sudoku.Solving.Manual.Buffer;
global using Sudoku.Solving.Manual.Checkers;
global using Sudoku.Solving.Manual.Searchers;
global using Sudoku.Solving.Manual.Searchers.DeadlyPatterns;
global using Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Extended;
global using Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Loops;
global using Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Polygons;
global using Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Rectangles;
global using Sudoku.Solving.Manual.Searchers.DeadlyPatterns.Squares;
global using Sudoku.Solving.Manual.Searchers.LastResorts;
global using Sudoku.Solving.Manual.Searchers.SingleDigitPatterns;
global using Sudoku.Solving.Manual.Searchers.Singles;
global using Sudoku.Solving.Manual.Searchers.Wings;
global using Sudoku.Solving.Manual.Steps;
global using Sudoku.Solving.Manual.Steps.Chains;
global using Sudoku.Solving.Manual.Steps.DeadlyPatterns.Extended;
global using Sudoku.Solving.Manual.Steps.DeadlyPatterns.Loops;
global using Sudoku.Solving.Manual.Steps.DeadlyPatterns.Polygons;
global using Sudoku.Solving.Manual.Steps.DeadlyPatterns.Rectangles;
global using Sudoku.Solving.Manual.Steps.DeadlyPatterns.Squares;
global using Sudoku.Solving.Manual.Steps.Fishes;
global using Sudoku.Solving.Manual.Steps.Intersections;
global using Sudoku.Solving.Manual.Steps.LastResorts;
global using Sudoku.Solving.Manual.Steps.RankTheory;
global using Sudoku.Solving.Manual.Steps.SingleDigitPatterns;
global using Sudoku.Solving.Manual.Steps.Singles;
global using Sudoku.Solving.Manual.Steps.Subsets;
global using Sudoku.Solving.Manual.Steps.Wings;
global using Sudoku.Solving.Manual.Text;
global using Sudoku.Techniques;
global using Sudoku.Versioning;
global using static System.Math;
global using static System.Numerics.BitOperations;
global using static Sudoku.Constants.Tables;
global using static Sudoku.Solving.Manual.Buffer.FastProperties;
global using static Sudoku.Solving.Manual.Constants;

#if DEBUG
[assembly: InternalsVisibleTo("Sudoku.Test")]
#endif

[module: SkipLocalsInit]