global using System.Buffers;
global using System.Collections;
global using System.Collections.Generic;
global using System.ComponentModel;
global using System.Diagnostics;
global using System.Diagnostics.CodeAnalysis;
global using System.Linq;
global using System.Reflection;
global using System.Runtime.CompilerServices;
global using System.Runtime.InteropServices;
global using System.Text;
global using System.Text.Json.Serialization;
global using System.Text.RegularExpressions;
global using static System.Math;
global using static System.MathF;
global using static System.Numerics.BitOperations;

#if FEATURE_GENERIC_MATH && FEATURE_GENERIC_MATH_IN_ARG
global using System.Runtime.Versioning;
#endif

[assembly: InternalsVisibleTo("Sudoku.Core")]