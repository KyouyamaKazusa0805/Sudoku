using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku.Data;
using Sudoku.Windows;
using static System.Runtime.InteropServices.CallingConvention;
using static System.Runtime.InteropServices.CharSet;
using static System.Runtime.InteropServices.UnmanagedType;

namespace Sudoku.Solving.BruteForces.Bitwise
{
	/// <summary>
	/// Provides a sukaku solver using bitwise method.
	/// </summary>
	[SuppressMessage("Performance", "CA1822:Mark members as static", Justification = "<Pending>")]
	[SuppressMessage("Globalization", "CA2101:Specify marshaling for P/Invoke string arguments", Justification = "<Pending>")]
	public sealed class SukakuBitwiseSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => Resources.GetValue("BitwiseSukaku");


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			string str = grid.ToString("~");
			var sb = new StringBuilder(730);
			var stopwatch = new Stopwatch();

			try
			{
				stopwatch.Start();
				nint count = Solve32(str, sb, 2);
				stopwatch.Stop();

				return count switch
				{
					0 => throw new NoSolutionException(grid),
					1 =>
						new AnalysisResult(
							solverName: SolverName,
							puzzle: grid,
							solution: Grid.Parse(sb.ToString()),
							hasSolved: true,
							elapsedTime: stopwatch.Elapsed,
							additional: null),
					_ => throw new MultipleSolutionsException(grid)
				};
			}
			catch (Exception ex1)
			{
				try
				{
					stopwatch.Restart();
					nint count = Solve64(str, sb, 2);
					stopwatch.Stop();

					return count switch
					{
						0 => throw new NoSolutionException(grid),
						1 =>
							new AnalysisResult(
								solverName: SolverName,
								puzzle: grid,
								solution: Grid.Parse(sb.ToString()),
								hasSolved: true,
								elapsedTime: stopwatch.Elapsed,
								additional: null),
						_ => throw new MultipleSolutionsException(grid)
					};
				}
				catch (Exception ex2)
				{
					return new AnalysisResult(
						solverName: SolverName,
						puzzle: grid,
						solution: null,
						hasSolved: false,
						elapsedTime: stopwatch.Elapsed,
						additional: $"{ex1.Message}{Environment.NewLine}{ex2.Message}");
				}
			}
		}

		/// <summary>
		/// Check the validity of the puzzle.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="solutionIfUnique">
		/// (<see langword="out"/> parameter) The solution.
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public bool CheckValidity(IReadOnlyGrid grid, [NotNullWhen(true)] out string? solutionIfUnique) =>
			CheckValidity(grid.ToString("~"), out solutionIfUnique);

		/// <summary>
		/// Check the validity of the puzzle.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <param name="solutionIfUnique">
		/// (<see langword="out"/> parameter) The solution.
		/// </param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public bool CheckValidity(string grid, [NotNullWhen(true)] out string? solutionIfUnique)
		{
			var sb = new StringBuilder(82);
			if (Solve(grid, sb, 2) == 1)
			{
				solutionIfUnique = sb.ToString();
				return true;
			}
			else
			{
				solutionIfUnique = null;
				return false;
			}
		}

		/// <summary>
		/// The inner solver.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solution">
		/// The solution. <see langword="null"/> if you does not want to use this result.
		/// </param>
		/// <param name="limit">The limit.</param>
		/// <returns>The number of all solutions.</returns>
		public nint Solve(string puzzle, StringBuilder? solution, nint limit)
		{
			try
			{
				return Solve32(puzzle, solution, limit);
			}
			catch
			{
				try
				{
					return Solve64(puzzle, solution, limit);
				}
				catch
				{
					return 0;
				}
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
		[DllImport("Sudoku.BitwiseSolver (x86).dll", EntryPoint = "Solve", CallingConvention = StdCall, CharSet = Ansi)]
		private static extern nint Solve32(
			[MarshalAs(LPStr)] string puzzle, [MarshalAs(LPStr)] StringBuilder? solution, nint limit);

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
		[DllImport("Sudoku.BitwiseSolver (x64).dll", EntryPoint = "Solve", CallingConvention = StdCall, CharSet = Ansi)]
		private static extern nint Solve64(
			[MarshalAs(LPStr)] string puzzle, [MarshalAs(LPStr)] StringBuilder? solution, nint limit);
	}
}
