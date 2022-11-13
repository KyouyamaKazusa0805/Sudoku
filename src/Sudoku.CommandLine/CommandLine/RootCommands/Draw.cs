namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a draw command.
/// </summary>
[SupportedOSPlatform("windows")]
[RootCommand("draw", "Draw a sudoku grid onto a picture.")]
[SupportedArguments("draw")]
[Usage("draw -s <size> -o <offset> -g <grid> -p <path>", "Draw a grid onto a picture, with specified size and outside blank (pixels), then output the picture to the local path.")]
public sealed class Draw : IExecutable
{
	/// <summary>
	/// The size of the picture.
	/// </summary>
	[DoubleArgumentsCommand('s', "size", "Indicates the size of the picture.", IsRequired = true)]
	[CommandConverter<NumericConverter<float>>]
	public float Size { get; set; }

	/// <summary>
	/// The outside offset.
	/// </summary>
	[DoubleArgumentsCommand('o', "offset", "Indicates the blank between the grid lines and the picture border.")]
	[CommandConverter<NumericConverter<float>>]
	public float OutsideOffset { get; set; } = 10;

	/// <summary>
	/// The extra footer.
	/// </summary>
	[DoubleArgumentsCommand('f', "footer", "Indicates the extra footer text displayed below the picture.")]
	public string? ExtraFooter { get; set; }

	/// <summary>
	/// The output path.
	/// </summary>
	[DoubleArgumentsCommand('p', "path", "Indicates the output path.", IsRequired = true)]
	public string? OutputPath { get; set; }

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", "Indicates the sudoku grid as string representation.")]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; } = Grid.Empty;


	/// <inheritdoc/>
	public void Execute()
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

			Terminal.WriteLine($"Success. Please visit the path '{OutputPath}' to view the file.");
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
