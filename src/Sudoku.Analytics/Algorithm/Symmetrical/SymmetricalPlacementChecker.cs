using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sudoku.Concepts;

namespace Sudoku.Algorithm.Symmetrical;

/// <summary>
/// Represents a checker type that checks for technique Gurth's symmetrical placement.
/// </summary>
public static class SymmetricalPlacementChecker
{
	/// <summary>
	/// The internal methods.
	/// </summary>
	private static readonly unsafe delegate*<ref readonly Grid, out SymmetricType, out Digit?[]?, out Mask, bool>[] Checkers = [
		&Diagonal,
		&AntiDiagonal,
		&Central
	];


	/// <summary>
	/// Try to get the its mapping rule for the specified grid via the specified symmetric type.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="symmetricType">The symmetric type to be checked.</param>
	/// <param name="mappingDigits">The mapping digits returned.</param>
	/// <param name="selfPairedDigitsMask">A mask that contains a list of digits self-paired.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the grid is a symmetrical-placement pattern.</returns>
	/// <exception cref="ArgumentOutOfRangeException">
	/// Throws when the argument <paramref name="symmetricType"/> is not <see cref="SymmetricType.Central"/>,
	/// <see cref="SymmetricType.Diagonal"/> or	<see cref="SymmetricType.AntiDiagonal"/>.
	/// </exception>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public static unsafe bool IsSymmetry(
		this scoped ref readonly Grid grid,
		SymmetricType symmetricType,
		[NotNullWhen(true)] out Digit?[]? mappingDigits,
		out Mask selfPairedDigitsMask
	)
	{
		if (symmetricType is not (SymmetricType.Central or SymmetricType.Diagonal or SymmetricType.AntiDiagonal))
		{
			throw new ArgumentOutOfRangeException(nameof(symmetricType));
		}

#nullable disable
		var index = symmetricType switch { SymmetricType.Diagonal => 0, SymmetricType.AntiDiagonal => 1, _ => 2 };
		return Checkers[index](in grid, out _, out mappingDigits, out selfPairedDigitsMask);
#nullable restore
	}

	/// <summary>
	/// Try to get the symmetric type and its mapping rule for the specified grid.
	/// </summary>
	/// <param name="grid">The grid.</param>
	/// <param name="symmetricType">The symmetric type returned.</param>
	/// <param name="mappingDigits">The mapping digits returned.</param>
	/// <param name="selfPairedDigitsMask">A mask that contains a list of digits self-paired.</param>
	/// <returns>A <see cref="bool"/> result indicating whether the grid is a symmetrical-placement pattern.</returns>
	public static unsafe bool GetSymmetricType(
		this scoped ref readonly Grid grid,
		out SymmetricType symmetricType,
		[NotNullWhen(true)] out Digit?[]? mappingDigits,
		out Mask selfPairedDigitsMask
	)
	{
#nullable disable
		foreach (var functionPointer in Checkers)
		{
			if (functionPointer(in grid, out symmetricType, out mappingDigits, out selfPairedDigitsMask))
			{
				return true;
			}
		}
#nullable restore

		symmetricType = SymmetricType.None;
		mappingDigits = null;
		selfPairedDigitsMask = 0;
		return false;
	}


	private static bool Diagonal(
		scoped ref readonly Grid grid,
		out SymmetricType symmetricType,
		[NotNullWhen(true)] out Digit?[]? mappingDigits,
		out Mask selfPairedDigitsMask
	)
	{
		scoped var mapping = (stackalloc Digit?[9]);
		mapping.Clear();
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 8 - i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (8 - j) * 9 + (8 - i);
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					goto ReturnFalse;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						goto ReturnFalse;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						goto ReturnFalse;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						goto ReturnFalse;
					}
				}
			}
		}

		symmetricType = SymmetricType.Diagonal;
		mappingDigits = [.. mapping];
		selfPairedDigitsMask = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (mapping[digit] switch { { } currentDigit => currentDigit == digit, _ => true })
			{
				selfPairedDigitsMask |= (Mask)(1 << digit);
			}
		}

		return true;

	ReturnFalse:
		symmetricType = SymmetricType.None;
		mappingDigits = null;
		selfPairedDigitsMask = 0;
		return false;
	}

	private static bool AntiDiagonal(
		scoped ref readonly Grid grid,
		out SymmetricType symmetricType,
		[NotNullWhen(true)] out Digit?[]? mappingDigits,
		out Mask selfPairedDigitsMask
	)
	{
		scoped var mapping = (stackalloc Digit?[9]);
		mapping.Clear();
		for (var i = 0; i < 9; i++)
		{
			for (var j = 0; j < 8 - i; j++)
			{
				var c1 = i * 9 + j;
				var c2 = (8 - j) * 9 + (8 - i);
				var condition = grid.GetState(c1) == CellState.Empty;
				if (condition ^ grid.GetState(c2) == CellState.Empty)
				{
					// One of two cells is empty. Not this symmetry.
					goto ReturnFalse;
				}

				if (condition)
				{
					continue;
				}

				var d1 = grid.GetDigit(c1);
				var d2 = grid.GetDigit(c2);
				if (d1 == d2)
				{
					var o1 = mapping[d1];
					if (o1 is null)
					{
						mapping[d1] = d1;
						continue;
					}

					if (o1 != d1)
					{
						goto ReturnFalse;
					}
				}
				else
				{
					var o1 = mapping[d1];
					var o2 = mapping[d2];
					if (o1.HasValue ^ o2.HasValue)
					{
						goto ReturnFalse;
					}

					if (o1 is null || o2 is null)
					{
						mapping[d1] = d2;
						mapping[d2] = d1;
						continue;
					}

					// 'o1' and 'o2' are both not null.
					if (o1 != d2 || o2 != d1)
					{
						goto ReturnFalse;
					}
				}
			}
		}

		symmetricType = SymmetricType.AntiDiagonal;
		mappingDigits = [.. mapping];
		selfPairedDigitsMask = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (mapping[digit] switch { { } currentDigit => currentDigit == digit, _ => true })
			{
				selfPairedDigitsMask |= (Mask)(1 << digit);
			}
		}

		return true;

	ReturnFalse:
		symmetricType = SymmetricType.None;
		mappingDigits = null;
		selfPairedDigitsMask = 0;
		return false;
	}

	private static bool Central(
		scoped ref readonly Grid grid,
		out SymmetricType symmetricType,
		[NotNullWhen(true)] out Digit?[]? mappingDigits,
		out Mask selfPairedDigitsMask
	)
	{
		scoped var mapping = (stackalloc Digit?[9]);
		mapping.Clear();
		for (var cell = 0; cell < 40; cell++)
		{
			var anotherCell = 80 - cell;
			var condition = grid.GetState(cell) == CellState.Empty;
			if (condition ^ grid.GetState(anotherCell) == CellState.Empty)
			{
				// One of two cell is empty, not central symmetry type.
				goto ReturnFalse;
			}

			if (condition)
			{
				continue;
			}

			var d1 = grid.GetDigit(cell);
			var d2 = grid.GetDigit(anotherCell);
			if (d1 == d2)
			{
				var o1 = mapping[d1];
				if (o1 is null)
				{
					mapping[d1] = d1;
					continue;
				}

				if (o1 != d1)
				{
					goto ReturnFalse;
				}
			}
			else
			{
				var o1 = mapping[d1];
				var o2 = mapping[d2];
				if (o1 is not null ^ o2 is not null)
				{
					goto ReturnFalse;
				}

				if (o1 is null || o2 is null)
				{
					mapping[d1] = d2;
					mapping[d2] = d1;
					continue;
				}

				// 'o1' and 'o2' are both not null.
				if (o1 != d2 || o2 != d1)
				{
					goto ReturnFalse;
				}
			}
		}

		symmetricType = SymmetricType.AntiDiagonal;
		mappingDigits = [.. mapping];
		selfPairedDigitsMask = 0;
		for (var digit = 0; digit < 9; digit++)
		{
			if (mapping[digit] switch { { } currentDigit => currentDigit == digit, _ => true })
			{
				selfPairedDigitsMask |= (Mask)(1 << digit);
			}
		}

		return true;

	ReturnFalse:
		symmetricType = SymmetricType.None;
		mappingDigits = null;
		selfPairedDigitsMask = 0;
		return false;
	}
}
