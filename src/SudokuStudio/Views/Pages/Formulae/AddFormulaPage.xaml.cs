namespace SudokuStudio.Views.Pages.Formulae;

/// <summary>
/// Represents a page that can add formulae.
/// </summary>
public sealed partial class AddFormulaPage : Page
{
	/// <summary>
	/// Initializes an <see cref="AddFormulaPage"/> instance.
	/// </summary>
	public AddFormulaPage() => InitializeComponent();


	/// <summary>
	/// Manually clear error messages.
	/// </summary>
	private void ManuallyClearErrorMessages()
	{
		Error_NameBox.Text = string.Empty;
		Error_FileIdBox.Text = string.Empty;
		Error_DescriptionBox.Text = string.Empty;
		Error_FormulaExpressionBox.Text = string.Empty;
	}

	/// <summary>
	/// Returns the page back to parent.
	/// </summary>
	private void ReturnBack() => App.GetMainWindow(this).NavigateToPage<UserDefinedFormulaePage>();

	/// <summary>
	/// Try to check validity of values input.
	/// </summary>
	/// <param name="textBlock">The text block that will display error message.</param>
	/// <param name="errorString">The error string that will display error message.</param>
	/// <returns>A <see cref="bool"/> value indicating the validation result.</returns>
	private bool CheckValidity([NotNullWhen(false)] out TextBlock? textBlock, [NotNullWhen(false)] out string? errorString)
	{
		// Name.
		if (string.IsNullOrWhiteSpace(NameBox.Text))
		{
			textBlock = Error_NameBox;
			errorString = ResourceDictionary.Get("AddFormulaPage_Error_NameIsEmptyOrWhitespace", App.CurrentCulture);
			return false;
		}

		// File ID.
		foreach (var character in FileIdBox.Text)
		{
			if (Array.IndexOf(io::Path.GetInvalidPathChars(), character) != -1
				|| Array.IndexOf(io::Path.GetInvalidFileNameChars(), character) != -1)
			{
				textBlock = Error_FileIdBox;
				errorString = ResourceDictionary.Get("AddFormulaPage_Error_FileIdContainsInvalidCharacters", App.CurrentCulture);
				return false;
			}
		}
		if (File.Exists($@"{CommonPaths.Formulae}\{FileIdBox.Text}{FileExtensions.UserFormulae}"))
		{
			textBlock = Error_FileIdBox;
			errorString = ResourceDictionary.Get("AddFormulaPage_Error_FileAlreadyExists", App.CurrentCulture);
			return false;
		}

		// Description.
		if (string.IsNullOrWhiteSpace(DescriptionBox.Text))
		{
			textBlock = Error_DescriptionBox;
			errorString = ResourceDictionary.Get("AddFormulaPage_Error_DescriptionIsEmptyOrWhitespace", App.CurrentCulture);
			return false;
		}

		// Technique selector.
		if (TechniqueSelector.SelectedTechniques.Count == 0)
		{
			textBlock = Error_TechniqueSelector;
			errorString = ResourceDictionary.Get("AddFormulaPage_Error_MustSelectAtLeastOneTechnique", App.CurrentCulture);
			return false;
		}
		var appliedSteps = new HashSet<Type>();
		var keys = new List<Technique>();
		foreach (var (technique, correspondingStepType) in
			from technique in TechniqueSelector.SelectedTechniques
			select (technique, technique.GetSuitableStepType()))
		{
			if (correspondingStepType is null)
			{
				textBlock = Error_TechniqueSelector;
				errorString = ResourceDictionary.Get("AddFormulaPage_Error_TechniqueNotReferToStep", App.CurrentCulture);
				return false;
			}
			if (appliedSteps.Add(correspondingStepType))
			{
				keys.Add(technique);
			}

			if (appliedSteps.Count >= 2)
			{
				textBlock = Error_TechniqueSelector;
				errorString = string.Format(
					ResourceDictionary.Get("AddFormulaPage_Error_ChosenTechniquesCoverMultipleSteps", App.CurrentCulture),
					keys[0].GetName(App.CurrentCulture),
					keys[1].GetName(App.CurrentCulture)
				);
				return false;
			}
		}

		// Expression.
		// TODO: Check for expression.

		(textBlock, errorString) = (null, null);
		return true;
	}


	private void ApplyButton_Click(object sender, RoutedEventArgs e)
	{
		ManuallyClearErrorMessages();

		if (!CheckValidity(out var textBlock, out var errorString))
		{
			textBlock.Text = errorString;
			return;
		}

		var expression = new FormulaExpression
		{
			Name = NameBox.Text,
			FileId = FileIdBox.Text,
			Description = DescriptionBox.Text,
			AppliedTechniques = TechniqueSelector.SelectedTechniques,
			Expression = FormulaExpressionBox.Text
		};
		expression.SaveToLocal();

		// Return the page back.
		ReturnBack();
	}

	private void CancelButton_Click(object sender, RoutedEventArgs e) => ReturnBack();

	private void TechniqueSelector_SelectedTechniquesChanged(TechniqueView sender, TechniqueViewSelectedTechniquesChangedEventArgs e)
	{
		var comma = ResourceDictionary.Get("_Token_Comma", App.CurrentCulture);
		AppliedTechniqueDisplayer.Text = string.Join(
			comma,
			[.. from technique in e.TechniqueSet select technique.GetName(App.CurrentCulture)]
		);
	}
}
