using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;
using Sudoku.Data;
using static System.Runtime.InteropServices.CharSet;
using static System.Runtime.InteropServices.CallingConvention;
using static System.Runtime.InteropServices.UnmanagedType;
using ImmutableString = System.String;
using CStyleString = System.Text.StringBuilder;
#if TARGET_64BIT
using native_int = System.Int32;
#else
using native_int = System.Int16;
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
			string str = grid.ToString(".");
			var sb = new CStyleString(82);
			var stopwatch = new Stopwatch();

			try
			{
				stopwatch.Start();
				int count = Solve32(str, sb, 2);
				stopwatch.Stop();

				return count switch
				{
					0 => throw new NoSolutionException(grid),
					1 =>
						new AnalysisResult(
							puzzle: grid,
							solverName: SolverName,
							hasSolved: true,
							solution: Grid.Parse(sb.ToString()),
							elapsedTime: stopwatch.Elapsed,
							solvingList: null,
							additional: null,
							stepGrids: null),
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
						1 =>
							new AnalysisResult(
								puzzle: grid,
								solverName: SolverName,
								hasSolved: true,
								solution: Grid.Parse(sb.ToString()),
								elapsedTime: stopwatch.Elapsed,
								solvingList: null,
								additional: null,
								stepGrids: null),
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
						additional: $"{ex1.Message}{Environment.NewLine}{ex2.Message}",
						stepGrids: null);
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
		public bool CheckValidity(IReadOnlyGrid grid, [NotNullWhen(true)] out IReadOnlyGrid? solutionIfUnique)
		{
			var sb = new CStyleString(82);
			if (Solve(grid.ToString("0"), sb, 2) == 1)
			{
				solutionIfUnique = Grid.Parse(sb.ToString());
				return true;
			}
			else
			{
				solutionIfUnique = null;
				return false;
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
		public bool CheckValidity(ImmutableString grid, [NotNullWhen(true)] out string? solutionIfUnique)
		{
			var sb = new CStyleString(82);
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
		public native_int Solve(ImmutableString puzzle, CStyleString? solution, native_int limit)
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
		private static extern native_int Solve32(
			[MarshalAs(LPStr)] ImmutableString puzzle,
			[MarshalAs(LPStr)] CStyleString? solution,
#if TARGET_64BIT
			[MarshalAs(I4)]
#else
			[MarshalAs(I2)]
#endif
			native_int limit);

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
		private static extern native_int Solve64(
			[MarshalAs(LPStr)] ImmutableString puzzle,
			[MarshalAs(LPStr)] CStyleString? solution,
#if TARGET_64BIT
			[MarshalAs(I4)]
#else
			[MarshalAs(I2)]
#endif
			native_int limit);
	}
}
