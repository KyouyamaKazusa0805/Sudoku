global using System.Buffers;
global using System.ComponentModel;
global using System.Reflection;
global using System.Runtime.InteropServices;
global using static System.Math;
global using static System.MathF;
global using static System.Numerics.BitOperations;

#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
global using System.Runtime.Versioning;
#endif

[assembly: InternalsVisibleTo("Sudoku.Core")]