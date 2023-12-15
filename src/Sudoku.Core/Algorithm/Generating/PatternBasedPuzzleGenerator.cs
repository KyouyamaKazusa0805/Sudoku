using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.SourceGeneration;
using Sudoku.Algorithm.Solving;
using Sudoku.Concepts;
using static System.Numerics.BitOperations;

namespace Sudoku.Algorithm.Generating;

/// <summary>
/// Represents a generator that is based on pattern.
/// </summary>
/// <param name="seedPattern"><inheritdoc cref="Pattern" path="/summary"/></param>
/// <param name="seedGivens"><inheritdoc cref="SeedGivens" path="/summary"/></param>
[StructLayout(LayoutKind.Auto)]
[Equals]
[GetHashCode]
[ToString]
[method: MethodImpl(MethodImplOptions.AggressiveInlining)]
public ref partial struct PatternBasedPuzzleGenerator(
	[Data(DataMemberKinds.Field, RefKind = "ref readonly")] ref readonly CellMap seedPattern,
	[Data(DataMemberKinds.Field, RefKind = "ref readonly")] ref readonly CandidateMap seedGivens
)
{
	/// <summary>
	/// The internal solver.
	/// </summary>
	private readonly BitwiseSolver _solver = new();

	/// <summary>
	/// Indicates the test grid.
	/// </summary>
	private Grid _testGrid;


	/// <summary>
	/// Indicates the predefind pattern used.
	/// </summary>
	public readonly ref readonly CellMap Pattern => ref _seedPattern;

	/// <summary>
	/// Indicates the predefined given digits used.
	/// </summary>
	public readonly ref readonly CandidateMap SeedGivens => ref _seedGivens;


	/// <summary>
	/// Try to generate a puzzle using the specified pattern.
	/// </summary>
	/// <param name="cancellationToken">The cancellation token that can cancel the operation.</param>
	/// <returns>A valid <see cref="Grid"/> pattern that has a specified pattern, with specified digits should be filled in.</returns>
	/// <exception cref="InvalidOperationException">Throws when the specified </exception>
	[UnscopedRef]
	public unsafe ref readonly Grid Generate(CancellationToken cancellationToken = default)
	{
		_testGrid = Grid.Empty;
		var uncoveredCandidates = CandidateMap.Empty;
		foreach (var candidate in _seedGivens)
		{
			var cell = candidate / 9;
			var digit = candidate % 9;
			if (_seedPattern.Contains(cell))
			{
				_testGrid.SetDigit(cell, digit);
			}
			else
			{
				uncoveredCandidates.Add(candidate);
			}
		}

		_testGrid.Fix();
		if (_solver.Solve(_testGrid.IsEmpty ? Grid.EmptyString : _testGrid.ToString("0"), null, 2) == 0)
		{
			throw new InvalidOperationException("The select pattern is invalid because the predefined digits make the pattern incorrect.");
		}

		if (_testGrid.SolutionGrid is { IsValid: true } solution && CheckSolutionValidity(in solution, in uncoveredCandidates))
		{
			// The puzzle is set as a valid puzzle. Just return.
			return ref _testGrid;
		}

		var gridCopied = _testGrid;
		fixed (Grid* pGrid = &_testGrid)
		{
			var pGridCopied = pGrid;
			while (true)
			{
				// Firstly, copy the original template.
				_testGrid = gridCopied;

				// Set the cells with the digits randomly selected.
				foreach (var cell in
					from cell in _seedPattern.ToArray()
					orderby PopCount((uint)pGridCopied->GetCandidates(cell)) descending
					select cell)
				{
					var candidates = _testGrid.GetCandidates(cell);
					var digit = candidates.SetAt(Random.Shared.Next(0, PopCount((uint)candidates)));
					_testGrid.SetDigit(cell, digit);
				}

				// Check validity.
				solution = _testGrid.SolutionGrid;
				if (!solution.IsUndefined && CheckSolutionValidity(in solution, in uncoveredCandidates))
				{
					return ref _testGrid;
				}

				cancellationToken.ThrowIfCancellationRequested();
			}
		}
	}

	/// <summary>
	/// Checks the validity of the solution to the uncovered candidates.
	/// </summary>
	private readonly bool CheckSolutionValidity(scoped ref readonly Grid solution, scoped ref readonly CandidateMap uncoveredCandidates)
	{
		foreach (var uncoveredCandidate in uncoveredCandidates)
		{
			if (solution.GetDigit(uncoveredCandidate / 9) != uncoveredCandidate % 9)
			{
				return false;
			}
		}

		return true;
	}
}
