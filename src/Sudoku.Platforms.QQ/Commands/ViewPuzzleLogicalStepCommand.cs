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

				index--;
				var grid = Grid.Parse(lines[index]);
				if (Solver.Solve(grid) is not { IsSolved: true, SolvingPath: var solvingPath })
				{
					await e.SendMessageAsync(R.MessageFormat("PuzzleHasMultipleSolutionsOrNoSolution"));
					break;
				}

				var foundStepInfo = solvingPath.FirstOrDefaultSelector(
					(pair, _) => pair.Step.Name.Contains(techniqueName),
					static (pair, i) => (pair, i)
				);
				if (foundStepInfo is not var ((stepGrid, step), stepIndex))
				{
					await e.SendMessageAsync(string.Format(R.MessageFormat("SpecifiedStepNameIsNotFound")!, techniqueName));
					break;
				}

				var picturePath = InternalReadWrite.GenerateCachedPicturePath(
					() => ISudokuPainter.Create(1000)
						.WithCanvasOffset(20)
						.WithGrid(stepGrid)
						.WithRenderingStep(step)
						.WithPreferenceSettings(static pref => pref.ShowLightHouse = false)
						.WithFontScale(1.0M, .4M)
						.WithFooterText($"@{libName} #{index + 1} [{stepIndex + 1}]")
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

				break;
			}
			default:
			{
				await e.SendMessageAsync(string.Format(R.MessageFormat("ArgFormatIsInvalid")!, CommandName));
				break;
			}
		}

		return true;
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Render the <see cref="IStep"/> instance onto the target painter.
	/// </summary>
	/// <param name="this">The <see cref="ISudokuPainter"/> instance.</param>
	/// <param name="step">The step.</param>
	/// <returns>The <see cref="ISudokuPainter"/> instance.</returns>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	[SupportedOSPlatform("windows")]
	public static ISudokuPainter WithRenderingStep(this ISudokuPainter @this, IStep step)
		=> step switch
		{
			{ Views: [var view], Conclusions: var conclusions } => @this.AddNodes(view).WithConclusions(conclusions).WithRenderingCandidates(true),
			{ Conclusions: var conclusions } => @this.WithConclusions(conclusions).WithRenderingCandidates(true)
		};

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
	public static TResult? FirstOrDefaultSelector<T, TResult>(
		this ImmutableArray<T> @this,
		Func<T, int, bool> predicate,
		Func<T, int, TResult> selector
	) where TResult : struct
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
