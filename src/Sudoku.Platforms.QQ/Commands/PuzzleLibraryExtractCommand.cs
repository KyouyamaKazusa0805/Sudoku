namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a puzzle library extract command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class PuzzleLibraryExtractCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("ExtractPuzzle")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		if (e is not { GroupId: var groupId })
		{
			return true;
		}

		await e.SendMessageAsync(R.MessageFormat("AnalyzePuzzleLibrary")!);

		return args switch
		{
			[] => InternalReadWrite.ReadLibraries(groupId) switch
			{
				null or [] => await sendLibraryNullOrEmptyMessageAsync(),
				[{ Name: var name, PuzzleFilePath: var path }] => await extractPuzzleAsync(name, path, groupId),
				_ => await sendLibraryNotUniqueMessageAsync()
			},
			_ => args.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries) switch
			{
				[var libName] => InternalReadWrite.ReadLibrary(groupId, libName) switch
				{
					{ Name: var name, PuzzleFilePath: var path } => await extractPuzzleAsync(name, path, groupId),
					_ => await sendLibraryNotFoundMessageAsync()
				},
				[var groupNameOrId, var libName, var numberLabel] => await getMatchedGroups(groupNameOrId) switch
				{
					[] => await sendGroupNameOrIdNotFoundMessageAsync(),
					[{ Id: var targetGroupId }] => InternalReadWrite.ReadLibrary(targetGroupId, libName) switch
					{
						{ Name: var name, PuzzleFilePath: var path } => int.TryParse(numberLabel.TrimStart('\uff03', '#'), out var index) switch
						{
							true => await extractPuzzleAsync(name, path, targetGroupId, index - 1),
							_ => await sendNumberLabelIsNotNumberMessageAsync()
						},
						_ => await sendLibraryNotFoundMessageAsync()
					},
					{ Length: > 1 } => await sendAmbiguousMatchedGroupsMessageAsync(),
				},
				_ => await sendArgFormatInvalidMessageAsync()
			}
		};


		static async Task<Group[]> getMatchedGroups(string groupNameOrId)
			=> (
				from @group in await AccountManager.GetGroupsAsync()
				where groupNameOrId switch
				{
					['Q' or 'q', '\uff1a' or ':', .. var expr] => @group.Id == groupNameOrId,
					['N' or 'n', '\uff1a' or ':', .. var expr] => @group.Name == groupNameOrId,
					_ => new[] { @group.Name, @group.Id }.Any(e => e == groupNameOrId)
				}
				select @group
			).ToArray();

		async Task<bool> sendArgFormatInvalidMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(string.Format(R.MessageFormat("ArgFormatIsInvalid")!, CommandName));
			return true;
		}

		async Task<bool> sendGroupNameOrIdNotFoundMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R.MessageFormat("SpecifiedGroupNameOrIdNotFound"));
			return true;
		}

		async Task<bool> sendNumberLabelIsNotNumberMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R.MessageFormat("NumberIsInvalid"));
			return true;
		}

		async Task<bool> sendAmbiguousMatchedGroupsMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R.MessageFormat("AmbiguousGroupNameOrIdFound"));
			return true;
		}

		async Task<bool> sendLibraryNullOrEmptyMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R.MessageFormat("PuzzleLibraryIsNullOrEmpty")!);
			return true;
		}

		async Task<bool> sendLibraryNotUniqueMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(R.MessageFormat("PuzzleLibraryIsNotUnique")!);
			return true;
		}

		async Task<bool> sendLibraryNotFoundMessageAsync()
		{
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(string.Format(R.MessageFormat("PuzzleLibraryIsNotFound")!, args));
			return true;
		}

		async Task<bool> extractPuzzleAsync(string name, string path, string groupId, int? specifiedNumber = null)
		{
			var lines = (
				from line in await File.ReadAllLinesAsync(path)
				where !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _)
				select line
			).ToArray();
			if (lines.Length == 0)
			{
				goto PuzzleIsBroken;
			}

			var libs = InternalReadWrite.ReadLibraryConfiguration(groupId);
			if (libs is null)
			{
				goto PuzzleIsBroken;
			}

			var lib = libs.PuzzleLibraries.FirstOrDefault(lib => lib.Name == name);
			if (lib is null)
			{
				goto PuzzleIsBroken;
			}

			var index = specifiedNumber switch { { } l => l, _ => lib.FinishedPuzzlesCount };
			if (specifiedNumber is null && index >= lines.Length)
			{
				goto PuzzleLibraryIsAllFinished;
			}

			if (specifiedNumber is { } specifiedIndex && (specifiedIndex < 0 || specifiedIndex >= lines.Length))
			{
				goto SpecifiedPuzzleLibraryIndexIsOutOfRange;
			}

			var grid = Grid.Parse(lines[index]);
			switch (Solver.Solve(grid))
			{
				case { UnhandledException: WrongStepException { CurrentInvalidGrid: var currentGrid, WrongStep: var wrongStep } }:
				{
					await e.SendMessageAsync(string.Format(R.MessageFormat("WrongStepEncountered")!, wrongStep.ToString()));
					await Task.Delay(2.Seconds());

					var picturePath = InternalReadWrite.GenerateCachedPicturePath(
						() => ISudokuPainter.Create(1000)
							.WithCanvasOffset(20)
							.WithGrid(currentGrid)
							.WithRenderingStep(wrongStep)
							.WithFontScale(1.0M, .4M)
					)!;

					await e.SendMessageAsync(new ImageMessage { Path = picturePath });

					File.Delete(picturePath);

					break;
				}
				case { IsSolved: true, DifficultyLevel: var diffLevel, SolvingStepsCount: var stepsCount } analysisResult:
				{
					GridAutoFiller.Fill(ref grid);

					var comma = R.Token("Comma")!;
					var picturePath = InternalReadWrite.GenerateCachedPicturePath(
						() => ISudokuPainter.Create(1000)
							.WithCanvasOffset(20)
							.WithGrid(grid)
							.WithRenderingCandidates(diffLevel >= DifficultyLevel.Hard)
							.WithFontScale(1.0M, .4M)
							.WithFooterText($"@{name} #{index + 1}")
					)!;

					const string separator = "---";
					await e.SendMessageAsync(new ImageMessage { Path = picturePath });
					await Task.Delay(3.Seconds());
					await e.SendMessageAsync(
						new MessageChainBuilder()
							.Plain(R["AnalysisResultIs"]!)
							.Plain(Environment.NewLine)
							.Plain(separator)
							.Plain(Environment.NewLine)
							.Plain($"{R["LibraryNameIs"]!}{name}")
							.Plain(Environment.NewLine)
							.Plain($"{R["PuzzleLibraryIndexIs"]!}#{index + 1}")
							.Plain(Environment.NewLine)
							.Plain(separator)
							.Plain(Environment.NewLine)
							.Plain(analysisResult.ToString(SolverResultFormattingOptions.ShowElapsedTime))
							.Build()
					);

					File.Delete(picturePath);

					if (specifiedNumber is null)
					{
						lib.FinishedPuzzlesCount++;

						InternalReadWrite.WriteLibraryConfiguration(libs);
					}

					break;
				}
				default:
				{
					goto PuzzleIsBroken;
				}
			}

			return true;

		PuzzleIsBroken:
			await e.SendMessageAsync(R.MessageFormat("PuzzleLibraryIsBroken")!);
			return true;

		PuzzleLibraryIsAllFinished:
			await e.SendMessageAsync(R.MessageFormat("CurrentLibIsFullyFinished")!);
			return true;

		SpecifiedPuzzleLibraryIndexIsOutOfRange:
			await e.SendMessageAsync(R.MessageFormat("SpecifiedPuzzleLibIndexIsOutOfRange"));
			return true;
		}
	}
}
