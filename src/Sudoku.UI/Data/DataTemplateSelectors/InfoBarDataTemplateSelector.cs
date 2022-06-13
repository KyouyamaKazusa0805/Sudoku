namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that selects the <see cref="DataTemplate"/> between
/// <see cref="InfoBarMessage"/> and <see cref="HyperlinkMessage"/> as the model types.
/// </summary>
/// <seealso cref="InfoBarMessage"/>
/// <seealso cref="HyperlinkMessage"/>
public sealed class InfoBarDataTemplateSelector : ModelDataTemplateSelector
{
	/// <summary>
	/// Indicates the data template that is used by the type <see cref="PlainMessage"/>.
	/// </summary>
	[DataTemplateModelType<PlainMessage>]
	public DataTemplate PlainMessageTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="HyperlinkMessage"/>.
	/// </summary>
	[DataTemplateModelType<HyperlinkMessage>]
	public DataTemplate HyperlinkMessageTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="ManualSolverResultMessage"/>.
	/// </summary>
	[DataTemplateModelType<ManualSolverResultMessage>]
	public DataTemplate AnalysisResultTemplate { get; set; } = null!;
}
