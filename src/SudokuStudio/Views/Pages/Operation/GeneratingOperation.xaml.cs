using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Documents;
using Sudoku.Algorithm.Generating;
using Sudoku.Algorithm.Ittoryu;
using Sudoku.Analytics;
using Sudoku.Analytics.Categorization;
using Sudoku.Concepts;
using SudokuStudio.ComponentModel;
using SudokuStudio.Interaction.Conversions;
using SudokuStudio.Views.Attached;
using static SudokuStudio.Strings.StringsAccessor;

namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Indicates the generating operation command bar.
/// </summary>
public sealed partial class GeneratingOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// Initializes a <see cref="GeneratingOperation"/> instance.
	/// </summary>
	public GeneratingOperation()
	{
		InitializeComponent();
		SetMemoryOptions();
	}


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	/// <summary>
	/// Update control selection via user configuration.
	/// </summary>
	private void SetMemoryOptions()
	{
		var uiPref = ((App)Application.Current).Preference.UIPreferences;
		var comma = GetString("_Token_Comma");
		var openBrace = GetString("_Token_OpenBrace");
		var closedBrace = GetString("_Token_ClosedBrace");
		TextBlockBindable.SetInlines(
			GeneratorStrategyTooltip,
			[
				new Run().WithText($"{null:AnalyzePage_GeneratingStrategySelected}"),
				new LineBreak(),
				new Run().WithText($"{(uiPref.CanRestrictGeneratingGivensCount, uiPref.GeneratedPuzzleGivensCount) switch
				{
					(false, _) or (_, -1) => (GetString("AnalyzePage_GeneratedPuzzleGivensNoRestriction"), string.Empty),
					_ => (uiPref.GeneratedPuzzleGivensCount, GetString("AnalyzePage_NumberOfGivens"))
				}:AnalyzePage_GeneratedPuzzleGivensIs}"),
				new LineBreak(),
				new Run().WithText($"{DifficultyLevelConversion.GetNameWithDefault(
					uiPref.GeneratorDifficultyLevel,
					GetString("DifficultyLevel_None")
				):AnalyzePage_SelectedDifficultyLevelIs}"),
				new LineBreak(),
				new Run().WithText($"{GetString($"SymmetricType_{uiPref.GeneratorSymmetricPattern}"):AnalyzePage_SelectedSymmetricTypeIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.SelectedTechnique switch
				{
					var t and not 0 => $"{t.GetName()}{openBrace}{t.GetEnglishName()}{closedBrace}",
					_ => GetString("TechniqueSelector_NoTechniqueSelected"),
				}:AnalyzePage_SelectedTechniqueIs}"),
				new LineBreak(),
				new Run().WithText($"{(
				uiPref.GeneratedPuzzleShouldBeMinimal
					? GetString("AnalyzePage_IsAMinimal")
					: GetString("AnalyzePage_IsNotMinimal")
				):AnalyzePage_SelectedMinimalRuleIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratedPuzzleShouldBePearl switch
				{
					true => GetString("GeneratingStrategyPage_PearlPuzzle"),
					false => GetString("GeneratingStrategyPage_NormalPuzzle"),
					//_ => GetString("GeneratingStrategyPage_DiamondPuzzle")
				}:AnalyzePage_SelectedDiamondRuleIs}"),
				new LineBreak(),
				new Run().WithText($"{uiPref.GeneratorDifficultyLevel switch
				{
					DifficultyLevel.Easy => string.Format(GetString("AnalyzePage_IttoryuLength"), uiPref.IttoryuLength),
					_ => GetString("AnalyzePage_IttoryuPathIsNotLimited")
				}:AnalyzePage_SelectedIttoryuIs}")
			]
		);
	}


	private async void NewPuzzleButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		BasePage.IsGeneratorLaunched = true;
		BasePage.ClearAnalyzeTabsData();

		var processingText = GetString("AnalyzePage_GeneratorIsProcessing");
		var preferences = ((App)Application.Current).Preference.UIPreferences;
		var difficultyLevel = preferences.GeneratorDifficultyLevel;
		var symmetry = preferences.GeneratorSymmetricPattern;
		var minimal = preferences.GeneratedPuzzleShouldBeMinimal;
		var pearl = preferences.GeneratedPuzzleShouldBePearl;
		var technique = preferences.SelectedTechnique;
		var givensCount = preferences switch
		{
			{ CanRestrictGeneratingGivensCount: true, GeneratedPuzzleGivensCount: var count and not -1 } => count,
			_ => HodokuPuzzleGenerator.AutoClues
		};
		var ittoryuLength = preferences.IttoryuLength;
		var analyzer = ((App)Application.Current).GetAnalyzerConfigured(BasePage.SudokuPane, preferences.GeneratorDifficultyLevel);
		var ittoryuPathFinder = new IttoryuPathFinder
		{
			SupportedTechniques = [.. ((App)Application.Current).Preference.AnalysisPreferences.IttoryuSupportedTechniques]
		};
		using var cts = new CancellationTokenSource();
		BasePage._ctsForAnalyzingRelatedOperations = cts;

		try
		{
			Grid function() => gridCreator(analyzer, ittoryuPathFinder, new(difficultyLevel, symmetry, minimal, pearl, technique, givensCount, ittoryuLength));
			var grid = await Task.Run(function);
			if (grid.IsUndefined)
			{
				return;
			}

			if (((App)Application.Current).Preference.UIPreferences.SavePuzzleGeneratingHistory)
			{
				((App)Application.Current).PuzzleGeneratingHistory.Puzzles.Add(new() { BaseGrid = grid });
			}

			BasePage.SudokuPane.Puzzle = grid;
		}
		catch (TaskCanceledException)
		{
		}
		finally
		{
			BasePage._ctsForAnalyzingRelatedOperations = null;
			BasePage.IsGeneratorLaunched = false;
		}


		Grid gridCreator(Analyzer analyzer, IttoryuPathFinder finder, GeneratingDetails details)
		{
			try
			{
				return generatePuzzleCore(progress => DispatcherQueue.TryEnqueue(() =>
				{
					var count = progress.Count;
					BasePage.AnalyzeProgressLabel.Text = processingText;
					BasePage.AnalyzeStepSearcherNameLabel.Text = count.ToString();
				}), details, cts.Token, analyzer, finder) ?? Grid.Undefined;
			}
			catch (TaskCanceledException)
			{
				return Grid.Undefined;
			}
		}

		static Grid? generatePuzzleCore(
			Action<GeneratorProgress> reportingAction,
			GeneratingDetails details,
			CancellationToken cancellationToken,
			Analyzer analyzer,
			IttoryuPathFinder finder
		)
		{
			try
			{
				for (
					var (count, progress, (difficultyLevel, symmetry, minimal, pearl, technique, givensCount, ittoryuLength)) = (
						0,
						new SelfReportingProgress<GeneratorProgress>(reportingAction),
						details
					); ; count++, progress.Report(new(count)), cancellationToken.ThrowIfCancellationRequested()
				)
				{
					var grid = HodokuPuzzleGenerator.Generate(givensCount, symmetry, cancellationToken);

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

							if (basicCondition() && (ittoryuLength != -1 && foundIttoryu.Digits.Length >= ittoryuLength || ittoryuLength == -1))
							{
								return grid;
							}
							break;
						}
						case var _ when basicCondition():
						{
							return grid;
						}
					}


					bool basicCondition()
						=> (givensCount != -1 && grid.GivensCount == givensCount || givensCount == -1)
						&& analyzer.Analyze(in grid) is
						{
							IsSolved: true,
							IsPearl: var isPearl,
							DifficultyLevel: var puzzleDifficultyLevel,
							SolvingPath: var p
						}
						&& (difficultyLevel == 0 || puzzleDifficultyLevel == difficultyLevel)
						&& (minimal && grid.IsMinimal || !minimal)
						&& (pearl && isPearl is true || !pearl)
						&& (technique != 0 && p.HasTechnique(technique) || technique == 0);
				}
			}
			catch (OperationCanceledException)
			{
				return null;
			}
		}
	}
}

