namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a solve command.
/// </summary>
[RootCommand("solve", "To solve a sudoku grid using the specified algorithm.")]
[SupportedArguments("solve")]
[Usage("solve -g <grid> -m <method>", IsPattern = true)]
[Usage($"""solve -g "{SampleGrid}" -m bitwise""", "Solves a sudoku puzzle, using the bitwise algorithm.")]
public sealed class Solve : IExecutable
{
	/// <summary>
	/// Indicates the method to be used.
	/// </summary>
	[DoubleArgumentsCommand('m', "method", "Indicates the method to be used for solving a sudoku.")]
	[CommandConverter<EnumTypeConverter<SolveAlgorithm>>]
	public SolveAlgorithm SolveMethod { get; set; } = SolveAlgorithm.Bitwise;

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", "Indicates the grid used for being solved.", IsRequired = true)]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; }


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		if (Grid.GetSolution() is not { IsUndefined: false } solution)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
		}

		foreach (var type in
			from assembly in new[] { typeof(BacktrackingSolver).Assembly, typeof(LogicalSolver).Assembly }
			from type in assembly.GetTypes()
			where type.IsClass && (type.IsAssignableTo(typeof(ISimpleSolver)) || type.IsGenericAssignableTo(typeof(IComplexSolver<,>)))
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

			var name = fieldInfo.GetCustomAttribute<NameAttribute>()!.Name;
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
					var uriLink = (string?)type.GetProperty(nameof(ISimpleSolver.UriLink))?.GetValue(null);
					await Terminal.WriteLineAsync(
						$"""
						Puzzle: {Grid:#}
						Method name used: '{name}'{(uriLink is null ? string.Empty : $"\r\nURI link: '{uriLink}'")}
						---
						Solution: {solution:!}
						"""
					);

					break;
				}
				case IComplexSolver<LogicalSolver, LogicalSolverResult> puzzleSolver:
				{
					if (puzzleSolver.Solve(Grid, cancellationToken: cancellationToken) is not { IsSolved: true } solverResult)
					{
						throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
					}

					await Terminal.WriteLineAsync(
						$"""
						Puzzle: {Grid:#}
						Method name used: '{name}'
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
	}
}
