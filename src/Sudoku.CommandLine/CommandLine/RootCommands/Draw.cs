namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a draw command.
/// </summary>
[type:
	RootCommand("draw", DescriptionResourceKey = "_Description_Draw"),
	SupportedArguments("draw"),
	Usage("draw -s <size> -o <offset> -g <grid> -p <path>", IsPattern = true),
	Usage($"""draw -s 1000 -o 10 -g {SampleGrid} -p C:\Users\UserName\Desktop\output.png""", DescriptionResourceKey = "_Usage_Draw_1")]
[SupportedOSPlatform("windows")]
public sealed class Draw : IExecutable
{
	/// <summary>
	/// The size of the picture.
	/// </summary>
	[DoubleArgumentsCommand('s', "size", DescriptionResourceKey = "_Description_Size_Draw", IsRequired = true)]
	[CommandConverter<NumericConverter<float>>]
	public float Size { get; set; }

	/// <summary>
	/// The outside offset.
	/// </summary>
	[DoubleArgumentsCommand('o', "offset", DescriptionResourceKey = "_Description_OutsideOffset_Draw")]
	[CommandConverter<NumericConverter<float>>]
	public float OutsideOffset { get; set; } = 10;

	/// <summary>
	/// The extra footer.
	/// </summary>
	[DoubleArgumentsCommand('f', "footer", DescriptionResourceKey = "_Description_FooterText_Draw")]
	public string? ExtraFooter { get; set; }

	/// <summary>
	/// The output path.
	/// </summary>
	[DoubleArgumentsCommand('p', "path", DescriptionResourceKey = "_Description_OutputPath_Draw", IsRequired = true)]
	public string? OutputPath { get; set; }

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", DescriptionResourceKey = "_Description_Grid_Draw")]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; } = Grid.Empty;


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		if (OutsideOffset < 0)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgNumericValueBelowZero);
		}

		if (Grid.IsUndefined)
		{
			throw new CommandLineRuntimeException((int)ErrorCode.ArgGridIsUndefined);
		}

		var sudokuPainter = ISudokuPainter.Create((int)Size, (int)OutsideOffset)
			.WithGrid(Grid)
			.WithFooterTextIfNotNull(ExtraFooter);

		try
		{
			sudokuPainter.SaveTo(OutputPath!);

			await Terminal.WriteLineAsync(string.Format(R.MessageFormat("OutputSuccess")!, OutputPath));
		}
		catch (Exception ex)
		{
			var errorCode = ex switch
			{
				NotSupportedException => ErrorCode.ArgOutputPathExtensionNotSupported,
				ArgumentException => ErrorCode.ArgOutputPathIsInvalid
			};

			throw new CommandLineRuntimeException((int)errorCode);
		}
	}
}
