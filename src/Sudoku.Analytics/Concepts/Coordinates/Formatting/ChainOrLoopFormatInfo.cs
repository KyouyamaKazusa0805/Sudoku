namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a type that can format a <see cref="ChainOrLoop"/> instance.
/// </summary>
/// <seealso cref="ChainOrLoop"/>
public sealed class ChainOrLoopFormatInfo : FormatInfo<ChainOrLoop>
{
	/// <inheritdoc cref="RxCyConverter.MakeDigitBeforeCell"/>
	public bool MakeDigitBeforeCell { get; init; } = false;

	/// <inheritdoc cref="RxCyConverter.MakeLettersUpperCase"/>
	public bool MakeLettersUpperCase { get; init; } = false;

	/// <summary>
	/// Indicates whether strong and weak links inside a cell will be folded. By default it's <see langword="false"/>.
	/// </summary>
	public bool FoldLinksInCell { get; init; } = false;

	/// <inheritdoc cref="K9Converter.FinalRowLetter"/>
	public char FinalRowLetter { get; init; } = 'I';

	/// <summary>
	/// Indicates the default separator. By default it's a comma <c>", "</c>.
	/// </summary>
	public string DefaultSeparator { get; init; } = ", ";

	/// <summary>
	/// Indicates the connector text for strong links. By default it's a double equal sign <c>" == "</c>.
	/// </summary>
	public string StrongLinkConnector { get; init; } = " == ";

	/// <summary>
	/// Indicates the connector text for weak links. By default it's a double minus sign <c>" -- "</c>.
	/// </summary>
	public string WeakLinkConnector { get; init; } = " -- ";

	/// <inheritdoc cref="CoordinateConverter.NotationBracket"/>
	public NotationBracket NotationBracket { get; init; } = NotationBracket.None;

	/// <inheritdoc cref="RxCyConverter.DigitBracketInCandidateGroups"/>
	public NotationBracket DigitBracketInCandidateGroups { get; init; } = NotationBracket.None;

	/// <summary>
	/// Indicates a type that formats each node (a group of candidates) in the chain pattern.
	/// </summary>
	public CoordinateType NodeFormatType { get; init; } = CoordinateType.RxCy;


	/// <summary>
	/// Indicates the standard format.
	/// </summary>
	public static IFormatProvider Standard => new ChainOrLoopFormatInfo();

	/// <summary>
	/// Indicates Eureka chain format.
	/// Visit <see href="http://sudopedia.enjoysudoku.com/Eureka.html">this link</see> to learn more information
	/// about Eureka Notation.
	/// </summary>
	public static IFormatProvider Eureka
		=> new ChainOrLoopFormatInfo
		{
			MakeDigitBeforeCell = true,
			FoldLinksInCell = true,
			DefaultSeparator = "|",
			StrongLinkConnector = "=",
			WeakLinkConnector = "-",
			DigitBracketInCandidateGroups = NotationBracket.Round
		};


	/// <inheritdoc/>
	[return: NotNullIfNotNull(nameof(formatType))]
	public override object? GetFormat(Type? formatType) => formatType == typeof(ChainOrLoopFormatInfo) ? this : null;

	/// <inheritdoc/>
	public override ChainOrLoopFormatInfo Clone()
		=> new()
		{
			MakeDigitBeforeCell = MakeDigitBeforeCell,
			MakeLettersUpperCase = MakeLettersUpperCase,
			FoldLinksInCell = FoldLinksInCell,
			FinalRowLetter = FinalRowLetter,
			DefaultSeparator = DefaultSeparator,
			StrongLinkConnector = StrongLinkConnector,
			WeakLinkConnector = WeakLinkConnector,
			NodeFormatType = NodeFormatType,
			NotationBracket = NotationBracket,
			DigitBracketInCandidateGroups = DigitBracketInCandidateGroups
		};


	/// <inheritdoc/>
	protected override string FormatCore(ref readonly ChainOrLoop obj)
	{
		var candidateConverter = NodeFormatType.GetConverter() switch
		{
			RxCyConverter c => c with
			{
				MakeDigitBeforeCell = MakeDigitBeforeCell,
				MakeLettersUpperCase = MakeLettersUpperCase,
				DigitBracketInCandidateGroups = DigitBracketInCandidateGroups
			},
			K9Converter c => c with { MakeLettersUpperCase = MakeLettersUpperCase, FinalRowLetter = FinalRowLetter },
			ExcelCoordinateConverter c => c with { MakeLettersUpperCase = MakeLettersUpperCase },
			{ } tempConverter => tempConverter,
			_ => throw new InvalidOperationException()
		};
		candidateConverter = candidateConverter with { DefaultSeparator = DefaultSeparator, NotationBracket = NotationBracket };

		var needAddingBrackets = Enum.IsDefined(DigitBracketInCandidateGroups) && DigitBracketInCandidateGroups != NotationBracket.None;
		var span = obj.ValidNodes;
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (obj.WeakStartIdentity, 0); i < span.Length; linkIndex++, i++)
		{
			var inference = ChainOrLoop.Inferences[linkIndex & 1];
			ref readonly var nodeCandidates = ref span[i].Map;
			if (FoldLinksInCell)
			{
				// (1)a=(2)a-(2)b=(3)b => (1=2)a-(2=3)b
				var nodeCells = nodeCandidates.Cells;
				var nodeDigits = nodeCandidates.Digits;

				if (i != span.Length - 1)
				{
					// Check for the next node, determining whether two nodes use a same group of cells.
					ref readonly var nextNodeCandidates = ref span[i + 1].Map;
					var nextNodeCells = nextNodeCandidates.Cells;
					var nextNodeDigits = nextNodeCandidates.Digits;
					if (nodeCells == nextNodeCells)
					{
						if (MakeDigitBeforeCell)
						{
							if (needAddingBrackets)
							{
								sb.Append(DigitBracketInCandidateGroups.GetOpenBracket());
							}
							sb.Append(candidateConverter.DigitConverter(nodeDigits));
							sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
							sb.Append(candidateConverter.DigitConverter(nextNodeDigits));
							if (needAddingBrackets)
							{
								sb.Append(DigitBracketInCandidateGroups.GetClosedBracket());
							}
							sb.Append(nodeCells.ToString(candidateConverter));
						}
						else
						{
							sb.Append(nodeCells.ToString(candidateConverter));
							sb.Append(needAddingBrackets ? DigitBracketInCandidateGroups.GetOpenBracket() : "(");
							sb.Append(candidateConverter.DigitConverter(nodeDigits));
							sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
							sb.Append(candidateConverter.DigitConverter(nextNodeDigits));
							sb.Append(needAddingBrackets ? DigitBracketInCandidateGroups.GetClosedBracket() : ")");
						}
						goto AppendNextLinkToken;
					}
				}
			}

			sb.Append(nodeCandidates.ToString(candidateConverter));

		AppendNextLinkToken:
			if (i != span.Length - 1)
			{
				sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
			}
		}
		return sb.ToString();
	}

	/// <inheritdoc/>
	[DoesNotReturn]
	protected override ChainOrLoop ParseCore(string str) => throw new NotSupportedException();


	/// <inheritdoc cref="FormatCore(ref readonly ChainOrLoop)"/>
	[UnsafeAccessor(UnsafeAccessorKind.Method, Name = nameof(FormatCore))]
	internal static extern string FormatCoreUnsafeAccessor(ChainOrLoopFormatInfo @this, ref readonly ChainOrLoop obj);
}
