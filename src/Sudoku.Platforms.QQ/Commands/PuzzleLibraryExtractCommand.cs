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
			_ => InternalReadWrite.ReadLibrary(groupId, args) switch
			{
				{ Name: var name, PuzzleFilePath: var path } => await extractPuzzleAsync(name, path, groupId),
				_ => await sendLibraryNotFoundMessageAsync()
			}
		};


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

		async Task<bool> extractPuzzleAsync(string name, string path, string groupId)
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

			var index = lib.FinishedPuzzlesCount;
			if (index >= lines.Length)
			{
				goto PuzzleLibraryIsAllFinished;
			}

			var grid = Grid.Parse(lines[index]);
			if (Solver.Solve(grid) is not { IsSolved: true, DifficultyLevel: var diffLevel, SolvingStepsCount: var stepsCount } analysisResult)
			{
				goto PuzzleIsBroken;
			}

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

			lib.FinishedPuzzlesCount++;

			InternalReadWrite.WriteLibraryConfiguration(libs);

			return true;

		PuzzleIsBroken:
			await e.SendMessageAsync(R.MessageFormat("PuzzleLibraryIsBroken")!);
			return true;

		PuzzleLibraryIsAllFinished:
			await e.SendMessageAsync(R.MessageFormat("CurrentLibIsFullyFinished")!);
			return true;
		}
	}
}
