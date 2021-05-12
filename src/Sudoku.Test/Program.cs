#pragma warning disable CA1050

using System;
using System.Collections.Generic;
using Sudoku.Data;
using Sudoku.Resources;
using Sudoku.Solving.Manual;

Console.WriteLine("Hello, world!");

Console.WriteLine((string)TextResources.Current.Hello);

/// <summary>
/// Default
/// </summary>
public sealed class StepSearcher1 : StepSearcher
{
	/// <summary>
	/// Default
	/// </summary>
	public static TechniqueProperties Properties { get; } = new(20, "");

	/// <inheritdoc/>
	public override void GetAll(IList<StepInfo> accumulator, in SudokuGrid grid) => throw new NotImplementedException();
}