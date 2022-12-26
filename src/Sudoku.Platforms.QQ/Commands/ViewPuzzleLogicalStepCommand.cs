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


		async Task renderGridAndSendMessageAsync(Grid grid, StepInfoPredicate predicate, AsyncAction failedAction, FooterTextCreator creator)
		{
			if (Solver.Solve(grid) is not { IsSolved: true, SolvingPath: var solvingPath })
			{
				await e.SendMessageAsync(R.MessageFormat("PuzzleHasMultipleSolutionsOrNoSolution"));
				return;
			}

			var foundStepInfo = solvingPath.FirstOrDefaultSelector((pair, i) => predicate(pair, i), static (pair, i) => (pair, i));
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
					.WithPreferenceSettings(static pref => pref.ShowLightHouse = false)
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
	/// and projects it into the target value of type <typeparamref name="TResult"/>, using the specified converter <paramref name="selector"/>.
	/// </summary>
	/// <typeparam name="T">The type of each element.</typeparam>
	/// <typeparam name="TResult">The type of the projected result.</typeparam>
	/// <param name="this">The collection to be iterated.</param>
	/// <param name="predicate">The condition that the element should be satisfied.</param>
	/// <param name="selector">The converter method that projects the found element to the target one.</param>
	/// <returns>The projected value.</returns>
	public static TResult? FirstOrDefaultSelector<T, TResult>(this ImmutableArray<T> @this, PredicateWithIndex<T> predicate, ConverterWithIndex<T, TResult> selector) where TResult : struct
	{
		for (var i = 0; i < @this.Length; i++)
		{
			var element = @this[i];
			if (predicate(element, i))
			{
				return selector(element, i);
			}
		}

		return null;
	}
}

/// <summary>
/// Defines a condition that uses <see cref="Grid"/> and <see cref="IStep"/> to indicate a gathered information on a step.
/// </summary>
/// <param name="steppingPair">The pair of step information.</param>
/// <param name="stepIndex">The index of the step.</param>
/// <returns>A <see cref="bool"/> result indicating whether the condition is passed.</returns>
file delegate bool StepInfoPredicate((Grid SteppingGrid, IStep Step) steppingPair, int stepIndex);

/// <summary>
/// Defines an asynchronous action.
/// </summary>
/// <returns>The task that holds the operation of the asynchronous action.</returns>
file delegate Task AsyncAction();

/// <summary>
/// Defines a footer text creator.
/// </summary>
/// <param name="stepIndex">The step index.</param>
/// <returns>The footer text.</returns>
file delegate string FooterTextCreator(int stepIndex);

/// <summary>
/// Indicates the predicate with element's index.
/// </summary>
/// <typeparam name="T">The type of the element.</typeparam>
/// <param name="element">The element iterated.</param>
/// <param name="index">The index of the element iterated.</param>
/// <returns>A <see cref="bool"/> result indicating whether the condition is passed.</returns>
file delegate bool PredicateWithIndex<T>(T element, int index);

/// <summary>
/// Defines a converter method that uses an index.
/// </summary>
/// <typeparam name="T">The type of the base element.</typeparam>
/// <typeparam name="TResult">The type of converted and returned value.</typeparam>
/// <param name="element">The element.</param>
/// <param name="index">The index of the element in the list.</param>
/// <returns>The target element converted.</returns>
file delegate TResult ConverterWithIndex<T, TResult>(T element, int index);
