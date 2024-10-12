namespace Sudoku.Concepts.Coordinates.Formatting;

/// <summary>
/// Represents a type that can format a <see cref="ChainOrLoop"/> instance.
/// </summary>
/// <seealso cref="ChainOrLoop"/>
public sealed class ChainOrLoopFormatInfo : FormatInfo<ChainOrLoop>
{
	/// <summary>
	/// Initializes a <see cref="ChainOrLoopFormatInfo"/> instance.
	/// </summary>
	public ChainOrLoopFormatInfo()
	{
	}

	/// <summary>
	/// Copies converter options into the current instance.
	/// </summary>
	/// <param name="baseConverter">The base converter.</param>
	public ChainOrLoopFormatInfo(CoordinateConverter baseConverter)
		=> _ = baseConverter switch
		{
			RxCyConverter
			{
				MakeDigitBeforeCell: var makeDigitBeforeCell,
				MakeLettersUpperCase: var makeLettersUpperCase,
				DefaultSeparator: var defaultSeparator,
				NotationBracket: var notationBracket,
				DigitBracketInCandidateGroups: var digitBracketInCandidateGroups
			} => (
				NodeFormatType = CoordinateType.RxCy,
				MakeDigitBeforeCell = makeDigitBeforeCell,
				MakeLettersUpperCase = makeLettersUpperCase,
				DefaultSeparator = defaultSeparator,
				NotationBracket = notationBracket,
				DigitBracketInCandidateGroups = digitBracketInCandidateGroups
			),
			K9Converter
			{
				MakeLettersUpperCase: var makeLettersUpperCase,
				FinalRowLetter: var finalRowLetter,
				DefaultSeparator: var defaultSeparator,
				NotationBracket: var notationBracket
			} => (
				NodeFormatType = CoordinateType.K9,
				MakeLettersUpperCase = makeLettersUpperCase,
				FinalRowLetter = finalRowLetter,
				DefaultSeparator = defaultSeparator,
				NotationBracket = notationBracket
			),
			ExcelCoordinateConverter
			{
				MakeLettersUpperCase: var makeLettersUpperCase,
				DefaultSeparator: var defaultSeparator,
				NotationBracket: var notationBracket
			} => (
				MakeLettersUpperCase = makeLettersUpperCase,
				NodeFormatType = CoordinateType.Excel,
				DefaultSeparator = defaultSeparator,
				NotationBracket = notationBracket
			),
			LiteralCoordinateConverter
			{
				DefaultSeparator: var defaultSeparator,
				NotationBracket: var notationBracket
			} => (
				NodeFormatType = CoordinateType.Literal,
				DefaultSeparator = defaultSeparator,
				NotationBracket = notationBracket
			),
			_ => default(object?)
		};


	/// <inheritdoc cref="RxCyConverter.MakeDigitBeforeCell"/>
	public bool MakeDigitBeforeCell { get; init; } = false;

	/// <inheritdoc cref="RxCyConverter.MakeLettersUpperCase"/>
	public bool MakeLettersUpperCase { get; init; } = false;

	/// <summary>
	/// Indicates whether strong and weak links inside a cell will be folded. By default it's <see langword="false"/>.
	/// </summary>
	public bool FoldLinksInCell { get; init; } = false;

	/// <summary>
	/// Indicates whether digits are inlined in links. By default it's <see langword="false"/>.
	/// </summary>
	public bool InlineDigitsInLink { get; init; } = false;

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

	/// <summary>
	/// Indicates inlined digits separator. By default it's pipe operator <c>"|"</c>.
	/// </summary>
	public string InlinedDigitsSeparator { get; init; } = "|";

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

