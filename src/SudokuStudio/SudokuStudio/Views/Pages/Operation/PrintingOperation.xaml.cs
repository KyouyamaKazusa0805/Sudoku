namespace SudokuStudio.Views.Pages.Operation;

/// <summary>
/// Represents printing operation.
/// </summary>
public sealed partial class PrintingOperation : Page, IOperationProviderPage
{
	/// <summary>
	/// Initializes a <see cref="PrintingOperation"/> instance.
	/// </summary>
	public PrintingOperation() => InitializeComponent();


	/// <inheritdoc/>
	public AnalyzePage BasePage { get; set; } = null!;


	private void PrintAnalysisButton_Click(object sender, RoutedEventArgs e)
	{

	}
}

/// <summary>
/// Provdies with a PDF document creator that constructs analysis result of a puzzle.
/// </summary>
file sealed class AnalysisResultDocument : IDocument
{
	/// <inheritdoc/>
	public void Compose(IDocumentContainer container)
	{

	}

	/// <inheritdoc/>
	public DocumentMetadata GetMetadata() => DocumentMetadata.Default;
}
