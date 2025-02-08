namespace SudokuStudio.Views.Pages;

/// <summary>
/// Represents command-based drawing page.
/// </summary>
public sealed partial class CommandBasedDrawingPage : Page
{
	/// <summary>
	/// Indicates the load keyword.
	/// </summary>
	private const string LoadKeyword = "load";

	/// <summary>
	/// Indicates candidates on syntax.
	/// </summary>
	private const string CandidatesOnSyntax = "+c";

	/// <summary>
	/// Indicates candidates off syntax.
	/// </summary>
	private const string CandidatesOffSyntax = "-c";


	/// <summary>
	/// Initializes a <see cref="CommandBasedDrawingPage"/> instance.
	/// </summary>
	public CommandBasedDrawingPage() => InitializeComponent();


	[GeneratedRegex("""load\s+([+-]c\s+)?.+""", RegexOptions.Compiled | RegexOptions.IgnoreCase)]
	private static partial Regex LoadCommandPattern { get; }


	/// <summary>
	/// Try to parse command line from control <see cref="DrawingCommandTextBox"/>.
	/// </summary>
	/// <param name="valid">Indicates whether the syntax is valid.</param>
	/// <param name="grid">The grid created.</param>
	/// <param name="showCandidates">Indicates whether candidates should be displayed.</param>
	/// <returns>A <see cref="View"/> instance created.</returns>
	private View? TryParseCommandLine(out bool valid, out Grid? grid, out bool? showCandidates)
	{
		(grid, showCandidates) = (null, null);

		var text = DrawingCommandTextBox.Text;
		if (string.IsNullOrWhiteSpace(text))
		{
			valid = true;
			return null;
		}

		var lines = text.SplitBy('\r', '\n');

		// Find for grid showing command.
		foreach (var line in lines)
		{
			if (!line.StartsWith(LoadKeyword))
			{
				continue;
			}

			// Load keyword found. Check validity of syntax.
			// Syntax:
			//   load_command
			//     : 'load' ' ' ('+' | '-') 'c' ' ' puzzle_string
			//     | 'load' ' ' puzzle_string
			//     ;
			if (LoadCommandPattern.Match(line) is not { Success: true })
			{
				continue;
			}

			// Check candidate showing syntax.
			if (line.Contains(CandidatesOnSyntax) || line.Contains(CandidatesOffSyntax))
			{
				showCandidates = line.Contains(CandidatesOnSyntax);

				var indexStartingGrid = line.IndexOf('c') + 1;
				if (Grid.TryParse(line[indexStartingGrid..], out var g1))
				{
					grid = g1;
					break;
				}

				valid = false;
				return null;
			}

			if (Grid.TryParse(line[(line.IndexOf(' ') + 1)..], out var g2))
			{
				grid = g2;
				break;
			}

			valid = false;
			return null;
		}

		// Parses the command.
		var coordinateParser = Application.Current.AsApp().Preference.UIPreferences.ConceptNotationBasedKind.GetParser();
		valid = new DrawingParser().TryParse(text, out var result, coordinateParser);
		return result;
	}


	private void SudokuPane_Loaded(object sender, RoutedEventArgs e)
	{
		var app = Application.Current.AsApp();
		app.CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);
		app.MainSudokuPane = SudokuPane;
	}

	private void SudokuPane_ActualThemeChanged(FrameworkElement sender, object args)
		=> Application.Current.AsApp().CoverSettingsToSudokuPaneViaApplicationTheme(SudokuPane);

	private void ApplyButton_Click(object sender, RoutedEventArgs e)
	{
		if (sender is not Button { Name: nameof(ApplyButton) })
		{
			return;
		}

		var view = TryParseCommandLine(out _, out var nullableGrid, out var nullableShowCandidates);
		if (view is null)
		{
			return;
		}

		// Assign the view onto grid.
		if (nullableGrid is { } grid && nullableShowCandidates is { } showCandidates)
		{
			SudokuPane.Puzzle = grid;
			SudokuPane.DisplayCandidates = showCandidates;
		}
		SudokuPane.ViewUnit = new() { Conclusions = ReadOnlyMemory<Conclusion>.Empty, View = view };
	}
}
