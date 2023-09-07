// Copyright (C) 2008-12 Bernhard Hobiger
// 
// HoDoKu is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or (at your option) any later version.
// 
// HoDoKu is distributed in the hope that it will be useful, but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License along with HoDoKu. If not, see <http://www.gnu.org/licenses/>.
// 
// This code is actually a Java port of code posted by Glenn Fowler in the Sudoku Player's Forum (http://www.setbb.com/sudoku).
// Many thanks for letting me use it!

using Sudoku.Algorithm.Solving;

namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator that is implemented by HoDoKu.
/// </summary>
/// <remarks><b>
/// This type is special: we strongly recommend use <see langword="static"/> method
/// <see cref="Generate(SymmetricType, CancellationToken)"/> or <see cref="Generate(SymmetricType, in CellMap, CancellationToken)"/>
/// instead of calling parameterless constructor due to the internal mechanism.
/// </b></remarks>
/// <seealso cref="Generate(SymmetricType, CancellationToken)"/>
/// <seealso cref="Generate(SymmetricType, in CellMap, CancellationToken)"/>
public ref struct HodokuPuzzleGenerator
{
	/// <summary>
	/// Maximum number of tries when generating a puzzle using a pattern.
	/// </summary>
	private const int MaxTries = 1_000_000;


	/// <summary>
	/// A random generator for creating new puzzles.
	/// </summary>
	private static readonly Random Rng = new();

	/// <summary>
	/// Indicates the internal fast solver.
	/// </summary>
	private static readonly BitwiseSolver FastSolver = new();


	/// <summary>
	/// The order in which cells are set when generating a full grid.
	/// </summary>
	private readonly int[] _generateIndices;

	/// <summary>
	/// The recursion stack.
	/// </summary>
	private readonly RecursionStackEntry[] _stack;

	/// <summary>
	/// The final grid to be used.
	/// </summary>
	private Grid _newFullSudoku, _newValidSudoku;


	/// <summary>
	/// Creates a new instance of <see cref="HodokuPuzzleGenerator"/>.
	/// </summary>
	/// <remarks>
	/// <include
	///     file='../../global-doc-comments.xml'
	///     path='g/csharp9/feature[@name="parameterless-struct-constructor"]/target[@name="constructor"]' />
	/// </remarks>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public HodokuPuzzleGenerator()
	{
		_generateIndices = new int[81];
		_stack = new RecursionStackEntry[82];
		_stack.ForEachRef((scoped ref RecursionStackEntry element) => element = new());
	}


	/// <summary>
	/// <inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/summary"/>
	/// </summary>
	/// <param name="symmetricType">
	/// <inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/param[@name='symmetricType']"/>
	/// </param>
	/// <param name="pattern">The pattern indicating the states of selection on all cells.</param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns><inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/returns"/></returns>
	[SuppressMessage("Style", "IDE0011:Add braces", Justification = "<Pending>")]
	private Grid Generate(SymmetricType symmetricType, scoped in CellMap pattern, CancellationToken cancellationToken = default)
	{
		try
		{
			while (!GenerateForFullGrid()) ;

			if (pattern)
			{
				var ok = (bool?)false;
				for (var i = 0; i < MaxTries; i++)
				{
					if ((ok = GenerateInitPos(pattern, cancellationToken)) is not false)
					{
						break;
					}
				}
				if (ok is not true)
				{
					return Grid.Undefined;
				}
			}
			else
			{
				GenerateInitPos(symmetricType, cancellationToken);
			}

			return _newValidSudoku.FixedGrid;
		}
		catch (OperationCanceledException)
		{
			return Grid.Undefined;
		}
	}

	/// <summary>
	/// Takes a full sudoku from <see cref="_newFullSudoku"/> and generates a valid puzzle by deleting cells.
	/// If a deletion produces a grid with more than one solution it is of course undone.
	/// </summary>
	/// <param name="symmetricType">
	/// <inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/param[@name='symmetricType']"/>
	/// </param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	private void GenerateInitPos(SymmetricType symmetricType, CancellationToken cancellationToken = default)
	{
		// We start with the full board.
		(var maxPosToFill, var used, var usedCount, _newValidSudoku, var remainingClues) = (17, CellMap.Empty, 81, _newFullSudoku, 81);
		using scoped var candidateCells = new ValueList<Cell>(8);

		// Do until we have only 17 clues left or until all cells have been tried.
		while (remainingClues > maxPosToFill && usedCount > 1)
		{
			// Get the next position to try.
			var cell = Rng.Next(81);
			do
			{
				if (cell < 80)
				{
					cell++;
				}
				else
				{
					cell = 0;
				}
			}
			while (used.Contains(cell));
			used.Add(cell);
			usedCount--;

			if (_newValidSudoku.GetDigit(cell) == -1)
			{
				// Already deleted (symmetry).
				continue;
			}

			candidateCells.Clear();

			foreach (var tempCell in symmetricType.GetCells(cell / 9, cell % 9))
			{
				if (_newValidSudoku.GetDigit(tempCell) != -1)
				{
					candidateCells.Add(tempCell);
				}
			}
			if (candidateCells.Count == 0)
			{
				// The other end of our symmetric puzzle is already deleted.
				continue;
			}

			foreach (var candidateCell in candidateCells)
			{
				// Delete cell.
				_newValidSudoku.SetDigit(candidateCell, -1);
				used.Add(candidateCell);
				remainingClues--;
				if (candidateCell != cell)
				{
					usedCount--;
				}
			}

			if (!FastSolver.CheckValidity(_newValidSudoku.ToString("!0")))
			{
				// If not unique, revert deletion.
				foreach (var candidateCell in candidateCells)
				{
					_newValidSudoku.SetDigit(candidateCell, _newFullSudoku.GetDigit(candidateCell));
					remainingClues++;
				}
			}

			cancellationToken.ThrowIfCancellationRequested();
		}
	}

	/// <summary>
	/// Takes a full sudoku from <see cref="_newFullSudoku"/> and generates a valid puzzle by deleting the cells indicated
	/// by <paramref name="pattern"/>. If the resulting puzzle is invalid, <see langword="false"/> is returned and the caller
	/// is responsible for continuing the search.
	/// </summary>
	/// <param name="pattern">The pattern.</param>
	/// <param name="cancellationToken">
	/// <inheritdoc cref="Generate(SymmetricType, CancellationToken)" path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns>A <see cref="bool"/> result indicating whether the pattern is valid to be used.</returns>
	private bool? GenerateInitPos(scoped in CellMap pattern, CancellationToken cancellationToken = default)
	{
		_newValidSudoku = _newFullSudoku;
		for (var cell = 0; cell < 81; cell++)
		{
			if (!pattern.Contains(cell))
			{
				_newValidSudoku.SetDigit(cell, -1);
			}
		}

		return cancellationToken.IsCancellationRequested ? null : FastSolver.CheckValidity(_newValidSudoku.ToString("!0"));
	}

	/// <summary>
	/// Generate a solution grid.
	/// </summary>
	/// <returns>A <see cref="bool"/> value indicating whether the generation operation is succeeded.</returns>
	private bool GenerateForFullGrid()
	{
		// Limit the number of tries.
		var actTries = 0;

		// Generate a random order for setting the cells.
		for (var i = 0; i < 81; i++)
		{
			_generateIndices[i] = i;
		}

		for (var i = 0; i < 81; i++)
		{
			var (index1, index2) = (Rng.Next(81), Rng.Next(81));
			while (index1 == index2)
			{
				index2 = Rng.Next(81);
			}

			unsafe
			{
				fixed (int* p = _generateIndices)
				{
					PointerOperations.Swap(p + index1, p + index2);
				}
			}
		}

		// First set a new empty Sudoku.
		(_stack[0].SudokuGrid, var level, _stack[0].Cell) = (Grid.Empty, 0, -1);
		while (true)
		{
			// Get the next unsolved cell according to _generateIndices.
			if (_stack[level].SudokuGrid.EmptiesCount == 0)
			{
				// Generation is complete.
				_newFullSudoku = _stack[level].SudokuGrid;
				return true;
			}

			var index = -1;
			var actValues = _stack[level].SudokuGrid.ToArray();
			for (var i = 0; i < 81; i++)
			{
				var actTry = _generateIndices[i];
				if (actValues[actTry] == 0)
				{
					index = actTry;
					break;
				}
			}

			level++;
			_stack[level].Cell = (short)index;
			_stack[level].Candidates = _stack[level - 1].SudokuGrid.GetCandidates(index);
			_stack[level].CandidateIndex = 0;

			// Not too many tries...
			actTries++;
			if (actTries > 100)
			{
				return false;
			}

			// Go to the next level.
			var done = false;
			do
			{
				// This loop runs as long as the next candidate tried produces an invalid sudoku or until all candidates have been tried.
				// Fall back all levels, where nothing is to do anymore.
				while (_stack[level].CandidateIndex >= PopCount((uint)_stack[level].Candidates))
				{
					level--;
					if (level <= 0)
					{
						// No level with candidates left.
						done = true;
						break;
					}
				}
				if (done)
				{
					break;
				}

				// Try the next candidate.
				var nextCandidate = _stack[level].Candidates.SetAt(_stack[level].CandidateIndex++);

				// Start with a fresh sudoku.
				scoped ref var targetGrid = ref _stack[level].SudokuGrid;
				targetGrid = _stack[level - 1].SudokuGrid;
				targetGrid.SetDigit(_stack[level].Cell, nextCandidate);
				if (!checkValidityOnDuplicate(targetGrid, _stack[level].Cell))
				{
					// Invalid -> try next candidate.
					continue;
				}

				if (fillFastForSingles(ref targetGrid))
				{
					// Valid move, break from the inner loop to advance to the next level.
					break;
				}
			} while (true);
			if (done)
			{
				break;
			}
		}
		return false;


		static bool checkValidityOnDuplicate(scoped in Grid grid, Cell cell)
		{
			foreach (var peer in Peers[cell])
			{
				var digit = grid.GetDigit(peer);
				if (digit == grid.GetDigit(cell) && digit != -1)
				{
					return false;
				}
			}

			return true;
		}

		static bool fillFastForSingles(scoped ref Grid grid)
		{
			var emptyCells = grid.EmptyCells;

			// For hidden singles.
			for (var house = 0; house < 27; house++)
			{
				for (var digit = 0; digit < 9; digit++)
				{
					var houseMask = 0;
					for (var i = 0; i < 9; i++)
					{
						var cell = HouseCells[house][i];
						if (emptyCells.Contains(cell) && (grid.GetCandidates(cell) >> digit & 1) != 0)
						{
							houseMask |= 1 << i;
						}
					}

					if (IsPow2(houseMask))
					{
						// Hidden single.
						var cell = HouseCells[house][TrailingZeroCount(houseMask)];
						grid.SetDigit(cell, digit);
						if (!checkValidityOnDuplicate(grid, cell))
						{
							// Invalid.
							return false;
						}
					}
				}
			}

			// For naked singles.
			foreach (var cell in emptyCells)
			{
				var mask = grid.GetCandidates(cell);
				if (IsPow2(mask))
				{
					grid.SetDigit(cell, TrailingZeroCount(mask));
					if (!checkValidityOnDuplicate(grid, cell))
					{
						// Invalid.
						return false;
					}
				}
			}

			// Both hidden singles and naked singles are valid. Return true.
			return true;
		}
	}


	/// <summary>
	/// <inheritdoc cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)" path="/summary"/>
	/// </summary>
	/// <param name="symmetricType">The symmetric type to be specified. The value is <see cref="SymmetricType.Central"/> by default.</param>
	/// <param name="cancellationToken">
	/// <inheritdoc
	///     cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)"
	///     path="/param[@name='cancellationToken']"/>
	/// </param>
	/// <returns><inheritdoc cref="IPuzzleGenerator.Generate(IProgress{GeneratorProgress}?, CancellationToken)" path="/returns"/></returns>
	/// <exception cref="ArgumentException">Throws when the argument <paramref name="symmetricType"/> holds multiple flags.</exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static Grid Generate(SymmetricType symmetricType = SymmetricType.Central, CancellationToken cancellationToken = default)
		=> symmetricType.IsFlag()
			? new HodokuPuzzleGenerator().Generate(symmetricType, CellMap.Empty, cancellationToken)
			: throw new ArgumentException($"The argument '{nameof(symmetricType)}' is invalid because it holds multiple flags.");
}
