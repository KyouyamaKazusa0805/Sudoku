using System;
using System.Linq;
using System.Threading.Tasks;
using Sudoku.Data;
using Sudoku.Extensions;
using Sudoku.Globalization;
using Sudoku.Models;
using Sudoku.Solving.Manual;
using static Sudoku.Solving.Manual.TechniqueCode;

namespace Sudoku.Solving.Generating
{
	/// <summary>
	/// Provides a puzzle generator with the technique filter.
	/// </summary>
	public sealed class TechniqueFilteringPuzzleGenerator : HardPatternPuzzleGenerator
	{
		/// <summary>
		/// Indicates the default filter.
		/// </summary>
		private static readonly TechniqueCodeFilter DefaultFilter =
			new(
				LastDigit, FullHouse,
				HiddenSingleRow, HiddenSingleColumn, HiddenSingleBlock, NakedSingle,
				NakedPair, NakedPairPlus, HiddenPair, LockedPair,
				NakedTriple, NakedTriplePlus, HiddenTriple, LockedTriple,
				NakedQuadruple, NakedQuadruplePlus, HiddenQuadruple);

		/// <summary>
		/// The default manual solver.
		/// </summary>
		private static readonly ManualSolver ManualSolver = new();


		/// <inheritdoc/>
		public override SudokuGrid Generate() => Generate(DefaultFilter, null, CountryCode.Default);


		/// <summary>
		/// To generate a puzzle that contains the specified technique code.
		/// </summary>
		/// <param name="techniqueCodeFilter">
		/// The technique codes to filter. If the parameter is <see langword="null"/>,
		/// the process will use the default filter.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="countryCode">The country code.</param>
		/// <returns>The puzzle.</returns>
		public SudokuGrid Generate(
			TechniqueCodeFilter? techniqueCodeFilter, IProgress<IProgressResult>? progress,
			CountryCode countryCode = CountryCode.Default)
		{
			PuzzleGeneratingProgressResult defaultValue = default;
			var pr = new PuzzleGeneratingProgressResult(
				0, countryCode == CountryCode.Default ? CountryCode.EnUs : countryCode);
			ref var progressResult = ref progress is null ? ref defaultValue : ref pr;
			progress?.Report(progressResult);

			techniqueCodeFilter ??= DefaultFilter;


			while (true)
			{
				var puzzle = Generate(-1, progress, countryCode: countryCode);

				bool r;
				unsafe
				{
					r = ManualSolver.Solve(puzzle).Any(&p, techniqueCodeFilter);
				}

				if (r)
				{
					return puzzle;
				}
			}

			static bool p(TechniqueInfo step, in TechniqueCodeFilter techniqueCodeFilter) =>
				techniqueCodeFilter.Contains(step.TechniqueCode);
		}

		/// <summary>
		/// To generate a puzzle that contains the specified technique code asynchronizedly.
		/// </summary>
		/// <param name="techniqueCodeFilter">
		/// The technique codes to filter. If the parameter is <see langword="null"/>,
		/// the process will use the default filter.
		/// </param>
		/// <param name="progress">The progress.</param>
		/// <param name="countryCode">The globalization string.</param>
		/// <returns>The task.</returns>
		public async Task<SudokuGrid> GenerateAsync(
			TechniqueCodeFilter? techniqueCodeFilter, IProgress<IProgressResult>? progress,
			CountryCode countryCode = CountryCode.Default) =>
			await Task.Run(() => Generate(techniqueCodeFilter, progress, countryCode));
	}
}
