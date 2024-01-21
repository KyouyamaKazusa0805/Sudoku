namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that binds with a analyze tab page type that implements <see cref="IAnalyzeTabPage"/>.
/// </summary>
/// <param name="header">Indicates the header.</param>
/// <param name="iconSource">Indicates the icon source.</param>
/// <param name="page">Indicates the tab page.</param>
/// <seealso cref="IAnalyzeTabPage"/>
internal sealed partial class AnalyzeTabPageBindableSource(
	[RecordParameter(SetterExpression = "init")] string header,
	[RecordParameter(SetterExpression = "init")] IconSource iconSource,
	[RecordParameter(SetterExpression = "init")] IAnalyzeTabPage page
);
