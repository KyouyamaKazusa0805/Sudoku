namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the generating operation command bar.
/// </summary>
public sealed partial class GeneratingOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// The fields for core usages on counting puzzles.
	/// </summary>
	private int _generatingCount, _generatingFilteredCount;


	/// <summary>
	/// Initializes a <see cref="GeneratingOperation"/> instance.
	/// </summary>
	public GeneratingOperation() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <inheritdoc/>
	protected override void OnNavigatedTo(NavigationEventArgs e)
	{
		if (e.Parameter is AnalyzePage p)
		{
			SetGeneratingStrategyTooltip(p);
			RefreshPuzzleLibraryComboBox();
		}
	}

	/// <summary>
	/// Set generating strategy tooltip.
	/// </summary>
	/// <param name="basePage">The base page.</param>
	private void SetGeneratingStrategyTooltip(AnalyzePage basePage)
	{
		var constraints = ((App)Application.Current).Preference.ConstraintPreferences.Constraints;
		TextBlockBindable.SetInlines(
			GeneratorStrategyTooltip,
			[new Run { Text = string.Join(Environment.NewLine, from c in constraints select c.ToString(App.CurrentCulture)) }]
		);
	}

	/// <summary>
	/// Refreshes puzzle library for combo box.
	/// </summary>
	private void RefreshPuzzleLibraryComboBox()
	{
		var libs = LibrarySimpleBindableSource.GetLibraries();
		(PuzzleLibraryChoser.Visibility, LibraryPuzzleFetchButton.Visibility, LibSeparator.Visibility) = libs.Length != 0
			? (Visibility.Visible, Visibility.Visible, Visibility.Visible)
			: (Visibility.Collapsed, Visibility.Collapsed, Visibility.Collapsed);
		PuzzleLibraryChoser.ItemsSource = (from lib in libs select new LibrarySimpleBindableSource(lib)).ToArray();

		var lastFileId = ((App)Application.Current).Preference.UIPreferences.FetchingPuzzleLibrary;
		PuzzleLibraryChoser.SelectedIndex = Array.FindIndex(libs, match) is var index and not -1 ? index : 0;


		bool match(LibraryInfo lib) => lib is { FileId: var f } && f == lastFileId;
	}

	/// <summary>
	/// Handle generating operation.
	/// </summary>
	/// <typeparam name="T">The type of the progress data provider.</typeparam>
	/// <param name="onlyGenerateOne">Indicates whether the generator engine only generates for one puzzle.</param>
	/// <param name="gridStateChanger">
	/// The method that can change the state of the target grid. This callback method will be used for specify the grid state
	/// when a user has set the techniques that must be appeared.
	/// </param>
	/// <param name="gridTextConsumer">An action that consumes the generated <see cref="string"/> grid text code.</param>
	/// <returns>The task that holds the asynchronous operation.</returns>
	private async Task HandleGeneratingAsync<T>(
		bool onlyGenerateOne,
		GridStateChanger<Analyzer>? gridStateChanger = null,
		Action<string>? gridTextConsumer = null
	) where T : struct, IEquatable<T>, IProgressDataProvider<T>
	{
		BasePage.IsGeneratorLaunched = true;
		BasePage.ClearAnalyzeTabsData();

		var processingText = SR.Get("AnalyzePage_GeneratorIsProcessing", App.CurrentCulture);
		var constraints = ((App)Application.Current).Preference.ConstraintPreferences.Constraints;
		var difficultyLevel = (from c in constraints.OfType<DifficultyLevelConstraint>() select c.DifficultyLevel) is [var dl] ? dl : default;
		var analyzer = ((App)Application.Current).GetAnalyzerConfigured(BasePage.SudokuPane, difficultyLevel);
		var ittoryuFinder = new DisorderedIttoryuFinder(TechniqueIttoryuSets.IttoryuTechniques);

		using var cts = new CancellationTokenSource();
		BasePage._ctsForAnalyzingRelatedOperations = cts;

		try
		{
			(_generatingCount, _generatingFilteredCount) = (0, 0);
			if (onlyGenerateOne)
			{
				if (await Task.Run(taskEntry) is { IsUndefined: false } grid)
				{
					h(ref grid, analyzer);
				}
			}
			else
			{
				while (true)
				{
					if (await Task.Run(taskEntry) is { IsUndefined: false } grid)
					{
						h(ref grid, analyzer);

						_generatingFilteredCount++;
						continue;
					}

					break;
				}
			}
		}
		catch (OperationCanceledException)
		{
		}
		finally
		{
			BasePage._ctsForAnalyzingRelatedOperations = null;
			BasePage.IsGeneratorLaunched = false;
		}


		static (int, int) b(BetweenRule betweenRule, int start, int end)
			=> betweenRule switch
			{
				BetweenRule.BothOpen => (start + 1, end - 1),
				BetweenRule.LeftOpen => (start + 1, end),
				BetweenRule.RightOpen => (start, end + 1),
				_ => (start, end)
			};

		void h(ref Grid grid, Analyzer analyzer)
		{
			gridStateChanger?.Invoke(ref grid, analyzer);
			gridTextConsumer?.Invoke($"{grid:#}");
		}

		unsafe Grid taskEntry()
		{
			return coreHandler(
				constraints,
				constraints.OfType<PrimarySingleConstraint>() is [{ Primary: SingleTechniqueFlag.FullHouse }] // hasFullHouseConstraint
					&& !constraints.Has<SymmetryConstraint>() // hasSymmetryConstraint
					|| constraints.OfType<TechniqueSetConstraint>() is [{ Techniques: [Technique.FullHouse] }] // hasFullHouseConstraintInTechniqueSet
					? &handlerFullHouse
					: &handlerDefault,
				progressReporter,
				cts.Token,
				analyzer,
				ittoryuFinder
			);


			static Grid handlerFullHouse(int givens, SymmetricType _, CancellationToken ct)
				=> new FullHousePrimaryGenerator { EmptyCellsCount = givens == -1 ? -1 : 81 - givens }.GenerateUnique(ct);

			static Grid handlerDefault(int givens, SymmetricType symmetry, CancellationToken ct)
				=> new Generator().Generate(givens, symmetry, ct);

			void progressReporter(T progress)
			{
				DispatcherQueue.TryEnqueue(dispatchingHandler);


				void dispatchingHandler()
				{
					BasePage.AnalyzeProgressLabel.Text = processingText;
					BasePage.AnalyzeStepSearcherNameLabel.Text = progress.ToDisplayString();
				}
			}
		}

		unsafe Grid coreHandler(
			ConstraintCollection constraints,
			delegate*<Cell, SymmetricType, CancellationToken, Grid> gridCreator,
			Action<T> reporter,
			CancellationToken cancellationToken,
			Analyzer analyzer,
			DisorderedIttoryuFinder finder
		)
		{
			var rs = Random.Shared;
			var chosenSymmetries = from c in constraints.OfType<SymmetryConstraint>() select c.SymmetricTypes;
			var chosenGivensCount =
				from c in constraints.OfType<CountBetweenConstraint>()
				let betweenRule = c.BetweenRule
				let pair = (Start: c.Range.Start.Value, End: c.Range.End.Value)
				let targetPair = c.CellState switch { CellState.Given => (pair.Start, pair.End), CellState.Empty => (81 - pair.End, 81 - pair.Start) }
				select (betweenRule, targetPair);
			var chosenDifficultyLevels =
				from c in constraints.OfType<DifficultyLevelConstraint>()
				select c.ValidDifficultyLevels.GetAllFlags().ToArray();
			var ittoryu = constraints.OfType<IttoryuConstraint>() is [var ic] ? ic : null;
			var symmetries = (chosenSymmetries is [var p] ? p : SymmetryConstraint.AllSymmetricTypes) switch
			{
				SymmetryConstraint.InvalidSymmetricType => [],
				SymmetryConstraint.AllSymmetricTypes => Enum.GetValues<SymmetricType>(),
				var symmetricTypes and not 0 => symmetricTypes.GetAllFlags(),
				_ => [SymmetricType.None]
			};

			var chosenGivensCountSeed = chosenGivensCount is [var (br, (start, end))] ? b(br, start, end) : (-1, -1);
			var givensCount = chosenGivensCountSeed is (var s and not -1, var e and not -1) ? rs.Next(s, e + 1) : -1;
			var difficultyLevel = chosenDifficultyLevels is [var d]
				? d[rs.Next(0, d.Length)]
				: DifficultyLevelConstraint.AllValidDifficultyLevelFlags;

			var progress = new SelfReportingProgress<T>(reporter);

			while (true)
			{
				var grid = gridCreator(givensCount, symmetries[rs.Next(0, symmetries.Length)], cancellationToken);
				if (grid.IsUndefined)
				{
					// Cancel the task if 'Grid.Undefined' is encountered.
					// The value can be returned by method 'HodokuPuzzleGenerator.Generate' if cancelled.
					throw new OperationCanceledException();
				}

				if (grid.IsEmpty)
				{
					// The other case that return an invalid value.
					goto ReportState;
				}

				var analysisResult = analyzer.Analyze(in grid);
				switch (difficultyLevel, analysisResult.DifficultyLevel)
				{
					case (DifficultyLevel.Easy, DifficultyLevel.Easy):
					{
						// Optimize: transform the grid if worth.
						if (ittoryu is { Operator: ComparisonOperator.Equality, Rounds: 1 })
						{
							switch (finder.FindPath(in grid))
							{
								case { IsComplete: true } foundIttoryu:
								{
									grid.MakeIttoryu(foundIttoryu);
									goto CheckIttoryuConstraint;
								}
								default:
								{
									break;
								}
							}
							break;
						}

					CheckIttoryuConstraint:
						// Check for ittoryu and ittoryu length constraint if worth.
						if (!(ittoryu?.Check(new(in grid, analysisResult)) ?? true))
						{
							break;
						}

						// Check for the last constraints.
						if ((constraints - ittoryu).IsValidFor(new(in grid, analysisResult)))
						{
							return grid;
						}
						break;
					}
					default:
					{
						if (constraints.IsValidFor(new(in grid, analysisResult)))
						{
							return grid;
						}
						break;
					}
				}

			ReportState:
				progress.Report(T.Create(++_generatingCount, _generatingFilteredCount));
				cancellationToken.ThrowIfCancellationRequested();
			}
		}
	}


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
		=> await HandleGeneratingAsync<GeneratorProgress>(
			true,
			gridTextConsumer: gridText =>
			{
				var grid = Grid.Parse(gridText);
				if (((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
				{
					((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = grid });
				}

				BasePage.SudokuPane.Puzzle = grid;
			}
		);

	private async void LibraryPuzzleFetchButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var library = ((LibrarySimpleBindableSource)PuzzleLibraryChoser.SelectedValue).Library;
		if (!library.Any())
		{
			// There is no puzzle can be selected.
			return;
		}

		var types = ((App)Application.Current).Preference.LibraryPreferences.LibraryPuzzleTransformations;
		BasePage.SudokuPane.Puzzle = await library.RandomReadOneAsync(types);
		BasePage.ClearAnalyzeTabsData();
	}

	private void PuzzleLibraryChooser_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var source = ((LibrarySimpleBindableSource)PuzzleLibraryChoser.SelectedValue).Library;
		((App)Application.Current).Preference.UIPreferences.FetchingPuzzleLibrary = source.FileId;
	}

	private async void BatchGeneratingToLibraryButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var dialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			Title = SR.Get("AnalyzePage_AddPuzzleToLibraryDialogTitle", App.CurrentCulture),
			IsPrimaryButtonEnabled = true,
			DefaultButton = ContentDialogButton.Primary,
			PrimaryButtonText = SR.Get("AnalyzePage_AddPuzzleToLibraryDialogSure", App.CurrentCulture),
			CloseButtonText = SR.Get("AnalyzePage_AddPuzzleToLibraryDialogCancel", App.CurrentCulture),
			Content = new SaveToLibraryDialogContent { AvailableLibraries = LibraryBindableSource.GetLibrariesFromLocal() }
		};
		if (await dialog.ShowAsync() != ContentDialogResult.Primary)
		{
			return;
		}

		var appendToLibraryTask = static (string _, CancellationToken _ = default) => default(Task)!;
		switch ((SaveToLibraryDialogContent)dialog.Content)
		{
			case { SelectedMode: 0, SelectedLibrary: LibraryBindableSource { Library: var lib } }:
			{
				appendToLibraryTask = lib.AppendPuzzleAsync;
				break;
			}
			case { SelectedMode: 1, IsNameValidAsFileId: true } content:
			{
				var libraryCreated = new LibraryInfo(CommonPaths.Library, content.FileId);
				libraryCreated.Initialize();
				libraryCreated.Name = content.LibraryName is var name and not (null or "") ? name : null;
				libraryCreated.Author = content.LibraryAuthor is var author and not (null or "") ? author : null;
				libraryCreated.Description = content.LibraryDescription is var description and not (null or "") ? description : null;
				libraryCreated.Tags = content.LibraryTags is { Count: not 0 } tags ? [.. tags] : null;
				appendToLibraryTask = libraryCreated.AppendPuzzleAsync;
				break;
			}
		}

		await HandleGeneratingAsync<FilteredGeneratorProgress>(
			false,
			static (ref Grid grid, Analyzer analyzer) =>
			{
				var analysisResult = analyzer.Analyze(in grid);
				if (analysisResult is not { IsSolved: true, InterimGrids: var interimGrids, InterimSteps: var interimSteps })
				{
					return;
				}

				var techniques = TechniqueSets.None;
				foreach (var constraint in ((App)Application.Current).Preference.ConstraintPreferences.Constraints)
				{
					switch (constraint)
					{
						case TechniqueConstraint { Techniques: var t }:
						{
							techniques |= t;
							break;
						}
						case TechniqueCountConstraint { Technique: var technique } and not { Operator: ComparisonOperator.Equality, LimitCount: 0 }:
						{
							techniques.Add(technique);
							break;
						}
					}
				}

				foreach (var (g, s) in StepMarshal.Combine(interimGrids, interimSteps))
				{
					if (techniques.Contains(s.Code))
					{
						grid = g;
						break;
					}
				}
			},
			async gridText =>
			{
				await appendToLibraryTask($"{gridText}{Environment.NewLine}");

				if (((App)Application.Current).Preference.UIPreferences.AlsoSaveBatchGeneratedPuzzlesIntoHistory
					&& ((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
				{
					((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = Grid.Parse(gridText) });
				}
			}
		);
	}

	private async void BatchGeneratingButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (!BasePage.EnsureUnsnapped(true))
		{
			return;
		}

		var fsp = new FileSavePicker();
		fsp.Initialize(this);
		fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fsp.SuggestedFileName = SR.Get("Sudoku", App.CurrentCulture);
		fsp.AddFileFormat(FileFormats.PlainText);

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath })
		{
			return;
		}

		await HandleGeneratingAsync<FilteredGeneratorProgress>(
			false,
			static (ref Grid grid, Analyzer analyzer) =>
			{
				var analysisResult = analyzer.Analyze(in grid);
				if (analysisResult is not { IsSolved: true, InterimGrids: var interimGrids, InterimSteps: var interimSteps })
				{
					return;
				}

				var techniques = TechniqueSets.None;
				foreach (var constraint in ((App)Application.Current).Preference.ConstraintPreferences.Constraints)
				{
					switch (constraint)
					{
						case TechniqueConstraint { Techniques: var t }:
						{
							techniques |= t;
							break;
						}
						case TechniqueCountConstraint { Technique: var technique } and not { Operator: ComparisonOperator.Equality, LimitCount: 0 }:
						{
							techniques.Add(technique);
							break;
						}
					}
				}

				foreach (var (g, s) in StepMarshal.Combine(interimGrids, interimSteps))
				{
					if (techniques.Contains(s.Code))
					{
						grid = g;
						break;
					}
				}
			},
			gridText =>
			{
				File.AppendAllText(filePath, $"{gridText}{Environment.NewLine}");

				if (((App)Application.Current).Preference.UIPreferences.AlsoSaveBatchGeneratedPuzzlesIntoHistory
					&& ((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
				{
					((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = Grid.Parse(gridText) });
				}
			}
		);
	}
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	/// <summary>
	/// Randomly read one puzzle in the specified file, and return it.
	/// </summary>
	/// <param name="this">Indicates the current instance.</param>
	/// <param name="transformTypes">
	/// Indicates the available transform type that the chosen grid can be transformed.
	/// Use <see cref="TransformType"/>.<see langword="operator"/> |(<see cref="TransformType"/>, <see cref="TransformType"/>)
	/// to combine multiple flags.
	/// </param>
	/// <param name="cancellationToken">The cancellation token that can cancel the current asynchronous operation.</param>
	/// <returns>A <see cref="Task{TResult}"/> of <see cref="Grid"/> instance as the result.</returns>
	/// <exception cref="InvalidOperationException">Throw when the library file is not initialized.</exception>
	/// <seealso href="http://tinyurl.com/choose-a-random-element">Choose a random element from a sequence of unknown length</seealso>
	public static async Task<Grid> RandomReadOneAsync(
		this LibraryInfo @this,
		TransformType transformTypes = TransformType.None,
		CancellationToken cancellationToken = default
	)
	{
		if (!@this.IsInitialized)
		{
			throw new InvalidOperationException(SR.ExceptionMessage("FileShouldBeInitializedFirst"));
		}

		var rng = new Random();
		var numberSeen = 0U;
		Unsafe.SkipInit<Grid>(out var chosen);
		await foreach (var text in @this.EnumerateTextAsync(cancellationToken))
		{
			if ((uint)rng.Next() % ++numberSeen == 0)
			{
				chosen = Grid.Parse(text);
			}
		}

		chosen.Transform(transformTypes);
		return chosen;
	}
}
