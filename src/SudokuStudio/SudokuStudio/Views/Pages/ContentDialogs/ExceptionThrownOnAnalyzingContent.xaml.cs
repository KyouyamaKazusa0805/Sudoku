namespace SudokuStudio.Views.Pages.ContentDialogs;

/// <summary>
/// Indicates the page that is used as a content for content dialog when the exception is thrown during a puzzle is being analyzed.
/// </summary>
public sealed partial class ExceptionThrownOnAnalyzingContent : Page
{
	/// <summary>
	/// Initializes an <see cref="ExceptionThrownOnAnalyzingContent"/> instance.
	/// </summary>
	public ExceptionThrownOnAnalyzingContent() => InitializeComponent();


	/// <summary>
	/// <para>Indicates the thrown exception.</para>
	/// <para><inheritdoc cref="ErrorStepDialogContent.ErrorStepText" path="//summary/para[2]"/></para>
	/// </summary>
	/// <value><inheritdoc cref="ThrownException" path="//summary/para[1]"/></value>
	public Exception ThrownException { set => ExceptionDisplayer.Text = value.ToString(); }
}
