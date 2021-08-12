namespace Sudoku.Solving.Manual.Uniqueness.Rects
{
	partial class UrStepSearcher
	{
		partial void CheckType1(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void CheckType2(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType3Naked(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType3Hidden(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType4(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckType5(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void CheckType6(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckHidden(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check2D(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check2B1SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check2D1SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check3X(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3X2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3N2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3U2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check3E2SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int cornerCell, in Cells otherCellsMap, int index);
		partial void Check4X3SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void Check4C3SL(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckWing(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int size, int index);
		partial void CheckSdc(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, bool arMode, short comparer, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
		partial void CheckUnknownCoveringUnique(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, short comparer, int d1, int d2, int index);
		partial void CheckGuardianUnique(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, short comparer, int d1, int d2, int index);
		partial void CheckGuardianAvoidable(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, short comparer, int d1, int d2, int index);
		partial void CheckHiddenSingleAvoidable(IList<UrStepInfo> accumulator, in SudokuGrid grid, int[] urCells, int d1, int d2, int corner1, int corner2, in Cells otherCellsMap, int index);
	}
}
