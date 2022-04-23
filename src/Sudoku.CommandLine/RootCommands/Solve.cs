namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a solve command.
/// </summary>
[RootCommand("solve", "To solve a sudoku grid using the specified algorithm.")]
[SupportedArguments(new[] { "solve" })]
[Usage("solve -g <grid> -m <method>", IsFact = true)]
[Usage($"""solve -g "{SampleGrid}" -m bitwise""", "Solves a sudoku puzzle, using the bitwise algorithm.")]
public sealed class Solve : IExecutable
{
	/// <summary>
	/// Indicates the method to be used.
	/// </summary>
	[Command('m', "method", "Indicates the method to be used for solving a sudoku.")]
	[CommandConverter(typeof(EnumTypeConverter<SolveAlgorithm>))]
	public SolveAlgorithm SolveMethod { get; set; } = SolveAlgorithm.Bitwise;

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[Command('g', "grid", "Indicates the grid used for being solved.", IsRequired = true)]
	[CommandConverter(typeof(GridConverter))]
	public Grid Grid { get; set; }


	/// <inheritdoc/>
	public void Execute()
	{
		if (Grid.Solution is not { IsUndefined: false } solution)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
		}

		string? methodNameUsed = null;
		foreach (var type in
			from type in typeof(ISimpleSolver).Assembly.GetTypes()
			where type.IsClass && type.IsAssignableTo(typeof(ISimpleSolver))
			let parameterlessConstructor = type.GetConstructor(Array.Empty<Type>())
			where parameterlessConstructor is not null
			select type)
		{
			var fieldInfo = typeof(SolveAlgorithm).GetField(SolveMethod.ToString())!;
			var actualType = fieldInfo.GetCustomAttribute<RouteToTypeAttribute>()!.TypeToRoute;
			if (actualType != type)
			{
				continue;
			}

			string? uriLink = (string?)type.GetProperty(nameof(ISimpleSolver.UriLink))!.GetValue(null);
			string name = fieldInfo.GetCustomAttribute<NameAttribute>()!.Name;
			switch (Activator.CreateInstance(type))
			{
				case ISimpleSolver simpleSolver:
				{
					if (simpleSolver.Solve(Grid, out _) is not true)
					{
						throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
					}

					// .NET Runtime issue: If the type does not implement 'IFormattable',
					// the format string is meaningless to be used in the interpolated string holes.
					// In this invocation, type 'Grid' does not implement the type 'IFormattable',
					// therefore, we cannot use the interpolated string syntax like '$"{grid:#}"'
					// to get the same result as 'grid.ToString("#")'; on contrast, 'grid.ToString("#")'
					// as expected will be replaced with 'grid.ToString()'.
					// Same reason for the below output case.
					string uriLinkRealStr = uriLink is null ? string.Empty : $"\r\nURI link: '{uriLink}'";
					Terminal.WriteLine(
						$"""
						Puzzle: {Grid:#}
						Method name used: '{name}'{uriLinkRealStr}
						---
						Solution: {solution:!}
						"""
					);

					break;
				}
				case IComplexSolver<ManualSolverResult> puzzleSolver:
				{
					if (puzzleSolver.Solve(Grid) is not { IsSolved: true } solverResult)
					{
						throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
					}

					Terminal.WriteLine(
						$"""
						Puzzle: {Grid:#}
						Method name used: '{methodNameUsed}'
						---
						Solution: {solution:!}
						Solving details:
						{solverResult}
						"""
					);

					break;
				}
			}
		}

		if (methodNameUsed is null)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgMethodIsInvalid);
		}
	}
}
