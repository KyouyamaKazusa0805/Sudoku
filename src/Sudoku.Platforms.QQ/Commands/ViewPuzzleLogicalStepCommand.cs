namespace Sudoku.Platforms.QQ.Commands;

/// <summary>
/// Defines a view puzzle logical step command.
/// </summary>
[Command]
[SupportedOSPlatform("windows")]
file sealed class ViewPuzzleLogicalStepCommand : Command
{
	/// <inheritdoc/>
	public override string CommandName => R.Command("ViewStep")!;

	/// <inheritdoc/>
	public override CommandComparison ComparisonMode => CommandComparison.Prefix;


	/// <inheritdoc/>
	protected override async Task<bool> ExecuteCoreAsync(string args, GroupMessageReceiver e)
	{
		var groupId = e.GroupId;
		switch (args.Split(' ', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries))
		{
			case [var puzzleKeyword, var puzzleStr, var techniqueName] when puzzleKeyword == R.CommandSegment("Puzzle")!:
			{
				if (!Grid.TryParse(puzzleStr, out var puzzle))
				{
					await e.SendMessageAsync(R.MessageFormat("PuzzleStrIsInvalid")!);
					break;
				}

				await renderGridAndSendMessageAsync(
					Grid.Parse(puzzleStr),
					(pair, _) => pair.Step.Name.Contains(techniqueName),
					async () => await e.SendMessageAsync(string.Format(R.MessageFormat("SpecifiedStepNameIsNotFound")!, techniqueName)),
					static stepIndex => $"{R["SpecifiedPuzzle"]!} [{stepIndex + 1}]"
				);

				break;
			}
			case [var puzzleLibKeyword, var libName, var numberLabel, var techniqueName] when puzzleLibKeyword == R.CommandSegment("PuzzleLib")!:
			{
				if (InternalReadWrite.ReadLibrary(groupId, libName) is not { PuzzleFilePath: var filePath })
				{
					await e.SendMessageAsync(R.MessageFormat("NoSpecifiedLibExists"));
					break;
				}

				if (!int.TryParse(numberLabel.TrimStart('#', '\uff03'), out var index))
				{
					await e.SendMessageAsync(R.MessageFormat("NumberIsInvalid"));
					break;
				}

				var lines = (
					from line in await File.ReadAllLinesAsync(filePath)
					where !string.IsNullOrWhiteSpace(line) && Grid.TryParse(line, out _)
					select line
				).ToArray();
				if (lines.Length == 0)
				{
					await e.SendMessageAsync(R.MessageFormat("PuzzleLibraryIsBroken")!);
					break;
				}

				if (index <= 0 || index > lines.Length)
				{
					await e.SendMessageAsync(R.MessageFormat("SpecifiedPuzzleLibIndexIsOutOfRange"));
					break;
				}

				await renderGridAndSendMessageAsync(
					Grid.Parse(lines[index - 1]),
					(pair, _) => pair.Step.Name.Contains(techniqueName),
					async () => await e.SendMessageAsync(string.Format(R.MessageFormat("SpecifiedStepNameIsNotFound")!, techniqueName)),
					stepIndex => $"@{libName} #{index + 1} [{stepIndex + 1}]"
				);

				break;
			}
			default:
			{
				await e.SendMessageAsync(string.Format(R.MessageFormat("ArgFormatIsInvalid")!, CommandName));
				break;
			}
		}

		return true;


		async Task renderGridAndSendMessageAsync(
			Grid grid,
			Func<(Grid SteppingGrid, IStep Step), int, bool> predicate,
			Func<Task> failedAction,
			Func<int, string> creator
		)
		{
			if (Solver.Solve(grid) is not { IsSolved: true, SolvingPath: var solvingPath })
			{
				await e.SendMessageAsync(R.MessageFormat("PuzzleHasMultipleSolutionsOrNoSolution"));
				return;
			}

			var foundStepInfo = solvingPath.FirstOrDefaultConvert(predicate, static (pair, i) => (pair, i));
			if (foundStepInfo is not var ((stepGrid, step), stepIndex))
			{
				await failedAction();
				return;
			}

			var picturePath = InternalReadWrite.GenerateCachedPicturePath(
				() => ISudokuPainter.Create(1000)
					.WithCanvasOffset(20)
					.WithGrid(stepGrid)
					.WithRenderingStep(step)
					.WithFontScale(1.0M, .4M)
					.WithFooterText(creator(stepIndex))
			)!;

			await e.SendMessageAsync(new ImageMessage { Path = picturePath });
			await Task.Delay(2.Seconds());
			await e.SendMessageAsync(
				new MessageChainBuilder()
					.Plain(R["StepInfoIs"]!)
					.Plain(Environment.NewLine)
					.Plain(step.ToString())
					.Build()
			);

			File.Delete(picturePath);
		}
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Try to find the first element that satisfies the specified condition <paramref name="predicate"/>,
	/// and projects it into the target value of type <typeparamref name="TResult"/>, using the specified converter <paramref name="converter"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <typeparam name="TResult">The type of the projected result.</typeparam>
	/// <param name="this">The collection to be iterated.</param>
	/// <param name="predicate">The condition that the element should be satisfied.</param>
	/// <param name="converter">The converting method that projects the found element to the target one.</param>
	/// <returns>The projected value.</returns>
	public static TResult? FirstOrDefaultConvert<T, TResult>(
		this ImmutableArray<T> @this,
		Func<T, int, bool> predicate,
		Func<T, int, TResult> converter
	) where TResult : struct
	{
		for (var i = 0; i < @this.Length; i++)
		{
			var element = @this[i];
			if (predicate(element, i))
			{
				return converter(element, i);
			}
		}

		return null;
	}
}
