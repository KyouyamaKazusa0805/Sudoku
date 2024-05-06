namespace SudokuStudio.Views.Pages.Formulae;

/// <summary>
/// Represents built-in formulae page.
/// </summary>
public sealed partial class BuiltInFormulaePage : Page
{
	/// <summary>
	/// Indicates the tab ('<c>\t</c>').
	/// </summary>
	private static readonly string Tab = "\t";

	/// <summary>
	/// Indicates the spaces.
	/// </summary>
	private static readonly string Spaces = new(' ', 4);


	/// <summary>
	/// Initializes a <see cref="BuiltInFormulaePage"/> instance.
	/// </summary>
	public BuiltInFormulaePage() => InitializeComponent();


	private async void Page_LoadedAsync(object sender, RoutedEventArgs e)
	{
		foreach (var type in typeof(Factor).Assembly.GetTypes())
		{
			if (type.BaseType != typeof(Factor))
			{
				continue;
			}

			// TODO: Get parameter types.

			var instance = (Factor)Activator.CreateInstance(type)!;
			FormulaeDisplayer.Children.Add(
				new SettingsCard
				{
					HeaderIcon = new FontIcon { Glyph = "\uE943" },
					Header = instance.GetName(App.CurrentCulture),
					Content = new TextBlock
					{
						Text = formulaExpressionUpdate(instance.FormulaExpressionString),
						FontFamily = new("Cascadia Code")
					}
				}
			);

			await Task.Delay(100);
		}


		static string formulaExpressionUpdate(string originalExpression)
		{
			var parameterFormat = ResourceDictionary.Get("ParameterName", App.CurrentCulture);

			var interim = ArgsPattern().Replace(originalExpression, argsEvaluator);
			interim = interim.Replace(Tab, Spaces);
			interim = BracePattern().Replace(interim, m);
			return interim;


			string argsEvaluator(Match match) => string.Format(parameterFormat, int.Parse(m(match)) + 1);
		}

		static string m(Match match) => match.Groups[1].Value;
	}

	[GeneratedRegex("""\([\w\[\]\?]+\)args!?\[(\d+)\]!?""", RegexOptions.Compiled)]
	private static partial Regex ArgsPattern();

	[GeneratedRegex("""\(([^\)]+)\)(?=\.)""", RegexOptions.Compiled)]
	private static partial Regex BracePattern();
}
