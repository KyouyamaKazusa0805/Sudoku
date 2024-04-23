namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a grid converter that uses inline Susser format, by using square brackets to describe candidates,
/// removing triplet set.
/// </summary>
/// <param name="NegateEliminationsTripletRule">
/// <inheritdoc cref="SusserGridConverter" path="/param[@name='NegateEliminationsTripletRule']"/>
/// </param>
public sealed partial record InlineSusserGridConverter(bool NegateEliminationsTripletRule = false) : IConceptConverter<Grid>
{
	/// <summary>
	/// Indicates the dot character.
	/// </summary>
	private const char Dot = '.';

	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';

	/// <summary>
	/// Indicates the modifiable prefix character.
	/// </summary>
	private const char ModifiablePrefix = '+';


	/// <summary>
	/// Indicates the default instance. The properties set are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="NegateEliminationsTripletRule"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly InlineSusserGridConverter Default = new(NegateEliminationsTripletRule: false) { Placeholder = Dot };

	/// <summary>
	/// Indicates the instance whose inner properties are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="NegateEliminationsTripletRule"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly InlineSusserGridConverter DefaultZero = new(NegateEliminationsTripletRule: false) { Placeholder = Zero };


	/// <summary>
	/// Indicates the placeholder of the grid text formatter.
	/// </summary>
	/// <value>The new placeholder text character to be set. The value must be <c>'.'</c> or <c>'0'</c>.</value>
	/// <returns>The placeholder text.</returns>
	[ImplicitField]
	public required char Placeholder
	{
		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		get => _placeholder;

		[MethodImpl(MethodImplOptions.AggressiveInlining)]
		init => _placeholder = value switch
		{
			Zero or Dot => value,
			_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("PlaceholderMustBeZeroOrDot"))
		};
	}


	/// <inheritdoc/>
	public FuncRefReadOnly<Grid, string> Converter
		=> (ref readonly Grid grid) =>
		{
			var sb = new StringBuilder(162);
			var originalGrid = Grid.Parse((SusserGridConverter.Default with { WithCandidates = false }).Converter(in grid));

			var eliminatedCandidates = (CandidateMap)[];
			for (var c = 0; c < 81; c++)
			{
				var state = grid.GetState(c);
				if (state == CellState.Empty && !originalGrid.IsUndefined)
				{
					// Check if the value has been set 'true' and the value has already deleted at the grid
					// with only givens and modifiables.
					foreach (var i in (Mask)(originalGrid[c] & Grid.MaxCandidatesMask))
					{
						if (!grid.GetExistence(c, i))
						{
							// The value is 'false', which means the digit has already been deleted.
							eliminatedCandidates.Add(c * 9 + i);
						}
					}
				}
			}
			eliminatedCandidates = NegateEliminationsTripletRule ? eliminatedCandidates : negateElims(in grid, in eliminatedCandidates);

			var candidatesDistribution = eliminatedCandidates.CellDistribution;
			for (var c = 0; c < 81; c++)
			{
				_ = grid.GetState(c) switch
				{
					CellState.Empty => sb.Append(
						candidatesDistribution.TryGetValue(c, out var digits) && digits != 0
							? $"[{string.Concat([.. from digit in digits select (digit + 1).ToString()])}]"
							: Placeholder.ToString()
					),
					CellState.Modifiable => sb.Append(ModifiablePrefix).Append(grid.GetDigit(c) + 1),
					CellState.Given => sb.Append(grid.GetDigit(c) + 1),
					_ => throw new InvalidOperationException(ResourceDictionary.ExceptionMessage("InvalidStateOnParsing"))
				};
			}

			return sb.ToString();


			static CandidateMap negateElims(ref readonly Grid grid, ref readonly CandidateMap eliminatedCandidates)
			{
				var eliminatedCandidatesCellDistribution = eliminatedCandidates.CellDistribution;
				var result = (CandidateMap)[];
				foreach (var cell in grid.EmptyCells)
				{
					if (eliminatedCandidatesCellDistribution.TryGetValue(cell, out var digitsMask))
					{
						foreach (var digit in digitsMask)
						{
							result.Add(cell * 9 + digit);
						}
					}
				}

				return result;
			}
		};
}
