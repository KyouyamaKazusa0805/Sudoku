using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku.Data;
using Sudoku.Runtime;
#if BIT64
using nint = System.Int32;
#else
using nint = System.Int16;
#endif

namespace Sudoku.Solving.BruteForces.Bitwise
{
	/// <summary>
	/// Provides a solver using bitwise method.
	/// </summary>
	public sealed class BitwiseSolver : Solver
	{
		/// <inheritdoc/>
		public override string SolverName => "Bitwise";


		/// <inheritdoc/>
		public override AnalysisResult Solve(IReadOnlyGrid grid)
		{
			var str = grid.ToString(".");
			var sb = new StringBuilder(81);
			var stopwatch = new Stopwatch();

			try
			{
				stopwatch.Start();
				int count = Solve32(str, sb, 2);
				stopwatch.Stop();

				return count switch
				{
					0 => throw new NoSolutionException(grid),
					1 => new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: true,
						solution: Grid.Parse(sb.ToString()),
						elapsedTime: stopwatch.Elapsed,
						solvingList: null,
						additional: null),
					_ => throw new MultipleSolutionsException(grid)
				};
			}
			catch (Exception ex1)
			{
				try
				{
					stopwatch.Restart();
					int count = Solve64(str, sb, 2);
					stopwatch.Stop();

					return count switch
					{
						0 => throw new NoSolutionException(grid),
						1 => new AnalysisResult(
							puzzle: grid,
							solverName: SolverName,
							hasSolved: true,
							solution: Grid.Parse(sb.ToString()),
							elapsedTime: stopwatch.Elapsed,
							solvingList: null,
							additional: null),
						_ => throw new MultipleSolutionsException(grid)
					};
				}
				catch (Exception ex2)
				{
					return new AnalysisResult(
						puzzle: grid,
						solverName: SolverName,
						hasSolved: false,
						solution: null,
						elapsedTime: stopwatch.Elapsed,
						solvingList: null,
						additional: $"{ex1.Message}{Environment.NewLine}{ex2.Message}");
				}
			}
		}

		/// <summary>
		/// Check the validity of the puzzle.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public bool CheckValidity(IReadOnlyGrid grid) =>
			Solve(grid.ToString("0"), null, 2) == 1;

		/// <summary>
		/// Check the validity of the puzzle.
		/// </summary>
		/// <param name="grid">The grid.</param>
		/// <returns>The <see cref="bool"/> result.</returns>
		public bool CheckValidity(string grid) => Solve(grid, null, 2) == 1;

		/// <summary>
		/// The inner solver.
		/// </summary>
		/// <param name="puzzle">The puzzle.</param>
		/// <param name="solution">
		/// The solution. <see langword="null"/> if you does not want to use this result.
		/// </param>
		/// <param name="limit">The limit.</param>
		/// <returns>The number of all solutions.</returns>
		public int Solve(string puzzle, StringBuilder? solution, int limit)
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
		[DllImport("Sudoku.BitwiseSolver (x86).dll",
			EntryPoint = "Solve", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.StdCall)]
		private static extern nint Solve32(
			[MarshalAs(UnmanagedType.LPStr)] string puzzle,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder? solution,
#if BIT64
			[MarshalAs(UnmanagedType.I4)]
#else
			[MarshalAs(UnmanagedType.I2)]
#endif
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
		[DllImport("Sudoku.BitwiseSolver (x64).dll",
			EntryPoint = "Solve", CharSet = CharSet.Ansi,
			CallingConvention = CallingConvention.StdCall)]
		private static extern nint Solve64(
			[MarshalAs(UnmanagedType.LPStr)] string puzzle,
			[MarshalAs(UnmanagedType.LPStr)] StringBuilder? solution,
#if BIT64
			[MarshalAs(UnmanagedType.I4)]
#else
			[MarshalAs(UnmanagedType.I2)]
#endif
			nint limit);
	}
}
