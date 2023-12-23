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


	/// <summary>
	/// The method called by <see cref="Page_Loaded(object, RoutedEventArgs)"/> and <see cref="Page_Unloaded(object, RoutedEventArgs)"/>.
	/// </summary>
	private void OnSaveFileFailed(AnalyzePage _, SaveFileFailedEventArgs e)
		=> (e.Reason switch { SaveFileFailedReason.UnsnappingFailed => ErrorDialog_ProgramIsSnapped }).IsOpen = true;


	private async void PrintAnalysisButton_ClickAsync(object sender, RoutedEventArgs e)
	{
		if (BasePage.AnalysisResultCache is not { } analyzerResult)
		{
			ErrorDialog_AnalysisResultNotExist.IsOpen = true;
			return;
		}

		var document = await new AnalysisResultDocumentCreator
		{
			Comment = GetString("AnalyzePage_GenerateComment"),
			AnalysisResult = analyzerResult
		}.CreateDocumentAsync();

		if (!BasePage.EnsureUnsnapped(true))
		{
			return;
		}

		var fsp = new FileSavePicker();
		fsp.Initialize(this);
		fsp.SuggestedStartLocation = PickerLocationId.DocumentsLibrary;
		fsp.SuggestedFileName = GetString("SuggestedFileName_Output");
		fsp.AddFileFormat(FileFormats.PortableDocument);

		if (await fsp.PickSaveFileAsync() is not { Path: var filePath })
		{
			return;
		}

		document.GeneratePdf(filePath);
	}

	private void Page_Loaded(object sender, RoutedEventArgs e) => BasePage.SaveFileFailed += OnSaveFileFailed;

	private void Page_Unloaded(object sender, RoutedEventArgs e) => BasePage.SaveFileFailed -= OnSaveFileFailed;
}

/// <summary>
/// Provides with a PDF document creator that constructs analysis result of a puzzle.
/// </summary>
file sealed class AnalysisResultDocumentCreator
{
	/// <summary>
	/// Indicates the title style.
	/// </summary>
	internal static readonly TextStyle TitleStyle = TextStyle.Default.FontSize(16).SemiBold().SupportChineseCharacters();

	/// <summary>
	/// Indicates the default style.
	/// </summary>
	internal static readonly TextStyle DefaultStyle = TextStyle.Default.SupportChineseCharacters();


	/// <summary>
	/// Indicates the comment to be set.
	/// </summary>
	public string? Comment { get; init; }

	/// <summary>
	/// Indicates the analysis result to be used.
	/// </summary>
	public required AnalyzerResult AnalysisResult { get; init; }


	/// <summary>
	/// Creates a PDF document.
	/// </summary>
	/// <returns>A task that handles the operation, and returns a PDF document.</returns>
	public async Task<Document> CreateDocumentAsync()
		=> await Task.Run(
			() => Document.Create(
				dc => dc.Page(
					page =>
					{
						page.Margin(50);
						page.Header().Element(
							c => c
								.Row(
									row =>
									{
										row.RelativeItem().Column(
											column =>
											{
												column.Item().Text(GetString("AnalyzePage_AnalysisResultReportPdfTitle")).Style(TitleStyle);
												column.Item()
													.Text(
														static text =>
														{
															text.Span(GetString("AnalyzePage_GenerateDate")).SemiBold().Style(DefaultStyle);
															text.Span($"{DateTime.Now:d}").Style(DefaultStyle);
														}
													);
												column.Item()
													.Text(
														text =>
														{
															text.Span(GetString("AnalyzePage_PuzzleIs")).SemiBold().Style(DefaultStyle);
															text.Span($"{AnalysisResult.Puzzle:#}").Style(DefaultStyle);
														}
													);
											}
										);
									}
								)
						);

						page.Content().Element(
							c => c
								.PaddingVertical(40)
								.Column(
									column =>
									{
										column.Spacing(5);
										column.Item().Element(
											c => c
												.Table(
													table =>
													{
														table.ColumnsDefinition(
															static columns =>
															{
																columns.RelativeColumn();
																columns.RelativeColumn();
																columns.ConstantColumn(80);
																columns.ConstantColumn(80);
																columns.ConstantColumn(80);
															}
														);

														table.Header(
															static header =>
															{
																header.Cell().Element(cellStyle).Text(GetString("AnalyzePage_TechniqueOrTechniqueGroupName"));
																header.Cell().Element(cellStyle).AlignRight().Text(GetString("AnalyzePage_TechniqueCount"));
																header.Cell().Element(cellStyle).AlignRight().Text(GetString("AnalyzePage_DifficultyLevel"));
																header.Cell().Element(cellStyle).AlignRight().Text(GetString("AnalyzePage_DifficultyTotal"));
																header.Cell().Element(cellStyle).AlignRight().Text(GetString("AnalyzePage_DifficultyMax"));


																static PdfContainer cellStyle(PdfContainer container)
																	=> container
																		.DefaultTextStyle(DefaultStyle)
																		.PaddingVertical(5)
																		.BorderBottom(1)
																		.BorderColor(PdfColors.Black);
															}
														);

														foreach (var element in SummaryViewBindableSource.CreateListFrom(AnalysisResult))
														{
															table.Cell().Element(cellStyle).Text(element.TechniqueName);
															table.Cell().Element(cellStyle).AlignRight().Text(element.CountOfSteps.ToString());
															table.Cell().Element(cellStyle).AlignRight().Text(DifficultyLevelConversion.GetName(element.DifficultyLevel));
															table.Cell().Element(cellStyle).AlignRight().Text($"{element.TotalDifficulty:0.0}");
															table.Cell().Element(cellStyle).AlignRight().Text($"{element.MaximumDifficulty:0.0}");


															static PdfContainer cellStyle(PdfContainer container)
																=> container
																	.DefaultTextStyle(DefaultStyle)
																	.BorderBottom(1)
																	.BorderColor(PdfColors.Grey.Lighten2)
																	.PaddingVertical(5);
														}
													}
												)
										);

										if (!string.IsNullOrWhiteSpace(Comment))
										{
											column.AddComment(Comment);
										}
									}
								)
						);
					}
			)
		)
	);
}

/// <include file='../../global-doc-comments.xml' path='g/csharp11/feature[@name="file-local"]/target[@name="class" and @when="extension"]'/>
file static class Extensions
{
	public static void AddComment(this ColumnDescriptor @this, string comment)
		=> @this.Item()
			.PaddingTop(25)
			.Element(
				c => c
					.Background(PdfColors.Grey.Lighten3)
					.Padding(10)
					.Column(
						column =>
						{
							column.Spacing(5);
							column.Item()
								.DefaultTextStyle(AnalysisResultDocumentCreator.TitleStyle)
								.Text(GetString("AnalyzePage_GenerateCommentTitle"));
							column.Item()
								.DefaultTextStyle(AnalysisResultDocumentCreator.DefaultStyle)
								.Text(comment);
						}
					)
			);

	public static TextStyle SupportChineseCharacters(this TextStyle @this) => @this.Fallback(static f => f.FontFamily("Microsoft YaHei UI"));
}
