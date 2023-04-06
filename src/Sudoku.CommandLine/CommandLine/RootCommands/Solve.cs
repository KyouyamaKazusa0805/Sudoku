namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a solve command.
/// </summary>
[RootCommand("solve", DescriptionResourceKey = "_Description_Solve")]
[SupportedArguments("solve")]
[Usage("solve -g <grid> -m <method>", IsPattern = true)]
[Usage($"""solve -g "{SampleGrid}" -m bitwise""", DescriptionResourceKey = "_Usage_Solve_1")]
public sealed class Solve : IExecutable
{
	/// <summary>
	/// Indicates the method to be used.
	/// </summary>
	[DoubleArgumentsCommand('m', "method", DescriptionResourceKey = "_Description_SolveMethod_Solve")]
	[CommandConverter<EnumTypeConverter<SolveAlgorithm>>]
	public SolveAlgorithm SolveMethod { get; set; } = SolveAlgorithm.Bitwise;

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", DescriptionResourceKey = "_Description_Grid_Solve", IsRequired = true)]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; }


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		if (Grid.SolutionGrid is not { IsUndefined: false } solution)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
		}

		foreach (var type in
			from assembly in new[] { typeof(BacktrackingSolver).Assembly, typeof(Analyzer).Assembly }
			from type in assembly.GetTypes()
			where type.IsClass && (type.IsAssignableTo(typeof(ISolver)) || type.IsGenericAssignableTo(typeof(IAnalyzer<,>)))
			let parameterlessConstructor = type.GetConstructor(Type.EmptyTypes)
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
				case ISolver simpleSolver:
				{
					if (simpleSolver.Solve(Grid, out _) is not true)
					{
						throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
					}

#if false
					// .NET Runtime issue: If the type does not implement 'IFormattable',
					// the format string is meaningless to be used in the interpolated string holes.
					// In this invocation, type 'Grid' does not implement the type 'IFormattable',
					// therefore, we cannot use the interpolated string syntax like '$"{grid:#}"'
					// to get the same result as 'grid.ToString("#")'; on contrast, 'grid.ToString("#")'
					// as expected will be replaced with 'grid.ToString()'.
					// Same reason for the below output case.
#endif
					var uriLink = (string?)type.GetProperty(nameof(ISolver.UriLink))?.GetValue(null);
					await Terminal.WriteLineAsync(
						string.Format(
							R["_MessageFormat_SolveResult"]!,
							Grid.ToString("#"),
							name,
							uriLink is null ? string.Empty : $"\r\n{R["_MessageFormat_UriLinkIs"]!} {uriLink}",
							solution.ToString("!")
						)
					);

					break;
				}
				case IAnalyzer<Analyzer, AnalyzerResult> puzzleSolver:
				{
					if (puzzleSolver.Analyze(Grid, cancellationToken: cancellationToken) is not { IsSolved: true } solverResult)
					{
						throw new CommandLineRuntimeException((int)ErrorCode.ArgGridValueIsNotUnique);
					}

					await Terminal.WriteLineAsync(
						string.Format(
							R["_MessageFormat_AnalysisResult"]!,
							Grid.ToString("#"),
							name,
							solution.ToString("!"),
							solverResult.ToString()
						)
					);

					break;
				}
			}
		}
	}
}
