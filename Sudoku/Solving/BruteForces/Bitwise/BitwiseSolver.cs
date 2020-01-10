using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Sudoku.Data.Meta;

namespace Sudoku.Solving.BruteForces.Bitwise
{
	public sealed class BitwiseSolver : Solver
	{
		public override string SolverName => "Bitwise";


		public override AnalysisResult Solve(Grid grid)
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
					0 => new AnalysisResult(
						solverName: SolverName,
						hasSolved: false,
						solution: null,
						elapsedTime: stopwatch.Elapsed,
						solvingList: null,
						additional: null),
					1 => new AnalysisResult(
						solverName: SolverName,
						hasSolved: true,
						solution: Grid.Parse(sb.ToString()),
						elapsedTime: stopwatch.Elapsed,
						solvingList: null,
						additional: null),
					_ => new AnalysisResult(
						solverName: SolverName,
						hasSolved: false,
						solution: Grid.Parse(sb.ToString()),
						elapsedTime: stopwatch.Elapsed,
						solvingList: null,
						additional: null),
				};
			}
			catch (Exception ex1)
			{
				try
				{
					int count = Solve64(str, sb, 2);
					return count switch
					{
						0 => new AnalysisResult(
							solverName: SolverName,
							hasSolved: false,
							solution: null,
							elapsedTime: stopwatch.Elapsed,
							solvingList: null,
							additional: null),
						1 => new AnalysisResult(
							solverName: SolverName,
							hasSolved: true,
							solution: Grid.Parse(sb.ToString()),
							elapsedTime: stopwatch.Elapsed,
							solvingList: null,
							additional: null),
						_ => new AnalysisResult(
							solverName: SolverName,
							hasSolved: false,
							solution: Grid.Parse(sb.ToString()),
							elapsedTime: stopwatch.Elapsed,
							solvingList: null,
							additional: null),
					};
				}
				catch (Exception ex2)
				{
					return new AnalysisResult(
						solverName: SolverName,
						hasSolved: false,
						solution: null,
						elapsedTime: TimeSpan.Zero,
						solvingList: null,
						additional: $"{ex1}{Environment.NewLine}{ex2}");
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
		[DllImport("Sudoku.BitwiseSolver (x86).dll", EntryPoint = "Solve", CharSet = CharSet.Unicode)]
		private static extern int Solve32(
			[MarshalAs(UnmanagedType.LPWStr)] string puzzle,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder solution,
			[MarshalAs(UnmanagedType.I4)] int limit);

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
		[DllImport("Sudoku.BitwiseSolver (x64).dll", EntryPoint = "Solve", CharSet = CharSet.Unicode)]
		private static extern int Solve64(
			[MarshalAs(UnmanagedType.LPWStr)] string puzzle,
			[MarshalAs(UnmanagedType.LPWStr)] StringBuilder solution,
			[MarshalAs(UnmanagedType.I4)] int limit);
	}
}
