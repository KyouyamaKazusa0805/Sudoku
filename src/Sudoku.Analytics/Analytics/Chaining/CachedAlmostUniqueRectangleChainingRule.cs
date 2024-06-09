namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.AlmostUniqueRectangle"/>).
/// </summary>
/// <example>
/// Test example:
/// <code><![CDATA[
/// 4...+892...9..+2..8+4+8.+2+46..+91.8.3...+25.2..7.4....6+25..1.+2.78.59+4.....4...2.....2.5.:714 337 183 386 687
/// 5...2.6....9....1..4...5..372..4..+5+6.9+4+5...8...57....1.+53..9..44...5.8..+97.4...6.:718 821 726 826 227 831 233 937 366 968 171 271 874 186 286 686 789 296
/// ]]></code>
/// </example>
/// <seealso cref="LinkType.AlmostUniqueRectangle"/>
internal sealed partial class CachedAlmostUniqueRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	public override void CollectStrongLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
		{
			if ((EmptyCells & urCells) != urCells)
			{
				// Four cells must be empty.
				continue;
			}

			// Collect valid digits.
			// A valid UR digit can be filled inside a UR pattern twice,
			// meaning the placement of this digit must be diagonally arranged to make it. For example,
			//   x  .
			//   .  x
			// can be filled 2 times. However, pattern
			//   .  x
			//   .  x
			// cannot.
			// Just check for spanned rows and columns, determining whether both the numbers of spanned rows and columns are 2.
			var validDigitsMask = (Mask)0;
			var allDigitsMask = grid[in urCells];
			foreach (var digit in allDigitsMask)
			{
				var cellsToFillDigit = CandidatesMap[digit] & urCells;
				if (PopCount((uint)cellsToFillDigit.RowMask) == 2 && PopCount((uint)cellsToFillDigit.ColumnMask) == 2)
				{
					validDigitsMask |= (Mask)(1 << digit);
				}
			}
			if (PopCount((uint)validDigitsMask) < 2)
			{
				// Not enough digits to form a UR.
				continue;
			}

			foreach (var digitPair in validDigitsMask.GetAllSets().GetSubsets(2))
			{
				var urDigitsMask = (Mask)(1 << digitPair[0] | 1 << digitPair[1]);
				var otherDigitsMask = (Mask)(allDigitsMask & (Mask)~urDigitsMask);
				var ur = new UniqueRectangle(in urCells, urDigitsMask, otherDigitsMask);
				switch (PopCount((uint)otherDigitsMask))
				{
					case 1:
					{
						Type1Strong(otherDigitsMask, in urCells, ur, linkDictionary);
						break;
					}
					case 2:
					{
						Type2Strong(otherDigitsMask, in urCells, ur, linkDictionary);
						goto default;
					}
					default:
					{
						Type4Strong(otherDigitsMask, in grid, in urCells, ur, linkDictionary);
						break;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectWeakLinks(ref readonly Grid grid, LinkDictionary linkDictionary)
	{
		foreach (CellMap urCells in UniqueRectangleModule.PossiblePatterns)
		{
			if ((EmptyCells & urCells) != urCells)
			{
				// Four cells must be empty.
				continue;
			}

			// Collect valid digits.
			var validDigitsMask = (Mask)0;
			var allDigitsMask = grid[in urCells];
			foreach (var digit in allDigitsMask)
			{
				var cellsToFillDigit = CandidatesMap[digit] & urCells;
				if (PopCount((uint)cellsToFillDigit.RowMask) == 2 && PopCount((uint)cellsToFillDigit.ColumnMask) == 2)
				{
					validDigitsMask |= (Mask)(1 << digit);
				}
			}
			if (PopCount((uint)validDigitsMask) < 2)
			{
				// No enough digits to form a UR.
				continue;
			}

			foreach (var digitPair in validDigitsMask.GetAllSets().GetSubsets(2))
			{
				var (digit1, digit2) = (digitPair[0], digitPair[1]);
				var urDigitsMask = (Mask)(1 << digit1 | 1 << digit2);
				var otherDigitsMask = (Mask)(allDigitsMask & (Mask)~urDigitsMask);
				if (PopCount((uint)otherDigitsMask) < 2)
				{
					continue;
				}

				var ur = new UniqueRectangle(in urCells, urDigitsMask, otherDigitsMask);
				switch (PopCount((uint)otherDigitsMask))
				{
					case 1:
					{
						Type1Weak(otherDigitsMask, in urCells, ur, linkDictionary);
						break;
					}
					case 2:
					{
						Type2Weak(otherDigitsMask, in urCells, ur, linkDictionary);
						goto default;
					}
					default:
					{
						break;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	public override void CollectExtraViewNodes(ref readonly Grid grid, ChainPattern pattern, ref View[] views)
	{
		var (view, id) = (views[0], ColorIdentifier.Auxiliary3);
		foreach (var link in pattern.Links)
		{
			if (link.GroupedLinkPattern is UniqueRectangle { Cells: var cells, DigitsMask: var digitsMask })
			{
				// If the cell has already been colorized, we should change the color into UR-categorized one.
				foreach (var cell in cells)
				{
					foreach (var digit in (Mask)(grid.GetCandidates(cell) & digitsMask))
					{
						var candidate = cell * 9 + digit;
						if (view.FindCandidate(candidate) is { } candidateViewNode)
						{
							view.Remove(candidateViewNode);
						}
						view.Add(new CandidateViewNode(id, candidate));
					}
				}
				foreach (var cell in cells)
				{
					if (view.FindCell(cell) is { } cellViewNode)
					{
						view.Remove(cellViewNode);
					}
					view.Add(new CellViewNode(id, cell));
				}
			}
		}
	}


	partial void Type1Strong(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary);
	partial void Type2Strong(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary);
	partial void Type4Strong(short otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary);
	partial void Type1Weak(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary);
	partial void Type2Weak(short otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary);
}
