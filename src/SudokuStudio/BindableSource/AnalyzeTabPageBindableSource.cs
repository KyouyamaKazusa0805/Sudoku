namespace SudokuStudio.BindableSource;

/// <summary>
/// Represents a type that binds with a analyze tab page type that implements <see cref="IAnalyzerTab"/>.
/// </summary>
/// <param name="header">Indicates the header.</param>
/// <param name="iconSource">Indicates the icon source.</param>
/// <param name="page">Indicates the tab page.</param>
/// <seealso cref="IAnalyzerTab"/>
internal sealed partial class AnalyzeTabPageBindableSource(
	[PrimaryCosntructorParameter(SetterExpression = "init")] string header,
	[PrimaryCosntructorParameter(SetterExpression = "init")] IconSource iconSource,
	[PrimaryCosntructorParameter(SetterExpression = "init")] IAnalyzerTab page
);
