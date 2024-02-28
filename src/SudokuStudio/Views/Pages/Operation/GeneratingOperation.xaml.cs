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
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		TextBlockBindable.SetInlines(
			GeneratorStrategyTooltip,
			[
				new Run().WithText($"{null:AnalyzePage_GeneratingStrategySelected}"),
				new LineBreak(),
				new Run().WithText($"{(uiPref.CanRestrictGeneratingGivensCount, uiPref.GeneratedPuzzleGivensCount) switch
				{
					(false, _) or (_, -1) => (ResourceDictionary.Get("AnalyzePage_GeneratedPuzzleGivensNoRestriction", App.CurrentCulture), string.Empty),
					_ => (uiPref.GeneratedPuzzleGivensCount, ResourceDictionary.Get("AnalyzePage_NumberOfGivens", App.CurrentCulture))
				}:AnalyzePage_GeneratedPuzzleGivensIs}"),
				new LineBreak(),
				new Run().WithText($"{DifficultyLevelConversion.GetNameWithDefault(
					uiPref.GeneratorDifficultyLevel,
					ResourceDictionary.Get("DifficultyLevel_None", App.CurrentCulture)
				):AnalyzePage_SelectedDifficultyLevelIs}"),
				new LineBreak(),
				new Run().WithText($"{ResourceDictionary.Get($"SymmetricType_{uiPref.GeneratorSymmetricPattern}", App.CurrentCulture):AnalyzePage_SelectedSymmetricTypeIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratorSelectedTechniques switch
				{
					[var f] => string.Format(ResourceDictionary.Get("AnalyzePage_SingleTechniquesSelected", App.CurrentCulture), f.GetName(App.CurrentCulture)),
					[var f, ..] and { Count: var fc } => string.Format(ResourceDictionary.Get("AnalyzePage_MultipleTechniquesSelected", App.CurrentCulture), f.GetName(App.CurrentCulture), fc),
					_ => ResourceDictionary.Get("TechniqueSelector_NoTechniqueSelected", App.CurrentCulture),
				}:AnalyzePage_SelectedTechniqueIs}"),
				new LineBreak(),
				new Run().WithText($"{(
				uiPref.GeneratedPuzzleShouldBeMinimal
					? ResourceDictionary.Get("AnalyzePage_IsAMinimal", App.CurrentCulture)
					: ResourceDictionary.Get("AnalyzePage_IsNotMinimal", App.CurrentCulture)
				):AnalyzePage_SelectedMinimalRuleIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratedPuzzleShouldBePearl switch
				{
					true => ResourceDictionary.Get("GeneratingStrategyPage_PearlPuzzle", App.CurrentCulture),
					false => ResourceDictionary.Get("GeneratingStrategyPage_NormalPuzzle", App.CurrentCulture),
					//_ => ResourceDictionary.Get("GeneratingStrategyPage_DiamondPuzzle", App.CurrentCulture)
				}:AnalyzePage_SelectedDiamondRuleIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratorDifficultyLevel switch
				{
					DifficultyLevel.Easy => string.Format(ResourceDictionary.Get("AnalyzePage_IttoryuLength", App.CurrentCulture), uiPref.IttoryuLength),
					_ => ResourceDictionary.Get("AnalyzePage_IttoryuPathIsNotLimited", App.CurrentCulture)
				}:AnalyzePage_SelectedIttoryuIs}")
			]
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


		bool match(Library lib) => lib is { FileId: var f } && f == lastFileId;
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

		var processingText = ResourceDictionary.Get("AnalyzePage_GeneratorIsProcessing", App.CurrentCulture);
		var constraints = ((App)Application.Current).Preference.ConstraintPreferences.Constraints;
		var difficultyLevel = constraints.FindFirst(static (DifficultyLevelConstraint constraint) => constraint.DifficultyLevel);
		var analyzer = ((App)Application.Current).GetAnalyzerConfigured(BasePage.SudokuPane, difficultyLevel);
		var ittoryuFinder = new DisorderedIttoryuFinder(((App)Application.Current).Preference.AnalysisPreferences.IttoryuSupportedTechniques);

		using var cts = new CancellationTokenSource();
		BasePage._ctsForAnalyzingRelatedOperations = cts;

		try
		{
			(_generatingCount, _generatingFilteredCount) = (0, 0);
			if (onlyGenerateOne)
			{
				if (await Task.Run(task) is { IsUndefined: false } grid)
				{
					gridStateChanger?.Invoke(ref grid, analyzer);
					gridTextConsumer?.Invoke(grid.ToString("#"));
				}
			}
			else
			{
				while (true)
				{
					if (await Task.Run(task) is { IsUndefined: false } grid)
					{
						gridStateChanger?.Invoke(ref grid, analyzer);
						gridTextConsumer?.Invoke(grid.ToString("#"));

						_generatingFilteredCount++;
						continue;
					}

					break;
				}
			}


			Grid task() => gridCreator(analyzer, ittoryuFinder);
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			BasePage._ctsForAnalyzingRelatedOperations = null;
			BasePage.IsGeneratorLaunched = false;
		}


		Grid gridCreator(Analyzer analyzer, DisorderedIttoryuFinder finder)
		{
			try
			{
				return generatePuzzleCore(
					progress => DispatcherQueue.TryEnqueue(
						() =>
						{
							BasePage.AnalyzeProgressLabel.Text = processingText;
							BasePage.AnalyzeStepSearcherNameLabel.Text = progress.ToDisplayString();
						}
					), cts.Token, analyzer, finder
				) ?? Grid.Undefined;
			}
			catch (TaskCanceledException)
			{
				return Grid.Undefined;
			}
		}

		Grid? generatePuzzleCore(
			Action<T> reportingAction,
			CancellationToken cancellationToken,
			Analyzer analyzer,
			DisorderedIttoryuFinder finder
		)
		{
			try
			{
				var symmetriesOriginal = constraints.FindFirst(static (SymmetryConstraint c) => c.SymmetricTypes, (SymmetricType)0b_0111_1111);
				var symmetries = (SymmetricType[])[
					.. symmetriesOriginal.GetAllFlags(),

					// A rescue - if we select all of symmetric types here, we will implicitly add "SymmetricType.None".
					.. (SymmetricType[])(symmetriesOriginal == (SymmetricType)0b_0111_1111 ? [SymmetricType.None] : [])
				];
				var symmetry = symmetries[Random.Shared.Next(0, symmetries.Length)];
				var (s, e) = constraints.FindFirst(
					static c => c is CountBetweenConstraint { CellState: CellState.Given },
					static c =>
					{
						var constraint = (CountBetweenConstraint)c;
						var (s, e) = (constraint.Range.Start.Value, constraint.Range.End.Value);
						return constraint.BetweenRule switch
						{
							BetweenRule.BothOpen => (s + 1, e - 1),
							BetweenRule.LeftOpen => (s + 1, e),
							BetweenRule.RightOpen => (s, e + 1),
							_ => (s, e)
						};
					}
				);
				var givensCount = (s, e) == (0, 0) ? -1 : Random.Shared.Next(s, e + 1);
				var difficultyLevels = constraints.FindFirst(static (DifficultyLevelConstraint c) => c.ValidDifficultyLevels, (DifficultyLevel)0b_0001_1111).GetAllFlags();
				var difficultyLevel = difficultyLevels[Random.Shared.Next(0, difficultyLevels.Length)];
				var ittoryuLength = constraints.FindFirst(static (IttoryuLengthConstraint c) => c.Length, -1);
				var progress = new SelfReportingProgress<T>(reportingAction);
				while (true)
				{
					var grid = new HodokuPuzzleGenerator().Generate(givensCount, symmetry, cancellationToken);
					var analyzerResult = analyzer.Analyze(in grid);

					switch (difficultyLevel)
					{
						case DifficultyLevel.Easy:
						{
							// Optimize: transform the grid if worth.
							var foundIttoryu = finder.FindPath(in grid);
							if (ittoryuLength >= 5 && foundIttoryu.Digits.Length >= 5)
							{
								grid.MakeIttoryu(foundIttoryu);
							}

							if (constraints.IsValidFor(new(in grid, analyzerResult))
								&& (ittoryuLength != -1 && foundIttoryu.Digits.Length >= ittoryuLength || ittoryuLength == -1))
							{
								return grid;
							}
							break;
						}
						default:
						{
							if (constraints.IsValidFor(new(in grid, analyzerResult)))
							{
								return grid;
							}
							break;
						}
					}

					progress.Report(T.Create(++_generatingCount, _generatingFilteredCount));
					cancellationToken.ThrowIfCancellationRequested();
				}
			}
			catch (OperationCanceledException)
			{
				return null;
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

	private void PuzzleLibraryChoser_SelectionChanged(object sender, SelectionChangedEventArgs e)
	{
		var source = ((LibrarySimpleBindableSource)PuzzleLibraryChoser.SelectedValue).Library;
		((App)Application.Current).Preference.UIPreferences.FetchingPuzzleLibrary = source.FileId;
	}

	private async void BatchGeneratingToLibraryButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		var dialog = new ContentDialog
		{
			XamlRoot = XamlRoot,
			Title = ResourceDictionary.Get("AnalyzePage_AddPuzzleToLibraryDialogTitle", App.CurrentCulture),
			IsPrimaryButtonEnabled = true,
			DefaultButton = ContentDialogButton.Primary,
			PrimaryButtonText = ResourceDictionary.Get("AnalyzePage_AddPuzzleToLibraryDialogSure", App.CurrentCulture),
			CloseButtonText = ResourceDictionary.Get("AnalyzePage_AddPuzzleToLibraryDialogCancel", App.CurrentCulture),
			Content = new SaveToLibraryDialogContent { AvailableLibraries = LibraryBindableSource.GetLibrariesFromLocal() }
		};
		if (await dialog.ShowAsync() != ContentDialogResult.Primary)
		{
			return;
		}

		var appendToLibraryTask = static (string _, CancellationToken _ = default) => default(Task)!;
		switch ((SaveToLibraryDialogContent)dialog.Content)
		{
			case { SelectedMode: 0, SelectedLibrary: LibraryBindableSource { LibraryInfo: var lib } }:
			{
				appendToLibraryTask = lib.AppendPuzzleAsync;
				break;
			}
			case { SelectedMode: 1, IsNameValidAsFileId: true } content:
			{
				var libraryCreated = new Library(CommonPaths.Library, content.FileId);
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
			static (scoped ref Grid grid, Analyzer analyzer) =>
			{
				var analyzerResult = analyzer.Analyze(in grid);
				if (!analyzerResult.IsSolved)
				{
					return;
				}

				var techniques = new TechniqueSet();
				foreach (var constraint in ((App)Application.Current).Preference.ConstraintPreferences.Constraints)
				{
					if (constraint is TechniqueConstraint { Technique: var technique }
						and not { Operator: ComparisonOperator.Equality, LimitCount: 0 })
					{
						techniques.Add(technique);
					}
				}

				foreach (var (g, s) in analyzerResult.SolvingPath)
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
		fsp.SuggestedFileName = ResourceDictionary.Get("Sudoku", App.CurrentCulture);
		fsp.AddFileFormat(FileFormats.PlainText);

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath })
		{
			return;
		}

		await HandleGeneratingAsync<FilteredGeneratorProgress>(
			false,
			static (scoped ref Grid grid, Analyzer analyzer) =>
			{
				var analyzerResult = analyzer.Analyze(in grid);
				if (!analyzerResult.IsSolved)
				{
					return;
				}

				var techniques = new TechniqueSet();
				foreach (var constraint in ((App)Application.Current).Preference.ConstraintPreferences.Constraints)
				{
					if (constraint is TechniqueConstraint { Technique: var technique }
						and not { Operator: ComparisonOperator.Equality, LimitCount: 0 })
					{
						techniques.Add(technique);
					}
				}

				foreach (var (g, s) in analyzerResult.SolvingPath)
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

/// <summary>
/// Defines a self-reporting progress type.
/// </summary>
/// <param name="handler"><inheritdoc cref="Progress{T}.Progress(Action{T})" path="/param[@name='handler']"/></param>
/// <typeparam name="T"><inheritdoc cref="Progress{T}" path="/typeparam[@name='T']"/></typeparam>
file sealed class SelfReportingProgress<T>(Action<T> handler) : Progress<T>(handler) where T : struct, IEquatable<T>, IProgressDataProvider<T>
{
	/// <inheritdoc cref="Progress{T}.OnReport(T)"/>
	public void Report(T value) => OnReport(value);
}

/// <summary>
/// The encapsulated type to describe the details for generating puzzles.
/// </summary>
/// <param name="DifficultyLevel">Indicates the difficulty level selected.</param>
/// <param name="SymmetricPattern">Indicates the symmetric pattern selected.</param>
/// <param name="CountOfGivens">Indicates the limit of givens count.</param>
/// <param name="IttoryuLength">Indicates the ittoryu length.</param>
file sealed record GeneratingDetails(DifficultyLevel DifficultyLevel, SymmetricType SymmetricPattern, int CountOfGivens, int IttoryuLength);

/// <summary>
/// Provides with extension methods on <see cref="Run"/>.
/// </summary>
/// <seealso cref="Run"/>
file static class RunExtensions
{
	/// <summary>
	/// Sets with <see cref="Run.Text"/> property, reducing indenting.
	/// </summary>
	/// <param name="this">The <see cref="Run"/> instance.</param>
	/// <param name="text">The text to be initialized.</param>
	public static Run WithText(this Run @this, scoped ref ResourceFetcher text)
	{
		@this.Text = text.ToString();
		return @this;
	}
}

/// <summary>
/// Represents a mechanism that allows you using interpolated string syntax to fetch resource text.
/// </summary>
/// <param name="literalLength">Indicates the whole length of the interpolated string.</param>
/// <param name="formattedCount">Indicates the number of the interpolated holes.</param>
/// <remarks>
/// Usage:
/// <code><![CDATA[
/// scoped ResourceFetcher expr = $"{value:ResourceKeyName}";
/// string s = expr.ToString();
/// ]]></code>
/// Here <c>value</c> will be inserted into the resource, whose related key value is specified as <c>ResourceKeyName</c> after colon token.
/// Its equivalent value is
/// <code><![CDATA[
/// string s = string.Format(ResourceDictionary.Get("ResourceKeyName", App.CurrentCulture), value);
/// ]]></code>
/// This type is implemented via an interpolated string handler pattern, same as <see cref="DefaultInterpolatedStringHandler"/>,
/// marked with <see cref="InterpolatedStringHandlerAttribute"/>.
/// </remarks>
/// <seealso cref="DefaultInterpolatedStringHandler"/>
/// <seealso cref="InterpolatedStringHandlerAttribute"/>
[InterpolatedStringHandler]
[StructLayout(LayoutKind.Auto)]
file ref struct ResourceFetcher(int literalLength, int formattedCount)
{
	/// <summary>
	/// <inheritdoc cref="ResourceFetcher" path="/param[@name='literalLength']"/>
	/// </summary>
	private readonly int _literalLength = literalLength;

	/// <summary>
	/// <inheritdoc cref="ResourceFetcher" path="/param[@name='formattedCount']"/>
	/// </summary>
	private readonly int _formattedCount = formattedCount;

	/// <summary>
	/// The internal format.
	/// </summary>
	private string? _format;

	/// <summary>
	/// The internal content.
	/// </summary>
	private object? _content;


	/// <inheritdoc cref="object.ToString"/>
	/// <exception cref="InvalidOperationException">Throws when the value is not initialized.</exception>
	public override readonly string ToString()
		=> _format switch
		{
			not null => _content switch
			{
				var (a, b, c) => string.Format(ResourceDictionary.Get(_format, App.CurrentCulture), a, b, c),
				var (a, b) => string.Format(ResourceDictionary.Get(_format, App.CurrentCulture), a, b),
				ITuple tuple => string.Format(ResourceDictionary.Get(_format, App.CurrentCulture), tuple.ToArray()),
				not null => string.Format(ResourceDictionary.Get(_format, App.CurrentCulture), _content),
				_ => ResourceDictionary.Get(_format, App.CurrentCulture)
			},
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("FormatCannotBeNull"))
		};

	/// <inheritdoc cref="DefaultInterpolatedStringHandler.AppendFormatted{T}(T, string?)"/>
	public void AppendFormatted(object? content, string format) => (_format, _content) = (format, content);
}
