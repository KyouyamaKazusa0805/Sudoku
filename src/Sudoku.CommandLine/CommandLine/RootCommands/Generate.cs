namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a generate command.
/// </summary>
[RootCommand("generate", DescriptionResourceKey = "_Description_Generate")]
[SupportedArguments("generate", "gen")]
[Usage("generate -m <method> -c <range>", IsPattern = true)]
[Usage("generate -m hard -c 24..30", DescriptionResourceKey = "_Usage_Generate_1")]
public sealed class Generate : IExecutable
{
	/// <summary>
	/// Indicates the range of givens that generated puzzle should be.
	/// </summary>
	[DoubleArgumentsCommand('c', "count", DescriptionResourceKey = "_Description_Range_Generate")]
	[CommandConverter<CellCountRangeConverter>]
	public (int Min, int Max) Range { get; set; } = (24, 30);

	/// <summary>
	/// Indicates the algorithm to generate the puzzle.
	/// </summary>
	[DoubleArgumentsCommand('m', "method", DescriptionResourceKey = "_Description_GenerateType_Generate")]
	[CommandConverter<EnumTypeConverter<GenerateType>>]
	public GenerateType GenerateType { get; set; } = GenerateType.HardPatternLike;


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		switch (GenerateType)
		{
			case GenerateType.HardPatternLike:
			{
				var generator = new HardLikePuzzleGenerator();
				while (true)
				{
					var targetPuzzle = generator.Generate(cancellationToken);
					var c = targetPuzzle.GivensCount;
					if (c < Range.Min || c >= Range.Max)
					{
						continue;
					}

					await Terminal.WriteLineAsync(string.Format(R["_MessageFormat_GeneratedPuzzleIs"]!, targetPuzzle.ToString("0")));

					return;
				}
			}
			case GenerateType.PatternBased:
			{
				var generator = new PatternBasedPuzzleGenerator();
				while (true)
				{
					var targetPuzzle = generator.Generate(cancellationToken);
					var c = targetPuzzle.GivensCount;
					if (c < Range.Min || c >= Range.Max)
					{
						continue;
					}

					await Terminal.WriteLineAsync(string.Format(R["_MessageFormat_GeneratedPuzzleIs"]!, targetPuzzle.ToString("0")));

					return;
				}
			}
			default:
			{
				throw new CommandLineRuntimeException((int)ErrorCode.MethodIsInvalid);
			}
		}
	}
}
