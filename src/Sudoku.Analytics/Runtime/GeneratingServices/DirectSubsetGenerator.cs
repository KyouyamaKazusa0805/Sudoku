namespace Sudoku.Runtime.GeneratingServices;

/// <summary>
/// Represents a generator that supports generating for puzzles that can be solved by only using Direct Subsets.
/// </summary>
public sealed class DirectSubsetGenerator : ComplexSingleBaseGenerator
{
	/// <inheritdoc/>
	public override TechniqueSet SupportedTechniques
		=> [
			Technique.NakedPairFullHouse, Technique.NakedPairCrosshatchingBlock, Technique.NakedPairCrosshatchingRow,
			Technique.NakedPairCrosshatchingColumn, Technique.NakedPairNakedSingle,
			Technique.NakedTripleFullHouse, Technique.NakedTripleCrosshatchingBlock, Technique.NakedTripleCrosshatchingRow,
			Technique.NakedTripleCrosshatchingColumn, Technique.NakedTripleNakedSingle,
			Technique.NakedQuadrupleFullHouse, Technique.NakedQuadrupleCrosshatchingBlock, Technique.NakedQuadrupleCrosshatchingRow,
			Technique.NakedQuadrupleCrosshatchingColumn, Technique.NakedQuadrupleNakedSingle,
			Technique.HiddenPairFullHouse, Technique.HiddenPairCrosshatchingBlock, Technique.HiddenPairCrosshatchingRow,
			Technique.HiddenPairCrosshatchingColumn, Technique.HiddenPairNakedSingle,
			Technique.HiddenTripleFullHouse, Technique.HiddenTripleCrosshatchingBlock, Technique.HiddenTripleCrosshatchingRow,
			Technique.HiddenTripleCrosshatchingColumn, Technique.HiddenTripleNakedSingle,
			Technique.HiddenQuadrupleFullHouse, Technique.HiddenQuadrupleCrosshatchingBlock, Technique.HiddenQuadrupleCrosshatchingRow,
			Technique.HiddenQuadrupleCrosshatchingColumn, Technique.HiddenQuadrupleNakedSingle,
			Technique.LockedPairFullHouse, Technique.LockedPairCrosshatchingBlock, Technique.LockedPairCrosshatchingRow,
			Technique.LockedPairCrosshatchingColumn, Technique.LockedPairNakedSingle,
			Technique.LockedTripleFullHouse, Technique.LockedTripleCrosshatchingBlock, Technique.LockedTripleCrosshatchingRow,
			Technique.LockedTripleCrosshatchingColumn, Technique.LockedTripleNakedSingle,
			Technique.LockedHiddenPairFullHouse, Technique.LockedHiddenPairCrosshatchingBlock, Technique.LockedHiddenPairCrosshatchingRow,
			Technique.LockedHiddenPairCrosshatchingColumn, Technique.LockedHiddenPairNakedSingle,
			Technique.LockedHiddenTripleFullHouse, Technique.LockedHiddenTripleCrosshatchingBlock, Technique.LockedHiddenTripleCrosshatchingRow,
			Technique.LockedHiddenTripleCrosshatchingColumn, Technique.LockedHiddenTripleNakedSingle
		];

	/// <inheritdoc/>
	protected override FuncRefReadOnly<Grid, Step, CellMap> InterimCellsCreator => LocalInterimCellCreator;

	/// <inheritdoc/>
	protected override FuncRefReadOnly<Step, bool> LocalStepFilter => static (ref readonly Step step) => step is DirectSubsetStep;