	/// <summary>
	/// Indicates B/B plot (Bilocation/Bivalue Plot) notation format.
	/// Visit <see href="http://forum.enjoysudoku.com/the-notation-used-in-nice-loops-and-sins-t3628.html">this link</see>
	/// to learn more information about this notation.
	/// </summary>
	public static IFormatProvider BivalueBilocationPlot
		=> new ChainOrLoopFormatInfo
		{
			InlineDigitsInLink = true,
			DefaultSeparator = "|",
			InlinedDigitsSeparator = "|",
			StrongLinkConnector = "=",
			WeakLinkConnector = "-",
			NotationBracket = NotationBracket.Square
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
			InlineDigitsInLink = InlineDigitsInLink,
			InlinedDigitsSeparator = InlinedDigitsSeparator,
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
#pragma warning disable format
		} with { DefaultSeparator = DefaultSeparator, NotationBracket = NotationBracket };
#pragma warning restore format

		var needAddingBrackets_Digits = Enum.IsDefined(DigitBracketInCandidateGroups) && DigitBracketInCandidateGroups != NotationBracket.None;
		var needAddingBrackets_Cells = Enum.IsDefined(NotationBracket) && NotationBracket != NotationBracket.None;
		var span = obj.ValidNodes;
		var sb = new StringBuilder();
		for (var (linkIndex, i) = (obj.WeakStartIdentity, 0); i < span.Length; linkIndex++, i++)
		{
			var inference = ChainOrLoop.Inferences[linkIndex & 1];
			ref readonly var nodeCandidates = ref span[i].Map;
			var nodeCells = nodeCandidates.Cells;
			var nodeDigits = nodeCandidates.Digits;
			ref readonly var nextNodeCandidates = ref i + 1 >= span.Length ? ref CandidateMap.Empty : ref span[i + 1].Map;
			var nextNodeCells = nextNodeCandidates.Cells;
			var nextNodeDigits = nextNodeCandidates.Digits;
			if (FoldLinksInCell && i != span.Length - 1 && nodeCells == nextNodeCells)
			{
				// (1)a=(2)a-(2)b=(3)b => (1=2)a-(2=3)b
				if (MakeDigitBeforeCell)
				{
					_ = needAddingBrackets_Digits ? sb.Append(DigitBracketInCandidateGroups.GetOpenBracket()) : sb;
					sb.Append(candidateConverter.DigitConverter(nodeDigits));
					sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
					sb.Append(candidateConverter.DigitConverter(nextNodeDigits));
					_ = needAddingBrackets_Digits ? sb.Append(DigitBracketInCandidateGroups.GetClosedBracket()) : sb;
					sb.Append(nodeCells.ToString(candidateConverter));
					i++;
				}
				else
				{
					sb.Append(nodeCells.ToString(candidateConverter));
					sb.Append(needAddingBrackets_Digits ? DigitBracketInCandidateGroups.GetOpenBracket() : "(");
					sb.Append(candidateConverter.DigitConverter(nodeDigits));
					sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
					sb.Append(candidateConverter.DigitConverter(nextNodeDigits));
					sb.Append(needAddingBrackets_Digits ? DigitBracketInCandidateGroups.GetClosedBracket() : ")");
				}
				goto AppendNextLinkToken;
			}

			if (InlineDigitsInLink)
			{
				// (1)a=(2)b => [a]=1|2=[b]
				_ = needAddingBrackets_Cells ? sb.Append(NotationBracket.GetOpenBracket()) : sb;
				sb.Append(nodeCells.ToString(candidateConverter));
				_ = needAddingBrackets_Cells ? sb.Append(NotationBracket.GetClosedBracket()) : sb;
			}
			else
			{
				sb.Append(nodeCandidates.ToString(candidateConverter));
			}

		AppendNextLinkToken:
			if (i != span.Length - 1)
			{
				if (InlineDigitsInLink)
				{
					sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
					_ = nodeDigits == nextNodeDigits
						? sb.Append(candidateConverter.DigitConverter(nodeDigits))
						: sb
							.Append(candidateConverter.DigitConverter(nodeDigits))
							.Append(InlinedDigitsSeparator)
							.Append(candidateConverter.DigitConverter(nextNodeDigits));
					sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
				}
				else
				{
					sb.Append(inference == Inference.Strong ? StrongLinkConnector : WeakLinkConnector);
				}
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
