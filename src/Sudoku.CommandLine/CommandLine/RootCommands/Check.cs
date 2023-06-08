namespace Sudoku.CommandLine.RootCommands;

/// <summary>
/// Represents a check command.
/// </summary>
[RootCommand("check", DescriptionResourceKey = "_Description_Check")]
[SupportedArguments("check")]
[Usage("check -g <grid> -t <type>", IsPattern = true)]
[Usage($"""check -g "{SampleGrid}" -t validity""", DescriptionResourceKey = "_Usage_Check_1")]
public sealed class Check : IExecutable
{
	/// <summary>
	/// Indicates the check type.
	/// </summary>
	[DoubleArgumentsCommand('t', "type", DescriptionResourceKey = "_Description_CheckType_Check")]
	[CommandConverter<EnumTypeConverter<CheckType>>]
	public CheckType CheckType { get; set; } = CheckType.Validity;

	/// <summary>
	/// Indicates the grid used.
	/// </summary>
	[DoubleArgumentsCommand('g', "grid", DescriptionResourceKey = "_Description_Grid_Check", IsRequired = true)]
	[CommandConverter<GridConverter>]
	public Grid Grid { get; set; }


	/// <inheritdoc/>
	public async Task ExecuteAsync(CancellationToken cancellationToken = default)
	{
		switch (CheckType)
		{
			case CheckType.Validity:
			{
				await Terminal.WriteLineAsync(
					string.Format(
						GetString("_MessageFormat_CheckValidityResult")!,
						Grid.ToString("#"),
						(Grid.IsValid ? GetString("_MessageFormat_Has") : GetString("_MessageFormat_DoesNotHave"))!
					)
				);

				return;
			}
			default:
			{
				throw new CommandLineRuntimeException((int)ErrorCode.ArgAttributeNameIsInvalid);
			}
		}
	}
}
