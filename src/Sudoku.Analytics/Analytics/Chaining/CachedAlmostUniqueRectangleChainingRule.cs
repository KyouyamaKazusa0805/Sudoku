namespace Sudoku.Analytics.Chaining;

/// <summary>
/// Represents a chaining rule on AUR rule (i.e. <see cref="LinkType.AlmostUniqueRectangle"/>).
/// </summary>
/// <example>
/// Test example:
/// <code><![CDATA[
/// 4...+892...9..+2..8+4+8.+2+46..+91.8.3...+25.2..7.4....6+25..1.+2.78.59+4.....4...2.....2.5.:714 337 183 386 687
/// 5...2.6....9....1..4...5..372..4..+5+6.9+4+5...8...57....1.+53..9..44...5.8..+97.4...6.:718 821 726 826 227 831 233 937 366 968 171 271 874 186 286 686 789 296
/// 3..5...6..1...+9..7..7...9...8.1...2..+32.6..4.1.+439+2...+24...6..5..3.2..8.8..+4..6.+2:815 816 117 817 824 425 631 532 834 136 338 139 439 541 745 757 857 859 767 774 177 178 989
/// ..5..2+1+63.2.4+3.5+9.9...6.+2+4....8....6.....74+8+1.7..1..5+2..2.8...+57.....6.9.9.3..8.4:821 823 832 833 734 341 441 541 342 442 343 361 363 966 778 183 691 595 198
/// ..2+67...11..+3.9.6....2..5....14....9.8.9...2.7..8.36...1.+7+96.5...6+53...79..1....+6:333 377 379 287 487 887 488 888 292 392 492 393 493 893
/// ..85......4+9+2.8..72...6.9...9....2....61.+2.7+97.+2...6.4..+7+9534..9...24..5..4....9.:521 538 345 551 562 365 191 691 891 192 197 199
/// 2..7+6..3..8...5.+61..6.4.9....1.7.6....25.67+13.......5...8.1.3..9..4....6.....3.8.:512 519 731 732 239 441 442 248 461 462 463 864 964 267 869 278 778 279 582 591 592 299 599
/// 2..7+6..3..8...5.+61..6.4.9....1.7.6....25.67+13...+1...5...8.1.3..9..4....6.....3.8.:512 816 519 731 732 239 839 441 442 248 461 462 463 267 869 278 778 279 582 591 592 497 299 599
/// ..7.+85.1.8..3..5...4..7...2.....7..42..8..3..+4.6.3..2..1....7..6..+7....8+7.4.5..6.:917 931 341 941 143 759 373 573 973 974 975 376 976 478 382 485 486 988 196 296 996
/// .2.8.+16...+47..5..36..+4.3.2...25..3....4.1...7.5...4..+2..81..+49.4..+9.82...9..4...8:913 519 925 941 745 945 951 163 363 664 765 965 967 775 682 785 694
/// 5.....9.+1.1..4..2...3..6..7..56..7.......7.5.8..+532+1.6.....12..2.+18......3.+26..1.:441 941 949 451 651 951 452 453 771 971 772 773 678 682 782
/// .2......1..6+1.9.3..+1.3.49........+123....17.5.1..5..6.+7.8...5.+1.6....+13....18...7.:411 511 711 811 413 513 713 813 525 427 429 639 839 452 952 454 954 362 363 265 866 275 983 285 391 295 395 696 299
/// ]]></code>
/// </example>
/// <seealso cref="LinkType.AlmostUniqueRectangle"/>
internal sealed partial class CachedAlmostUniqueRectangleChainingRule : ChainingRule
{
	/// <inheritdoc/>
	protected internal override void CollectLinks(
		ref readonly Grid grid,
		LinkDictionary strongLinks,
		LinkDictionary weakLinks,
		LinkOption linkOption,
		LinkOption alsLinkOption
	)
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
						Type1Strong(otherDigitsMask, in urCells, ur, strongLinks, linkOption);
						break;
					}
					case 2:
					{
						Type2Strong(otherDigitsMask, in urCells, ur, strongLinks, linkOption);
						goto default;
					}
					default:
					{
						Type4Strong(otherDigitsMask, in grid, in urCells, ur, strongLinks, linkOption);
						Type5Strong(otherDigitsMask, in grid, in urCells, ur, strongLinks, linkOption);
						break;
					}
				}
			}
		}
	}

	/// <inheritdoc/>
	protected internal override void CollectExtraViewNodes(ref readonly Grid grid, ChainOrLoop pattern, ref View[] views)
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


	partial void Type1Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type2Strong(Mask otherDigitsMask, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type4Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
	partial void Type5Strong(Mask otherDigitsMask, ref readonly Grid grid, ref readonly CellMap urCells, UniqueRectangle ur, LinkDictionary linkDictionary, LinkOption linkOption);
}
