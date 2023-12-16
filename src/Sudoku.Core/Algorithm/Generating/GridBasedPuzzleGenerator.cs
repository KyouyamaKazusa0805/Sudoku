using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using Sudoku.Algorithm.Solving;
using Sudoku.Concepts;
using Sudoku.Runtime.CompilerServices;
using static System.Numerics.BitOperations;
using static Sudoku.SolutionWideReadOnlyFields;

namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a grid-based puzzle generator.
/// </summary>
/// <param name="seedGrid"><inheritdoc cref="SeedGrid" path="/summary"/></param>
[StructLayout(LayoutKind.Auto)]
[LargeStructure]
[Equals]
[GetHashCode]
[ToString]
[method: DebuggerStepThrough]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct GridBasedPuzzleGenerator([Data(DataMemberKinds.Field, RefKind = "ref readonly")] ref readonly Grid seedGrid)
{
	/// <summary>
	/// The internal solver.
	/// </summary>
	private readonly BitwiseSolver _solver = new();

	/// <summary>
	/// Indicates the playground.
	/// </summary>
	private Grid _playground;

	/// <summary>
	/// Indicates the result grid.
	/// </summary>
	private Grid _resultGrid;


	/// <summary>
	/// Indicates the seed grid to be used.
	/// </summary>
	public readonly ref readonly Grid SeedGrid => ref _seedGrid;


	/// <summary>
	/// Try to generate a puzzle using the specified seed pattern.
	/// </summary>
	/// <param name="shuffleDigits">Indicates whether the method will shuffle digits, making the puzzle looking different with the seed.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A valid <see cref="Grid"/> to be used.</returns>
	[UnscopedRef]
	public ref readonly Grid Generate(bool shuffleDigits = false, CancellationToken cancellationToken = default)
	{
		try
		{
			_playground = _seedGrid.UnfixedGrid;
			getGrid(_solver, in _seedGrid, ref _playground, [.. _seedGrid.GivenCells], ref _resultGrid);
			return ref _resultGrid;
		}
		catch (OperationCanceledException)
		{
			return ref Grid.Undefined;
		}
		catch
		{
			throw;
		}


		void getGrid(
			BitwiseSolver solver,
			scoped ref readonly Grid seed,
			scoped ref Grid playground,
			Cell[] pattern,
			scoped ref Grid resultGrid
		)
		{
			while (true)
			{
				Random.Shared.Shuffle(pattern);

				var shuffleDigitsCount = Random.Shared.Next(1, 6);
				var selectedCells = pattern[..shuffleDigitsCount];
				foreach (var cell in selectedCells)
				{
					var duplicatedDigitsMask = (Mask)0;
					foreach (var c in PeersMap[cell])
					{
						if (playground.GetState(c) == CellState.Modifiable)
						{
							duplicatedDigitsMask |= (Mask)(1 << playground.GetDigit(c));
						}
					}

					var availableDigitsMask = (Mask)(Grid.MaxCandidatesMask & ~duplicatedDigitsMask);
					if (availableDigitsMask == 0)
					{
						// No available digits can be used.
						continue;
					}

					// Reset the digit.
					playground.SetDigit(cell, -1);
					playground.SetDigit(cell, availableDigitsMask.SetAt(Random.Shared.Next(0, PopCount((uint)availableDigitsMask))));
				}

				// Check validity.
				if (solver.CheckValidity(playground.ToString("!0")) && playground.FixedGrid is var @fixed && seed != @fixed)
				{
					if (shuffleDigits)
					{
						ShuffleDigitsFor10Times(ref @fixed);
					}

					resultGrid = @fixed;
					return;
				}

				// Revert.
				foreach (var cell in selectedCells)
				{
					playground.SetMask(cell, (Mask)(Grid.ModifiableMask | (Mask)(1 << seed.GetDigit(cell))));
				}

				cancellationToken.ThrowIfCancellationRequested();
			}
		}
	}

	/// <summary>
	/// Batch generating puzzles.
	/// </summary>
	/// <param name="times">The times of the pattern will be tried.</param>
	/// <param name="shuffleDigits"><inheritdoc cref="Generate(bool, CancellationToken)" path="/param[@name='shuffleDigits']"/></param>
	/// <param name="progress">The <see cref="IProgress{T}"/> instance that can report the progess.</param>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A list of valid <see cref="Grid"/> values.</returns>
	public ReadOnlySpan<Grid> BatchGenerate(
		int times,
		bool shuffleDigits = false,
		IProgress<GeneratorProgress>? progress = null,
		CancellationToken cancellationToken = default
	)
	{
		var resultList = new List<Grid>(times);

		// Iterate for the specified times.
		for (var i = 1; i <= times;)
		{
			// Try to generate a puzzle no matter the puzzle is duplicated with one of elements stored in the current collection.
			// Make the reference to be mutable because the return value is always points to the field of this type (i.e. '_resultGrid').
			scoped ref var puzzle = ref Ref.AsMutableRef(in Generate(cancellationToken: cancellationToken));

			// Check for duplicate.
			var isDupe = false;
			foreach (ref readonly var tempGrid in CollectionsMarshal.AsSpan(resultList))
			{
				if (tempGrid == puzzle)
				{
					isDupe = true;
					break;
				}
			}
			if (!isDupe)
			{
				resultList.AddRef(in puzzle);
				progress?.Report(new(i));
				i++;
			}

			cancellationToken.ThrowIfCancellationRequested();
		}

		// Shuffle digits if worth.
		// We should shuffle values here because we cannot determine whther two grids having been shuffled are same.
		if (shuffleDigits)
		{
			foreach (ref var puzzle in CollectionsMarshal.AsSpan(resultList))
			{
				ShuffleDigitsFor10Times(ref puzzle);
			}
		}

		// Return the span.
		return CollectionsMarshal.AsSpan(resultList);
	}

	/// <summary>
	/// Shuffle digits for 10 times.
	/// </summary>
	/// <param name="puzzle">The puzzle to be shuffle.</param>
	private static void ShuffleDigitsFor10Times(scoped ref Grid puzzle)
	{
		for (var times = 0; times < 10; times++)
		{
			var d1 = Random.Shared.Next(0, 9);
			int d2;
			do
			{
				d2 = Random.Shared.Next(0, 9);
			} while (d1 == d2);

			puzzle.SwapTwoDigits(d1, d2);
		}
	}
}
