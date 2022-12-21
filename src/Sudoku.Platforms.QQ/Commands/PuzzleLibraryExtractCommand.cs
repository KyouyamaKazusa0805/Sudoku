namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a puzzle library extract command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class PuzzleLibraryExtractCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.CommandName("ExtractPuzzle")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { GroupId: var groupId })
		{
			return true;
		}

		await e.SendMessageAsync(R["_MessageFormat_AnalyzePuzzleLibrary"]!);

		return args switch
		{
			[] => InternalReadWrite.ReadLibraries(groupId) switch
			{
				null or [] => await sendLibraryNullOrEmptyMessageAsync(),
				[{ Name: var name, PuzzleFilePath: var path }] => await extractPuzzleAsync(name, path),
				_ => await sendLibraryNotUniqueMessageAsync()
			},
			_ => InternalReadWrite.ReadLibrary(groupId, args) switch
			{
				{ Name: var name, PuzzleFilePath: var path } => await extractPuzzleAsync(name, path),
				_ => await sendLibraryNotFoundMessageAsync()
			}
		};


		async Task<bool> sendLibraryNullOrEmptyMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R["_MessageFormat_PuzzleLibraryIsNullOrEmpty"]!);
			return true;
		}

		async Task<bool> sendLibraryNotUniqueMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R["_MessageFormat_PuzzleLibraryIsNotUnique"]!);
			return true;
		}

		async Task<bool> sendLibraryNotFoundMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(string.Format(R["_MessageFormat_PuzzleLibraryIsNotFound"]!, args));
			return true;
		}

		async Task<bool> extractPuzzleAsync(string name, string path)
		{
			var lines = await File.ReadAllLinesAsync(path);
			if (lines.Length == 0)
			{
				goto PuzzleIsBroken;
			}

			var gridCode = Rng.NextInArray(lines, out var index);
			if (Grid.TryParse(gridCode, out var grid))
			{
				if (Solver.Solve(grid) is
					{
						IsSolved: true,
						MaxDifficulty: var diff,
						TotalDifficulty: var total,
						SolvingStepsCount: var stepsCount
					} analysisResult)
				{
					var comma = R["_Token_Comma"]!;
					var footerText = $"@{name} #{index + 1}{comma}{R["DiffRatingIs"]!} {diff:0.0}{comma}{R["TotalDiffRatingIs"]!}{total:0.0}";
					var picturePath = InternalReadWrite.GenerateCachedPicturePath(
						() => ISudokuPainter.Create(1000)
							.WithGrid(grid)
							.WithRenderingCandidates(false)
							.WithFooterText(footerText)
					)!;

					await e.SendMessageAsync(new ImageMessage { Path = picturePath });
					await Task.Delay(3.Seconds());
					await e.SendMessageAsync(
						new MessageChainBuilder()
							.Plain(R["AnalysisResultIs"]!)
							.Plain(Environment.NewLine)
							.Plain("---")
							.Plain(Environment.NewLine)
							.Plain($"{R["LibraryNameIs"]!}{name}")
							.Plain(Environment.NewLine)
							.Plain($"{R["PuzzleLibraryIndexIs"]!}#{index + 1}")
							.Plain(Environment.NewLine)
							.Plain("---")
							.Plain(Environment.NewLine)
							.Plain(analysisResult.ToString(SolverResultFormattingOptions.None))
							.Build()
					);

					// TODO: Update finished table, avoiding the puzzle to be re-extracted.

					File.Delete(picturePath);
				}
				else
				{
					goto PuzzleIsBroken;
				}
			}
			else
			{
				goto PuzzleIsBroken;
			}

			return true;

		PuzzleIsBroken:
			await e.SendMessageAsync(R["_MessageFormat_PuzzleLibraryIsBroken"]!);
			return true;
		}
	}
}
