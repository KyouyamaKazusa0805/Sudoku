using System;

namespace Sudoku.UI.Data.DataTemplateSelectors;

/// <summary>
/// Defines a data template selector that selects the <see cref="DataTemplate"/> between
/// <see cref="InfoBarMessage"/> and <see cref="HyperlinkMessage"/> as the model types.
/// </summary>
/// <seealso cref="InfoBarMessage"/>
/// <seealso cref="HyperlinkMessage"/>
public sealed class InfoBarDataTemplateSelector : DataTemplateSelector
{
	/// <summary>
	/// Indicates the data template that is used by the type <see cref="PlainMessage"/>.
	/// </summary>
	public DataTemplate PlainMessageTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="HyperlinkMessage"/>.
	/// </summary>
	public DataTemplate HyperlinkMessageTemplate { get; set; } = null!;

	/// <summary>
	/// Indicates the data template that is used by the type <see cref="ManualSolverResultMessage"/>.
	/// </summary>
	public DataTemplate AnalysisResultTemplate { get; set; } = null!;


	/// <inheritdoc/>
	/// <exception cref="InvalidOperationException">
	/// Throws when the type of the argument <paramref name="item"/> doesn't derive
	/// from <see cref="InfoBarMessage"/>.
	/// </exception>
	protected override DataTemplate SelectTemplateCore(object item) =>
		item switch
		{
			PlainMessage => PlainMessageTemplate,
			HyperlinkMessage => HyperlinkMessageTemplate,
			ManualSolverResultMessage => AnalysisResultTemplate,
			_ => throw new InvalidOperationException("The type is invalid.")
		};
}
