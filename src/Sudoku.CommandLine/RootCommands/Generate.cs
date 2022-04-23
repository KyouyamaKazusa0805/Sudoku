namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a generate command.
/// </summary>
[RootCommand("generate", "To generate a sudoku puzzle.")]
[SupportedArguments(new[] { "generate", "gen" })]
[Usage("generate -m <method> -c <range>", IsFact = true)]
[Usage("""generate -m hard -c 24..30""", "Generates a sudoku puzzle, which contains givens of number between 24 and 30, and using the hard-pattern algorithm to generate puzzle.")]
public sealed class Generate : IRootCommand
{
	/// <summary>
	/// Indicates the range of givens that generated puzzle should be.
	/// </summary>
	[Command('c', "count", "The range of given cells that generated puzzle should be.")]
	[CommandConverter(typeof(CellCountRangeConverter))]
	public (int Min, int Max) Range { get; set; } = (24, 30);

	/// <summary>
	/// Indicates the algorithm to generate the puzzle.
	/// </summary>
	[Command('m', "method", "The method that defines what algorithm used for generating a sudoku puzzle.")]
	[CommandConverter(typeof(EnumTypeConverter<GenerateType>))]
	public GenerateType GenerateType { get; set; } = GenerateType.HardPatternLike;


	/// <inheritdoc/>
	public void Execute()
	{
		switch (GenerateType)
		{
			case GenerateType.HardPatternLike:
			{
				var generator = new HardPatternPuzzleGenerator();
				while (true)
				{
					var targetPuzzle = generator.Generate();
					int c = targetPuzzle.GivensCount;
					if (c < Range.Min || c >= Range.Max)
					{
						continue;
					}

					Terminal.WriteLine($"""The puzzle generated: '{targetPuzzle:0}'""");

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
