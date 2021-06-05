#nullable disable

#pragma warning disable CA1050

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Solving.Manual;

Console.WriteLine();

/// <summary></summary>
public sealed class TempStepSearcher : StepSearcher
{
	/// <summary></summary>
	public static TechniqueProperties Properties { get; }


	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) =>
		throw new NotImplementedException();
}