/// <summary>
/// Defines a self-reporting progress type.
/// </summary>
/// <param name="handler"><inheritdoc cref="Progress{T}.Progress(Action{T})" path="/param[@name='handler']"/></param>
/// <typeparam name="T"><inheritdoc cref="Progress{T}" path="/typeparam[@name='T']"/></typeparam>
file sealed class SelfReportingProgress<T>(Action<T> handler) : Progress<T>(handler)
{
	/// <inheritdoc cref="Progress{T}.OnReport(T)"/>
	public void Report(T value) => OnReport(value);
}

/// <summary>
/// The encapsulated type to describe the details for generating puzzles.
/// </summary>
/// <param name="DifficultyLevel">Indicates the difficulty level selected.</param>
/// <param name="SymmetricPattern">Indicates the symmetric pattern selected.</param>
/// <param name="ShouldBeMinimal">Indicates whether generated puzzles should be minimal.</param>
/// <param name="ShouldBePearl">Indicates whether generated puzzles should be pearl.</param>
/// <param name="SelectedTechnique">Indicates the selected technique that you want it to be appeared in generated puzzles.</param>
/// <param name="CountOfGivens">Indicates the limit of givens count.</param>
/// <param name="IttoryuLength">Indicates the ittoryu length.</param>
file sealed record GeneratingDetails(
	DifficultyLevel DifficultyLevel,
	SymmetricType SymmetricPattern,
	bool ShouldBeMinimal,
	bool ShouldBePearl,
	Technique SelectedTechnique,
	int CountOfGivens,
	int IttoryuLength
);

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
	public static Run WithText(this Run @this, [InterpolatedStringHandlerArgument] scoped ref FormatHandler text)
	{
		@this.Text = text.ToString();
		return @this;
	}
}

/// <summary>
/// The internal format handler type.
/// </summary>
[InterpolatedStringHandler]
file ref struct FormatHandler(int _, int __)
{
	/// <summary>
	/// The inforamtion for character length and hole count.
	/// </summary>
	[SuppressMessage("CodeQuality", "IDE0052:Remove unread private members", Justification = "<Pending>")]
	private readonly int _ = _, __ = __;

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
				var (a, b, c) => string.Format(GetString(_format), a, b, c),
				var (a, b) => string.Format(GetString(_format), a, b),
				ITuple tuple => string.Format(GetString(_format), tuple.ToArray()),
				object => string.Format(GetString(_format), _content),
				_ => GetString(_format)
			},
			_ => throw new InvalidOperationException("The format cannot be null.")
		};

	/// <inheritdoc cref="DefaultInterpolatedStringHandler.AppendFormatted{T}(T, string?)"/>
	public void AppendFormatted(object? content, string format) => (_format, _content) = (format, content);
}
