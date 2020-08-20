using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.Bf.Bitwise
{
	[SuppressUnmanagedCodeSecurity]
	[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "<挂起>")]
	public sealed class BitwiseSolver : BruteForceSolver
	{
		public override string Name => "Bitwise";


		public override Grid? Solve(Grid grid, out AnalysisInfo analysisInfo)
		{
			var str = grid.ToString("g0", null);
			var sb = new StringBuilder(81);
			var stopwatch = new Stopwatch();

			try
			{
				stopwatch.Start();

				analysisInfo = new(Name, null, stopwatch.Elapsed, true);
				return Solve32(str, sb, 2) != 1 ? null : Grid.Parse(sb.ToString());
			}
			catch (BadImageFormatException ex) when (ex.HResult == -2147024885)
			{
				stopwatch.Stop();

				// The magic number is 0x8007000B, which means
				// we use 32-bit dll file on 64-bit-based platform.
				// This is incompatible, so we catch this exception
				// and try using 64-bit dll to execute that unmanaged function.
				// 0x8007000B is not a `int` value, but a `uint` value,
				// so we should express the value by its complement value.
				analysisInfo = new(Name, null, stopwatch.Elapsed, false);
				return Solve64(str, sb, 2) != 1 ? null : Grid.Parse(sb.ToString());
			}
			catch
			{
				stopwatch.Stop();

				// Neither x86 nor x64 or wrong execution.
				// What the hell will return?
				analysisInfo = new(Name, null, stopwatch.Elapsed, false);
				return null;
			}
		}


		/// <summary>
		/// The core function of solving the puzzle based on x86 platform.
		/// </summary>
		/// <param name="puzzle">The puzzle Susser format string.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="limit">
		/// The limit count in solving the puzzle.
		/// You should pass the value by a positive integer at least 1.
		/// </param>
		/// <returns>The solution count of the puzzle.</returns>
		[DllImport("Sunniedoku.BitwiseSolver (x86).dll", EntryPoint = "Solve", CharSet = CharSet.Unicode)]
		private static extern nint Solve32(
			[MarshalAs(UnmanagedType.LPStr)] string puzzle,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder solution,
			nint limit);

		/// <summary>
		/// The core function of solving the puzzle based on x64 platform.
		/// </summary>
		/// <param name="puzzle">The puzzle Susser format string.</param>
		/// <param name="solution">The solution.</param>
		/// <param name="limit">
		/// The limit count in solving the puzzle.
		/// You should pass the value by a positive integer at least 1.
		/// </param>
		/// <returns>The solution count of the puzzle.</returns>
		[DllImport("Sunniedoku.BitwiseSolver (x64).dll", EntryPoint = "Solve", CharSet = CharSet.Unicode)]
		private static extern nint Solve64(
			[MarshalAs(UnmanagedType.LPStr)] string puzzle,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder solution,
			nint limit);
	}
}
