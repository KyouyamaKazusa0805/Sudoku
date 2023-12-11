using System.Diagnostics.CodeAnalysis;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.SourceGeneration;
using System.Text;
using System.Text.RegularExpressions;
using Sudoku.Concepts;

namespace Sudoku.Text.Converters;

/// <summary>
/// Represents a converter that converts a <see cref="Grid"/> into an equivalent <see cref="string"/> representation
/// using Susser formatting rule.
/// </summary>
/// <param name="WithCandidates">
/// <para>Indicates whether the formatter will reserve candidates as pre-elimination.</para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
/// <param name="WithModifiables">
/// <para>
/// Indicates whether the formatter will output and distinct modifiable and given digits.
/// If so, the modifiable digits will be displayed as <c>+digit</c>, where <c>digit</c> will be replaced
/// with the real digit number (from 1 to 9).
/// </para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
/// <param name="ShortenSusser">
/// <para>Indicates whether the formatter will shorten the final text.</para>
/// <para>The default value is <see langword="false"/>.</para>
/// </param>
public partial record SusserGridConverter(bool WithCandidates = false, bool WithModifiables = false, bool ShortenSusser = false) : GridConverter
{
	/// <summary>
	/// Indicates the modifiable prefix character.
	/// </summary>
	protected const char ModifiablePrefix = '+';

	/// <summary>
	/// Indicates the line separator character used by shortening Susser format.
	/// </summary>
	private const char LineLimit = ',';

	/// <summary>
	/// Indicates the star character used by shortening Susser format.
	/// </summary>
	private const char Star = '*';

	/// <summary>
	/// Indicates the dot character.
	/// </summary>
	private const char Dot = '.';

	/// <summary>
	/// Indicates the zero character.
	/// </summary>
	private const char Zero = '0';

	/// <summary>
	/// Indicates the pre-elimination prefix character.
	/// </summary>
	private const char PreeliminationPrefix = ':';


	/// <summary>
	/// Indicates the default instance. The property set are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="WithCandidates"/>: <see langword="false"/></item>
	/// <item><see cref="WithModifiables"/>: <see langword="false"/></item>
	/// <item><see cref="ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly SusserGridConverter Default = new() { Placeholder = Dot };

	/// <summary>
	/// Indicates the instance whose inner properties are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'.'</c></item>
	/// <item><see cref="WithCandidates"/>: <see langword="true"/></item>
	/// <item><see cref="WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly SusserGridConverter Full = new(WithCandidates: true, WithModifiables: true) { Placeholder = Dot };

	/// <summary>
	/// Indicates the instance whose inner properties are:
	/// <list type="bullet">
	/// <item><see cref="Placeholder"/>: <c>'0'</c></item>
	/// <item><see cref="WithCandidates"/>: <see langword="true"/></item>
	/// <item><see cref="WithModifiables"/>: <see langword="true"/></item>
	/// <item><see cref="ShortenSusser"/>: <see langword="false"/></item>
	/// </list>
	/// </summary>
	public static readonly SusserGridConverter FullZero = new(WithCandidates: true, WithModifiables: true) { Placeholder = Zero };



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
			_ => throw new InvalidOperationException($"The placeholder character invalid; expected: '{Zero}' or '{Dot}'.")
		};
	}


	/// <inheritdoc/>
	public override GridNotationConverter Converter
		=> (scoped ref readonly Grid grid) =>
		{
			scoped var sb = new StringHandler(162);
			var originalGrid = this switch
			{
				{ WithCandidates: true, ShortenSusser: false } => Grid.Parse((this with { WithCandidates = false }).Converter(in grid)),
				_ => Grid.Undefined
			};

			var eliminatedCandidates = CandidateMap.Empty;
			for (var c = 0; c < 81; c++)
			{
				var state = grid.GetState(c);
				if (state == CellState.Empty && !originalGrid.IsUndefined && WithCandidates)
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

				switch (state)
				{
					case CellState.Empty:
					{
						sb.Append(Placeholder);
						break;
					}
					case CellState.Modifiable:
					{
						switch (this)
						{
							case { WithModifiables: true, ShortenSusser: false }:
							{
								sb.Append(ModifiablePrefix);
								sb.Append(grid.GetDigit(c) + 1);
								break;
							}
							case { Placeholder: var p }:
							{
								sb.Append(p);
								break;
							}
						}
						break;
					}
					case CellState.Given:
					{
						sb.Append(grid.GetDigit(c) + 1);
						break;
					}
					default:
					{
						throw new InvalidOperationException("The specified state is invalid.");
					}
				}
			}

			var elimsStr = new HodokuTripletConverter().Converter(eliminatedCandidates);
			var @base = sb.ToStringAndClear();
			return ShortenSusser
				? shorten(@base, Placeholder)
				: $"{@base}{(string.IsNullOrEmpty(elimsStr) ? string.Empty : $"{PreeliminationPrefix}{elimsStr}")}";


			static string shorten(string @base, char placeholder)
			{
				// lang = regex
				var placeholderPattern = placeholder == Dot ? """\.+""" : """0+""";
				scoped var resultSpan = (stackalloc char[81]);
				var spanIndex = 0;
				for (var i = 0; i < 9; i++)
				{
					var characterIndexStart = i * 9;
					var sliced = @base[characterIndexStart..(characterIndexStart + 9)];
					switch (Regex.Matches(sliced, placeholderPattern))
					{
						case []:
						{
							// Can't find any simplifications.
							Unsafe.CopyBlock(
								ref Ref.AsByteRef(ref resultSpan[characterIndexStart]),
								in Ref.AsReadOnlyByteRef(in Ref.GetRefFirstChar(sliced)),
								sizeof(char) * 9
							);
							spanIndex += 9;
							break;
						}
						case var collection:
						{
							switch (new HashSet<Match>(collection, EqualityComparer.Instance))
							{
								case { Count: 1 } set when set.First() is { Length: var firstLength }:
								{
									// All matches are same-length.
									for (var j = 0; j < 9;)
									{
										if (sliced[j] == placeholder)
										{
											resultSpan[spanIndex++] = Star;
											j += firstLength;
										}
										else
										{
											resultSpan[spanIndex++] = sliced[j];
											j++;
										}
									}
									break;
								}
								case var set:
								{
									var match = set.MaxBy(static m => m.Length)!.Value;
									var pos = sliced.IndexOf(match);
									for (var j = 0; j < 9; j++)
									{
										if (j == pos)
										{
											resultSpan[spanIndex++] = Star;
											j += match.Length;
										}
										else
										{
											resultSpan[spanIndex++] = sliced[j];
											j++;
										}
									}
									break;
								}
							}
							break;
						}
					}

					if (i != 8)
					{
						resultSpan[spanIndex++] = LineLimit;
					}
				}

				return resultSpan[..spanIndex].ToString();
			}
		};
}

/// <summary>
/// Represents a comparer instance that compares two <see cref="Match"/> instances via their length.
/// </summary>
/// <seealso cref="Match"/>
file sealed class EqualityComparer : IEqualityComparer<Match>
{
	/// <summary>
	/// The singleton instance.
	/// </summary>
	public static readonly EqualityComparer Instance = new();


	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public bool Equals(Match? x, Match? y) => (x?.Value.Length ?? -1) == (y?.Value.Length ?? -1);

	/// <inheritdoc/>
	[MethodImpl(MethodImplOptions.AggressiveInlining)]
	public int GetHashCode([DisallowNull] Match? obj) => obj?.Value.Length ?? -1;
}
