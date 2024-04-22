namespace Sudoku.Analytics.Steps;

/// <summary>
/// Provides with a step that is a <b>Complex Fish</b> technique.
/// </summary>
/// <param name="conclusions"><inheritdoc/></param>
/// <param name="views"><inheritdoc/></param>
/// <param name="options"><inheritdoc/></param>
/// <param name="digit"><inheritdoc/></param>
/// <param name="baseSetsMask"><inheritdoc/></param>
/// <param name="coverSetsMask"><inheritdoc/></param>
/// <param name="exofins">
/// Indicates the fins, positioned outside the fish.
/// They will be defined if cover sets cannot be fully covered, with the number of cover sets being equal to the number of base sets.
/// </param>
/// <param name="endofins">
/// Indicates the fins, positioned inside the fish. Generally speaking, they will be defined if they are in multiple base sets.
/// </param>
/// <param name="isFranken">
/// Indicates whether the fish is a Franken fish. If <see langword="true"/>, a Franken fish; otherwise, a Mutant fish.
/// </param>
/// <param name="isSashimi"><inheritdoc/></param>
/// <param name="isCannibalism">Indicates whether the fish contains any cannibalism.</param>
/// <param name="isSiamese"><inheritdoc/></param>
public sealed partial class ComplexFishStep(
	Conclusion[] conclusions,
	View[]? views,
	StepSearcherOptions options,
	Digit digit,
	HouseMask baseSetsMask,
	HouseMask coverSetsMask,
	[PrimaryConstructorParameter] ref readonly CellMap exofins,
	[PrimaryConstructorParameter] ref readonly CellMap endofins,
	[PrimaryConstructorParameter] bool isFranken,
	bool? isSashimi,
	[PrimaryConstructorParameter] bool isCannibalism,
	bool isSiamese = false
) : FishStep(conclusions, views, options, digit, baseSetsMask, coverSetsMask, exofins | endofins, isSashimi, isSiamese)
{
	/// <inheritdoc/>
	public override int BaseDifficulty => 32;

	/// <inheritdoc/>
	public override Technique Code
	{
		get
		{
			// Creates a buffer to store the characters that isn't a space or a bar.
			var name = internalName();
			var buffer = (stackalloc char[name.Length]);
			var bufferLength = 0;
			foreach (var ch in name)
			{
				if (ch is not ('-' or ' '))
				{
					buffer[bufferLength++] = ch;
				}
			}

			return Enum.Parse<Technique>(buffer[..bufferLength]);


			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			ReadOnlySpan<char> internalName()
			{
				var finKindStr = finKind() is var finModifier and not FishFinKind.Normal
					? IsSiamese ? $"Siamese {finModifier} " : $"{finModifier} "
					: string.Empty;
				var shapeKindStr = shapeKind() is var shapeModifier and not FishShapeKind.Basic ? $"{shapeModifier} " : string.Empty;
				return $"{finKindStr}{shapeKindStr}{TechniqueMarshal.GetFishEnglishName(Size)}";
			}

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			FishFinKind finKind()
				=> IsSashimi switch { true => FishFinKind.Sashimi, false => FishFinKind.Finned, _ => FishFinKind.Normal };

			[MethodImpl(MethodImplOptions.AggressiveInlining)]
			FishShapeKind shapeKind() => IsFranken ? FishShapeKind.Franken : FishShapeKind.Mutant;
		}
	}

	/// <inheritdoc/>
	public override FormatInterpolation[] FormatInterpolationParts
		=> [new(EnglishLanguage, [InternalNotation]), new(ChineseLanguage, [InternalNotation])];

	/// <inheritdoc/>
	public override FactorCollection Factors
		=> [
			new ComplexFishSizeFactor(),
			new ComplexFishIsSashimiFactor(),
			new ComplexFishShapeFactor(),
			new ComplexFishCannibalismFactor()
		];


	/// <inheritdoc/>
	public override bool Equals([NotNullWhen(true)] Step? other)
		=> other is ComplexFishStep comparer
		&& Digit == comparer.Digit
		&& BaseSetsMask == comparer.BaseSetsMask && CoverSetsMask == comparer.CoverSetsMask
		&& Exofins == comparer.Exofins && Endofins == comparer.Endofins;
}