	private static CellMap LocalInterimCellCreator(ref readonly Grid g, ref readonly Step s)
	{
		var step = (DirectSubsetStep)s;
		var result = CellMap.Empty;
		switch (step.SubsetTechnique)
		{
			case Technique.LockedPair or Technique.LockedTriple:
			case Technique.NakedPair or Technique.NakedTriple or Technique.NakedQuadruple:
			{
				foreach (var cell in HousesMap[step.SubsetHouse])
				{
					if (g.GetState(cell) != CellState.Empty)
					{
						result.Add(cell);
					}
				}

				var otherDigitsMask = (Mask)(Grid.MaxCandidatesMask & ~step.SubsetDigitsMask);
				var excludersDigitsMaskFromSubsetHouse = (Mask)0;

				// Find excluders from subset house.
				foreach (var otherDigit in otherDigitsMask)
				{
					foreach (var sharedHouse in step.SubsetCells.SharedHouses)
					{
						foreach (var cell in HousesMap[sharedHouse])
						{
							if (g.GetDigit(cell) == otherDigit && (otherDigitsMask >> otherDigit & 1) != 0)
							{
								excludersDigitsMaskFromSubsetHouse |= (Mask)(1 << otherDigit);
								goto Next;
							}
						}
					}

				Next:;
				}

				// Find excluders from cells belong to each house seeing each subset cell.
				foreach (var otherDigit in otherDigitsMask)
				{
					if ((excludersDigitsMaskFromSubsetHouse >> otherDigit & 1) != 0)
					{
						foreach (var sharedHouse in step.SubsetCells.SharedHouses)
						{
							foreach (var cell in HousesMap[sharedHouse])
							{
								if (g.GetDigit(cell) == otherDigit)
								{
									result.Add(cell);
									goto Next;
								}
							}
						}

					Next:;
					}
					else
					{
						foreach (var otherCell in step.SubsetCells)
						{
							if (g.GetState(otherCell) == CellState.Empty)
							{
								foreach (var peerCell in PeersMap[otherCell])
								{
									if (g.GetDigit(peerCell) == otherDigit)
									{
										result.Add(peerCell);
										break;
									}
								}
							}
						}
					}
				}
				break;
			}
#if false
			case Technique.LockedHiddenPair or Technique.LockedHiddenTriple:
			case Technique.HiddenPair or Technique.HiddenTriple or Technique.HiddenQuadruple:
			{
				break;
			}
#endif
		}

		switch (step.BasedOn)
		{
			case Technique.FullHouse or Technique.NakedSingle:
			{
				var targetCell = step.Cell;
				var targetDigit = step.Digit;
				for (var digit = 0; digit < 9; digit++)
				{
					if (digit == targetDigit)
					{
						continue;
					}

					// Check whether the digit has already marked accroding to the former steps.
					var cellsToCheck = PeersMap[targetCell] & result;
					var digitHasAlreadyMarked = false;
					foreach (var cell in cellsToCheck)
					{
						if (g.GetDigit(cell) == digit)
						{
							digitHasAlreadyMarked = true;
							break;
						}
					}
					if (digitHasAlreadyMarked)
					{
						continue;
					}

					foreach (var cell in PeersMap[targetCell])
					{
						if (g.GetState(cell) != CellState.Empty && g.GetDigit(cell) == digit)
						{
							result.Add(cell);
							break;
						}
					}
				}
				goto default;
			}
			case var basedOn and (Technique.CrosshatchingBlock or Technique.CrosshatchingRow or Technique.CrosshatchingColumn):
			{
				var targetHouse = step.Cell.ToHouse(
					basedOn switch
					{
						Technique.CrosshatchingBlock => HouseType.Block,
						Technique.CrosshatchingRow => HouseType.Row,
						_ => HouseType.Column
					}
				);
				foreach (var cell in HousesMap[targetHouse])
				{
					if (g.GetState(cell) != CellState.Empty)
					{
						result.Add(cell);
					}
				}
				goto default;
			}
			default:
			{
				foreach (var cell in HousesMap[step.SubsetHouse])
				{
					if (g.GetState(cell) != CellState.Empty)
					{
						result.Add(cell);
					}
				}
				foreach (var node in s.Views![0])
				{
					if (node is CircleViewNode { Cell: var cell } && g.GetState(cell) != CellState.Empty)
					{
						result.Add(cell);
					}
				}
				break;
			}
		}
		return result;
	}
}
